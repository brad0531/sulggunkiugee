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

    private int MonstercurrentHP;
    private int MonstermaxHP;
    private int MonsterattackPower;
    void Update()
    {
        // 몬스터 스탯 세팅
        GameManager.Instance.setMonster(new System.Tuple<int, int>(stageNum, monsterIndex));
        MonstermaxHP = GameManager.Instance.getMonsterMaxHP();
        MonstercurrentHP = GameManager.Instance.getMonsterHP();
        MonsterattackPower = GameManager.Instance.getMonsterATK();
    }
    public void MonsterTakeDamage(int damage)
    {
        MonstercurrentHP -= damage;
        MonstercurrentHP = Mathf.Max(0, MonstercurrentHP);
        if (MonstercurrentHP <= 0)
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
        return MonstercurrentHP <= 0;
    }
    public int GetATK()
    {
        return MonsterattackPower;
    }

    public int GetCurrentHP()
    {
        return MonstercurrentHP;
    }

    public int GetMaxHP()
    {
        return MonstermaxHP;
    }
}
