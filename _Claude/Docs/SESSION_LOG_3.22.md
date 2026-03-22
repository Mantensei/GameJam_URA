# 実装ルーム セッションログ（2026-03-22）

## このルームの制約

- 既存 `_Claude/Scripts/` のコードはレガシーなので参照不要
- 一気に何個も作らず、個別に一つ一つ作る
- 指示を受けていない場合は何もしない。すべてのタスクは指示を受けてから行う
- GAME_DESIGN.md は変更箇所が多いので軽く読む程度で良い
- ユーザーへの質問は絶対にAskUserQuestionで行え、質問の回答をチャットに手打ちさせるな
- ユーザーが開いているファイルは常に一番最初に確認すること

---

## 前セッションからの引き継ぎ（完了分）

### タイトル画面 ✅
- TitleScreen.uxml / .uss / TitleScreenView.cs 作成済み
- ポップアップ（遊び方・クレジット）実装済み
- Time.timeScale=0でゲーム停止、スタートで再開

### フェーズ1→2の切り替え ✅
- URA_PlayerController.cs でタイムアップ検出→OrderScreen表示

### メモUI改修 ✅
- MenuViewを左寄せ・単列表示に変更
- 〇×マーク機能（左クリック=○、右クリック=×、中クリック=リセット）

### 注文履歴（OrderLog）✅
- OrderLog.cs: 静的クラス、IDishItem参照を保持
- クリックした客の料理はRevealed（料理名表示）
- メモの〇×マークを履歴先頭に表示

### 客ホバーハイライト ✅
- `CustomerHoverManager.cs` — 純粋クラス（非MonoBehaviour）、ポインター距離で最近接の客を検出
- `SpriteOutlineHighlight.cs` — CustomerHoverManager.Instance参照で色変え
- `URA_PlayerController.TryInspectCustomer()` — マネージャーのHoveredCustomer参照に簡略化

### DebugFileLogger改善 ✅
- `[Conditional("UNITY_EDITOR")]` に変更、呼び出し側の`#if UNITY_EDITOR`が不要に

### LifecycleObserverFactory ✅
- `MantenseiLib/Core/Systems/Event/LifecycleObserverFactory.cs`
- メソッドチェーンでイベント登録

### OrderScreen改修 ✅
- 「注文に進む」「もう一日調べる」の2ボタンに変更
- `onOrderConfirmed`（→フェーズ3へ）、`onRetry`（→フェーズ1再チャレンジ）イベント追加

---

## 今セッションで実装した内容

### JudgeScreen全面改修 ✅
- **レイアウト変更**: 提供履歴を削除。左にMenuPopup風メニュー一覧、右に注文一覧の2カラム構成
- **メモ引き継ぎ**: フェーズ2の○×マークがMenuView.GetMarkPrefix()経由でJudgeScreenに反映
- **注文操作**: メニュークリック→注文一覧に追加（トグル）、注文一覧クリックで取り消し
- **ポイント制判定**: `Score(k) = 0.5 * k * (k + 19)`、累計100点以上でクリア
  - 1品=10pt、2品=21pt、3品=33pt、8品=108pt（一発クリア可能）
  - まとめ注文でボーナス倍率、リスク＆リターンのジレンマ
- **注文済み品はグレーアウト**（SoldClass）、再選択不可
- **複数回注文可能**: ポイント累積
- ファイル: `JudgeScreen.uxml` / `JudgeScreen.uss` / `JudgeScreenView.cs`

### NovelPopup（新規）✅
- ノベル風テキストボックス。画面下部に全幅表示
- クリックで次のメッセージへ進行、全メッセージ表示後にonCompletedイベント発火
- ファイル: `NovelPopup.uxml` / `NovelPopup.uss` / `NovelPopupView.cs`
- UIViewType / UIViewHub に NovelPopup 追加済み

### フロー接続 ✅
- **フェーズ2→3**: `OrderScreen.onOrderConfirmed` → `JudgeScreen.Show()` （URA_PlayerController.OnOrderConfirmed）
- **フェーズ2→1**: `OrderScreen.onRetry` → タイマーリセット（URA_PlayerController.OnRetry）
- **フェーズ3→判定**: 注文ボタン → ドボン判定 → NovelPopupで店主セリフ実況
- **クリア/ゲームオーバー→タイトル**: `JudgeScreen.onCleared/onGameOver` → `TitleScreen.Show()`

### 注文判定フロー（店主セリフ付き）
- 注文ボタン押下 → JudgeScreen表示したまま → NovelPopupで店主が実況
- **ドボン**: 「{品名}ね...」→「こんなもん頼む舌バカに食わせるものはないわ！」→「帰ってちょうだい！」→ タイトルへ
- **セーフ＆未クリア**: 「おいしい？ ふん、当然ね」→「まだまだ食べられるわよね？ もっと頼みなさい」→ JudgeScreenで追加注文可能
- **クリア**: 「{品名}...？」→「アナタ...見込みあるわね！」→「コトッ」→「...これはサービスよ」→ タイトルへ
- ドボン判定はLINQのFirstOrDefaultで検出

### 店主キャラクター像
- 女性、サバサバした口調
- セーフ時: 「いっぱい食べなさい！」「あなた、見る目あるのね！」
- ドボン時: 「はぁ...注文これだけ？」「ぷっ...見る目ないわね？」
- 参照: `CustomerMineAI.cs` 253-267行

---

## TODO（残タスク）

### 画面
- [ ] OP画面
- [ ] フェーズ移行画面（フェーズ1→フェーズ2への遷移演出）
- [ ] ED画面（結果・ゲームオーバー含む）— 現在はタイトルに戻るで仮対応

### シーン配置
- [ ] NovelPopupのUIDocument + NovelPopupView をUIViewHubの子に配置（未配置なら）
- [ ] JudgeScreenのUIDocument + JudgeScreenView をUIViewHubの子に配置（未配置なら）

### 演出
- [ ] BGM/SE（既存システム利用）

### 未確認事項
- OrderScreenの提供履歴の〇×表示が正しく動作するか未確認
- CustomerHoverManagerのhoverRadius=1fが適切か未調整

---

## ゲームフロー全体像

```
タイトル → フェーズ1（観察） → タイムアップ → フェーズ2（メモ整理）
  ├─「注文に進む」→ フェーズ3（メニューから注文一覧を組んで一括判定）
  │   ├─ 全品セーフ＆100点未満 → NovelPopup「まだまだね」→ JudgeScreenで追加注文
  │   ├─ 全品セーフ＆100点以上 → NovelPopup「裏メニュー教えてあげる」→ タイトルへ
  │   └─ ドボン含む → NovelPopup「帰ってちょうだい！」→ タイトルへ
  └─「もう一日調べる」→ メモ引き継ぎ → フェーズ1（タイマーリセット）
```

---

## 関連資料
- 客AI設計 → `AI_DESIGN.md`
- ゲーム仕様 → `GAME_DESIGN.md`
- 設計原則 → `DESIGN_NOTES.md`
- API詳細 → `API_REFERENCE.md`
