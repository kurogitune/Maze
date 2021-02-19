using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Exception;

public class BuySellSelctSy : MonoBehaviour
{
    public GameObject ButtonUI;//ボタンオブジェクト
    public GameObject Oya;//ボタンを作成する上での親
    [Header("アイテム装備の説明")]
    public Text ExplanatoryText;

    [Header("個数選択UI")]
    public GameObject CountSelctUI;
    [Header("データ選択戻るボタン")]
    public Button[] DataSelctBackButton;
    [Header("個数表示テキスト")]
    public Text CountText;
    [Header("価格表示テキスト")]
    public Text QuantityMoney;
    [Header("個数選択カーソル初期位置")]
    public Button SelctCursorPoz;
    public  ShopSy Sy;//販売システム
    public AudioClip SE;

    List<GameObject> ButtunObj = new List<GameObject>(0);//ボタンオブジェクトの箱リスト
    int Quantity=1;//販売売却個数
    int MaxQuantiy = 0;//現在の所持数
    DataBase SelctData;//選択しているデータ
    bool BuyData;//購入か売却を判定用
    bool CountSelctcheck;//個数選択中か

    private void Update()//表示テキスト変更処理
    {
        if (!CountSelctcheck) return;
        CountText.text = string.Format("{0}個", Quantity);

        if (BuyData)
        {
            if (SelctData.WeponDatas.WeponSetData)//装備データだったら
            {
                QuantityMoney.text = string.Format("{0}㌷", SelctData.WeponDatas.WeponSetData.Buymoney * Quantity);
            }
            else if (SelctData.ItemDatas.ItemSetData)//アイテムデータだったら
            {
                QuantityMoney.text = string.Format("{0}㌷", SelctData.ItemDatas.ItemSetData.Buymoney * Quantity);
            }
        }
        else
        {
            if (SelctData.WeponDatas.WeponSetData)//装備データだったら
            {
                QuantityMoney.text = string.Format("{0}㌷", SelctData.WeponDatas.WeponSetData.Sellmoney * Quantity);
            }
            else if (SelctData.ItemDatas.ItemSetData)//アイテムデータだったら
            {
                QuantityMoney.text = string.Format("{0}㌷", SelctData.ItemDatas.ItemSetData.Sellmoney * Quantity);
            }
        }    
    }

    public void ButtunIN(List<DataBase> ButtonUIData, List<DataBase> WeponLis, List<DataBase> ItemLis, bool Buy)//ボタン作成　データのリスト,購入か売却
    {
        CountSelctUI.SetActive(false);
        for (int i=0;i<ButtonUIData.Count ; i++)
        {
            GameObject g = Instantiate(ButtonUI);
            g.transform.parent = Oya.transform;
            g.GetComponent<ButtonSelctSy>().NoIN(GetComponent<BuySellSelctSy>(),ButtonUIData[i], WeponLis, ItemLis, Buy);
            ButtunObj.Add(g);

            if (i == 0) g.GetComponent<Button>().Select();
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


    public void ButtonTreu(DataBase SelctsData,bool buy,int MAXCount)//ボタンが押されたら  選択されたデータ,購入か売却
    {
        SelctData = SelctsData;
        BuyData = buy;
        MaxQuantiy = MAXCount;
        CountSelctUI.SetActive(true);
        for (int i = 0; i < ButtunObj.Count; i++)
        {
            ButtunObj[i].GetComponent<Button>().interactable = false;//falseにすることでボタンを押せなくする
        }
        if(buy) DataSelctBackButton[0].interactable = false;
        else  DataSelctBackButton[1].interactable = false;
        CountSelctcheck = true;
        SelctCursorPoz.Select();
    }

    public void Decision()//決定
    {
        Sy.BuyNoIN(SelctData, BuyData,Quantity);
        Cancel();
    }

    public void Cancel()//キャンセル
    {
        CountSelctUI.SetActive(false);
        for (int i = 0; i < ButtunObj.Count; i++)
        {
            ButtunObj[i].GetComponent<Button>().interactable = true;//falseにすることでボタンを押せなくする
        }
         DataSelctBackButton[0].interactable = true;
         DataSelctBackButton[1].interactable = true;

        Quantity = 1;
        MaxQuantiy = 0;
        SelctData = null;
        CountSelctcheck = BuyData = false;
        ButtunObj[0].GetComponent<Button>().Select();
    }

    public void QuantityCount(int i)//個数を変更
    {
        Quantity += i;
        if (BuyData)
        {

        }
        else
        {         
            if (Quantity > MaxQuantiy) Quantity = MaxQuantiy;
        }

        if (Quantity < 1) Quantity = 1;
    }

                                //更新先のデータ,プレイヤー所持装備リスト,プレイヤー所持アイテムリスト
    public void PlayerDataUPDate(DataBase INData,List<DataBase>WeponLis,List<DataBase>ItemLis)//プレイヤーのデータが更新時に読み込め  ボタンUiデータを更新
    {
        for(int i=0; i<ButtunObj.Count; i++)
        {
            ButtunObj[i].GetComponent<ButtonSelctSy>().DataUPData(INData,WeponLis,ItemLis);
        }
    }

    public void CursorcursorMove(DataBase UIData)//カーソルが移動されたら
    {
        AudioSystem.SEPlaye(SE);
        if (UIData.WeponDatas.WeponSetData)
        {
            ExplanatoryText.text = string.Format("{0}", UIData.WeponDatas.WeponSetData.ExplanatoryText);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (UIData.ItemDatas.ItemSetData)
        {
            ExplanatoryText.text = string.Format("{0}", UIData.ItemDatas.ItemSetData.ExplanatoryText);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
