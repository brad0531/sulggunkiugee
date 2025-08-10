using System;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public int[] ItemCost = { 3500, 225, 550 };
    public void onClick(int index)
    {
        int money = GameManager.Instance.getMoney();

        if (money < ItemCost[index])
            return;

        Debug.Log($"아이템 구매::{index} / 잔액 : {money - ItemCost[index]}");

        GameManager.Instance.setMoney(money - ItemCost[index]);

        if (index == 0) //숙취해소제
        {
            GameManager.Instance.UserData.record.SoMac = Math.Max(0, GameManager.Instance.UserData.record.SoMac - 1);
        }
        else if (index == 1)
        {
            GameManager.Instance.setLiver(GameManager.Instance.getLiver() - 30);
            GameManager.Instance.setHP(GameManager.Instance.getHP() + (int)((double)GameManager.Instance.getMaxHP() * 0.05));
        }
        else if (index == 2)
        {
            GameManager.Instance.setHP(GameManager.Instance.getHP() + (int)((double)GameManager.Instance.getMaxHP() * 0.05));
            for (int i = 0; i < 11; i++)
            {
                GameManager.Instance.UserData.effects_cool[i] -= GameManager.ONE_SECOND * 5; //5초 완화
            }
        }
    }

    void Start()
    {
        
    }
}
