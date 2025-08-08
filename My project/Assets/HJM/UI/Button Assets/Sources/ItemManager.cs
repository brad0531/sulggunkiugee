using System;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public int[] ItemCost = { 3500, 225, 550 };
    void onClick(int index)
    {
        int money = GameManager.Instance.getMoney();

        if (money < ItemCost[index])
            return;

        GameManager.Instance.setMoney(money - ItemCost[index]);
        if (index == 0) //숙취해소제
        {
            GameManager.Instance.UserData.record.SoMac = Math.Max(0, GameManager.Instance.UserData.record.SoMac - 1);
        }
        else if (index == 1)
        {
            
        }
    }
}
