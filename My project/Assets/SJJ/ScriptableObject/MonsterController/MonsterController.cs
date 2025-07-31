using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEditor.EditorTools;
using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;


public class MonsterController : MonoBehaviour
{
    public delegate void MonsterDieEvent(MonsterController monster);
    public static event MonsterDieEvent IsMonsterDie;
    public static event Action<MonsterController> OnMonsterCompletelyDestroyed;


    [Header("몬스터 식별 정보")]
    public (int, int, int) stage = (1, 0, 0);
    public int count = 50; // 몬스터 개수

    #region Private Fields

    private int MonstercurrentHP;
    private int MonstermaxHP;
    private int MonsterattackPower;

    #endregion
    void Start()
    {
        SetMonsterStats();
    }
    public void SetMonsterStats()
    {
        GameManager.Instance.setStage(new Tuple<int, int, int>(stage.Item1, stage.Item2, stage.Item3));
        GameManager.Instance.setMonster(new Tuple<int, int, int>(stage.Item1, stage.Item2, stage.Item3));
        MonstermaxHP = GameManager.Instance.getMonsterMaxHP();
        MonstercurrentHP = GameManager.Instance.getMonsterHP();
        MonsterattackPower = GameManager.Instance.getMonsterATK();
        Debug.Log($"몬스터의 체력과 공격력을 불러옵니다. 현재 체력은 {MonstercurrentHP}, 공격력은 {MonsterattackPower}입니다.");
    }
    public void MonsterTakeDamage(int damage)
    {
        MonstercurrentHP -= damage;
        MonstercurrentHP = Mathf.Max(0, MonstercurrentHP);
        if (MonstercurrentHP <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log($"[몬스터 사망] 스테이지 {stage.Item1}-{stage.Item2}-{stage.Item3}");
        // GameManager.instance.BeatMonster();
        if (IsMonsterDie != null)
            IsMonsterDie(this);
        // 0.5초 지연
        StartCoroutine(DelayedInactivate(0.5f));
    }
    private IEnumerator DelayedInactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (OnMonsterCompletelyDestroyed != null)
            OnMonsterCompletelyDestroyed(this);

        gameObject.SetActive(false);
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
