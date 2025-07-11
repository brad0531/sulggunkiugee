using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 예시 이동속도
    public float attackRange = 3f; //예시 공격거리(몬스터와 만나는 거리)
    public int attackPower;
    public Animator animator;
    public Transform monster;             // 타겟 몬스터 Transform
    private bool isAttacking = false;
    void Start()
    {
        GameObject found = GameObject.FindWithTag("Monster");
        if (found != null)
            monster = found.transform;
    }
    void Update()
    {
        MoveForward();

    }
    void Attack()
    {
        // 데미지 계산
        attackPower = GameManager.Instance.getATK();
        int damage = attackPower;
        // 공격 애니메이션 실행
        if (animator != null)
        {
           animator.SetTrigger("Attack");
        }
        // 콘솔에 데미지 결과 출력
        Debug.Log($"플레이어가 몬스터를 공격! 데미지: {damage}, 몬스터 남은 체력: currentmosterHP - damage");
        
    }

    void MoveForward()
    {
        if (monster == null || isAttacking) return;

        float distanceX = GetDistanceToMonster();
        if (distanceX > attackRange)
        {
            // 전진
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            //animator.SetBool("isMoving", true);
        }
        else
        {
            // 공격
            //animator.SetTrigger("Attack");
            isAttacking = true;
            Attack();
        }
    }
    private float GetDistanceToMonster()
    {
        return Mathf.Abs(monster.position.x - transform.position.x);
    }
}
