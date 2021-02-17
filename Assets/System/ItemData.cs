using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemdataSet")]//指定の場所にスクリプトオブジェクトを作成ボタンを追加
public class ItemData : ScriptableObject
{
    public int ItemNo;
    public string ItemName;
    public bool Canibuy;//買えるか
    public int Buymoney;//買う金額
    public bool CaniSell;//売れるか
    public int Sellmoney;//買取金額
    public float RecoveryP;//回復量
    public ItemType Type;
    public string ExplanatoryText;//説明文
}

public enum ItemType//アイテムの種類
{
    Nodata,
    Recovery,//回復系
    Other//その他
}
