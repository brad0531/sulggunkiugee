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

        if (index >= 6 && GameManager.Instance.UserData.record.Soju_Record[index - 6] == 0) //소주 음용 회수 초과
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

        if (index >= 6) //소주 경우
        {
            GameManager.Instance.UserData.record.Soju_Record[index - 6]--;
        }

        int isAcrossed = 0;
        for (int i = 0; i < 6; i++) //맥주 중에서 하나라도 활성화되면
        {
            if (GameManager.Instance.isEffectsOn((GameManager.Alcohol_index)i))
            {
                isAcrossed++;
                break;
            }
        }
        for (int i = 6; i < 11; i++) //소주 중에서 하나라도 활성화되면
        {
            if (GameManager.Instance.isEffectsOn((GameManager.Alcohol_index)i))
            {
                isAcrossed++;
                break;
            }
        }

        if (isAcrossed == 2) //소맥 활성화
        {
            GameManager.Instance.UserData.record.SoMac++;
        }
    }
}
