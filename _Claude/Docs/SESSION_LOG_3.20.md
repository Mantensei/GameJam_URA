# 実装ルーム セッションログ（2026-03-20）

## このルームの制約

- 既存 `_Claude/Scripts/` のコードはレガシーなので参照不要
- 一気に何個も作らず、個別に一つ一つ作る
- 指示を受けていない場合は何もしない。すべてのタスクは指示を受けてから行う
- GAME_DESIGN.md は変更箇所が多いので軽く読む程度で良い
- ユーザーへの質問は絶対にAskUserQuestionで行え、質問の答えを手打ちさせるな

---

## 完了した作業

### CustomerData SOスクリプト参照エラーの修正
- 原因：`CustomerTable.cs`に`CustomerData`と`CustomerTable`の2つのSOクラスが同居していた
- Unityはファイル名と不一致のSOクラスのシリアライゼーション参照が不安定になる
- 対処：`CustomerData`を`CustomerData.cs`に分離
- CLAUDE.mdに「MonoBehaviour/ScriptableObjectは1ファイル1クラス」ルールを明文化

### INorma → IUraTask リネーム
- `INorma` → `IUraTask`、`CompleteNorma()` → `Complete()`、変数名`norma` → `task`
- `INormaProvider` → `IUraTaskProvider`、`GetAllNormas()` → `GetAllTasks()`
- 型名は`IUraTask`（System.Threading.Taskとの衝突回避）、変数名は`task`（Uraを付けない）
- IUraTask自体はノルマ/ドボンの役割を持たない。StageData側のリストで振り分ける方式
- 影響ファイル：Norma.cs, MenuItem.cs, MenuNorma.cs, NormaData.cs, StageData.cs, MenuView.cs, RegisterView.cs, Commenter.cs, CustomerAI.cs, StageDebugPanel.cs

### データのCSV外部化（ステップ3c完了）

#### 方針変更
- ファイル形式：TSV → CSV（`.tsv`はUnityのResources.Load<TextAsset>が非対応のため）
- 全ステージ1ファイル統合（`menu.csv`にstage_idカラムで区分）
- SerializeField不要（Resources.Loadで直接読み込み）
- メニュータスクはCSVから一律生成し、StageDataのnormaCount/dobonCountでシャッフル後に振り分け

#### 実装済み
1. **CsvParser**（`MantenseiLib/Core/Utilities/CsvParser.cs`）
   - TsvParserと同構造、区切り文字をカンマに変更
   - `CsvParser.Load(resourcePath)` / `CsvParser.Parse(TextAsset)` / `CsvRow`

2. **menu.csv**（`Resources/Stages/Tables/menu.csv`）
   - カラム：stage_id, name, price, category
   - stage0（旧tutorial）とstage1のメニュー各8品

3. **StageData.Init() 改修**
   - `LoadMenuItems()` — CSVからstage_idでフィルタしてMenuItem生成
   - シャッフル後、先頭normaCount個→ノルマ、次dobonCount個→ドボン、残り→ダミー
   - `BuildNonMenuTasks()` — 非メニュー系タスク（CommentNorma等）はNormaTable SOから取得
   - `menuCount` SerializeField削除

4. **DataNormalizer**（`_Claude/Scripts/Editor/DataNormalizer.cs`）
   - メニュー：GameJam → データ正規化
   - menu.csvをステージ順→カテゴリ順（メイン→サイド→ドリンク）→値段順にソート
   - 今後のデータ正規化処理は全てここに追加する方針
---

## 未実装（次回のステップ）
- [ ] **ステップ3d：旧SO群の廃止** — MenuNorma SO、NormaTable等の完全廃止
- [ ] **CommentのCSV化** — StubCommentsのハードコードをCSVに移行
- [ ] **stage0のデータ参照** — stage0がstage1のデータを直接参照する仕組み（重複データ解消）

## 設計メモ
- UnityのResources.Load<TextAsset>は .txt, .html, .xml, .json, .csv, .yaml 等を認識。.tsv は非対応
- `GameJam_URA.MenuItem`（データクラス）と`UnityEditor.MenuItem`（属性）が名前衝突する。エディタコードでは完全修飾が必要

## 関連資料
- 客AI設計 → `AI_DESIGN.md`
- ゲーム仕様 → `GAME_DESIGN.md`
- 設計原則 → `DESIGN_NOTES.md`
