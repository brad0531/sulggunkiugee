using UnityEngine;
using UnityEngine.UI;
public class Snack : MonoBehaviour
{
    public void BuySnack(int index)
    {
        int money = GameManager.Instance.getMoney();
        int cost = GameManager.Instance.Snacks[index][0];
        int liver = GameManager.Instance.getLiver();
        if (money < cost) //돈 부족
            return;

        if (!GameManager.Instance.isSnackCoolTimeEnd(index))
        {
            Debug.Log("쿨타임 중입니다..");
            return;
        }

        money -= cost;
        GameManager.Instance.setMoney(money);
        GameManager.Instance.effects_on(index);
        GameManager.Instance.setLiver(liver + GameManager.Instance.Info_Alcohol[index][4]);
        Debug.Log("과자 효과 ON");
    }
}
