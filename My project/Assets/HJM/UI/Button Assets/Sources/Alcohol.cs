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
        {
            Debug.Log("쿨타임 중입니다..");
            return;
        }

        money -= cost;
        GameManager.Instance.setMoney(money);
        GameManager.Instance.effects_on(index);
        Debug.Log("술 효과 ON");
    }
}
