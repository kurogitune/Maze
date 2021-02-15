using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

 [CreateAssetMenu(menuName = "WeapondataSet")]
public class WeaponData : ScriptableObject//武器データ用　変更する場合こいつを変更する
{
    public int WeponNo;
    public string WeaponName;
}


