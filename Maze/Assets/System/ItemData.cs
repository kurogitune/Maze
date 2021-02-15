using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemdataSet")]//指定の場所にスクリプトオブジェクトを作成ボタンを追加
public class ItemData : ScriptableObject
{
    [Header("アイテム番号")]
    public int No;

    [Header("体力回復量")]
    public float HpRecovery;

    [Header("武器番号")]
    public int Weapon_No;

    [Header("武器アイテム取得量")]
    public int Weapon_ItemGet;

    [Header("アイテム種類")]
    public Itemtype Type;
}

public enum Itemtype//アイテムのタイプ指定
{
    DataNull,  //データなし

    HP_Recovery,//体力回復

    Weapon_Get,//武器ゲット

    Weapon_ItemGet//武器アイテム（矢など）ゲット
}
