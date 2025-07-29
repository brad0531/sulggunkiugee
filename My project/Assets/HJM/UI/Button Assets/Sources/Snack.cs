using UnityEditor.Playables;
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
        GameManager.Instance.setLiver(liver - GameManager.Instance.Snacks[index][1]);
        GameManager.Instance.EatSnack(index);
        Debug.Log($"현재 간 수치 {GameManager.Instance.UserData.liver}");
    }
}
