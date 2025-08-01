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
    List<GameObject> ALL_UI = new List<GameObject>();
    void Start(){
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        // 각 버튼의 GameObject를 리스트에 저장
        foreach (Button btn in allButtons)
        {
            ALL_UI.Add(btn.gameObject);
        }
        if (GameManager.Instance.getAllStage().Item1 == 0) //튜토리얼
        {
            foreach (GameObject obj in ALL_UI)
            {
                obj.GetComponent<Button>().interactable = false;
            }
        }
    }
}
