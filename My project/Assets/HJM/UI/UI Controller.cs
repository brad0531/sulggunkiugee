using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using System;
using System.Data;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    public List<GameObject> ALL_UI = new List<GameObject>();
    public GameObject ATK;
    public GameObject Skill;
    public GameObject Diamond;
    private int Last_index;
    void Start()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        // 각 버튼의 GameObject를 리스트에 저장
        foreach (Button btn in allButtons)
        {
            ALL_UI.Add(btn.gameObject);
        }

        Last_index = Dialogue.Instance.index;

        if (GameManager.Instance.getAllStage().Item1 == 0) //튜토리얼
        {
            foreach (GameObject obj in ALL_UI)
            {
                obj.GetComponent<Button>().interactable = false;
            }
        }

        Debug.Log($"UI {ALL_UI.Count}개 발견");
    }

    void Update() {
        if (Last_index != Dialogue.Instance.index)
        {
            Last_index = Dialogue.Instance.index;

            if (Last_index == 5)
            {
                ATK.GetComponent<Button>().interactable = true;
            }
            else if (Last_index == 8)
            {
                ATK.GetComponent<Button>().interactable = false;
            }
            else if (Last_index == 12)
            {
                Diamond.GetComponent<Button>().interactable = true;
            }
            else if (Last_index == 13)
            {
                Diamond.GetComponent<Button>().interactable = false;
                Skill.GetComponent<Button>().interactable = true;
            }
            else if (Last_index == 14)
            {
                foreach (GameObject obj in ALL_UI)
                {
                    obj.GetComponent<Button>().interactable = true;
                }
            }
        }
    }
}
