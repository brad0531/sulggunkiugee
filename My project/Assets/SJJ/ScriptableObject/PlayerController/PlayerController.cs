using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NUnit.Framework.Constraints;
using Unity.Android.Types;
using UnityEngine;

/// <summary>
/// Handles player movement, attacking, and interactions with monsters.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    #region Inspector Fields
    [Header("플레이어 스탯")]
    [SerializeField, Min(0f)] private float moveSpeed = 3f;
    [SerializeField, Min(0f)] private float attackRange = 1f;

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
    private int _originalAttackPower; // 버프 해제를 위해 원본 저장
    private int _currentMonsterIndex;
    private int _currentStage;
    private Vector3 respawnPosition;

    private bool _isPlayerAttacking;
    private bool _isMonsterAttacking;
    private bool _isPlayerDead;
    #endregion

    #region Properties
    public bool IsPlayerDead => _isPlayerDead;

    private Transform CurrentMonster =>
        (_currentMonsterIndex >= 0 && _currentMonsterIndex < monsters.Count) ? monsters[_currentMonsterIndex] : null;
    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        // 싱글턴(오브젝트가 중복되지 않고 하나만 존재하도록) 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //애니메이터 자동할당
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CSVloading();
        InitializePlayerStats();
        RegisterMonsters();
    }

    private void Update()
    {
        if (_isPlayerDead) return;

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
        _originalAttackPower = _playerAttackPower; // 버프 복원용 저장
        respawnPosition = transform.position;
    }

    public void RegisterMonsters()
    {
        monsters.Clear();

        var allMonsters = GameObject.FindGameObjectsWithTag("enemy");
        Array.Sort(allMonsters, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        for (int i = 0; i < allMonsters.Length; i++)
        {
            allMonsters[i].SetActive(true);
        }

        int mainStage = 1;
        int subStage = 0;
        int monsterIdx = 0;

        foreach (var monsterObj in allMonsters)
        {
            // 유효한 스테이지인지 확인
            while (!GameManager.Instance.isVaildStage(new Tuple<int, int, int>(mainStage, subStage, monsterIdx)))
            {
                monsterIdx = 0;
                subStage++;
                if (!GameManager.Instance.isVaildStage(new Tuple<int, int, int>(mainStage, subStage, monsterIdx)))
                    return;
            }

            monsters.Add(monsterObj.transform);

            var ctrl = monsterObj.GetComponent<MonsterController>();
            if (ctrl != null)
            {
                ctrl.mainStage = mainStage;
                ctrl.subStage = subStage;
                ctrl.monsterIndex = monsterIdx;
                ctrl.InitializeMonsterStats();
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
        if (!GameManager.Instance.canPlayerAttack() || Vector3.Distance(CurrentMonster.position, transform.position) > attackRange)
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
        _isPlayerAttacking = false;
    }

    private void ApplyPlayerDamage()
    {
        _playerAttackPower = GameManager.Instance.getATK();
        DealDamageToMonster(CurrentMonster, _playerAttackPower, true);
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
        if (ctrl != null && !ctrl.IsDead)
        {
            ctrl.MonsterAttackAnimation();

            _playerHP = GameManager.Instance.getHP() - ctrl.GetATK();
            GameManager.Instance.setHP(_playerHP);
            Debug.Log($"[Monster Attack] Damage={ctrl.GetATK()}, Player HP={_playerHP}");

            if (_playerHP <= 0)
                HandlePlayerDeath();
        }

        GameManager.Instance.MonsterAttack();
        _isMonsterAttacking = false;
    }

    private void DealDamageToMonster(Transform monster, int damage, bool isNormalAttack)
    {
        if (monster == null) return;

        var ctrl = monster.GetComponent<MonsterController>();
        if (ctrl != null && !ctrl.IsDead)
        {
            ctrl.MonsterTakeDamage(damage);
            if (isNormalAttack && _isSulshinActive)
                UpdateSulshinDamage(damage);
        }
    }

    #endregion

    #region Skills
    public static PlayerController Instance { get; private set; }



    private bool _isSulshinActive = false;
    private double _sulshinAccumulatedDamage = 0;
    private bool _isSulshinDamageReady = false;
    private Coroutine _sulshinCoroutine;

    public void SkillActive(int index, int weight, int hitcount)
    {
        if (CurrentMonster == null) return;

        if (index >= 0 && index < 7)
        {
            StartCoroutine(RepeatedSkillAttack(weight, hitcount));
        }
        else if (index == 7)
        {
            if (!_isSulshinActive && !_isSulshinDamageReady)
            {
                _isSulshinActive = true;
                _sulshinAccumulatedDamage = 0;
                _sulshinCoroutine = StartCoroutine(SulshinBuffRoutine());
            }
            else if (_isSulshinDamageReady)
            {
                double finalDamage = _sulshinAccumulatedDamage * (weight / 100.0);
                DealDamageToMonster(CurrentMonster.transform, (int)finalDamage, false);
                _isSulshinDamageReady = false;
                _sulshinAccumulatedDamage = 0;
            }
        }
    }
    private IEnumerator RepeatedSkillAttack(int weight, int hitcount)
    {
        for (int i = 0; i < hitcount; i++)
        {
            double skillDamage = _playerAttackPower * (weight / 100.0);
            DealDamageToMonster(CurrentMonster.transform, (int)skillDamage, false);
            UpdateSulshinDamage(skillDamage);

            if (i < hitcount - 1)
                yield return new WaitForSeconds(0.2f);
        }
    }

    private void UpdateSulshinDamage(double damage)
    {
        if (_isSulshinActive)
            _sulshinAccumulatedDamage += damage;
    }

    private IEnumerator SulshinBuffRoutine()
    {
        _originalAttackPower = _playerAttackPower; // 원본 저장
        _playerAttackPower = (int)(_playerAttackPower * 1.2);

        float timer = 0f;
        while (timer < 8f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _playerAttackPower = _originalAttackPower; // 원래 값 복원
        _isSulshinActive = false;
        _isSulshinDamageReady = true;
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

    private void OnMonsterDie(MonsterController dead) => _isPlayerAttacking = false;
    private void OnMonsterDestroyed(MonsterController dead) { _currentMonsterIndex++; _isPlayerAttacking = false; }

    private void HandlePlayerDeath()
    {
        if (_isPlayerDead) return;
        _isPlayerDead = true;
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Die");
        if (fadeInOut != null) StartCoroutine(fadeInOut.FadeIn());
        StartCoroutine(PlayerRespawnDelay(2.0f));
    }

    public void HandlePlayerRespawn()
    {
        RegisterMonsters();
        _playerHP = GameManager.Instance.getMaxHP();
        transform.position = respawnPosition;
        _currentMonsterIndex = 0;
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

    #region CSV Data
    [System.Serializable]
    public class ActData { public string Name; public string Act; }
    public List<ActData> Info_Acts = new List<ActData>();

    public void CSVloading() => LoadActs();

    private int LoadActs()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Acts/act.csv");
        if (!File.Exists(path))
            return 1;

        using (StreamReader sr = new StreamReader(path))
        {
            sr.ReadLine(); // 헤더
            while (!sr.EndOfStream)
            {
                string[] values = sr.ReadLine().Split(',');
                if (values.Length < 2) continue;
                Info_Acts.Add(new ActData { Name = values[0], Act = values[1] });
            }
        }
        return 0;
    }
    #endregion
}