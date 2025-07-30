using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class EffectUIManager : MonoBehaviour
{
    public Transform parentPath;
    public List<Pair<Transform, int>> parentPanels = new List<Pair<Transform, int>>();

    void Start()
    {
        foreach (Transform child in parentPath)
        {
            if (child.GetComponent<Image>() != null)
            {
                parentPanels.Add(new Pair<Transform, int>(child, parentPanels.Count));
            }
        }
        Debug.Log($"효과 패널 {parentPanels.Count}개 발견하였습니다.");
    }
    public void Update()
    {
        parentPanels.Sort((Pair<Transform, int> a, Pair<Transform, int> b) =>
        {
            int A = GameManager.Instance.getRestAlcoholCoolTime(a.Second);
            int B = GameManager.Instance.getRestAlcoholCoolTime(b.Second);
            if (A < B)
                return -1;
            if (A > B)
                return 1;
            return 0;
        });

        for (int i = 0; i < parentPanels.Count; i++)
        {
            parentPanels[i].First.SetSiblingIndex(i);
            parentPanels[i].First.gameObject.SetActive(GameManager.Instance.isEffectsOn((GameManager.Alcohol_index)parentPanels[i].Second));
        }

        
    }
}
