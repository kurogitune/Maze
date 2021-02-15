using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Item_Editor : EditorWindow
{
    [MenuItem("Editor/Itemditor")]
    static void Open()
    {
        if (!Directory.Exists(Application.dataPath + "/AssetObj"))//フォルダー確認
        {
            Debug.Log("AssetObjフォルダー作成");
            Directory.CreateDirectory(Application.dataPath + "/AssetObj");//ない場合作成
            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }

        if(!Directory.Exists(Application.dataPath+ "/AssetObj/ItemAsset"))
        {
            Directory.CreateDirectory(Application.dataPath + "/AssetObj/ItemAsset");
            Debug.Log("ItemAssetフォルダー作成");
            AssetDatabase.Refresh();//Unityのファイル表示を更新  
        }

        GetWindow<Item_Editor>("ItemEditor"); // タイトル名を指定
    }


    Object obj;

    //こいつらは処理用
    Object Temporarilyobj;
    bool DataON, DataIN,NameIn;
    int WaponeItemData, WaponData;
    float HPRecoveryData;
    string Name;
    Itemtype ItemTypData;
    private void OnGUI()
    {

        EditorGUILayout.BeginHorizontal(GUI.skin.box);//ここから横並び
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);//ここから縦並び
            {
                obj = EditorGUILayout.ObjectField("アイテム", obj, typeof(ItemData), true);//オブジェクトを代入
                                                                                       //名前　Object　ファイル形式指定　なぜかtrue
                Name = EditorGUILayout.TextField("データ追加", Name);

                if (Name!=""&Event.current.type == EventType.KeyDown & Event.current.keyCode == KeyCode.Return)//Enterが押されたら処理
                {
                    ItemData data = CreateInstance<ItemData>();//Assetオブジェクトを作成
                    string path = AssetDatabase.GenerateUniqueAssetPath("Assets/AssetObj/ItemAsset/" + Name + ".asset");//作成したAssetオブジェクトの保存先
                    AssetDatabase.CreateAsset(data, path);//Assetオブジェクトを保存
                    Name = "";
                    AssetDatabase.Refresh();//Unityのファイル表示を更新  
                }
                EditorGUILayout.LabelField("追加方法　名前を入力してReturnを２回");
            }
            EditorGUILayout.EndVertical();//ここまで縦並び


            EditorGUILayout.BeginVertical(GUI.skin.box);//ここから縦並び
            {
                EditorGUILayout.LabelField("アイテムデータ変更");
                if (obj != null)//代入先がある場合
                {
                    ItemData data = (ItemData)obj;//強制的に変換(ここを使いたいデータに変更)
                    if (obj != Temporarilyobj)//代入先が変更されたら
                    {
                        //一時保存データ初期化
                        Temporarilyobj = obj;
                        DataReset(data);
                        ItemTypData = data.Type;
                    }

#pragma warning disable CS0618 // 型またはメンバーが古い形式です
                    ItemTypData = (Itemtype)EditorGUILayout.EnumPopup("アイテムタイプ", ItemTypData);
#pragma warning restore CS0618 // 型またはメンバーが古い形式です

                    if (data.Type != ItemTypData)//タイプを変更したらデータ全て初期化
                    {
                        Temporarilyobj = obj;
                        data.Type = ItemTypData;
                        data.HpRecovery = data.Weapon_ItemGet = data.Weapon_No = 0;
                        HPRecoveryData = data.HpRecovery;
                        WaponData = data.Weapon_No;
                        WaponeItemData = data.Weapon_ItemGet;
                        ItemTypData = data.Type;
                    }

                    switch (data.Type)
                    {
                        case Itemtype.HP_Recovery:
                            HPRecoveryData = EditorGUILayout.FloatField("回復量", HPRecoveryData);
                            break;

                        case Itemtype.Weapon_Get:
                            WaponData = EditorGUILayout.IntField("武器番号", WaponData);
                            break;

                        case Itemtype.Weapon_ItemGet:
                            WaponeItemData = EditorGUILayout.IntField("武器アイテム取得量", WaponeItemData);
                            break;
                    }

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {

                        bool Data_change = true;

                        if (Data_change&data.HpRecovery!=HPRecoveryData|| Data_change & data.Weapon_No != WaponData
                         || Data_change & data.Weapon_ItemGet != WaponeItemData) Data_change = false;//データが更新されたか

                        EditorGUI.BeginDisabledGroup(Data_change);//こいつで囲んだボタンをおせなくする
                        if (GUILayout.Button("Reset"))
                        {
                            //一時保存データ初期化
                            Temporarilyobj = obj;
                            DataReset(data);
                            ItemTypData = data.Type;
                        }

                        if (GUILayout.Button("Save"))
                        {
                            //データ保存

                            switch (data.Type)
                            {
                                case Itemtype.HP_Recovery:
                                    data.HpRecovery = HPRecoveryData;
                                    data.Weapon_ItemGet = data.Weapon_No = 0;
                                    break;

                                case Itemtype.Weapon_Get:
                                    data.Weapon_No = WaponData;
                                    data.HpRecovery = data.Weapon_ItemGet = 0;
                                    break;

                                case Itemtype.Weapon_ItemGet:
                                    data.Weapon_ItemGet = WaponeItemData;
                                    data.HpRecovery = data.Weapon_No = 0;
                                    break;
                            }
                            //一時保存データ初期化
                            Temporarilyobj = obj;
                            DataReset(data);
                            ItemTypData = data.Type;
                        }
                        EditorGUI.EndDisabledGroup();//ここまで
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();//ここまで縦並び
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    public void DataReset(ItemData data)
    {       
        switch (data.Type)
        {
            case Itemtype.HP_Recovery:
                data.Weapon_No = data.Weapon_ItemGet = 0;
                HPRecoveryData = data.HpRecovery;
                break;

            case Itemtype.Weapon_Get:
                data.HpRecovery = data.Weapon_ItemGet = 0;
                WaponData = data.Weapon_No;
                break;

            case Itemtype.Weapon_ItemGet:
                data.HpRecovery = data.Weapon_No = 0;
                WaponeItemData = data.Weapon_ItemGet;
                break;
        }
        AssetDatabase.Refresh();//Unityのファイル表示を更新  
    }
}
