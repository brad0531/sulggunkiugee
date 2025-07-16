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

    public void LevelupHP()
    {
        int money = GameManager.Instance.getMoney();
        int level = GameManager.Instance.getHPlevel();
        int cost = GameManager.Instance.LoadHPcost(level);

        if (cost == -1) //최대 레벨 도달 시
            return;

        money -= cost;
        level++;

        GameManager.Instance.setMoney(money);
        GameManager.Instance.setMaxHP(GameManager.Instance.LoadHP_Per_Level(level));
        GameManager.Instance.UserData.status_levels[1]++; // HP 레벨업
        Debug.Log($"레벨업 성공! 현재 HP : {GameManager.Instance.getMaxHP()} | 현재 잔고 : {GameManager.Instance.getMoney()}");
    }

    public void LevelupAttackSpeed()
    {
        int money = GameManager.Instance.getMoney();
        int level = GameManager.Instance.getATK_speedlevel();
        int cost = GameManager.Instance.LoadATKSpeedcost(level);

        if (cost == -1) //최대 레벨 도달 시
            return;

        money -= cost;
        level++;

        GameManager.Instance.setMoney(money);
        GameManager.Instance.setAttackSpeed(GameManager.Instance.LoadAttackSpeed_Per_Level(level));
        GameManager.Instance.UserData.status_levels[2]++; // ATK 레벨업
        Debug.Log($"레벨업 성공! 현재 ATK speed : {GameManager.Instance.getATK_speed()} | 현재 잔고 : {GameManager.Instance.getMoney()}");
    }

    public void LevelupCRIpercent()
    {
        int money = GameManager.Instance.getMoney();
        int level = GameManager.Instance.getCRIpercentlevel();
        int cost = GameManager.Instance.LoadCRIpercentcost(level);

        if (cost == -1) //최대 레벨 도달 시
            return;

        money -= cost;
        level++;

        GameManager.Instance.setMoney(money);
        GameManager.Instance.setCRIpercent(GameManager.Instance.LoadCritPercent_Per_Level(level));
        GameManager.Instance.UserData.status_levels[3]++; // CRI percent 레벨업
        Debug.Log($"레벨업 성공! 현재 치명타 확률 : {GameManager.Instance.getCritPercent()} | 현재 잔고 : {GameManager.Instance.getMoney()}");
    }
}