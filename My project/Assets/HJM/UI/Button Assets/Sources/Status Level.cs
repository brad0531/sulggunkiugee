using UnityEngine;

public class StatusLevel : MonoBehaviour
{
    public void LevelupATK()
    {
        int money = GameManager.Instance.getMoney();
        int level = GameManager.Instance.getATKlevel();
        int cost = GameManager.Instance.LoadATKcost(level);

        if (cost == -1) //최대 레벨 도달 시
            return;

        money -= cost;
        level++;

        GameManager.Instance.setMoney(money);
        GameManager.Instance.setATK(GameManager.Instance.LoadATK_Per_Level(level));
        GameManager.Instance.UserData.status_levels[0]++; // ATK 레벨업
        Debug.Log($"레벨업 성공! 현재 ATK : {GameManager.Instance.getATK()} | 현재 잔고 : {GameManager.Instance.getMoney()}");
    }
}