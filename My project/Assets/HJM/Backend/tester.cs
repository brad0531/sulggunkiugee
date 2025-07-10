using UnityEngine;

public class tester : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"현재 공격력은 {GameManager.Instance.getATK()}");
    }

}
