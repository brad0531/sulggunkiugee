using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Data;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Playables;

public class StageBaseManager : MonoBehaviour
{
    public Transform Target; //패널 확장/축소 대
    public Text StageShower;
    public Transform CloneParentTarget;
    public GameObject original; // 복제할 원본 오브젝트
    private List<GameObject> clones = new List<GameObject>(); // 복제한 오브젝트 저장 리스트
    public void OnClick()
    {
        Target.gameObject.SetActive(!Target.gameObject.activeInHierarchy);
        return;
    }
    public void StageMove(int Main, int Sub)
    {
        Debug.Log($"{Main}-{Sub}으로 이동하고 싶습니다.");
        if (GameManager.Instance.UserData.Max_stage.Main > Main || (GameManager.Instance.UserData.Max_stage.Main == Main && GameManager.Instance.UserData.Max_stage.Sub > Sub))
            return;
        GameManager.Instance.setStage(new Tuple<int, int, int> (Main, Sub, 0));
    }
    public void Update()
    {
        StageShower.text = $"{GameManager.Instance.getStage().Item1}-{GameManager.Instance.getStage().Item2}";
    }
    public void Start()
    {

        for (int i = 1; i <= 6; i++)
        {
            for (int t = 0; t <= 6; t++)
            {
                int A = i;
                int B = t;
                GameObject newClone = Instantiate(original, CloneParentTarget);

                Text textComponent = newClone.GetComponentInChildren<Text>();
                if (textComponent != null)
                {
                    textComponent.text = $"{i}-{t}";
                }

                Button buttonComponent = newClone.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => { this.StageMove(A, B); });
                }

                // 리스트에 추가
                clones.Add(newClone);
            }
        }

        original.SetActive(false);
    }
}
