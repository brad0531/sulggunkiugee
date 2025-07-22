using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework.Internal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public FadeInOut fadeInOut;

    public float moveSpeed = 100f; // 예시 이동속도
    public float attackRange = 100f; // 예시 공격거리(몬스터와 만나는 거리)
    public float moveDistance = 30f; // 적 처치 후 X축으로 이동할 거리(몬스터 간격)
    public int PlayerHP;
    public int PlayerattackPower;
    public Animator animator;
    public List<Transform> monsters;             // 타겟 몬스터 Transform
    private int currentMonsterIndex = 0; // 현재 타겟 몬스터 인덱스
    private bool isPlayerAttacking = false;
    private bool isMonsterAttacking = false;
    private bool isPlayerDead = false;

    Transform CurrentMonster
    {
        get
        {
            if (monsters == null || monsters.Count == 0) return null;
            if (currentMonsterIndex >= monsters.Count) return null;
            return monsters[currentMonsterIndex];
        }
    }
    void Start()
    {
        // 플레이어 스탯 설정
        PlayerHP = GameManager.Instance.getHP();
        PlayerattackPower = GameManager.Instance.getATK();
        Debug.Log($"플레이어의 체력과 공격력을 불러옵니다. 현재 체력은 {PlayerHP}, 공격력은 {PlayerattackPower}입니다.");
        RegisterMonsters();
    }
    void Update()
    {
        // 사망 판별
        if (isPlayerDead) return;
        
        MoveForward();
        // 플레이어가 몬스터를 공격
        if (GameManager.Instance.canPlayerAttack() && GetDistanceToMonster() < attackRange)
        {
            //animator.SetTrigger("Attack");
            isPlayerAttacking = true;
            PlayerAttack();
            GameManager.Instance.PlayerAttack();
        }
        // 몬스터가 플레이어를 공격
        if (GameManager.Instance.canMonsterAttack() && GetDistanceToMonster() < attackRange)
        {
            isMonsterAttacking = true;
            MonsterAttack();
            GameManager.Instance.MonsterAttack();
        }
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        MonsterController.IsMonsterDie -= OnMonsterDie;
    }

    void OnMonsterDie(MonsterController deadMonster)
    {
        currentMonsterIndex++;
        isPlayerAttacking = false;
    }

    void MonsterAttack()
    {
        MonsterController monsterCtrl = CurrentMonster.GetComponent<MonsterController>();
        PlayerHP = GameManager.Instance.getHP();
        // 유저가 강화하는 걸 대비, HP 설정 후 뎀지 계산
        PlayerHP -= monsterCtrl.GetATK();
        Debug.Log($"몬스터가 플레이어를 공격! 데미지: {monsterCtrl.GetATK()}, 플레이어 남은 체력: {PlayerHP}");
        if (PlayerHP <= 0)
        {
            PlayerDie();
        }
        GameManager.Instance.setHP(PlayerHP);
    }
    void PlayerAttack()
    {
        // 데미지 계산
        PlayerattackPower = GameManager.Instance.getATK();
        
        int damage = PlayerattackPower;

        // 몬스터에 데미지 적용
        MonsterController monsterCtrl = CurrentMonster.GetComponent<MonsterController>();
        if (monsterCtrl != null && !monsterCtrl.IsDead())
        {
            monsterCtrl.MonsterTakeDamage(damage);
        }
        // 공격 애니메이션 실행
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 콘솔에 데미지 결과 출력
        Debug.Log($"플레이어가 몬스터를 공격! 데미지: {damage}, 몬스터 남은 체력: {monsterCtrl.GetCurrentHP()}");
    }

    void MoveForward()
    {
        if (CurrentMonster == null || isPlayerAttacking) return;

        float distanceX = GetDistanceToMonster();
        if (distanceX > attackRange)
        {
            // 전진
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
            animator.SetTrigger("Attack");
        }
    }
    private void PlayerDie()
    { 
        // 중복 실행 방지
        if (isPlayerDead)
        {
            return;
        }
        PlayerHP = 0;
        Debug.Log("플레이어가 사망했습니다. 전투를 중지합니다.");
        isPlayerDead = true;
        // 페이드 인 (미완)
        if (fadeInOut != null)
            StartCoroutine(fadeInOut.FadeIn());
    }
    private float GetDistanceToMonster()
    {
        return Mathf.Abs(CurrentMonster.position.x - transform.position.x);
    }

    private void RegisterMonsters()
    {
        // 현재 스테이지 번호 가져오기
        int stageNum = GameManager.Instance.getStage().Item1;
        int monsterIndex = 0;

        // "Monster" 태그를 가진 모든 오브젝트 가져오기
        GameObject[] monsterObjects = GameObject.FindGameObjectsWithTag("Monster");
        monsters = new List<Transform>();

        // X좌표 기준 오름차순 정렬
        System.Array.Sort(monsterObjects, (a, b) =>
            a.transform.position.x.CompareTo(b.transform.position.x)
        );

        for (int i = 0; i < monsterObjects.Length; i++)
        {
            // 유효한 (stageNum, monsterIndex)를 찾을 때까지 다음 스테이지로 넘김
            while (!GameManager.Instance.isVaildStage(new Tuple<int, int>(stageNum, monsterIndex)))
            {
                stageNum++;
                monsterIndex = 0;

                // 스킵
                if (!GameManager.Instance.isVaildStage(new Tuple<int, int>(stageNum, monsterIndex)))
                {
                    Debug.LogWarning($"[스테이지 등록 실패] (stage: {stageNum}, index: {monsterIndex})는 유효하지 않은 몬스터 데이터입니다.");
                    return; 
                }
            }

            GameObject obj = monsterObjects[i];
            monsters.Add(obj.transform);

            MonsterController mc = obj.GetComponent<MonsterController>();
            if (mc != null)
            {
                mc.stageNum = stageNum;
                mc.monsterIndex = monsterIndex;

                Debug.Log($"[몬스터 등록] 이름: {obj.name}, 스테이지: {stageNum}, 인덱스: {monsterIndex}, X: {obj.transform.position.x}");
            }

            monsterIndex++;
        }

        // 몬스터 사망 이벤트 구독 추가
        MonsterController.IsMonsterDie -= OnMonsterDie;
        MonsterController.IsMonsterDie += OnMonsterDie;
    }

}
