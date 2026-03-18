#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameJam_URA
{
    //実行を確認したら関数の中のコードは消す。

    public class EditorSandbox
    {
        //ファイルを更新したらMenuItemに現在の日本時間を入れる
        [UnityEditor.MenuItem("GameJam/Run Sandbox (03-18 17:30)")]
        static void Run()
        { }
    }
}
#endif
