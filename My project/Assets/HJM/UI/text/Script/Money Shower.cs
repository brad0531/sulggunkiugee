using UnityEngine;
using UnityEngine.UI;
public class MoneyShower : MonoBehaviour
{
    public Text money;
    // Update is called once per frame
    void Update()
    {
        money.text = $"{GameManager.Instance.getMoney()}원";
    }
}
