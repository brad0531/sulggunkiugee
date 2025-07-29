using UnityEngine;
using UnityEngine.UI;
public class Alcohol : MonoBehaviour
{
    public void BuyAlcohol(int index)
    {
        GameManager.Alcohol_index idx = (GameManager.Alcohol_index)index;
        int money = GameManager.Instance.getMoney();
        int cost = GameManager.Instance.Load_Alcohol_Cost(idx);
        int liver = GameManager.Instance.getLiver();
        
        if (money < cost) //돈 부족
            return;

        if (!GameManager.Instance.isAlcoholCoolTimeEnd(idx))
        {
            Debug.Log("쿨타임 중입니다..");
            return;
        }

        money -= cost;
        GameManager.Instance.setMoney(money);
        GameManager.Instance.effects_on(index);
        GameManager.Instance.setLiver(liver + GameManager.Instance.Info_Alcohol[index][4]);
        Debug.Log($"현재 간 수치 {GameManager.Instance.UserData.liver}");
    }
}
