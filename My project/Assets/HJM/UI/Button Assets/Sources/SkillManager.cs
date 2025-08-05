using UnityEngine;

public class SkillManager : MonoBehaviour
{
    int tmp = -1;
    void onClick_SkillButton(int index)
    {
        if (GameManager.Instance.UserData.skill_set[index] == -1)
            return;
        if (!GameManager.Instance.isSkill_CoolTimeEnd(index))
            return;
        
    }

    void SwapPosition(int index)
    {
        tmp = index;
        
    }

    void SceneSwap()
    {
        tmp = -1;
    }
}
