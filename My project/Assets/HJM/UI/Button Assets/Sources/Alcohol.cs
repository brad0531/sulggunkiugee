using UnityEngine;
using UnityEngine.UI;
public class Alcohol : MonoBehaviour
{
    public void BuyAlcohol(GameManager.Alcohol_index index)
    {
        int money = GameManager.Instance.getMoney();
        int cost = GameManager.Instance.Load_Alcohol_Cost(index);

        if (money < cost) //돈 부족
            return;

        if (!GameManager.Instance.isCoolTimeEnd(index))
            return;

        money -= cost;
        GameManager.Instance.setMoney(money);
    }
}
