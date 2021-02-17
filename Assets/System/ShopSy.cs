using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("買うUI")]
    public GameObject BuyShopUI;
    [Header("売るUI")]
    public GameObject SellShopUI;
    [Header("所持金表示テキスト")]
    public Text ManeyText;
    [Header("武器防具リスト")]
    public WeponList WeponDataList;
    [Header("アイテムリスト")]
    public ItemList ItemDataList;

    AudioSource SEPlayer;
    public AudioClip Se;

    PlayerTes PlayerSy;//プレイヤースクリプト
    int PlayerManey;//プレイヤー所持金
    List<DataBase> WeponHaveData = new List<DataBase>(0);//プレイヤー所持装備リスト
    List<DataBase> ItemHaveData = new List<DataBase>(0);//プレイヤー所持アイテムリスト

    string TextSt;//表示する文字
    int BunCount, Bun;//文字数　現在の文字数
    float TTime;//テキスト表示間隔管理用
    bool ShopIN, BunEnd, ShopEndz, Shopping;//ショップに入ったか　文表示が終了したか  ショップ終了か 買い物中か

    List<WeaponData> WeponBuyData = new List<WeaponData>(0);//装備販売リスト
    int[] WeponBuyNo = new int[0];//販売する番号
    List<ItemData> ItemBuyData = new List<ItemData>(0);//アイテム販売リスト
    int[] ItemBuyNo = new int[0];//販売する番号
    // Start is called before the first frame update
    void Start()
    {
        SEPlayer = GetComponent<AudioSource>();
        TTime = TextInTime;
        ShopUIObj.SetActive(false);
        BuyShopUI.SetActive(false);
        SellShopUI.SetActive(false);
        ShopSelctUI.SetActive(false);
        BackImag.gameObject.SetActive(false);
        BunUI.gameObject.SetActive(false);
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!ShopIN) return;

        if (Bun < BunCount & !BunEnd)
        {
            TTime -= Time.deltaTime;
            if (TTime < 0)
            {
                Bun++;
                TTime = TextInTime;
                BunUI.text = TextSt.Substring(0, Bun);//こいつで表示する文字数指定できる（へぇー）
                SEPlayer.PlayOneShot(Se);
                if (!BunEnd & Bun >= BunCount)
                {
                    Bun = 0;
                    BunEnd = true;
                }
            }
        }

        if (BunEnd & !Shopping & Input.anyKeyDown)
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
                PlayerSy.PlayerIventsEnd();
                PlayerSy = null;
                BunEnd = ShopEndz = Shopping = ShopIN = false;
                return;
            }

            BackImag.gameObject.SetActive(true);
            ShopSelctUI.SetActive(true);
            ManeyText.transform.parent.gameObject.SetActive(true);
            BunUI.gameObject.SetActive(false);
            Shopping = true;
        }

    }

    public void PlayerSystemIN(PlayerTes PlayerSystem)//プレイヤーが話しかけてきたら
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
        ManeyText.text = string.Format("所持金 : {0}", PlayerManey);
    }

    public void BuyShopIN()
    {
        ShopSelctUI.SetActive(false);
        BuyShopUI.SetActive(true);
    }

    public void BuyShopEnd()
    {
        ShopSelctUI.SetActive(true);
        BuyShopUI.SetActive(false);
    }

    public void SellShopIN()
    {
        ShopSelctUI.SetActive(false);
        SellShopUI.SetActive(true);
    }

    public void SellShopEND()
    {
        ShopSelctUI.SetActive(true);
        SellShopUI.SetActive(false);
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


    public void BuyNoIN(int No)//購入した番号
    {

    }
}
