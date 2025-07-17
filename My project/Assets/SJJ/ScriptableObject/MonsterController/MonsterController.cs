using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEditor.EditorTools;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public delegate void MonsterDieEvent(MonsterController monster);
    public static event MonsterDieEvent IsMonsterDie;

    [Header("몬스터 식별 정보")]
    public int stageNum = 1;     // 스테이지 번호 (예: 1)
    public int monsterIndex = 0; // 몬스터 인덱스
    public int count = 10; // 몬스터 개수

    private int currentHP;
    private int maxHP;
    private int attackPower;
    void Start()
    {
        // GameManager에서 몬스터 스탯 설정
        //GameManager.Instance.setMonster(new System.Tuple<int, int>(stageNum, monsterIndex));
        // 아니 왜 안되는거에요 대체

        // 몬스터 스탯
        maxHP = 2;
        currentHP = maxHP;
        attackPower = 1; // 1은 모두 예시.

        //Debug.Log($"[몬스터 생성] 스테이지 {stageNum}-{monsterIndex} | HP: {currentHP}, ATK: {attackPower}");
    }
    public void MonsterTakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);
        // GameManager.Instance.setMonsterHP(currentHP); 왜 버그가 나지 일단 주석처리 해놓을게.
        if (currentHP <= 0)
        {
            Die();
        }
        //Debug.Log($"[몬스터 피격] 데미지: {damage}, 남은 체력: {currentHP}");
    }
    private void Die()
    {
        Debug.Log($"[몬스터 사망] 스테이지 {stageNum}-{monsterIndex}");
        // 이벤트로 플레이어에게 알림
        if (IsMonsterDie != null)
            IsMonsterDie(this);
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }
    public int GetATK()
    {
        return attackPower;
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }
}
