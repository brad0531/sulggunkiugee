using UnityEngine;

public class tester : MonoBehaviour
{
    void Start()
    {
        for(int i = 0; i < 10; i++)
            Debug.Log($"{GameManager.Instance.getScript(1, true, i).Item1}: {GameManager.Instance.getScript(1, true, i).Item2}");
    }

}
