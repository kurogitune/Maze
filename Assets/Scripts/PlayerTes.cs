using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exception;

public class PlayerTes : MonoBehaviour
{
    public float Speed;
    public LayerMask ShopMask;
    public int Money;//お金
    [Header("アイテムの最大所持数")]
    public int MaxItemCount;
    bool Ivents;

    List<DataBase> WeponHaveData = new List<DataBase>(0);//プレイヤー所持装備リスト
    List<DataBase> ItemHaveData = new List<DataBase>(0);//プレイヤー所持アイテムリスト
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        Ray Shopcheckray = new Ray(transform.position,transform.position);
        RaycastHit Hit;
        if (Input.GetKeyDown(KeyCode.E)&Physics.Raycast(Shopcheckray,out Hit,2,ShopMask)&!Ivents)
        {
            Hit.collider.gameObject.transform.parent.GetComponent<ShopSy>().PlayerSystemIN(GetComponent<PlayerTes>());
            Ivents = true;
        }

        if (Ivents) return;
        Vector3 Idou = Vector3.zero;
        Vector3 Yoko = Vector3.zero;
        if (MoveKey.Horizontal_Right())
        {
            Yoko = transform.right;
        }
        else if (MoveKey.Horizonta_Left())
        {
            Yoko = -transform.right;
        }

        if (MoveKey.Vertical_Up())
        {
            Idou = transform.forward;
        }
        else if (MoveKey.Vertical_Down())
        {
            Idou = -transform.forward;
        }
        else
        {

        }
        Idou *= Time.deltaTime * Speed;
        Yoko *= Time.deltaTime * Speed;
        transform.position += Idou;
        transform.position += Yoko;
    }

    public void PlayerIventsEnd()
    {
        Ivents = false;
    }

    public int MoneyOUT()//金額出力
    {
        return Money;
    }

    public void MoneyIN(int MoneyINData)//金額代入
    {
        Money += MoneyINData;
    }

    public List<DataBase> WeaponDataListOUT()//装備リストを出力
    {
        return WeponHaveData;
    }

    public void WeponDataIN(WeaponData Weponz,bool Buy,int WeponCount)//装備データを代入や削除 　処理する詳細情報　購入か売却か　購入売却した個数
    {
        if (Buy)//購入
        {
            bool Exists = false;
            for (int i=0;i<WeponHaveData.Count ; i++)//装備を持っているかを確認する
            {
                if (WeponHaveData[i].WeponDatas.WeponSetData)//装備を持っている場合
                {
                    WeponHaveData[i].WeponDatas.WeponCounts+= WeponCount;//持っている場合所持数を増やす
                    Exists = true;
                    break;
                }
            }

            if (!Exists)//持っていない場合
            {
                DataBase SetData = new DataBase();
                SetData.WeponDatas.WeponSetData =Weponz ;
                SetData.WeponDatas.WeponCounts += WeponCount;
                WeponHaveData.Add(SetData);//装備を所持処理
            } 
        }
        else
        {
            for (int i = 0; i < WeponHaveData.Count; i++)//装備を持っているかを確認する
            {
                if (WeponHaveData[i].WeponDatas.WeponSetData)//装備を持っている場合
                {
                    WeponHaveData[i].WeponDatas.WeponCounts -= WeponCount;//持っている場合所持数を減らす
                    break;
                }
            }
        }
    }

    public List<DataBase> ItemDataListOUT()//アイテムリストを出力
    {
        return ItemHaveData;
    }

    public void ItemDataIN(ItemData Itemz, bool Buy, int ItemCount)//装備データを代入や削除　　処理する詳細情報　購入か売却か　購入売却した個数
    {
        if (Buy)//購入
        {
            bool Exists = false;
            for (int i = 0; i < ItemHaveData.Count; i++)//装備を持っているかを確認する
            {
                if (ItemHaveData[i].ItemDatas.ItemSetData==Itemz)//装備を持っている場合
                {
                    ItemHaveData[i].ItemDatas.ItemCounts+=ItemCount;//持っている場合所持数を増やす
                    Exists = true;
                    break;
                }
            }

            if (!Exists)//持っていない場合
            {
                DataBase SetData = new DataBase();
                SetData.ItemDatas.ItemSetData = Itemz;
                SetData.ItemDatas.ItemCounts += ItemCount;
                ItemHaveData.Add(SetData);//装備を所持処理
            }
        }
        else
        {
            for (int i = 0; i < ItemHaveData.Count; i++)//装備を持っているかを確認する
            {
                if (ItemHaveData[i].ItemDatas.ItemSetData == Itemz)//装備を持っている場合
                {
                    ItemHaveData[i].ItemDatas.ItemCounts -= ItemCount;//持っている場合所持数を減らす
                    break;
                }
            }
        }
    }
}
