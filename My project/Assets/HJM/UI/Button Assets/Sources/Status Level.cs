using UnityEngine;
using UnityEngine.UI;
public class StatusLevel : MonoBehaviour
{
    public Text ATK_text;
    public Text HP_text;
    public Text AS_text;
    public Text CRI_text;
    public void Start()
    {
        ATK_text.text = $"+ {GameManager.Instance.getCurrentATK()}";
        HP_text.text = $"+ {GameManager.Instance.getCurrentMaxHP()}";
        AS_text.text = $"+ {GameManager.Instance.getCurrentATK_speed()}";
        CRI_text.text = $"+ {GameManager.Instance.getCurrentCritPercent()}";
    }
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
        ATK_text.text = $"+ {GameManager.Instance.getCurrentATK()}";
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
        //최대체력 증가량만큼 체력 회복
        GameManager.Instance.setHP(GameManager.Instance.getHP() + (GameManager.Instance.LoadHP_Per_Level(level) - GameManager.Instance.getCurrentMaxHP()));
        GameManager.Instance.setMaxHP(GameManager.Instance.LoadHP_Per_Level(level));
        GameManager.Instance.UserData.status_levels[1]++; // HP 레벨업
        HP_text.text = $"+ {GameManager.Instance.getCurrentMaxHP()}";
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
        GameManager.Instance.UserData.status_levels[2]++; // ATKSpeed 레벨업
        AS_text.text = $"+ {GameManager.Instance.getCurrentATK_speed()}";
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
        CRI_text.text = $"+ {GameManager.Instance.getCurrentCritPercent()}";
        Debug.Log($"레벨업 성공! 현재 치명타 확률 : {GameManager.Instance.getCritPercent()} | 현재 잔고 : {GameManager.Instance.getMoney()}");
    }
}