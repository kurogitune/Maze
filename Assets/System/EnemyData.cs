using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : ScriptableObject
{
    public int  EnemyNo;//敵番号
    public string EnemyName;//敵名前
    public int OffensiveP;//攻撃力 
    public int HP;//体力
    public EnemyType Type;//敵攻撃タイプ
    public string ExplanatoryText;//説明文
}

public enum EnemyType//敵タイプ
{
    Nodata,
    ShortDistance,//近距離
    LongFistance,//遠距離
}
