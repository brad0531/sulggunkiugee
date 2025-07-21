using UnityEngine;

public class tester : MonoBehaviour
{
    void Start()
    {
        for (int t = 1; t <= 6; t++)
        {
            //for (int i = 0; GameManager.Instance.getScript(t, true, i).Item1 != "오류 발생:: index 초과"; i++)
            //    Debug.Log($"{GameManager.Instance.getScript(t, true, i).Item1}: {GameManager.Instance.getScript(t, true, i).Item2}");
            //for (int i = 0; GameManager.Instance.getScript(t, false, i).Item1 != "오류 발생:: index 초과"; i++)
            //    Debug.Log($"{GameManager.Instance.getScript(t, false, i).Item1}: {GameManager.Instance.getScript(t, false, i).Item2}");
        }
    }

}
