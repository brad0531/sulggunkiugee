using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Handles player movement, attacking, and interactions with monsters.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    #region Inspector Fields
    [Header("플레이어 스탯")]
    [SerializeField, Min(0f)] private float moveSpeed = 500f;
    [SerializeField, Min(0f)] private float attackRange = 200f;
    [SerializeField, Min(0f)] private float moveDistanceAfterKill = 30f;

    [Header("애니메이터")]
    [SerializeField] private Animator animator;

    [Header("이펙트 & UI")]
    [SerializeField] private FadeInOut fadeInOut;

    [Header("몬스터 설정")]
    [SerializeField] private List<Transform> monsters = new List<Transform>();
    #endregion

    #region Private Fields
    private int _playerHP;
    private int _playerAttackPower;
    private int _currentMonsterIndex;
    private int _currentStage;
    private Vector3 respawnPosition;

    private bool _isPlayerAttacking;
    private bool _isMonsterAttacking;
    private bool _isPlayerDead;
    #endregion

    #region Properties
    public bool IsPlayerDead => _isPlayerDead;

    private Transform CurrentMonster
    {
        get
        {
            if (_currentMonsterIndex < 0 || _currentMonsterIndex >= monsters.Count)
                return null;
            return monsters[_currentMonsterIndex];
        }
    }
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        InitializePlayerStats();
        RegisterMonsters();
    }

    private void Update()
    {
        if (_isPlayerDead)
            return;

        if (!_isPlayerAttacking)
            HandleMovement();

        TryPlayerAttack();
        TryMonsterAttack();
    }

    private void OnDestroy()
    {
        MonsterController.IsMonsterDie -= OnMonsterDie;
        MonsterController.OnMonsterCompletelyDestroyed -= OnMonsterDestroyed;
    }
    #endregion

    #region Initialization
    private void InitializePlayerStats()
    {
        _playerHP = GameManager.Instance.getHP();
        _playerAttackPower = GameManager.Instance.getATK();
        Debug.Log($"[Player Stats] HP={_playerHP}, ATK={_playerAttackPower}");

        respawnPosition = transform.position; // 스폰 위치 저장
        Debug.Log($"[RespwanPoint] = {respawnPosition}");
    }

    private void RegisterMonsters()
    {

        var stageInfo = GameManager.Instance.getStage();
        int stageNum = stageInfo.Item1;
        int monsterIdx = 0;
        foreach (Transform monsterobj in monsters)
        {
            monsterobj.gameObject.SetActive(true);
        }
        var allMonsters = GameObject.FindGameObjectsWithTag("Monster");
        Array.Sort(allMonsters, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        
        monsters.Clear();
        foreach (var monsterObj in allMonsters)
        {
            while (!GameManager.Instance.isVaildStage(new Tuple<int, int>(stageNum, monsterIdx)))
            {
                stageNum++;
                monsterIdx = 0;
                if (!GameManager.Instance.isVaildStage(new Tuple<int, int>(stageNum, monsterIdx)))
                {
                    Debug.LogWarning($"[Stage Load Failed] stage={stageNum}, index={monsterIdx}");
                    return;
                }
            }

            monsters.Add(monsterObj.transform);
            var ctrl = monsterObj.GetComponent<MonsterController>();
            if (ctrl != null)
            {
                ctrl.stageNum = stageNum;
                ctrl.MonsterIndex = monsterIdx;
                Debug.Log($"[Monster Registered] {monsterObj.name} at Stage {stageNum}, Index {monsterIdx}");
            }
            monsterIdx++;
        }

        MonsterController.IsMonsterDie += OnMonsterDie;
        MonsterController.OnMonsterCompletelyDestroyed += OnMonsterDestroyed;
    }
    #endregion

    #region Movement & Attacks
    private void HandleMovement()
    {
        if (CurrentMonster == null)
        {
            animator.SetBool("isRunning", false);
            return;
        }

        float distance = Vector3.Distance(CurrentMonster.position, transform.position);
        bool shouldRun = distance >= attackRange;
        animator.SetBool("isRunning", shouldRun);

        if (shouldRun)
            transform.Translate(Vector3.right * (moveSpeed * Time.deltaTime));
    }

    private void TryPlayerAttack()
    {
        if (_isPlayerAttacking || CurrentMonster == null) return;

        float distance = Vector3.Distance(CurrentMonster.position, transform.position);
        if (!GameManager.Instance.canPlayerAttack() || distance > attackRange)
            return;

        StartCoroutine(PlayerAttackSequence());
    }

    private IEnumerator PlayerAttackSequence()
    {
        _isPlayerAttacking = true;
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(GetAnimationLength("Attack"));
        ApplyPlayerDamage();

        transform.Translate(Vector3.right * moveDistanceAfterKill);
        _isPlayerAttacking = false;
    }

    private void ApplyPlayerDamage()
    {
        _playerAttackPower = GameManager.Instance.getATK();
        int damage = _playerAttackPower;

        var ctrl = CurrentMonster.GetComponent<MonsterController>();
        if (ctrl != null && !ctrl.IsDead())
        {
            ctrl.MonsterTakeDamage(damage);
            Debug.Log($"[Player Attack] Damage={damage}, Monster HP={ctrl.GetCurrentHP()}");
        }
        GameManager.Instance.PlayerAttack();
    }

    private void TryMonsterAttack()
    {
        if (_isMonsterAttacking || CurrentMonster == null) return;

        float distance = Vector3.Distance(CurrentMonster.position, transform.position);
        if (!GameManager.Instance.canMonsterAttack() || distance > attackRange)
            return;

        StartCoroutine(MonsterAttackSequence());
    }

    private IEnumerator MonsterAttackSequence()
    {
        _isMonsterAttacking = true;
        yield return new WaitForSeconds(0.5f);

        var ctrl = CurrentMonster.GetComponent<MonsterController>();
        if (ctrl != null && !ctrl.IsDead())
        {
            _playerHP = GameManager.Instance.getHP() - ctrl.GetATK();
            GameManager.Instance.setHP(_playerHP);
            Debug.Log($"[Monster Attack] Damage={ctrl.GetATK()}, Player HP={_playerHP}");

            if (_playerHP <= 0)
                HandlePlayerDeath();
        }

        GameManager.Instance.MonsterAttack();
        _isMonsterAttacking = false;
    }
    #endregion

    #region Utilities & Events
    private float GetAnimationLength(string animName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == animName)
                return clip.length;
        return 1f;
    }

    private void OnMonsterDie(MonsterController dead)
    {
        _isPlayerAttacking = false;
    }

    private void OnMonsterDestroyed(MonsterController dead)
    {
        _currentMonsterIndex++;
        _isPlayerAttacking = false;
    }

    private void HandlePlayerDeath()
    {
        if (_isPlayerDead) return;
        _isPlayerDead = true;
        animator.SetTrigger("Die");
        Debug.Log("[Player Died]");

        if (fadeInOut != null)
        {
            StartCoroutine(fadeInOut.FadeIn());
        }
        StartCoroutine(PlayerRespawnDelay(1.0f));
    }

    public void HandlePlayerRespawn()
    {
        RegisterMonsters();                                         // 몬스터 재등록
        _playerHP = GameManager.Instance.getMaxHP();               // HP 초기화
        transform.position = respawnPosition;                      // 위치 복귀
        _currentMonsterIndex = 0;                                  // 인덱스 초기화
        _isPlayerDead = false;
    }

    private IEnumerator PlayerRespawnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentStage = Mathf.Max(1, _currentStage - 1);
        GameManager.Instance.setStage(new Tuple<int, int>(_currentStage, 0));
        HandlePlayerRespawn();
    }
    #endregion

}
