# 実装ルーム セッションログ（2026-03-20）

## このルームの制約

- 既存 `_Claude/Scripts/` のコードはレガシーなので参照不要
- 一気に何個も作らず、個別に一つ一つ作る
- 指示を受けていない場合は何もしない。すべてのタスクは指示を受けてから行う
- GAME_DESIGN.md は変更箇所が多いので軽く読む程度で良い
- ユーザーへの質問は絶対にAskUserQuestionで行え、質問の答えを手打ちさせるな

---

## TODO

### 好感度メーター制への移行（実装済み・未検証）
- [ ] Prefab上のCustomerAIをRemoveし、CustomerMineAIをアタッチする
- [ ] Unityで再生して動作確認（客の地雷フロー・好感度増減・クリア/ゲームオーバー判定）
- [ ] 好感度の増減値を調整（現在: 正解+10、地雷-25、初期50、クリア100、ゲームオーバー0）
- [ ] GAME_DESIGN.mdを新ルールに合わせて更新

### 変更済みファイル一覧
- `GameManager.cs` — 所持金→好感度（currentFavor, AddFavor, 初期値50）
- `CustomerMineAI.cs` — **新規**。地雷1品フロー（SitDown→Order→Eat→MineTrigger→Leave）
- `CustomerSpawner.cs` — BuildMineSchedule追加、StageData.MineCountで客数制御。旧メソッドはObsolete
- `RegisterView.cs` — 会計→好感度判定（正解+10/地雷-25、100クリア/0ゲームオーバー）
- `StageDebugPanel.cs` — 「残金」→「好感度」表示
- `StageData.cs` — mineCount SerializeField追加、CustomerBudget削除、dobonCountをmineCountで代替

### 未実装（前回からの引き継ぎ）
- [ ] ステップ3d：旧SO群の廃止（MenuNorma SO、NormaTable等）
- [ ] CommentのCSV化（StubCommentsのハードコードをCSVに移行）
- [ ] stage0のデータ参照（stage0がstage1のデータを直接参照する仕組み）

---

## 関連資料
- 客AI設計 → `AI_DESIGN.md`
- ゲーム仕様 → `GAME_DESIGN.md`
- 設計原則 → `DESIGN_NOTES.md`
