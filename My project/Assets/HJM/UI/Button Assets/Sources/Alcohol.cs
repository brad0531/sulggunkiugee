using UnityEngine;
using UnityEngine.UI;
public class Alcohol : MonoBehaviour
{
    public void BuyAlcohol(int index)
    {
        GameManager.Alcohol_index idx = (GameManager.Alcohol_index)index;
        int money = GameManager.Instance.getMoney();
        int cost = GameManager.Instance.Load_Alcohol_Cost(idx);

        if (money < cost) //돈 부족
            return;

        if (!GameManager.Instance.isCoolTimeEnd(idx))
            return;

        money -= cost;
        GameManager.Instance.setMoney(money);
    }
}
