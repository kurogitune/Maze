using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Object = UnityEngine.Object;//Objectの定義を指定

public class Weapo_Editor : EditorWindow//武器エディタ
{
    static string AssetFileName = "/AssetObj/WeapoAsset";//武器アセットオブジェクトの保存先フォルフダー
    static string AssetListFileName = "/AssetObj/AsstList";
    static string WeaponListDataPath = "/AssetObj/AsstList/WeapoDataList.asset";//リストの保存先のパス
    static WeponList WeponList = null;
    [MenuItem("Editor/WeapoEditor %#Z")]
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
            Debug.Log("WeapoAssetフォルダー作成");
            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }

        if (!Directory.Exists(Application.dataPath + AssetListFileName))
        {
            Directory.CreateDirectory(Application.dataPath + AssetListFileName);
            Debug.Log("AsstListフォルダー作成");
            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }

        if (!File.Exists(Application.dataPath + WeaponListDataPath))
        {
            AssetDatabase.CreateAsset(new WeponList(), "Assets" + WeaponListDataPath);//ない場合作成
            WeponList = (WeponList)AssetDatabase.LoadAssetAtPath("Assets" + WeaponListDataPath, typeof(WeponList));//ある場合データファイルロード
        }
        else
        {
            WeponList = (WeponList)AssetDatabase.LoadAssetAtPath("Assets" + WeaponListDataPath, typeof(WeponList));//ある場合データファイルロード
        }
        GetWindow<Weapo_Editor>("WeapoEditor"); // タイトル名を指定
    }


    int SetNo;//選択アイテム番号　
    int SelctitemNo = -1;//セレクト番号

    //こいつらは処理用
    string WeaponName = "";//武器名前
    int WeapoNo;//武器番号
    bool Canibuy;//買えるか
    int Buymoney;//買う金額
    bool CaniSell;//売れるか
    int Sellmoney;//買取金額
    int OffensiveP;//攻撃力 
    int DefenseP;//防御力
    int LoadingBullet;//残弾数
    WeponType Type;//武器タイプ
    string ExplanatoryText;//説明文

    //左表示関連
    WeaponData[] WeaponDataList=new WeaponData[0];//データ
    Vector2 LehtBox = Vector2.zero;
    Vector2 RigitBox = Vector2.zero;
    private void OnGUI()
    {
        //以下武器データ取得処理
        DirectoryInfo dir = new DirectoryInfo("Assets" + AssetFileName);//ファイルパス
        FileInfo[] fild = dir.GetFiles("*.asset");//こいつでファイル名を取得
        if (WeaponDataList.Length != fild.Length)
        {
            WeaponDataList = new WeaponData[fild.Length];
           for(int i=0;i<fild.Length ; i++)
           {
                string path = dir + "/" +fild[i].Name;
                string WeaponName = fild[i].Name.Substring(0, fild[i].Name.Length - 6);
                WeaponDataList[i] = (WeaponData) AssetDatabase.LoadAssetAtPath(path,typeof(WeaponData));
           }         
        }
        //ここまで

        using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
        {
            //以下データ選択表示処理
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                if (WeaponDataList.Length != 0)
                {
                    EditorGUILayout.LabelField("武器選択");
                    EditorGUILayout.LabelField("現在の武器数 : " + WeaponDataList.Length);

                    LehtBox = EditorGUILayout.BeginScrollView(LehtBox, GUI.skin.box);//選択枠
                    {
                        for (int i = 0; i < WeaponDataList.Length; i++)
                        {
                            if (GUILayout.Button(WeaponDataList[i].WeaponName))
                            {
                                if (i != SetNo) SetNo = i;
                            }
                            GUI.backgroundColor = GUI.color;
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.LabelField("NoData");
                    WeaponName = "";
                    WeapoNo = 0;

                }
            }
            //ここまで

            //以下データ変更処理
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))//ここから縦並び
            {
               RigitBox = EditorGUILayout.BeginScrollView(RigitBox, GUI.skin.box);//選択枠
              { 

                EditorGUI.BeginDisabledGroup(WeaponDataList.Length == 0);//こいつで囲んだボタンをおせなくする
                if(WeaponDataList.Length!=0) EditorGUILayout.LabelField("武器データ変更 : "+ WeaponDataList[SetNo].WeaponName);
                else EditorGUILayout.LabelField("武器データ変更");

                    WeaponName = EditorGUILayout.TextField("武器名前", WeaponName);
                    WeapoNo = EditorGUILayout.IntField("武器番号変更",WeapoNo);

                    Canibuy = EditorGUILayout.Toggle("買えるか",Canibuy);
                    EditorGUI.BeginDisabledGroup(!Canibuy);
                    Buymoney = EditorGUILayout.IntField("販売価格",Buymoney);
                    if (!Canibuy) Buymoney = 0;
                    EditorGUI.EndDisabledGroup();

                    CaniSell = EditorGUILayout.Toggle("売れるか", CaniSell);
                    EditorGUI.BeginDisabledGroup(!CaniSell);
                    Sellmoney = EditorGUILayout.IntField("買取価格", Sellmoney);
                    if (!CaniSell) Sellmoney = 0;
                    EditorGUI.EndDisabledGroup();

                    Type =(WeponType) EditorGUILayout.EnumPopup("装備タイプ",Type);
                    switch (Type)
                    {
                        case WeponType.Nodata:
                            EditorGUILayout.LabelField("設定項目なし");
                            LoadingBullet = DefenseP = OffensiveP = 0;
                            break;

                        case WeponType.Armor://防具
                            LoadingBullet = OffensiveP = 0;
                            DefenseP = EditorGUILayout.IntField("防御力",DefenseP);
                            break;

                        case WeponType.LongFistance://遠距離
                            DefenseP = 0;
                            OffensiveP = EditorGUILayout.IntField("攻撃力", OffensiveP);
                            LoadingBullet = EditorGUILayout.IntField("装填数",LoadingBullet);
                            break;
                        case WeponType.ShortDistance://近距離
                            LoadingBullet = DefenseP = 0;
                            OffensiveP = EditorGUILayout.IntField("攻撃力", OffensiveP);
                            break;
                    }

                    ExplanatoryText = EditorGUILayout.TextField("説明文",ExplanatoryText);
              }
              EditorGUI.EndDisabledGroup();//ここまで


              if (WeaponDataList.Length != 0)//代入先がある場合
              {
                  if (SetNo != SelctitemNo)//代入先が変更されたら
                  {
                      SelctitemNo = SetNo;
                      DataReset();                     
                  }

                  using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                  {
                      bool Data_change = true;

                        if (WeaponDataList[SelctitemNo].WeponNo != WeapoNo || WeaponDataList[SelctitemNo].WeaponName != WeaponName
                           || WeaponDataList[SelctitemNo].Canibuy != Canibuy || WeaponDataList[SelctitemNo].Buymoney != Buymoney 
                           || WeaponDataList[SelctitemNo].OffensiveP != OffensiveP || WeaponDataList[SelctitemNo].DefenseP != DefenseP
                           || WeaponDataList[SelctitemNo].Type != Type || WeaponDataList[SelctitemNo].LoadingBullet != LoadingBullet
                           || WeaponDataList[SelctitemNo].ExplanatoryText != ExplanatoryText || WeaponDataList[SelctitemNo].CaniSell != CaniSell
                           || WeaponDataList[SelctitemNo].Sellmoney != Sellmoney) Data_change = false;//データが更新されたか

                        EditorGUI.BeginDisabledGroup(Data_change);//こいつで囲んだボタンをおせなくする
                        if (GUILayout.Button("Reset"))
                        {
                            DataReset();
                        }

                        if (GUILayout.Button("Save"))
                        {
                          //データ保存
                            WeaponDataList[SelctitemNo].WeponNo=WeapoNo;
                            WeaponDataList[SelctitemNo].WeaponName= WeaponName;
                            WeaponDataList[SelctitemNo].Canibuy= Canibuy;
                            WeaponDataList[SelctitemNo].Buymoney = Buymoney;
                            WeaponDataList[SelctitemNo].CaniSell = CaniSell;
                            WeaponDataList[SelctitemNo].Sellmoney = Sellmoney;
                            WeaponDataList[SelctitemNo].OffensiveP = OffensiveP;
                            WeaponDataList[SelctitemNo].DefenseP = DefenseP;
                            WeaponDataList[SelctitemNo].LoadingBullet= LoadingBullet;
                            WeaponDataList[SelctitemNo].Type = Type;
                            WeaponDataList[SelctitemNo].ExplanatoryText = ExplanatoryText;

                            EditorUtility.SetDirty(WeaponDataList[SelctitemNo]);//指定したScriptObject変更を記録
                            AssetDatabase.SaveAssets();//ScriptObjectをセーブする
                            DataReset();
                        }
                        EditorGUI.EndDisabledGroup();//ここまで
                  }
              }
              EditorGUILayout.EndScrollView();
              
            }//ここまで
        }

        var e = Event.current;
        switch (e.type)
        {
            case EventType.ContextClick://右クリックメニュー
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("武器を追加"), false, () => { File_Set(); });
                menu.ShowAsContext();
                break;
        }
    }

    void DataReset()//一時保存データ初期化
    {
        WeapoNo = WeaponDataList[SelctitemNo].WeponNo;
        WeaponName = WeaponDataList[SelctitemNo].WeaponName;
        Canibuy = WeaponDataList[SelctitemNo].Canibuy;
        Buymoney = WeaponDataList[SelctitemNo].Buymoney;
        CaniSell = WeaponDataList[SelctitemNo].CaniSell;
        Sellmoney = WeaponDataList[SelctitemNo].Sellmoney;
        OffensiveP = WeaponDataList[SelctitemNo].OffensiveP;
        DefenseP = WeaponDataList[SelctitemNo].DefenseP;
        LoadingBullet = WeaponDataList[SelctitemNo].LoadingBullet;
        Type = WeaponDataList[SelctitemNo].Type;
        ExplanatoryText = WeaponDataList[SelctitemNo].ExplanatoryText;
    }

    void File_Set()//ファイル追加処理
    {
        string DefName = "NewWeponData.asset";//初期値の名前
        DefName = Path.GetFileNameWithoutExtension(AssetDatabase.GenerateUniqueAssetPath(Path.Combine("Assets"+ AssetFileName,DefName)));//同じ名前のものがあるかを判定
        var pas = EditorUtility.SaveFilePanelInProject("武器を追加",DefName,"asset","", "Assets" + AssetFileName);
        if (!string.IsNullOrEmpty(pas))//保存処理
        {
            string[] name1 = pas.Split('/');
            string WeaponName = name1[name1.Length - 1].Substring(0, name1[name1.Length - 1].Length - 6);
            WeaponData Savedata = new WeaponData();
            Savedata.WeaponName = WeaponName;//ファイル名を代入
            Savedata.WeponNo = WeaponDataList.Length + 1;

            AssetDatabase.CreateAsset(Savedata, pas);
            AssetDatabase.Refresh();
        }
    }

    private void OnDestroy()
    {
        WeponList.DataList = WeaponDataList;
        EditorUtility.SetDirty(WeponList);//指定したScriptObject変更を記録
        AssetDatabase.SaveAssets();//ScriptObjectをセーブする
    }
}
