using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework.Internal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public FadeInOut fadeInOut;

    public float moveSpeed = 5f; // 예시 이동속도
    public float attackRange = 3f; // 예시 공격거리(몬스터와 만나는 거리)
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

        // "Monster" 태그가 붙은 모든 오브젝트의 Transform을 리스트에 저장
        GameObject[] monsterObjects = GameObject.FindGameObjectsWithTag("Monster");
        monsters = new List<Transform>();
        // X좌표 기준 오름차순 정렬
    System.Array.Sort(monsterObjects, (a, b) =>
        a.transform.position.x.CompareTo(b.transform.position.x)
    );
        // 몬스터 인덱스 할당, 위치 조정을 여기서 해도 좋을 듯
        for (int  i = 0; i < monsterObjects.Length; i++)
        {
            if (!GameManager.Instance.isVaildStage(new Tuple<int, int>(GameManager.Instance.getStage().Item1, i)))
                continue;
            GameObject obj = monsterObjects[i];
            monsters.Add(obj.transform);
            MonsterController mc = obj.GetComponent<MonsterController>();
            if (mc != null)
            {
                mc.monsterIndex = i;
                Debug.Log($"[몬스터 등록] 이름: {obj.name}, index: {i}, X: {obj.transform.position.x}");
            }
        }

        // 몬스터 사망 이벤트 구독 추가
        MonsterController.IsMonsterDie += OnMonsterDie;
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
        PlayerHP -= monsterCtrl.GetATK();
        Debug.Log($"몬스터가 플레이어를 공격! 데미지: {monsterCtrl.GetATK()}, 플레이어 남은 체력: {PlayerHP}");
        if (PlayerHP <= 0)
        {
            PlayerDie();
        }
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
            //animator.SetBool("isMoving", true);
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
}
