using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Exception;
public class ShopSy : MonoBehaviour
{
    [Header("店のUIオブジェクト")]
    public GameObject ShopUIObj;
    [Header("表示するテキスト")]
    public Text BunUI;
    [Header("文字を表示する間隔")]
    public float TextInTime;
    [Header("バック画像")]
    public Image BackImag;
    [Header("売る買うかを選択するUI")]
    public GameObject ShopSelctUI;
    [Header("上記のボタン初期位置")]
    public Button SyokiButton;
    [Header("買うUI")]
    public GameObject BuyShopUI;
    [Header("売るUI")]
    public GameObject SellShopUI;
    [Header("販売売却ボタン表示UI")]
    public GameObject BuSeUI;
    [Header("所持金表示テキスト")]
    public Text ManeyText;
    [Header("武器防具リスト")]
    public WeponList WeponDataList;
    [Header("アイテムリスト")]
    public ItemList ItemDataList;
    [Header("販売売却のボタン管理")]
    public BuySellSelctSy BuSeSy;
    [Header("個数選択UI")]
    public GameObject CountSelctUI;

    public AudioClip Se;

    PlayerTes PlayerSy;//プレイヤースクリプト
    int PlayerManey;//プレイヤー所持金
    List<DataBase> WeponHaveData = new List<DataBase>(0);//プレイヤー所持装備リスト
    List<DataBase> ItemHaveData = new List<DataBase>(0);//プレイヤー所持アイテムリスト

    string TextSt;//表示する文字
    int BunCount, Bun;//文字数　現在の文字数
    float TTime;//テキスト表示間隔管理用
    bool ShopIN;//ショップに入ったか　   
    bool BunEnd;//文表示が終了したか
    bool ShopEndz;//ショップ終了か
    bool Shopping;//買い物中か

    List<WeaponData> WeponBuyData = new List<WeaponData>(0);//装備販売リスト
    int[] WeponBuyNo = new int[0];//販売する番号
    List<ItemData> ItemBuyData = new List<ItemData>(0);//アイテム販売リスト
    int[] ItemBuyNo = new int[0];//販売する番号
    List<DataBase> BuyData = new List<DataBase>(0);//販売するアイテムや装備データ
    List<DataBase> SellData = new List<DataBase>(0);//所持しているアイテム装備データ
    // Start is called before the first frame update
    void Start()
    {
        TTime = TextInTime;
        ShopUIObj.SetActive(false);
        BuyShopUI.SetActive(false);
        SellShopUI.SetActive(false);
        ShopSelctUI.SetActive(false);
        BackImag.gameObject.SetActive(false);
        BunUI.gameObject.SetActive(false);
        BuSeUI.SetActive(false);
        CountSelctUI.SetActive(false);
        ManeyText.transform.parent.gameObject.SetActive(false);
        BunUI.text = "";
        TextSt = "いらっしゃいませ";
        ManeyText.text = string.Format("所持金 : {0}", PlayerManey);

        for (int i = 0; i < WeponDataList.DataList.Length; i++)//装備販売リスト取得
        {
            if (WeponDataList.DataList[i].Canibuy) WeponBuyData.Add(WeponDataList.DataList[i]);
        }

        for (int i = 0; i < ItemDataList.DataList.Length; i++)//アイテム販売リスト取得
        {
            if (ItemDataList.DataList[i].Canibuy) ItemBuyData.Add(ItemDataList.DataList[i]);
        }

        for(int i=0;i< WeponBuyData.Count; i++)//販売するリストに代入
        {
            DataBase WeponD = new DataBase();
            WeponD.WeponDatas.WeponSetData = WeponBuyData[i];
            BuyData.Add(WeponD);
        }

        for (int i = 0; i < ItemBuyData.Count; i++) //販売するリストに代入
        {
            DataBase WeponD = new DataBase();
            WeponD.ItemDatas.ItemSetData = ItemBuyData[i];
            BuyData.Add(WeponD);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!ShopIN) return;

        if (Bun < BunCount & !BunEnd)//文表示システム
        {
            TTime -= Time.deltaTime;
            if (TTime < 0)
            {
                Bun++;
                TTime = TextInTime;
                BunUI.text = TextSt.Substring(0, Bun);//こいつで表示する文字数指定できる（へぇー）
                AudioSystem.SEPlaye(Se);
                if (!BunEnd & Bun >= BunCount)
                {
                    Bun = 0;
                    BunEnd = true;
                }
            }
        }

        if (BunEnd & !Shopping & Input.anyKeyDown)//文表示終了後の処理
        {
            if (ShopEndz)
            {
                TextSt = "いらっしゃいませ";
                BunUI.text = "";
                Bun = 0;
                BuyShopUI.SetActive(false);
                SellShopUI.SetActive(false);
                ShopSelctUI.SetActive(false);
                BackImag.gameObject.SetActive(false);
                BunUI.gameObject.SetActive(false);
                ShopUIObj.SetActive(false);
                BuSeUI.SetActive(false);
                PlayerSy.PlayerIventsEnd();
                PlayerSy = null;
                BunEnd = ShopEndz = Shopping = ShopIN = false;
                return;
            }

            BackImag.gameObject.SetActive(true);
            ShopSelctUI.SetActive(true);
            ManeyText.transform.parent.gameObject.SetActive(true);
            BunUI.gameObject.SetActive(false);
            SyokiButton.Select();
            Shopping = true;
        }

    }

    public void PlayerSystemIN(PlayerTes PlayerSystem)//プレイヤーが話しかけてきたら　プレイヤースクリプト
    {
        PlayerSy = PlayerSystem;
        PlayerDataupdate();
        ShopUIObj.SetActive(true);
        BunUI.gameObject.SetActive(true);
        BunCount = TextSt.Length;
        ShopIN = true;
    }

    void PlayerDataupdate()//プレイヤーの情報を更新
    {
        PlayerManey = PlayerSy.MoneyOUT();
        WeponHaveData = PlayerSy.WeaponDataListOUT();
        ItemHaveData = PlayerSy.ItemDataListOUT();
        ManeyText.text = string.Format("所持金 : {0}㌷", PlayerManey);

        SellData.Clear();
        for (int i = 0; i < WeponHaveData.Count; i++)//販売するリストに代入
        {
            SellData.Add(WeponHaveData[i]);
        }

        for (int i = 0; i < ItemHaveData.Count; i++) //販売するリストに代入
        {
            SellData.Add(ItemHaveData[i]);
        }
    }

    public void BuyShopIN()//販売開始
    {
        ShopSelctUI.SetActive(false);
        BuyShopUI.SetActive(true);
        BuSeUI.SetActive(true);
        BuSeSy.ButtunIN(BuyData, WeponHaveData,ItemHaveData,true);
    }

    public void BuyShopEnd()//販売終了
    {
        ShopSelctUI.SetActive(true);
        BuyShopUI.SetActive(false);
        BuSeUI.SetActive(false);
        BuSeSy.ButtonDes();
        SyokiButton.Select();
    }

    public void SellShopIN()//売却開始
    {
        ShopSelctUI.SetActive(false);
        SellShopUI.SetActive(true);
        BuSeUI.SetActive(true);
        BuSeSy.ButtunIN(SellData, WeponHaveData, ItemHaveData, false);
    }

    public void SellShopEND()//売却終了
    {
        ShopSelctUI.SetActive(true);
        SellShopUI.SetActive(false);
        BuSeUI.SetActive(false);
        BuSeSy.ButtonDes();
        SyokiButton.Select();
    }

    public void ShopEnd()
    {
        Shopping = BunEnd = false;
        TextSt = "ありがとうございました";
        BunCount = TextSt.Length;
        BunUI.gameObject.SetActive(true);
        BackImag.gameObject.SetActive(false);
        ManeyText.transform.parent.gameObject.SetActive(false);
        ShopSelctUI.SetActive(false);
        ShopEndz = true;
    }


    public void BuyNoIN(DataBase SelctData,bool buy,int Quantity)//選択したデータ　 選択されたデータ情報,購入か売却,個数
    {
        if (SelctData.WeponDatas.WeponSetData)//装備データだったら
        {
            PlayerSy.WeponDataIN(SelctData, buy,Quantity);
        }
        else if (SelctData.ItemDatas.ItemSetData)//アイテムデータだったら
        {
            PlayerSy.ItemDataIN(SelctData, buy, Quantity);
        }
        PlayerDataupdate();
        BuSeSy.PlayerDataUPDate(SelctData,WeponHaveData,ItemHaveData);
    }
}
