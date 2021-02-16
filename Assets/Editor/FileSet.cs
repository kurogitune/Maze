using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;//こいつをかくことでEditorでシーンを切り替え可能にする
using System.IO;


public class FileSet : EditorWindow//初期フォルダー作成エディタ
{
    static bool Error;//エラーか
    [MenuItem("Editor/FolderSet")]
    [System.Obsolete]
    static void Open()
    {
        try
        {
            FolderSet("Scripts");
            FolderSet("Model");
            FolderSet("Audio");
            FolderSet("Audio/SE");
            FolderSet("Audio/BGM");
            FolderSet("Animation");
            FolderSet("System");
            FolderSet("Resources");
            FolderSet("Material");
            FolderSet("Image");

            if (!FolderExist("Scenes"))
            {
                FolderSet("Scenes");
                SeenSet();
            }
            else SeenSet();  //必要最低限のSceneを作成

            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }
        catch
        {
            Error = true;
        }
        GetWindow<FileSet>();
    }

    [System.Obsolete]
    static void SeenSet()//シーン作成
    {
        if (!FileExist("Scenes/Title",FileType.unity))
        {
            EditorApplication.SaveScene(Application.dataPath + "/Scenes/Title.unity");//こいつでScenesを作製できる
            var Scenes = EditorBuildSettings.scenes;//ビルドシーンを取得
            ArrayUtility.Add(ref Scenes, new EditorBuildSettingsScene("Assets/Scenes/Title.unity", true));//ビルドシーンに指定シーンを差し込み
            EditorBuildSettings.scenes = Scenes;
        }

        if (!FileExist("Scenes/Game", FileType.unity))
        {
            EditorApplication.SaveScene(Application.dataPath + "/Scenes/Game.unity");
            var Scenes = EditorBuildSettings.scenes;
            ArrayUtility.Add(ref Scenes, new EditorBuildSettingsScene("Assets/Scenes/Game.unity", true));
            EditorBuildSettings.scenes = Scenes;
        }

        if (!FileExist("Scenes/End", FileType.unity))
        {
            EditorApplication.SaveScene(Application.dataPath + "/Scenes/End.unity");
            var Scenes = EditorBuildSettings.scenes;//ビルドシーンを取得
            ArrayUtility.Add(ref Scenes, new EditorBuildSettingsScene("Assets/Scenes/End.unity", true));//ビルドシーンに指定シーンを差し込み
            EditorBuildSettings.scenes = Scenes;
        }

        if (!FileExist("Scenes/Test", FileType.unity))
        {
            EditorApplication.SaveScene(Application.dataPath + "/Scenes/Test.unity");
        }

        EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/Title.unity");//指定したシーンを開く
    }

    static void FolderSet(string FilePath)//フォルダーを作成
    {
        if (!FolderExist(FilePath))
        {
            Directory.CreateDirectory(Application.dataPath +"/"+ FilePath);
        }
    }

    static bool FolderExist(string FilePath)//フォルダーが存在するか
    {
        return Directory.Exists(Application.dataPath + "/"+FilePath);
    }

    static bool FileExist(string FilePath,FileType Type)//指定Fileがあるかどうか
    {
        string Extension = "";
        switch (Type)
        {
            case FileType.unity:
                Extension = ".unity";
                break;
            
            case FileType.asset:
                Extension = ".asset";
                break;
        }
        return File.Exists(Application.dataPath +"/"+ FilePath+ Extension);
    }

    enum FileType//探すファイルタイプ
    {
        unity,
        asset
    }

    private void OnGUI()
    {
        if (Error) Debug.Log("フォルダー作成失敗");
        else Debug.Log("フォルダー作成成功");
        Close();//こいつで自分のエディタを閉れる
    }
}
