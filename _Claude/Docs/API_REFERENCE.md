# APIリファレンス

コメントルームで触れたAPIを都度追記する。Claudeが正しく使うための参照用。

## IMonoBehaviour
- 場所: `MantenseiLib/Core/Base/Interfaces/IMonoBehaviour.cs`
- MonoBehaviourの公開APIをインターフェースとして抽出。IProviderがこれを継承することで、インターフェース経由の参照をMonoBehaviourにキャストせずにtransform等の基本APIへアクセスできる

## IProvider
- 場所: `MantenseiLib/Features/Action/Character/Core/IProvider.cs`
- ジェネリックの型引数を隠蔽し、異なるProvider群を統一的に扱う（配列化等）ための非ジェネリック抽象層

## IProvider\<TEntity\>
- 場所: 同上
- 実体と参照を分離し、Reference経由でのみアクセスすることを強制する
- Safe()等の安全管理はIProviderの責務外（ブラックボックス）

## MoveCommand
- 場所: `MantenseiLib/Features/Action/Character/Component/Move/Movement.cs`
- 移動パラメータのパラメータオブジェクト。引数追加時の変更範囲爆発を防ぐ
- Normal（方向）とSpeed（速度）に分解して保持。Velocity経由での従来的なアクセスも可能

## IMoverProvider / IMoverEntity
- 場所: 同上
- IMoverProvider: 移動機能のProvider。データの規約としてのインターフェース。配列化・差し替えの単位
- IMoverEntity: 移動機能の実体規約。Move(MoveCommand)が基幹、Move(Vector2)等は窓口の拡張

## MoverBase
- 場所: 同上
- Walker/Flyer等の共通配線（rb2d取得等）をまとめるユーティリティ基底。型の単位はあくまでインターフェース

## UnityObjectSafetyExtensions
- 場所: `MantenseiLib/Core/Extensions/UnityObjectSafetyExtensions.cs`
- Unityのfake null問題（Destroyされたオブジェクトが`==null`でしか検知できない）をジェネリック文脈で解決する
- `IsSafe<T>()`: fake nullをジェネリック文脈で安全に判定する
- `Safe<T>(this T)`: fake nullをC#のnullに正規化。?.チェーンで安全にアクセス可能にする
- `Safe<T>(this T[])`: null配列を空配列に正規化。foreachのNullReferenceException防止

## PlayerReferenceHub
- 場所: `MantenseiLib/Features/Action/Character/Core/PlayerReferenceHub.cs`
- Facadeパターン。利用側が個々のProviderインターフェースを知らずに済む
- 各参照は?.Safe()?.で解決され、特定の機能が欠損・破壊されても他は独立して動作し続ける
- 現状はAwake時取得のみで、ランタイムでのProvider差し替えは今後の課題

## SceneLauncher / SceneMarker
- 場所: `MantenseiLib/Core/Systems/Event/Scene/`
- シーンロード完了とオブジェクト初期化完了を分離する
- SceneMarkerがStart()で自己登録し、呼び出し元に初期化完了を通知する
- **Why:** シーンロード完了時点ではシーン内オブジェクトの初期化（データ取得等）が終わっていない場合があり、アクセスするとNullReferenceExceptionが発生する

## Animator2D
- 場所: `MantenseiLib/Modules/Animation2D/`
- 複数アニメーションが同時にIsActive=trueでいられ、毎フレーム最高優先度のものが表示される
- 高優先度が終了すると自然に低優先度に戻る
- Play: IsActiveをtrueにして優先度競争に参加 / PlayOneShot: 一回限り / ForcePlay: 即座に強制再生

## InputActionManager
- 場所: `MantenseiLib/Modules/`
- 最高優先度のアクティブなInputMapだけを有効化する
- 例: UI(Priority:100)が開いている間はPlayer(Priority:0)の入力を自動無効化
- InputActionPresetが自己登録し、SetActive()でアクティブ/キャンセルを宣言するだけ
