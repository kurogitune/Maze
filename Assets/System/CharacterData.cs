using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName= "CharacterDataSet")]
public class CharacterData : ScriptableObject//キャラクター初期データ
{
    public int CharacterNo;//キャラクター番号
    public string CharacterName;//キャラクター名前
    public float HP;//体力
    public int Level;//レベル
    public string ExplanatoryText;//説明文
}

