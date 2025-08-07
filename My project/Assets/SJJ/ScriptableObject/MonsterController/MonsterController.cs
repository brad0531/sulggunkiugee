using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles monster stats, damage logic, death/respawn, and interaction with PlayerController.
/// </summary>
public class MonsterController : MonoBehaviour
{
    #region Events
    public delegate void MonsterDieEvent(MonsterController monster);
    public static event MonsterDieEvent IsMonsterDie;
    public static event Action<MonsterController> OnMonsterCompletelyDestroyed;
    #endregion

    #region Inspector Fields
    [Header("몬스터 식별 정보")]
    [Tooltip("Monster의 Stage 정보 (Main, Sub, Index)")]
    public int mainStage = 1;
    public int subStage = 0;
    public int monsterIndex = 0;

    [Header("애니메이터")]
    [SerializeField] private Animator animator;

    [Header("몬스터 능력치")]
    [SerializeField, Min(1)] private int defaultHP = 10;
    [SerializeField, Min(1)] private int defaultATK = 1;
    #endregion

    #region Private Fields
    private int _currentHP;
    private int _maxHP;
    private int _attackPower;

    private bool _isDead;
    #endregion

    #region Properties
    public bool IsDead => _isDead;
    public int GetATK() => _attackPower;
    public int GetCurrentHP() => _currentHP;
    public int GetMaxHP() => _maxHP;
    public (int, int, int) Stage => (mainStage, subStage, monsterIndex);
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Start()
    {
        InitializeMonsterStats();
    }
    #endregion

    #region Initialization
    public void InitializeMonsterStats()
    {
        GameManager.Instance.setStage(new Tuple<int, int, int>(mainStage, subStage, monsterIndex));
        GameManager.Instance.setMonster(new Tuple<int, int, int>(mainStage, subStage, monsterIndex));
        _maxHP = GameManager.Instance.getMonsterMaxHP();
        _currentHP = GameManager.Instance.getMonsterHP();
        _attackPower = GameManager.Instance.getMonsterATK();

        if (_maxHP <= 0) _maxHP = defaultHP;
        if (_currentHP <= 0) _currentHP = _maxHP;
        if (_attackPower <= 0) _attackPower = defaultATK;

        _isDead = false;

        Debug.Log($"[Monster Stats] HP={_currentHP}/{_maxHP}, ATK={_attackPower}, Stage={mainStage}-{subStage}-{monsterIndex}");
    }
    #endregion

    #region Combat & Damage
    public void MonsterTakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHP -= damage;
        _currentHP = Mathf.Max(0, _currentHP);

        Debug.Log($"[Monster Damaged] -{damage}, Remaining HP={_currentHP}, Stage={mainStage}-{subStage}-{monsterIndex}");

        if (_currentHP <= 0) Die();
    }

    public void MonsterAttackAnimation()
    {
        animator?.SetTrigger("Attack");
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        animator.ResetTrigger("Attack");
        animator?.SetTrigger("Die");
        Debug.Log($"[Monster Died] Stage {mainStage}-{subStage}-{monsterIndex}");

        GameManager.Instance.BeatMonster();
        IsMonsterDie?.Invoke(this);

        StartCoroutine(DelayedInactivate(0.5f));
    }

    private IEnumerator DelayedInactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnMonsterCompletelyDestroyed?.Invoke(this);
        gameObject.SetActive(false);
    }
    #endregion

    #region Public Methods (For External Call)
    public void Respawn((int main, int sub, int idx) stageInfo)
    {
        mainStage = stageInfo.main;
        subStage = stageInfo.sub;
        monsterIndex = stageInfo.idx;
        InitializeMonsterStats();
        if (animator != null) animator.Rebind();
        gameObject.SetActive(true);
        _isDead = false;
    }
    #endregion
}
