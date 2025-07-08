using UnityEngine;
using UnityEngine.UI;
public class HPBar_controller : MonoBehaviour
{
    Slider mySlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mySlider.value = GameManager.Instance.getHP();
        mySlider.maxValue = GameManager.Instance.getMaxHP();
    }

    // Update is called once per frame
    void Update()
    {
        mySlider.value = GameManager.Instance.getHP();
        mySlider.maxValue = GameManager.Instance.getMaxHP();
    }
}
