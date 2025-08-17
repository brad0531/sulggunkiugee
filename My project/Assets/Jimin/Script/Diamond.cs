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

    public bool inDungeon;
    public GameObject Puppet;

    #endregion
    private void Start()
    {
        _totalDamage = 0;
        diamond = 0;
        _startTime = Time.time;

        GameManager.Instance.setMonsterHP(1000000000);
        inDungeon = true;
        Debug.Log(GameManager.Instance.getDiamond());
    }
    private void Update()
    {
        if(inDungeon && Time.time - _startTime >= measurementDuration)
        { 
            PuppetMaxHP = GameManager.Instance.getMonsterMaxHP();
            PuppetcurrentHP = GameManager.Instance.getMonsterHP();
            _totalDamage = PuppetMaxHP - PuppetcurrentHP;
            diamond = (int)(_totalDamage / 10);

            SceneMoving.Instance.MoveToStage();
            AddDiamond();

            Debug.Log(GameManager.Instance.getDiamond());
        }
    }

    private void AddDiamond()
    {
        GameManager.Instance.setDiamond(GameManager.Instance.getDiamond() + diamond);
    }
}