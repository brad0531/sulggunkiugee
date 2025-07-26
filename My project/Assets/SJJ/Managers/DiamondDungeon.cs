using UnityEngine;

public class DiamondDungeon : MonoBehaviour
{
    [SerializeField] private float measurementDuration = 5f; // 측정 구간(초)
    [SerializeField] private int baseDamage = 100;           // 100 데미지를 기준으로
    [SerializeField] private float baseTime = 1f;            // 1초 기준
    private float _startTime;
    private float _totalDamage;
    private bool _isMeasuring;
    private void Update()
    {
        if (Time.time - _startTime >= measurementDuration)
        {
            _isMeasuring = false;
            float elapsed = Time.time - _startTime;
            float dps = _totalDamage / elapsed;
            float baseDps = baseDamage / baseTime;
            float performancePercent = (dps / baseDps) * 100f;
        }
    }
}
