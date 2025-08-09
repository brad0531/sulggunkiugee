using UnityEngine;
using UnityEngine.UI;
public class MoneyShower : MonoBehaviour
{
    public Text money, liver;
    // Update is called once per frame
    void Update()
    {
        money.text = $"{GameManager.Instance.getMoney()}골드";
        liver.text = $"간수치 : {GameManager.Instance.getLiver()}";
    }
}
