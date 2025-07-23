using UnityEngine;
using System;
public class tester : MonoBehaviour
{
    Tuple<int, int> tmp = new Tuple<int, int>(1, 0);
    void Start()
    {
        GameManager.Instance.setStage(new Tuple<int, int>(0, 0));
        tmp = GameManager.Instance.getStage();
    }

    void Update()
    {
        Debug.Log($"{tmp.Item1} {tmp.Item2} / {GameManager.Instance.getStage().Item1} {GameManager.Instance.getStage().Item2}");
    }
}
