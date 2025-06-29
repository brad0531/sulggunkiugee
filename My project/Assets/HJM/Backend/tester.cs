using UnityEngine;

public class tester : MonoBehaviour
{
    void Start()
    {
        var data = CSVManager.Instance.Data;
        foreach (var row in data)
        {
            Debug.Log(string.Join(" | ", row));
        }
    }

}
