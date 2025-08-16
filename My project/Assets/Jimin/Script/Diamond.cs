using UnityEngine;

public class Diamond : MonoBehaviour
{
    #region private field

    [SerializeField] private float measurementDuration = 30f; // 측정 구간(초)
    [SerializeField] private int baseDamage = 100;           // 100 데미지를 기준으로
    [SerializeField] private float baseTime = 1f;            // 1초 기준
    [SerializeField] private float diamondPercentage = 10f;  // 다이아 퍼센트

    private float _startTime;
    private float _totalDamage;
    private bool _isMeasuring;
    private int diamond;
    private int PuppetMaxHP;
    private int PuppetcurrentHP;

    #endregion
    private void Start()
    {

    }
    private void Update()
    {
        _totalDamage = PuppetMaxHP - PuppetcurrentHP;
        diamond = (int)(_totalDamage / 10.0);
    }

    private void AddDiamond()
    {
        GameManager.Instance.setDiamond(GameManager.Instance.getDiamond() + diamond);
    }
}