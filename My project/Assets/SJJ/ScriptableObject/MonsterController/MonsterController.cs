using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public delegate void MonsterDieEvent(MonsterController monster);
    public static event MonsterDieEvent IsMonsterDie;
    public int monsterhp = 1;

    public void MonsterTakeDamage(int damage)
    {
        monsterhp -= damage;
        if (monsterhp <= 0)
        {
            monsterhp = 0;
            // 몬스터 사망 이벤트
            if (IsMonsterDie != null)
                IsMonsterDie(this);
            Destroy(gameObject);
        }
    }

    public bool IsDead()
    {
        return monsterhp <= 0;
    }
}
