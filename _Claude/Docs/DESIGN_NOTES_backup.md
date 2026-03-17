# 設計指針メモ

コメントルームで設計者から学んだ設計思想を何でも蓄積する。
特に誤読しやすいものは重要だが、それに限らず気づいたことは無遠慮に記録する。あとで設計者が整理する。

## IMonoBehaviour
- インターフェース経由で受け取った参照をMonoBehaviourにキャストせずに基本API（transform等）へアクセスするための抽出
- IProviderがこれを継承することで、Provider群はキャスト不要でMonoBehaviourのAPIを使える

## IProvider / IProvider\<TEntity\>
- IProvider（非ジェネリック）：ジェネリックの型引数を隠蔽し、異なるProvider群を統一的に扱う（配列化等）ための抽象層
- IProvider\<TEntity\>（ジェネリック）：実体と参照を分離し、Reference経由でのみアクセスすることを強制する
- Safe()等の安全管理はブラックボックスであり、IProviderの責務ではない
- 根幹のインターフェースは「何であるか」と「なぜ存在するか」が不可分なので、summaryに設計意図を含めてよい

## UnityObjectSafetyExtensions
- Unityのfake null問題（Destroyされたオブジェクトが`==null`でしか検知できない）をジェネリック文脈で解決する
- Safe\<T\>(this T): fake nullをC#のnullに正規化し、?.チェーンで安全にアクセス可能にする
- Safe\<T\>(this T[]): null配列をforeachに渡した際のNullReferenceExceptionを防ぐ
- 2つの異なる問題（fake null / foreach安全化）を同名メソッドのオーバーロードで解決している

---

# プロジェクト全体の設計方針
以下はProvider固有ではなく、プロジェクト横断で適用される設計方針。

## 大原則：親は子を知らない、子は親を知る
- 子が親に自己登録する。親は子の存在を知らない。機能追加時は子を足すだけ、親は変更不要
- AI/Controllerも下位層に配置し「ちぎれる四肢」として扱う → GameObjectの有効/無効だけで切り替え可能
- 例外的に下層を知ってよいのはHub、Manager等の特殊モジュールのみ
- UnityのAnimatorは親が子を監視するため拡張性が死ぬ → 代わりにAnimator2D + Animation2DRegistererで「子が親に登録」を実現し、優先度ベースで自動解決

## モジュール配置：1モジュール = 自己完結した単位
- 機能 + 付随物（アニメーション、音等）を1つのモジュールとしてパッケージングする
- 機能追加はモジュールをぶら下げるだけ、追加設定ほぼ不要
- 付随物もちぎれる：アニメーションや音が不要なら消せばいい。機能本体には影響しない
- 機能とその付随物が別々の場所に散らばるのはロス

```
Player
├── Core/Data          … ステータス（HP, MP等）
├── Art/Visual         … Animator2D + SpriteRenderer（登録を受け付ける側）
├── Controller
│   ├── AI             … ちぎれる。有効/無効で切り替え
│   └── Controller     … ちぎれる。AIと排他
├── Component
│   ├── Sensor         … GroundChecker, CeillingChecker
│   └── Action
│       ├── Mover
│       │   ├── Walker … 機能本体
│       │   │   └── Anim  … 付随物（Animation2DRegisterer）
│       │   ├── Flyer
│       │   │   └── Anim
│       │   └── Runner
│       │       └── Anim
│       ├── Jumper
│       │   ├── StandardJumper
│       │   └── MultiJumper
│       └── Misc
│           ├── Reflector
│           └── AntiWallStic
├── Col                … Collider
└── Input
```

## パラメータオブジェクト
- 引数は増える可能性があるなら最初からクラスにすべき。引数追加時の変更範囲の爆発を防ぐ
- 例: MoveCommand。Move(Vector2)等のショートハンドはあくまで窓口の拡張。基幹のデータ受け渡しは常にCommandクラス経由
- GoFのCommand（実行の遅延/取り消し）ではなく、パラメータオブジェクトとしての実践的な設計判断が核心

## インターフェースとクラス継承の使い分け
- データ（契約）はインターフェースで定義する → 配列化・差し替えの単位
- クラス継承は「実装の便利屋」に限定 → 例: MoverBaseは共通配線のユーティリティ
- 配列にするなら `IMoverProvider[]` であって `MoverBase[]` ではない
- 具象基底クラスに依存すると、インターフェースだけ実装した別の実装を排除してしまう

## 優先度ベースの宣言的マネージャーパターン
- ステートマシンの問題：遷移先の指定が必要 → 3ステートでスパゲッティ化、遷移先未発見で「無」が実行される事故が起きる
- このパターンの解決：各機能が決めるのは「アクティブ」と「キャンセル」だけ。次に何を実行するかは自分で決めない
- 責務の分離：外部がアクティブ/キャンセルを決め、マネージャーが機械的に最高優先度を選び取る
- API設計方針：内部は複雑でもいいが、利用者にはシンプルに使わせる
- 適用例：Animator2D（アニメーション）、InputActionManager（入力）

## API設計方針：内部の複雑さを隠蔽し、利用者にはシンプルに使わせる
- 利用者が触るのはアクティブ/キャンセルだけ
- マネージャー内部で優先度解決、競合処理等の複雑さを引き受ける

---

## Animator2D
- 複数のアニメーションが同時にIsActive=trueでいられ、毎フレーム最高優先度のものが表示される
- 高優先度アニメーション（Fall等）が終了すると、自然に低優先度（Walk等）に戻る
- Play: IsActiveをtrueにして優先度競争に参加 / PlayOneShot: IsActiveを設定せず一回限り
- ForcePlay: 即座にStartAnimationを呼び強制再生開始

## InputActionManager
- 優先度ベースで最高優先度のアクティブなInputMapだけを有効化する
- 例: UI(Priority:100)が開いている間はPlayer(Priority:0)の入力を自動無効化
- InputActionPresetが自己登録し、SetActive()でアクティブ/キャンセルを宣言するだけ

## PlayerReferenceHub
- Facadeパターン：利用側が個々のProviderインターフェースを知らなくて済む
- 耐障害性：各参照はnull安全（?.Safe()?.）に解決され、特定の機能が欠損・破壊されても他は独立して動作し続ける
- 現状はAwakeで取得するだけで、ゲームプレイ中の動的なProvider差し替え機能はない
- ランタイムでのProvider切り替え（例: 通常移動→飛行）は今後やりたい機能

## SceneLauncher / SceneMarker
- シーンロード完了≠オブジェクト初期化完了の問題を解決する
- シーンロード後、シーン内オブジェクトの初期化（データ取得等）が終わっていない場合がある
- SceneMarkerがStart()で自己登録することで、確実に初期化済みのタイミングを呼び出し元に通知する
- 呼び出し元がシーン内データにアクセスした際のNullReferenceExceptionを防止

## ProviderComponentManager
- レガシークラス（設計者も記憶が曖昧）
- LifecycleObserverでProviderのGameObject無効化を検知し、デフォルトに自動復帰する仕組み
- PlayerReferenceHubのランタイム差し替え機能の先駆的な試みだった可能性

## LifecycleObserver
- GameObjectのライフサイクルイベント（OnEnable, OnDisable, OnDestroy等）を外部から購読するための動的アタッチ型コンポーネント
- Fluent APIでメソッドチェーン可能
