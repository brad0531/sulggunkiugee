using System.Collections.Generic;
using System.Threading;
using NUnit.Framework.Internal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 예시 이동속도
    public float attackRange = 3f; // 예시 공격거리(몬스터와 만나는 거리)
    public float moveDistance = 30f; // 적 처치 후 X축으로 이동할 거리(몬스터 간격)
    public int attackPower;
    public Animator animator;
    public List<Transform> monsters;             // 타겟 몬스터 Transform
    private int currentMonsterIndex = 0; // 현재 타겟 몬스터 인덱스
    private bool isAttacking = false;

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
        MoveForward();
        if (GameManager.Instance.canPlayerAttack() && GetDistanceToMonster() < attackRange)
        {
            // 공격
            //animator.SetTrigger("Attack");
            isAttacking = true;
            Attack();
            GameManager.Instance.PlayerAttack();
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
        isAttacking = false;
    }

    
    void Attack()
    {
        // 데미지 계산
        attackPower = GameManager.Instance.getATK();
        int damage = attackPower;

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
        if (CurrentMonster == null || isAttacking) return;

        float distanceX = GetDistanceToMonster();
        if (distanceX > attackRange)
        {
            // 전진
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            //animator.SetBool("isMoving", true);
        }
        
    }
    private float GetDistanceToMonster()
    {
        return Mathf.Abs(CurrentMonster.position.x - transform.position.x);
    }
}
