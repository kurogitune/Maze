using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySellSelctSy : MonoBehaviour
{
    public GameObject ButtonUI;
    public GameObject Oya;
    List<GameObject> ButtunObj=new List<GameObject>(0);
    ShopSy Sy;
    public void ButtunIN(int No,ShopSy ShSy)//ボタン作成
    {
         if(ShSy!=null) Sy = ShSy;
        for(int i=0;i<No ; i++)
        {
            GameObject g = Instantiate(ButtonUI);
            ButtunObj.Add(g);
            g.transform.parent = Oya.transform;
        }
    }

    public void ButtonDes()//ボタン削除
    {
        for (int i = 0; i < ButtunObj.Count; i++)
        {
            Destroy(ButtunObj[i]);
        }
        ButtunObj = new List<GameObject>(0);
    }
}
