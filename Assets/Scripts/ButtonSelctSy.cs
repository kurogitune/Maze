using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelctSy : MonoBehaviour
{
    int No;
    public Text Tex;
    BuySellSelctSy Sy;

    public void ButtonSelct()//選択番号を出力
    {
        
    }


    public void NoIN(int INNo,BuySellSelctSy SytemData)//データ代入
    {
        No = INNo;
      if(SytemData!=null)  Sy = SytemData;
    }
}
