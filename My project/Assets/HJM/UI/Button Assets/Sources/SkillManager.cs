using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    int goal = -1;
    Sprite goal_img;
    public void onClick_SkillButton(Transform target)
    {
        int index = int.Parse(target.name);
        if (goal != -1) //스킬 변경
        {
            SwapPosition(index, target.gameObject);
            return;
        }
        if (GameManager.Instance.UserData.skill_set[index] == -1)
            return;
        if (!GameManager.Instance.isSkill_CoolTimeEnd(index))
            return;
        GameManager.Instance.Skill_Use(index);
    }

    public void SwapPositionReady(int index, Sprite obj)
    {
        goal = index;
        goal_img = obj;
    }

    public void SwapPosition(int index, GameObject target)
    {
        int tmpindex = -1;

        for (int i = 0; i < 4; i++)
        {
            if (GameManager.Instance.UserData.skill_set[i] == index)
                tmpindex = i;
        }
        if (tmpindex != -1 && tmpindex != index)
            return;
        Image tmp = target.GetComponent<Image>();
        tmp.sprite = goal_img;
        goal = -1;
    }

    public void SceneSwap()
    {
        goal = -1;
    }
}
