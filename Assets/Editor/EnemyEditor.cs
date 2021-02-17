using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
public class EnemyEditor : EditorWindow//敵エディタ
{

    static string AssetFileName = "/AssetObj/EnemyrAsset";
    static string AssetListFileName = "/AssetObj/AsstList";
    static string CharaListDataPath = "/AssetObj/AsstList/EnemyDataList.asset";//リストの保存先のパス
    static EnemyList EnemyList = null;
    [MenuItem("Editor/EnemyEditor")]
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
            Debug.Log("EnemyrAssetフォルダー作成");
            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }


        if (!Directory.Exists(Application.dataPath + AssetListFileName))
        {
            Directory.CreateDirectory(Application.dataPath + AssetListFileName);
            Debug.Log("AsstListフォルダー作成");
            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }

        if (!File.Exists(Application.dataPath + CharaListDataPath))
        {
            AssetDatabase.CreateAsset(new EnemyList(), "Assets" + CharaListDataPath);//ない場合作成
            EnemyList = (EnemyList)AssetDatabase.LoadAssetAtPath("Assets" + CharaListDataPath, typeof(EnemyList));//ある場合データファイルロード
        }
        else
        {
            EnemyList = (EnemyList)AssetDatabase.LoadAssetAtPath("Assets" + CharaListDataPath, typeof(EnemyList));//ある場合データファイルロード
        }

        GetWindow<EnemyEditor>("EnemyEditor"); // タイトル名を指定
    }

    EnemyData[] EnemyDataList = new EnemyData[0];
    Vector2 v2 = Vector2.zero;
    Vector2 LehtBox = Vector2.zero;
    Vector2 RigitBox = Vector2.zero;
    int SelctNo = -1;
    int SetNo;

    //変更データ
    public int EnemyNo;//敵番号
    public string EnemyName;//敵名前
    public int OffensiveP;//攻撃力 
    public int HP;//体力
    public EnemyType Type;//敵攻撃タイプ
    public string ExplanatoryText;//説明文

    private void OnGUI()
    {
        //以下武器データ取得処理
        DirectoryInfo dir = new DirectoryInfo("Assets" + AssetFileName);//ファイルパス
        FileInfo[] fild = dir.GetFiles("*.asset");//こいつでファイル名を取得
        if (EnemyDataList.Length != fild.Length)
        {
            EnemyDataList = new EnemyData[fild.Length];
            for (int i = 0; i < fild.Length; i++)
            {
                string path = dir + "/" + fild[i].Name;
                string WeaponName = fild[i].Name.Substring(0, fild[i].Name.Length - 6);

                EnemyDataList[i] = (EnemyData)AssetDatabase.LoadAssetAtPath(path, typeof(EnemyData));
            }
        }
        //ここまで

        using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))//左枠
            {
                if (EnemyDataList.Length != 0)
                {
                    EditorGUILayout.LabelField("敵選択");
                    v2 = EditorGUILayout.BeginScrollView(v2, GUI.skin.box);
                    {
                        for (int i = 0; i < EnemyDataList.Length; i++)
                        {
                            if (GUILayout.Button(EnemyDataList[i].EnemyName))
                            {
                                if (i != SetNo) SetNo = i;
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.LabelField("No Data");
                    EnemyNo = 0;
                    EnemyName = "";
                }
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))//データ指定処理
            {
                RigitBox = EditorGUILayout.BeginScrollView(RigitBox, GUI.skin.box);
                {
                    EditorGUI.BeginDisabledGroup(EnemyDataList.Length == 0);
                    if (EnemyDataList.Length != 0) EditorGUILayout.LabelField("敵データ変更 : " + EnemyDataList[SetNo].EnemyName);
                    else EditorGUILayout.LabelField("敵データ変更");

                    EnemyName = EditorGUILayout.TextField("敵名前", EnemyName);
                    EnemyNo = EditorGUILayout.IntField("敵番号", EnemyNo);
                    OffensiveP = EditorGUILayout.IntField("攻撃力", OffensiveP);
                    HP = EditorGUILayout.IntField("体力", HP);
                    Type = (EnemyType)EditorGUILayout.EnumPopup("攻撃タイプ", Type);
                    ExplanatoryText = EditorGUILayout.TextField("説明文", ExplanatoryText);
                    EditorGUI.EndDisabledGroup();
                }
                


                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                {
                    bool DataWrong = true;//データ変更ありか

                    if (EnemyDataList.Length != 0)
                    {
                        if (SetNo != SelctNo)//選択データ変更
                        {
                            SelctNo = SetNo;
                            DataRset();
                        }

                        if (EnemyDataList[SelctNo].EnemyName != EnemyName || EnemyDataList[SelctNo].EnemyNo != EnemyNo ||
                        EnemyDataList[SelctNo].HP != HP || EnemyDataList[SelctNo].Type != Type || EnemyDataList[SelctNo].ExplanatoryText != ExplanatoryText) DataWrong = false;

                        EditorGUI.BeginDisabledGroup(DataWrong);
                        if (GUILayout.Button("Reset"))
                        {
                            DataRset();
                        }

                        if (GUILayout.Button("Save"))
                        {
                            EnemyDataList[SelctNo].EnemyName = EnemyName;
                            EnemyDataList[SelctNo].EnemyNo = EnemyNo;
                            EnemyDataList[SelctNo].HP = HP;
                            EnemyDataList[SelctNo].Type = Type;
                            EnemyDataList[SelctNo].ExplanatoryText = ExplanatoryText;
                            DataRset();
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                }
                   
                EditorGUILayout.EndScrollView();
            }     
        }

        var e = Event.current;
        switch (e.type)
        {
            case EventType.ContextClick://右クリックメニュー
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("敵データ追加"), false, () => { File_Set(); });
                menu.ShowAsContext();
                break;
        }
    }

    void File_Set()//ファイル追加処理
    {
        string DefName = "NewEnemyData.asset";//初期値の名前
        DefName = Path.GetFileNameWithoutExtension(AssetDatabase.GenerateUniqueAssetPath(Path.Combine("Assets" + AssetFileName, DefName)));//同じ名前のものがあるかを判定
        var pas = EditorUtility.SaveFilePanelInProject("敵データ追加", DefName, "asset", "", "Assets" + AssetFileName);
        if (!string.IsNullOrEmpty(pas))//保存処理
        {
            string[] name1 = pas.Split('/');
            string WeaponName = name1[name1.Length - 1].Substring(0, name1[name1.Length - 1].Length - 6);
            EnemyData Savedata = new EnemyData();
            Savedata.EnemyName = WeaponName;//ファイル名を代入
            Savedata.EnemyNo = EnemyDataList.Length + 1;
            AssetDatabase.CreateAsset(Savedata, pas);
            AssetDatabase.Refresh();
        }
    }

    void DataRset()
    {
        EnemyName = EnemyDataList[SelctNo].EnemyName;
        EnemyNo = EnemyDataList[SelctNo].EnemyNo;
        HP = EnemyDataList[SelctNo].HP;
        Type = EnemyDataList[SelctNo].Type;
        ExplanatoryText = EnemyDataList[SelctNo].ExplanatoryText;
    }


    private void OnDestroy()
    {
        EnemyList.DataList = EnemyDataList;
    }
}
