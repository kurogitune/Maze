using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [CreateAssetMenu(menuName = "WeapondataSet")]
public class WeaponData : ScriptableObject//武器データ用　変更する場合こいつを変更する
{
    public int WeponNo;//武器番号
    public string WeaponName;//武器名前
    public bool Canibuy;//買えるか
    public int Buymoney;//買う金額
    public bool CaniSell;//売れるか
    public int Sellmoney;//買取金額
    public int OffensiveP;//攻撃力 
    public int DefenseP;//防御力
    public int LoadingBullet;//残弾数
    public WeponType Type;//武器タイプ
    public string ExplanatoryText;//説明文
}

public enum WeponType//武器タイプ
{
    Nodata,
    ShortDistance,//近距離
    LongFistance,//遠距離
    Armor//防具
}


