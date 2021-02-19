using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class FontSetEditor : EditorWindow
{
    [MenuItem("Editor/FontSetEditor")]
    static void Open()
    {
        GetWindow<FontSetEditor>();
    }

    Font Fon;
    [System.Obsolete]
    private void OnGUI()
    {
        Fon = (Font)EditorGUILayout.ObjectField("フォント選択",Fon,typeof(Font));

        EditorGUI.BeginDisabledGroup(!Fon);
        if (GUILayout.Button("フォント設定"))
        {
            foreach (GameObject boj in FindObjectsOfType(typeof(GameObject)))
            {
                if (boj.GetComponent<Text>())
                {
                    boj.GetComponent<Text>().font = Fon;
                    Undo.RegisterCompleteObjectUndo(boj, "TextUISet");
                }
            }
            EditorSceneManager.SaveOpenScenes();//こいつでシーンを保存
        }
        EditorGUI.EndDisabledGroup();
    }
}
