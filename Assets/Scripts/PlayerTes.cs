﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exception;

public class PlayerTes : MonoBehaviour
{
    public float Speed;
    public LayerMask ShopMask;
    public int MoneyinPossession;//所持金
    [Header("アイテムの最大所持数")]
    public int MaxItemCount;
    public WeponList WeponLis;
    public ItemList ItemLis;
    bool Ivents;

    List<DataBase> WeponHaveData = new List<DataBase>(0);//プレイヤー所持装備リスト
    List<DataBase> ItemHaveData = new List<DataBase>(0);//プレイヤー所持アイテムリスト
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i< WeponLis.DataList.Length ; i++)
        {
            DataBase da = new DataBase();
            da.WeponDatas.WeponCounts = 10;
            da.WeponDatas.WeponSetData = WeponLis.DataList[i];
            WeponHaveData.Add(da);
        }
      
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
        return MoneyinPossession;
    }

    public void MoneyIN(int MoneyINData)//金額代入
    {
        MoneyinPossession += MoneyINData;
    }

    public List<DataBase> WeaponDataListOUT()//装備リストを出力
    {
        return WeponHaveData;
    }

    public void WeponDataIN(DataBase BuyData,bool Buy,int WeponCount)//装備データを代入や削除 　処理する詳細情報　購入か売却か　購入売却した個数
    {
        if (Buy)//購入
        {
            bool Exists = false;//装備があるかどうか
            for (int i=0;i<WeponHaveData.Count ; i++)//装備を持っているかを確認する
            {
                if (WeponHaveData[i].WeponDatas.WeponSetData.WeponNo==BuyData.WeponDatas.WeponSetData.WeponNo)//装備を持っている場合
                {
                    WeponHaveData[i].WeponDatas.WeponCounts+= WeponCount;//持っている場合所持数を増やす
                    Exists = true;
                    break;
                }
            }

            if (!Exists)//持っていない場合
            {
                DataBase SetData = new DataBase();
                SetData.WeponDatas.WeponSetData =BuyData.WeponDatas.WeponSetData;
                SetData.WeponDatas.WeponCounts += WeponCount;
                WeponHaveData.Add(SetData);//装備を所持処理
            } 
        }
        else//売却
        {
            for (int i = 0; i < WeponHaveData.Count; i++)//装備を持っているかを確認する
            {
                if (WeponHaveData[i].WeponDatas.WeponSetData.WeponNo == BuyData.WeponDatas.WeponSetData.WeponNo)//装備を持っている場合
                {
                    WeponHaveData[i].WeponDatas.WeponCounts -= WeponCount;//持っている場合所持数を減らす

                    if (WeponHaveData[i].WeponDatas.WeponCounts <= 0)//所持数が無くなった場合
                    {
                        WeponHaveData.Remove(WeponHaveData[i]);
                    }
                    break;
                }
            }
        }
    }

    public List<DataBase> ItemDataListOUT()//アイテムリストを出力
    {
        return ItemHaveData;
    }

    public void ItemDataIN(DataBase SellData, bool Buy, int ItemCount)//装備データを代入や削除　　処理する詳細情報　購入か売却か　購入売却した個数
    {
        if (Buy)//購入
        {
            bool Exists = false;
            for (int i = 0; i < ItemHaveData.Count; i++)//装備を持っているかを確認する
            {
                if (ItemHaveData[i].ItemDatas.ItemSetData.ItemNo==SellData.ItemDatas.ItemSetData.ItemNo)//装備を持っている場合
                {
                    ItemHaveData[i].ItemDatas.ItemCounts+=ItemCount;//持っている場合所持数を増やす
                    Exists = true;
                    break;
                }
            }

            if (!Exists)//持っていない場合
            {
                DataBase SetData = new DataBase();
                SetData.ItemDatas.ItemSetData = SellData.ItemDatas.ItemSetData;
                SetData.ItemDatas.ItemCounts += ItemCount;
                ItemHaveData.Add(SetData);//装備を所持処理
            }
        }
        else//売却
        {
            for (int i = 0; i < ItemHaveData.Count; i++)//装備を持っているかを確認する
            {
                if (ItemHaveData[i].ItemDatas.ItemSetData.ItemNo == SellData.ItemDatas.ItemSetData.ItemNo)//装備を持っている場合
                {
                    ItemHaveData[i].ItemDatas.ItemCounts -= ItemCount;//持っている場合所持数を減らす

                    if (ItemHaveData[i].WeponDatas.WeponCounts <= 0)//所持数が無くなった場合
                    {
                        ItemHaveData.Remove(ItemHaveData[i]);
                    }
                    break;
                }
            }
        }
    }
}
