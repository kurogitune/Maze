using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;

public class ObjectSet : EditorWindow//シーンに初期オブジェクトを作成
{
    static bool Error;

    [MenuItem("Editor/ObjSet")]
    static void Open()
    {
        try
        {
            if (!GameObject.Find("Canvas")) //Canvas作成
            {
                GameObject Canva = new GameObject("Canvas");
                Canva.AddComponent<Canvas>();
                Canva.AddComponent<CanvasScaler>();
                Canva.AddComponent<GraphicRaycaster>();
                Canva.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                Canva.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                Canva.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
                Canva.layer = 5;
                Undo.RegisterCompleteObjectUndo(Canva,"CanvasSet");//こいつでオブジェクトを作成した履歴を作らないと保存できない
            }

            if (!GameObject.Find("EventSystem"))//EventSystem作成
            {
                GameObject Event = new GameObject("EventSystem");
                Event.AddComponent<EventSystem>();
                Event.AddComponent<StandaloneInputModule>();
                Undo.RegisterCompleteObjectUndo(Event, "EventSystemSet");
            }

            if (!GameObject.Find("System"))
            {
                GameObject Sys=   new GameObject("System");
                Undo.RegisterCompleteObjectUndo(Sys, "SystemSet");
            }

            EditorSceneManager.SaveOpenScenes();//こいつでシーンを保存
        }
        catch 
        {
            Error = true; 
        }

        GetWindow<ObjectSet>();
    }

    private void OnGUI()
    {
        if (Error) Debug.Log("作成失敗");
        else Debug.Log("作成成功");
        Close();//こいつで自分のエディタを閉れる
    }
}
