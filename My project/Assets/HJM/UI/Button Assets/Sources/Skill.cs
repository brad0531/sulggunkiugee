using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

public class Skill : MonoBehaviour
{
    public Transform parent;
    public List<Text> Sub = new List<Text>();
    public List<Text> ButtonText = new List<Text>();
    void Start()
    {
        foreach (Transform obj in parent)
        {
            // Button 안의 Text 가져오기
            Button[] button = obj.GetComponentsInChildren<Button>(true);
            if (button != null)
            {
                Text buttonText = button[1].GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = getButtonText(ButtonText.Count);
                    ButtonText.Add(buttonText);
                }
            }

            // Skill Panel의 자식 중 Button이 아닌 Text 가져오기
            Text[] texts = obj.GetComponentsInChildren<Text>(true);
            foreach (Text t in texts)
            {
                // 부모에 Button이 없으면 일반 Text로 처리
                if (t.transform.parent.GetComponent<Button>() == null && t.transform.parent.GetComponent<Text>() != null)
                {
                    t.text = getSubText(Sub.Count);
                    Sub.Add(t);
                }
            }
        }
    }
    private string getButtonText(int index)
    {
        string result = "";
        if (GameManager.Instance.Load_Skill_Cost(index, GameManager.Instance.get_Skill_level(index)) == -1)
            result = "최대 레벨";
        else
            result = $"다이아몬드 {GameManager.Instance.Load_Skill_Cost(index, GameManager.Instance.get_Skill_level(index))}개";
        return result;
    }
    private string getSubText(int index)
    {
        string result = "";
        if (index == 7) //신이된다ㅏㅏㅏㅏ
            result = $"8초간 데미지 누적 후 {GameManager.Instance.Load_Skill_Damage(index, GameManager.Instance.get_Skill_level(index))}% 공격\n쿨타임 {GameManager.Instance.get_Skill_Cool(index)}초";
        else
            result = $"{GameManager.Instance.Load_Skill_Damage(index, GameManager.Instance.get_Skill_level(index))}% 데미지 공격\n쿨타임 {GameManager.Instance.get_Skill_Cool(index)}초";

        return result;
    }

    public void SkillUpgrade(int index)
    {
        int money = GameManager.Instance.getDiamond();
        int cost = GameManager.Instance.Load_Skill_Cost(index, GameManager.Instance.get_Skill_level(index));

        if (cost == -1) //최대레벨
            return;

        if (money < cost) //돈 부족
            return;

        money -= cost;
        GameManager.Instance.setDiamond(money);
        GameManager.Instance.UserData.skill_level[index]++;

        Sub[index].text = getSubText(index);
        ButtonText[index].text = getButtonText(index);
        Debug.Log($"스킬 레벨 업그레이드 :: {GameManager.Instance.get_Skill_level(index)}레벨");

    }
}
