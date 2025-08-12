using UnityEngine;
using UnityEngine.UI;
public class MoneyShower : MonoBehaviour
{
    public Text money, liver;
    // Update is called once per frame
    void Update()
    {
        money.text = $"{GameManager.Instance.getMoney()}";
        liver.text = $"{GameManager.Instance.getLiver()}";
    }
}
