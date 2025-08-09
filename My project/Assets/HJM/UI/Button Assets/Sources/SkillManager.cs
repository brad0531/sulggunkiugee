using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
public class SkillManager : MonoBehaviour
{

    int goal = -1;
    public List<Sprite> goal_img;
    public List<Transform> Skill_Panels;
    public void onClick_SkillButton(Transform target)
    {
        int index = int.Parse(target.name) - 1;
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
        PlayerController.Instance.SkillActive(index, GameManager.Instance.Load_Skill_Damage(index, GameManager.Instance.get_Skill_level(index)), GameManager.Instance.Load_Skill_HitCount(index));
    }

    public void SwapPositionReady(int index)
    {
        goal = index;
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
        tmp.sprite = goal_img[index];
        goal = -1;
    }

    public void SceneSwap()
    {
        goal = -1;
    }

    public void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            Skill_Panels[i].GetComponent<Image>().sprite = goal_img[GameManager.Instance.UserData.skill_set[i]];
        }
    }
}
