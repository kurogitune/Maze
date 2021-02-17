using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Item_Editor : EditorWindow
{
    static string AssetFileName = "/AssetObj/ItemAsset";//武器アセットオブジェクトの保存先フォルフダー
    static string AssetListFileName = "/AssetObj/AsstList";//データリスト保存フォルダーパス
    static string ItemListDataPath = "/AssetObj/AsstList/ItemDataList.asset";//リストの保存先のパス
    static ItemList ItemList = null;
    [MenuItem("Editor/Itemditor %#V")]
    static void Open()
    {
        if (!Directory.Exists(Application.dataPath + "/AssetObj"))//フォルダー確認
        {
            Debug.Log("AssetObjフォルダー作成");
            Directory.CreateDirectory(Application.dataPath + "/AssetObj");//ない場合作成
            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }

        if(!Directory.Exists(Application.dataPath+ AssetFileName))
        {
            Directory.CreateDirectory(Application.dataPath + AssetFileName);
            Debug.Log("ItemAssetフォルダー作成");
            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }

        if (!Directory.Exists(Application.dataPath + AssetListFileName))
        {
            Directory.CreateDirectory(Application.dataPath + AssetListFileName);
            Debug.Log("AsstListフォルダー作成");
            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }

        if (!File.Exists(Application.dataPath+ ItemListDataPath))
        {
            AssetDatabase.CreateAsset(new ItemList(), "Assets" + ItemListDataPath);//ない場合作成
            ItemList = (ItemList)AssetDatabase.LoadAssetAtPath("Assets" + ItemListDataPath, typeof(ItemList));//ある場合データファイルロード
        }
        else
        {
            ItemList = (ItemList)AssetDatabase.LoadAssetAtPath("Assets" + ItemListDataPath, typeof(ItemList));//ある場合データファイルロード
        }

        GetWindow<Item_Editor>("ItemEditor"); // タイトル名を指定
    }

    int SetNo, SelctitemNo = -1;//武器番号　　選択アイテム番号　セレクト番号
 
    //こいつらは処理用
    int ItemNo;//アイテム番号
    string ItemName;//アイテム名前
    bool Canibuy;//買えるか
    int Buymoney;//買う金額
    bool CaniSell;//売れるか
    int Sellmoney;//買取金額
    float RecoveryP;//回復量
    ItemType Type;
    string ExplanatoryText;//説明文

    //左表示関連
    ItemData[] ItemDataList = new ItemData[0];//データ
    Vector2 LehtBox = Vector2.zero;
    Vector2 RigitBox = Vector2.zero;
    private void OnGUI()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets" + AssetFileName);//ファイルパス
        FileInfo[] fild = dir.GetFiles("*.asset");//こいつでファイル名を取得
        if (ItemDataList.Length != fild.Length)
        {
            ItemDataList = new ItemData[fild.Length];
            for (int i = 0; i < fild.Length; i++)
            {
                string path = dir + "/" + fild[i].Name;
                string WeaponName = fild[i].Name.Substring(0, fild[i].Name.Length - 6);
                ItemDataList[i] = (ItemData)AssetDatabase.LoadAssetAtPath(path, typeof(ItemData));
            }
        }
        //ここまで

        using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
        {
            //以下データ選択表示処理
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                if (ItemDataList.Length != 0)
                {
                    EditorGUILayout.LabelField("アイテム選択");
                    EditorGUILayout.LabelField("現在のアイテム数 : " + ItemDataList.Length);

                    LehtBox = EditorGUILayout.BeginScrollView(LehtBox, GUI.skin.box);//選択枠
                    {
                        for (int i = 0; i < ItemDataList.Length; i++)
                        {
                            if (GUILayout.Button(ItemDataList[i].ItemName))
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
                    ItemName = "";
                    ItemNo = 0;

                }
            }
            //ここまで

            //以下データ変更処理
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))//ここから縦並び
            {
                RigitBox = EditorGUILayout.BeginScrollView(RigitBox, GUI.skin.box);//選択枠
                {

                    EditorGUI.BeginDisabledGroup(ItemDataList.Length == 0);//こいつで囲んだボタンをおせなくする
                    if (ItemDataList.Length != 0) EditorGUILayout.LabelField("アイテムデータ変更 : " + ItemDataList[SetNo].ItemName);
                    else EditorGUILayout.LabelField("アイテムデータ変更");

                    ItemName = EditorGUILayout.TextField("アイテムアイテム名前",ItemName);
                    ItemNo = EditorGUILayout.IntField("アイテム番号",ItemNo);

                    Canibuy = EditorGUILayout.Toggle("買えるか", Canibuy);
                    EditorGUI.BeginDisabledGroup(!Canibuy);
                    Buymoney = EditorGUILayout.IntField("販売価格", Buymoney);
                    if (!Canibuy) Buymoney = 0;
                    EditorGUI.EndDisabledGroup();

                    CaniSell = EditorGUILayout.Toggle("売れるか", CaniSell);
                    EditorGUI.BeginDisabledGroup(!CaniSell);
                    Sellmoney = EditorGUILayout.IntField("買取価格", Sellmoney);
                    if (!CaniSell) Sellmoney = 0;
                    EditorGUI.EndDisabledGroup();
                    
                    Type = (ItemType)EditorGUILayout.EnumPopup("アイテムタイプ",Type);

                    switch (Type)
                    {
                        case ItemType.Nodata:
                            RecoveryP = 0;
                            EditorGUILayout.LabelField("設定項目なし");
                            break;
                        case ItemType.Recovery:
                           RecoveryP=EditorGUILayout.FloatField("回復量",RecoveryP);
                            break;
                        case ItemType.Other:
                            RecoveryP = 0;
                            EditorGUILayout.LabelField("設定項目現段階なし");
                            break;
                    }
                    ExplanatoryText = EditorGUILayout.TextField("説明文", ExplanatoryText);
                }
                EditorGUI.EndDisabledGroup();//ここまで

                if (ItemDataList.Length != 0)//代入先がある場合
                {
                    if (SetNo != SelctitemNo)//代入先が変更されたら
                    {
                        SelctitemNo = SetNo;
                        DataReset();
                    }

                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                    {
                        bool Data_change = true;

                        if (ItemDataList[SelctitemNo].ItemNo != ItemNo|| ItemDataList[SelctitemNo].ItemName != ItemName
                            || ItemDataList[SelctitemNo].Canibuy != Canibuy || ItemDataList[SelctitemNo].Buymoney != Buymoney
                            || ItemDataList[SelctitemNo].RecoveryP != RecoveryP || ItemDataList[SelctitemNo].Type != Type
                            || ItemDataList[SelctitemNo].ExplanatoryText != ExplanatoryText || ItemDataList[SelctitemNo].CaniSell != CaniSell
                            || ItemDataList[SelctitemNo].Sellmoney != Sellmoney) Data_change = false;//データが更新されたか

                        EditorGUI.BeginDisabledGroup(Data_change);//こいつで囲んだボタンをおせなくする
                        if (GUILayout.Button("Reset"))
                        {
                            DataReset();
                        }

                        if (GUILayout.Button("Save"))
                        {
                            //データ保存
                            ItemDataList[SelctitemNo].ItemNo= ItemNo;
                            ItemDataList[SelctitemNo].ItemName= ItemName;
                            ItemDataList[SelctitemNo].Canibuy= Canibuy;
                            ItemDataList[SelctitemNo].Buymoney= Buymoney;
                            ItemDataList[SelctitemNo].CaniSell = CaniSell;
                            ItemDataList[SelctitemNo].Sellmoney = Sellmoney;
                            ItemDataList[SelctitemNo].RecoveryP= RecoveryP;
                            ItemDataList[SelctitemNo].Type= Type;
                            ItemDataList[SelctitemNo].ExplanatoryText= ExplanatoryText;
                            EditorUtility.SetDirty(ItemDataList[SelctitemNo]);//指定したScriptObject変更を記録
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
                menu.AddItem(new GUIContent("アイテムを追加"), false, () => { File_Set(); });
                menu.ShowAsContext();
                break;
        }
    }

    public void DataReset()//データを初期化
    {
        ItemNo = ItemDataList[SelctitemNo].ItemNo;
   　   ItemName= ItemDataList[SelctitemNo].ItemName;
        Canibuy = ItemDataList[SelctitemNo].Canibuy;
        Buymoney = ItemDataList[SelctitemNo].Buymoney;
        CaniSell = ItemDataList[SelctitemNo].CaniSell;
        Sellmoney = ItemDataList[SelctitemNo].Sellmoney;
        RecoveryP = ItemDataList[SelctitemNo].RecoveryP;
        Type = ItemDataList[SelctitemNo].Type;
        ExplanatoryText = ItemDataList[SelctitemNo].ExplanatoryText;
    }


    void File_Set()//ファイル追加処理
    {
        string DefName = "ItemData.asset";//初期値の名前
        DefName = Path.GetFileNameWithoutExtension(AssetDatabase.GenerateUniqueAssetPath(Path.Combine("Assets" + AssetFileName, DefName)));//同じ名前のものがあるかを判定
        var pas = EditorUtility.SaveFilePanelInProject("アイテムを追加", DefName, "asset", "", "Assets" + AssetFileName);
        if (!string.IsNullOrEmpty(pas))//保存処理
        {
            string[] name1 = pas.Split('/');
            string WeaponName = name1[name1.Length - 1].Substring(0, name1[name1.Length - 1].Length - 6);
            ItemData Savedata = new ItemData();
            Savedata.ItemName = WeaponName;//ファイル名を代入
            Savedata.ItemNo = ItemDataList.Length + 1;

            AssetDatabase.CreateAsset(Savedata, pas);
            AssetDatabase.Refresh();
        }
    }

    private void OnDestroy()
    {
        ItemList.DataList = ItemDataList;
    }
}
