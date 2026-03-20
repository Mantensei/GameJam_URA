#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameJam_URA.Editor
{
    public static class DataNormalizer
    {
        const string MenuCsvPath = "Assets/_GameJam_URA/Resources/Stages/Tables/menu.csv";
        const string CommentCsvPath = "Assets/_GameJam_URA/Resources/Stages/Tables/comment.csv";

        static readonly string[] StageOrder = { "stage0", "stage1", "stage2", "stage3" };
        static readonly string[] CategoryOrder = { "メイン", "サイド", "ドリンク" };
        static readonly string[] PhaseOrder = { "BeforeEat", "AfterEat", "Any" };

        [UnityEditor.MenuItem("GameJam/データ正規化")]
        static void Normalize()
        {
            NormalizeMenuCsv();
            NormalizeCommentCsv();
            AssetDatabase.Refresh();
            Debug.Log("[データ正規化] 完了");
        }

        static void NormalizeMenuCsv()
        {
            if (!File.Exists(MenuCsvPath))
            {
                Debug.LogWarning($"[データ正規化] {MenuCsvPath} が見つかりません");
                return;
            }

            var lines = File.ReadAllLines(MenuCsvPath);
            if (lines.Length < 2) return;

            var header = lines[0];
            var rows = new List<string[]>();
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.Length == 0) continue;
                rows.Add(line.Split(','));
            }

            var headers = header.Split(',');
            int stageCol = Array.IndexOf(headers, "stage_id");
            int categoryCol = Array.IndexOf(headers, "category");
            int priceCol = Array.IndexOf(headers, "price");

            rows.Sort((a, b) =>
            {
                int stgA = Array.IndexOf(StageOrder, a[stageCol].Trim());
                int stgB = Array.IndexOf(StageOrder, b[stageCol].Trim());
                if (stgA < 0) stgA = StageOrder.Length;
                if (stgB < 0) stgB = StageOrder.Length;
                if (stgA != stgB) return stgA.CompareTo(stgB);

                int catA = Array.IndexOf(CategoryOrder, a[categoryCol].Trim());
                int catB = Array.IndexOf(CategoryOrder, b[categoryCol].Trim());
                if (catA < 0) catA = CategoryOrder.Length;
                if (catB < 0) catB = CategoryOrder.Length;
                if (catA != catB) return catA.CompareTo(catB);

                int.TryParse(a[priceCol].Trim(), out int priceA);
                int.TryParse(b[priceCol].Trim(), out int priceB);
                return priceA.CompareTo(priceB);
            });

            var output = new List<string> { header };
            output.AddRange(rows.Select(r => string.Join(",", r)));
            File.WriteAllText(MenuCsvPath, string.Join("\n", output) + "\n");

            Debug.Log($"[データ正規化] menu.csv: {rows.Count}行ソート完了（stage_id → カテゴリ → 値段）");
        }

        static void NormalizeCommentCsv()
        {
            if (!File.Exists(CommentCsvPath))
            {
                Debug.LogWarning($"[データ正規化] {CommentCsvPath} が見つかりません");
                return;
            }

            var lines = File.ReadAllLines(CommentCsvPath);
            if (lines.Length < 2) return;

            var header = lines[0];
            var rows = new List<string[]>();
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.Length == 0) continue;
                rows.Add(line.Split(','));
            }

            var headers = header.Split(',');
            int stageCol = Array.IndexOf(headers, "stage_id");
            int phaseCol = Array.IndexOf(headers, "phase");

            rows.Sort((a, b) =>
            {
                int stgA = Array.IndexOf(StageOrder, a[stageCol].Trim());
                int stgB = Array.IndexOf(StageOrder, b[stageCol].Trim());
                if (stgA < 0) stgA = StageOrder.Length;
                if (stgB < 0) stgB = StageOrder.Length;
                if (stgA != stgB) return stgA.CompareTo(stgB);

                int phA = Array.IndexOf(PhaseOrder, a[phaseCol].Trim());
                int phB = Array.IndexOf(PhaseOrder, b[phaseCol].Trim());
                if (phA < 0) phA = PhaseOrder.Length;
                if (phB < 0) phB = PhaseOrder.Length;
                return phA.CompareTo(phB);
            });

            var output = new List<string> { header };
            output.AddRange(rows.Select(r => string.Join(",", r)));
            File.WriteAllText(CommentCsvPath, string.Join("\n", output) + "\n");

            Debug.Log($"[データ正規化] comment.csv: {rows.Count}行ソート完了（stage_id → phase）");
        }
    }
}
#endif
