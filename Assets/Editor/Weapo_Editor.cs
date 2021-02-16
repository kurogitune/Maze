using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using Object = UnityEngine.Object;//Objectの定義を指定

public class Weapo_Editor : EditorWindow//武器エディタ
{

    static string AssetFileName = "/AssetObj/WeapoAsset";//武器アセットオブジェクトの保存先フォルフダー
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
        GetWindow<Weapo_Editor>("WeapoEditor"); // タイトル名を指定
    }


    //こいつらは処理用
    string WeapoName="";//武器名前
    int  ItemNo, SelctitemNo=-1;//武器番号　　選択アイテム番号　セレクト番号
    int WeapoNo;

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
                                if (i != ItemNo) ItemNo = i;
                            }
                            GUI.backgroundColor = GUI.color;
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.LabelField("NoData");
                    WeapoName = "";
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
                if(WeaponDataList.Length!=0) EditorGUILayout.LabelField("武器データ変更 : "+ WeaponDataList[ItemNo].WeaponName);
                else EditorGUILayout.LabelField("武器データ変更");

               
              }

                EditorGUI.EndDisabledGroup();//ここまで

                    if (WeaponDataList.Length != 0)//代入先がある場合
                    {
                        if (ItemNo != SelctitemNo)//代入先が変更されたら
                        {
                            SelctitemNo = ItemNo;
                            DataReset();                     
                        }

                        using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                        {
                            bool Data_change = true;

                            if (Data_change) Data_change = false;//データが更新されたか

                            EditorGUI.BeginDisabledGroup(Data_change);//こいつで囲んだボタンをおせなくする
                            if (GUILayout.Button("Reset"))
                            {
                                DataReset();
                            }

                            if (GUILayout.Button("Save"))
                            {
                                //データ保存
                               
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
}
