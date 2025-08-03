using System.IO;
using System.Linq;
using UnityEngine;

public class MonsterSaveManager : MonoBehaviour
{
    private string SavePath => Path.Combine(Application.persistentDataPath, "monsters_save.json");

    public void SaveMonsters()
    {
        var allMonsters = Object.FindObjectsByType<MonsterController>(FindObjectsSortMode.None);

        var saveList = allMonsters.Select(mon =>
        {
            return new MonsterSaveData
            {
                stage1 = mon.mainStage,
                stage2 = mon.subStage,
                stage3 = mon.monsterIndex,
                hp = mon.GetCurrentHP(),
                isDead = mon.IsDead
            };
        }).ToArray();

        var wrapper = new MonstersSaveWrapper { monsters = saveList };

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[몬스터 저장] {SavePath}");
    }

    public void LoadMonsters()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[몬스터 불러오기] 저장 파일이 없습니다.");
            return;
        }
        string json = File.ReadAllText(SavePath);
        var wrapper = JsonUtility.FromJson<MonstersSaveWrapper>(json);

        var allMonsters = Object.FindObjectsByType<MonsterController>(FindObjectsSortMode.None);
        foreach (var saved in wrapper.monsters)
        {
            var monster = allMonsters.FirstOrDefault(mon =>
                mon.mainStage == saved.stage1 &&
                mon.subStage == saved.stage2 &&
                mon.monsterIndex == saved.stage3
            );

            if (monster != null)
            {
                typeof(MonsterController).GetField("MonstercurrentHP", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                         .SetValue(monster, saved.hp);

                if (saved.isDead && !monster.IsDead)
                {
                    monster.MonsterTakeDamage(monster.GetCurrentHP());
                }

                Debug.Log($"[몬스터 복원] {saved.stage1}-{saved.stage2}-{saved.stage3}, HP: {saved.hp}");
            }
            else
                Debug.LogWarning($"[저장된 몬스터를 찾지 못함] stage={saved.stage1}-{saved.stage2}-{saved.stage3}");
        }
    }
}
