using UnityEngine;

public class Diamond : MonoBehaviour
{
    #region private field

    [SerializeField] private float measurementDuration = 5f; // 측정 구간(초)
    [SerializeField] private int baseDamage = 100;           // 100 데미지를 기준으로
    [SerializeField] private float baseTime = 1f;            // 1초 기준
    [SerializeField] private float diamondPercentage = 10f;  // 다이아 퍼센트

    private float _startTime;
    private float _totalDamage;
    private bool _isMeasuring;
    private int diamond;

    #endregion
    private void Start()
    {
        diamond = GameManager.Instance.getDiamond();
        Debug.Log($"[Diamond] = {diamond}");
    }
    private void Update()
    {
        if (Time.time - _startTime >= measurementDuration && _isMeasuring)
        {
            _isMeasuring = false;
            // _totalDamage = GameManager.Instance.
            float elapsed = Time.time - _startTime;
            float dps = _totalDamage / elapsed;
            float baseDps = baseDamage / baseTime;
            float performancePercent = (dps / baseDps) * 100f;
            diamond = (int)(_totalDamage / diamondPercentage);
        }
        else if (Time.time - _startTime >= measurementDuration && !_isMeasuring)
        {
            _isMeasuring = true;
            AddDiamond();
        }
    }

    private void AddDiamond()
    {
        GameManager.Instance.setDiamond(GameManager.Instance.getDiamond() + diamond);
    }
}