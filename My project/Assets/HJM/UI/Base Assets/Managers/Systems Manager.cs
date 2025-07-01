using UnityEngine;

public class UISwitchManager : MonoBehaviour
{
    public GameObject[] panels; // Systems Base 하위 패널들을 이 배열에 넣음

    public void ShowPanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
        }
    }
}
