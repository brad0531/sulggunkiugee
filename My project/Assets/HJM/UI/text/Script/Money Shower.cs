using UnityEngine;
using UnityEngine.UI;
public class MoneyShower : MonoBehaviour
{
    public Text money, liver, diamond;
    // Update is called once per frame
    void Update()
    {
        money.text = $"{GameManager.Instance.getMoney()}";
        liver.text = $"{GameManager.Instance.getLiver()}";
        diamond.text = $"{GameManager.Instance.getDiamond()}";
    }
}
