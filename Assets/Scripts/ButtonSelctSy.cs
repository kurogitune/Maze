using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelctSy : MonoBehaviour//各ボタンの情報箱
{
    DataBase InformationData;
    bool BuySell;//購入(true)か売却(false)か
    public Text NameText;//アイテム装備名前
    public Text CountText;//所持数
    public Text AmountmoneyText;//金額
    BuySellSelctSy Sy;

    public void ButtonSelct()//選択データを出力
    {
        if (InformationData.WeponDatas.WeponSetData)//装備データだったら
        {
            Sy.ButtonTreu(InformationData, BuySell, InformationData.WeponDatas.WeponCounts);
        }
        else if (InformationData.ItemDatas.ItemSetData)//アイテムデータだったら
        {
            Sy.ButtonTreu(InformationData, BuySell, InformationData.ItemDatas.ItemCounts);
        }
    }

    public void CasolMove()//カーソル移動での選択している情報を出力
    {
        Sy.CursorcursorMove(InformationData);
    }


    public void NoIN(BuySellSelctSy SytemData,DataBase INData, List<DataBase> WeponLis, List<DataBase> ItemLis, bool Buy)//ボタン作成時にデータ代入　ボタン管理システム　自分に入れられたデータ　購入か売却　
    {
        transform.localScale = new Vector3(1, 1, 1);
        if (SytemData != null) Sy = SytemData;
        BuySell = Buy;
        InformationData = INData;//初期データ代入
        DataUPData(INData, WeponLis, ItemLis);
    }

    public void DataUPData(DataBase UPDateData, List<DataBase> WeponLis, List<DataBase> ItemLis)//UI更新　 //自分のデータではない場合は無視  データ番号で判定
    {
        if (UPDateData.WeponDatas.WeponSetData)//装備データだったら
        {

            if (!InformationData.WeponDatas.WeponSetData) return;//同じデータ型ではない場合
            else if (InformationData.WeponDatas.WeponSetData.WeponNo != UPDateData.WeponDatas.WeponSetData.WeponNo) return;
        }
        else if (UPDateData.ItemDatas.ItemSetData)//アイテムデータだったら
        {
            if (!InformationData.ItemDatas.ItemSetData) return;//同じデータ型ではない場合
            else　if (InformationData.ItemDatas.ItemSetData.ItemNo != UPDateData.ItemDatas.ItemSetData.ItemNo) return;
        }

        InformationData = UPDateData;//更新に伴いデータも更新

        if (InformationData.WeponDatas.WeponSetData)//装備データだったら
        {
            NameText.text = string.Format("{0}", InformationData.WeponDatas.WeponSetData.WeaponName);
           
            if (BuySell)
            {
                AmountmoneyText.text = string.Format("{0}㌷", InformationData.WeponDatas.WeponSetData.Buymoney);//販売価格

                int WeponCount = 0;//所持数
                for(int i=0;i< WeponLis.Count; i++)
                {
                    if(UPDateData.WeponDatas.WeponSetData.WeponNo== WeponLis[i].WeponDatas.WeponSetData.WeponNo)
                    {
                        WeponCount = WeponLis[i].WeponDatas.WeponCounts;
                        break;
                    }
                }              
                CountText.text = string.Format("×{0}",WeponCount);
            } 
            else
            {             
                AmountmoneyText.text = string.Format("{0}㌷", InformationData.WeponDatas.WeponSetData.Sellmoney);//売却価格 
                CountText.text = string.Format("×{0}", InformationData.WeponDatas.WeponCounts);
            } 
        }
        else if (InformationData.ItemDatas.ItemSetData)//アイテムデータだったら
        {
            NameText.text = string.Format("{0}", InformationData.ItemDatas.ItemSetData.ItemName);

            if (BuySell) 
            { 
                AmountmoneyText.text = string.Format("{0}㌷", InformationData.ItemDatas.ItemSetData.Buymoney);//販売価格

                int ItemCount = 0;//所持数
                for (int i = 0; i < ItemLis.Count; i++)
                {
                    if (UPDateData.ItemDatas.ItemSetData.ItemNo == ItemLis[i].ItemDatas.ItemSetData.ItemNo)
                    {
                        ItemCount = ItemLis[i].ItemDatas.ItemCounts;
                        break;
                    }
                }
                CountText.text = string.Format("×{0}", ItemCount);
            } 
            else
            {
                AmountmoneyText.text = string.Format("{0}㌷", InformationData.ItemDatas.ItemSetData.Sellmoney);//売却価格
                CountText.text = string.Format("×{0}", InformationData.ItemDatas.ItemCounts);
            }
        }
    }
}
