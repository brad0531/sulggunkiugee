using System;
using UnityEngine;

[Serializable]
public class MonsterSaveData
{
    public int stage1;
    public int stage2;
    public int stage3;
    public int hp;
    public bool isDead;
}

[Serializable]
public class MonstersSaveWrapper
{
    public MonsterSaveData[] monsters;
}
