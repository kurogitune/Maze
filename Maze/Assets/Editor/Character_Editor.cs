using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Character_Editor : EditorWindow
{
    static string AssetFileName = "/AssetObj/CharacterAsset";
    [MenuItem("Editor/CharaEditor %#X")]
    static void Open()
    {
        if (!Directory.Exists(Application.dataPath + "/AssetObj"))//フォルダー確認
        {
            Debug.Log("AssetObjフォルダー作成");
            Directory.CreateDirectory(Application.dataPath + "/AssetObj");//ない場合作成
            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }

        if (!Directory.Exists(Application.dataPath + AssetFileName))
        {
            Directory.CreateDirectory(Application.dataPath + AssetFileName);
            Debug.Log("CharacterAssetフォルダー作成");
            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }

        GetWindow<Character_Editor>("CharacterEditor"); // タイトル名を指定
    }

    CharacterData[] CharaDataList = new CharacterData[0];
    Vector2 v2 = Vector2.zero;
    Vector2 LehtBox = Vector2.zero;
    Vector2 RigitBox = Vector2.zero;
    int SelctNo = -1;
    int ItemNo;

    //変更データ
    string CharaName;//キャラクター名前
    int CharacterNo;//キャラクター番号

    private void OnGUI()
    {
        //以下武器データ取得処理
        DirectoryInfo dir = new DirectoryInfo("Assets" + AssetFileName);//ファイルパス
        FileInfo[] fild = dir.GetFiles("*.asset");//こいつでファイル名を取得
        if (CharaDataList.Length != fild.Length)
        {
            CharaDataList = new CharacterData[fild.Length];
            for (int i = 0; i < fild.Length; i++)
            {
                string path = dir + "/" + fild[i].Name;
                string WeaponName = fild[i].Name.Substring(0, fild[i].Name.Length - 6);

                CharaDataList[i] = (CharacterData)AssetDatabase.LoadAssetAtPath(path, typeof(CharacterData));
            }
        }
        //ここまで

        using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))//左枠
            {
                if (CharaDataList.Length != 0)
                {
                    EditorGUILayout.LabelField("キャラ選択");
                    v2 = EditorGUILayout.BeginScrollView(v2, GUI.skin.box);
                    {
                        for (int i = 0; i < CharaDataList.Length; i++)
                        {
                            if (GUILayout.Button(CharaDataList[i].CharacterName))
                            {
                                if (i != ItemNo) ItemNo = i;
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.LabelField("No Data");
                    CharacterNo = 0;
                    CharaName = "";
                }
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))//データ指定処理
            {
                RigitBox = EditorGUILayout.BeginScrollView(RigitBox, GUI.skin.box);

                EditorGUI.BeginDisabledGroup(CharaDataList.Length == 0);

            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
            {
                bool DataWrong = true;//データ変更ありか
                if (CharaDataList.Length != 0)
                {
                    if (ItemNo != SelctNo)//選択データ変更
                    {
                        SelctNo = ItemNo;
                        DataRset();
                    }

                    if (CharaDataList[SelctNo].CharacterName != CharaName) DataWrong = false;

                    EditorGUI.BeginDisabledGroup(DataWrong);
                    if (GUILayout.Button("Reset"))
                    {
                        DataRset();
                    }

                    if (GUILayout.Button("Save"))
                    {
                        DataRset();
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        var e = Event.current;
        switch (e.type)
        {
            case EventType.ContextClick://右クリックメニュー
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("キャラデータ追加"), false, () => { File_Set(); });
                menu.ShowAsContext();
                break;
        }
    }

    void File_Set()//ファイル追加処理
    {
        string DefName = "NewCharaData.asset";//初期値の名前
        DefName = Path.GetFileNameWithoutExtension(AssetDatabase.GenerateUniqueAssetPath(Path.Combine("Assets" + AssetFileName, DefName)));//同じ名前のものがあるかを判定
        var pas = EditorUtility.SaveFilePanelInProject("キャラデータ追加", DefName, "asset", "", "Assets" + AssetFileName);
        if (!string.IsNullOrEmpty(pas))//保存処理
        {
            string[] name1 = pas.Split('/');
            string WeaponName = name1[name1.Length - 1].Substring(0, name1[name1.Length - 1].Length - 6);
            CharacterData Savedata = new CharacterData();
            Savedata.CharacterName = WeaponName;//ファイル名を代入
            Savedata.CharacterNo = CharaDataList.Length +1;
            AssetDatabase.CreateAsset(Savedata, pas);
            AssetDatabase.Refresh();
        }
    }

    void DataRset()
    {

    }
}

