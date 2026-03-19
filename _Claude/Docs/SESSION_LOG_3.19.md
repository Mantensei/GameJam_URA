# 実装ルーム セッションログ（2026-03-19）

## このルームの制約

- 既存 `_Claude/Scripts/` のコードはレガシーなので参照不要
- 一気に何個も作らず、個別に一つ一つ作る
- 指示を受けていない場合は何もしない。すべてのタスクは指示を受けてから行う
- GAME_DESIGN.md は変更箇所が多いので軽く読む程度で良い
- ユーザーへの質問はAskUserQuestionで行う

## 今回やったこと

- StageData: BuildStageData → Init() に改名、ビルド結果をメンバー保持 + Normas/Dobons/MenuList プロパティ公開
- MenuItem を MenuNorma.cs に、Comment を CommentNorma.cs に移動（ファイル整理）
- NormaDataEditor: 同一アセット重複登録をサイレント除外、ログをDebug.Logに統一
- NormaData.GetAllNormas: ランタイム側でもname重複除去
- RegisterView: 「裏メニュー注文」ボタン削除、「お会計」クリックで食事代減額 + ドボン罰金減額 + ノルマ判定
- GameManager: AddMoney(int) 追加
- URADebugger + StageDebugPanel 作成（F1トグルでノルマ/ドボン/メニュー/所持金チートシート表示）
- Commenter 作成（CommentView表示 + 感想選択時にCommentNorma達成判定）
- CommentView 実装（感想リスト動的生成、選択イベント発火）
- RestaurantInputHandler.TryComment 接続

## フキダシ実装（自動作業）

### 作成ファイル
- `Assets/_GameJam_URA/Scripts/UI/uGUI/SpeechBubble.cs` — 個々のフキダシ（追従・自動消滅）
- `Assets/_GameJam_URA/Scripts/UI/uGUI/SpeechBubbleManager.cs` — プール管理・動的生成API

### 編集ファイル
- `UIViewHub.cs` — `SpeechBubbleManager SpeechBubble` プロパティ追加
- `Commenter.cs` — Say() 内でフキダシ表示呼び出し追加

### Claudeの判断ログ（要確認）
1. **配置先:** `_Claude/` ではなく `Scripts/UI/uGUI/` に配置した。理由: レガシーではない新規本体コードのため。uGUIとUIToolkitでフォルダを分けた
2. **UIViewHubへの参照方式:** SpeechBubbleManagerはIUIViewではないので `GetComponents` 自動取得に乗れない。`FindAnyObjectByType` でキャッシュする形にした。遅延取得（初回アクセス時にFind）
3. **フキダシの生成方式:** 全てコードで動的生成（Prefab不要）。見た目変更時はCreateBubble()内を修正する。Prefab化した方がデザイン調整しやすいかもしれないが、まずは動くことを優先
4. **SpeechBubbleManagerのSerializeField:** defaultDuration, defaultOffset, bubbleWidth等をSerializeFieldにした。Inspectorで微調整可能にするため（CLAUDE.mdのSerializeField最小化方針に対して例外的）
5. **フキダシのCanvas:** 個々のフキダシがWorld Space Canvasを持つ構造。1マネージャー1Canvasにまとめる案もあるが、個別追従のシンプルさを優先

### セットアップ手順（手動作業）
- シーンに空GameObjectを作成し `SpeechBubbleManager` をアタッチ

## TODO（次のルームへ引き継ぎ）

- Resources/Stages/ フォルダ作成とStageDataアセット配置（手動作業）
- 生成したNormaアセットをNormaDataに格納（手動作業）
- プレイヤーGameObjectにCommenterコンポーネントをアタッチ（手動作業）
- SpeechBubbleManagerをシーンに配置（手動作業）
- 行動ログ（注文履歴・発言履歴）を記録するクラスの作成
- 客AI・スポーンシステムの再実装
- MenuNorma.csの「Can't assign script」ダイアログの原因調査（未解決）
