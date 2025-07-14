using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;
using System.Data;
using System.Linq.Expressions;
public class Utility
{
    public long get_times() //utc 시간 (표준 시간)
    {
        return DateTime.UtcNow.Ticks;
    }
}
public class GameManager : MonoBehaviour
{

    #region 데이터 선언
    private Utility utility = new Utility();
    public static GameManager Instance { get; private set; }
    private bool isTesting = true;
    [Header("CSV 파일 상대 경로 (StreamingAssets 기준)")]
    public string costFileName = "Costs/Cost_CSV.csv"; //코스트 데이터
    public string UserData_FilePath = "UserData/UserData.json";
    private List<KeyValuePair<string, int>[]> csvData = new List<KeyValuePair<string, int>[]>();
    public List<List<int>> CostData = new List<List<int>>();
    public UserData_type UserData = new UserData_type(); //유저 데이터 저장 변수
    private List<int> ATK_levels_lists = new List<int>();
    private List<int> HP_levels_lists = new List<int>();
    private List<double> ATKspeed_levels_lists = new List<double>();
    private List<double> CRIpercent_levels_lists = new List<double>();
    private Dictionary<Pair<int, int>, Pair<int, int>> Monster_lists = new Dictionary<Pair<int, int>, Pair<int, int>>();
    #endregion


    private void Awake()
    {
        // 싱글턴(오브젝트가 중복되지 않고 하나만 존재하도록) 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경에도 유지
            LoadData_all();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    #region csv데이터 로딩/세이브
    public void LoadData_all()
    {
        int result = 0;
        result += LoadCost();
        result += LoadATKdatas();
        result += LoadATKspeeddatas();
        result += LoadCRIpercentdatas();
        result += LoadHPdatas();
        result += LoadMonsterdatas();
        result += LoadUserData();
        if (result > 0)
            Debug.LogError($"----------------------------------------\n데이터 불러오는 중 오류 발생 :: {result}개\n");
    }
    public void Setting()
    {
        UserData.ATK = LoadATK_Per_Level(0);
        UserData.ATK_speed = LoadAttackSpeed_Per_Level(0);
        UserData.CritPercent = LoadCritPercent_Per_Level(0);
        UserData.MaxHP = UserData.HP = LoadHP_Per_Level(0);
        UserData.money = 1000000;
        UserData.liver = 0;

        for (int i = 0; i < 4; i++)
        {
            UserData.status_levels.Add(0);
        }

        for (int i = 0; i < 7; i++)
        {
            UserData.skill_level.Add(0);
        }
    }
    public void SaveUserData()
    {
        string json = JsonUtility.ToJson(this.UserData, true); // true = 보기 좋게 정렬
        string path = Path.Combine(Application.persistentDataPath, UserData_FilePath);
        File.WriteAllText(path, json);
        Debug.Log("저장 완료: " + path);
    }
    private int LoadUserData()
    {
        string path = Path.Combine(Application.persistentDataPath, UserData_FilePath);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            UserData = JsonUtility.FromJson<UserData_type>(json);
            if (isTesting)
                Debug.Log("불러오기 완료");
            return 0;
        }
        else
        {
            Debug.LogWarning("LoadData:: 저장된 파일이 없습니다. 유저 데이터 초기화.");
            Setting();
            return 0;
        }
    }
    private int LoadCost()
    {
        string path = Path.Combine(Application.streamingAssetsPath, costFileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"Cost CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }

        CostData.Clear();
        int index = 0;
        using (StreamReader sr = new StreamReader(path))
        {
            sr.ReadLine(); //첫 번째 행 index 패스
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');
                CostData.Add(new List<int>());
                foreach (string obj in values)
                {
                    if (!int.TryParse(obj, out int result))
                    {
                        // 변환 실패: int로 바꿀 수 없는 값이면 넘어감
                        continue;
                    }
                    CostData[index].Add(result);
                }

                index++;
            }
        }
        if (isTesting)
            Debug.Log($"CSV 로드 완료: {CostData.Count}줄\n");
        return 0;
    }
    public void ReloadCSV()
    {

    }

    //강화 레벨별 스탯 반환함수

    private int LoadATKdatas()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Level Data/Attack.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"ATK 레벨별 CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }
        ATK_levels_lists.Clear();
        using (StreamReader sr = new StreamReader(path))
        {
            sr.ReadLine(); //첫 번째 행 index 패스
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');
                foreach (string obj in values)
                {
                    if (!int.TryParse(obj, out int result))
                    {
                        // 변환 실패: int로 바꿀 수 없는 값이면 넘어감
                        continue;
                    }
                    ATK_levels_lists.Add(result);
                }
            }
        }
        if (isTesting)
            Debug.Log($"ATK 레벨별 CSV 로드 완료: {ATK_levels_lists.Count}개\n");
        return 0;
    }
    private int LoadHPdatas()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Level Data/HP.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"HP 레벨별 CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }
        using (StreamReader sr = new StreamReader(path))
        {
            sr.ReadLine(); //첫 번째 행 index 패스
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');
                foreach (string obj in values)
                {
                    if (!int.TryParse(obj, out int result))
                    {
                        // 변환 실패: int로 바꿀 수 없는 값이면 넘어감
                        continue;
                    }
                    HP_levels_lists.Add(result);
                }
            }
        }
        if (isTesting)
            Debug.Log($"HP 레벨별 CSV 로드 완료: {HP_levels_lists.Count}개\n");
        return 0;
    }
    private int LoadATKspeeddatas()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Level Data/AttackSpeed.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"AttackSpeed 레벨별 CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }
        using (StreamReader sr = new StreamReader(path))
        {
            sr.ReadLine(); //첫 번째 행 index 패스
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');
                foreach (string obj in values)
                {
                    if (!double.TryParse(obj, out double result))
                    {
                        // 변환 실패: double로 바꿀 수 없는 값이면 넘어감
                        continue;
                    }
                    ATKspeed_levels_lists.Add(result);
                }
            }
        }
        if (isTesting)
            Debug.Log($"AttackSpeed 레벨별 CSV 로드 완료: {ATKspeed_levels_lists.Count}개\n");
        return 0;
    }
    private int LoadCRIpercentdatas()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Level Data/CritPercent.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"CritPercent 레벨별 CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }
        using (StreamReader sr = new StreamReader(path))
        {
            sr.ReadLine(); //첫 번째 행 index 패스
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');
                foreach (string obj in values)
                {
                    if (!double.TryParse(obj, out double result))
                    {
                        // 변환 실패: double로 바꿀 수 없는 값이면 넘어감
                        continue;
                    }
                    CRIpercent_levels_lists.Add(result);
                }
            }
        }
        if (isTesting)
            Debug.Log($"CritPercent 레벨별 CSV 로드 완료: {CRIpercent_levels_lists.Count}개\n");
        return 0;
    }

    // 몬스터 데이터 가져오기
    private int LoadMonsterdatas()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Monster/Normal_Monster.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"Normal Monster 레벨별 CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }
        using (StreamReader sr = new StreamReader(path))
        {
            List<Pair<int, int>> index = new List<Pair<int, int>>();
            int tmp = 0;
            string line = sr.ReadLine();
            string[] values = line.Split(',');

            foreach (string obj in values)
            {
                if (tmp++ == 0) //비어있으면 (stage)
                    continue;
                string[] stage_str = obj.Split('_');
                int F = int.Parse(stage_str[0]);
                int S = int.Parse(stage_str[1]);

                index.Add(new Pair<int, int>(F, S));
            }

            line = sr.ReadLine();
            values = line.Split(',');
            tmp = 0;

            foreach (string obj in values)
            {
                if (!int.TryParse(obj, out int result))
                {
                    // 변환 실패: int로 바꿀 수 없는 값이면 넘어감
                    continue;
                }
                Monster_lists.Add(index[tmp], new Pair<int, int>(result, 0));
                tmp++;
            }

            line = sr.ReadLine();
            values = line.Split(',');
            tmp = 0;
            foreach (string obj in values)
            {
                if (!int.TryParse(obj, out int result))
                {
                    // 변환 실패: int로 바꿀 수 없는 값이면 넘어감
                    continue;
                }
                Monster_lists[index[tmp]].Second = result;
                tmp++;
            }
        }
        if (isTesting)
            Debug.Log($"Normal Monster CSV 로드 완료: {Monster_lists.Count}개\n");
        return 0;
    }

    #endregion


    #region 불러온 데이터 가져오기
    //강화 비용 관련 함수들입니다.
    public int LoadATKcost(int level)
    {
        if (level >= CostData[0].Count)
            return -1;
        return CostData[0][level];
    }
    public int LoadHPcost(int level)
    {
        if (level >= CostData[1].Count)
            return -1;
        return CostData[1][level];
    }
    public int LoadCRIpercentcost(int level)
    {
        if (level >= CostData[3].Count)
            return -1;
        return CostData[3][level];
    }
    public int LoadATKSpeedcost(int level)
    {
        if (level >= CostData[2].Count)
            return -1;
        return CostData[2][level];
    }
    public int LoadATK_Per_Level(int level)
    {
        if (level >= ATK_levels_lists.Count)
            return -1;
        return ATK_levels_lists[level];
    }
    public double LoadAttackSpeed_Per_Level(int level)
    {
        if (level >= ATKspeed_levels_lists.Count)
            return -1;
        return ATKspeed_levels_lists[level];
    }
    public double LoadCritPercent_Per_Level(int level)
    {
        if (level >= CRIpercent_levels_lists.Count)
            return -1;
        return CRIpercent_levels_lists[level];
    }
    public int LoadHP_Per_Level(int level)
    {
        if (level >= HP_levels_lists.Count)
            return -1;
        return HP_levels_lists[level];
    }
    #endregion


    #region 레벨 데이터 가져오기
    public int getATKlevel()
    {
        return UserData.status_levels[0];
    }
    public int getHPlevel()
    {
        return UserData.status_levels[1];
    }
    public int getATK_speedlevel()
    {
        return UserData.status_levels[2];
    }
    public int getCRIpercentlevel()
    {
        return UserData.status_levels[3];
    }
    #endregion


    #region 유저 데이터 가져오기
    public int getATK()
    {
        return UserData.ATK;
    }
    public int getATKData()
    {
        return UserData.ATK;
    }
    public int getMoney()
    {
        return UserData.money;
    }
    public int getLiver()
    {
        return UserData.liver;
    }
    public int getHP()
    {
        return UserData.HP;
    }
    public int getMaxHP()
    {
        return UserData.MaxHP;
    }
    public double getCritPercent()
    {
        return UserData.CritPercent;
    }
    public double getATK_speed()
    {
        return UserData.ATK_speed;
    }
    #endregion


    #region 유저 데이터 setter
    public void setHP(int HP)
    {
        UserData.HP = Math.Min(HP, getMaxHP()); //에이 설마 maxHP를 넘는 HP 데이터를 주겠어?? 대응
    }
    public void setATK(int ATK)
    {
        UserData.ATK = ATK;
    }
    public void setMaxHP(int HP)
    {
        UserData.MaxHP = HP;
    }
    public void setCRIpercent(double percent)
    {
        UserData.CritPercent = percent;
    }
    public void setMoney(int money)
    {
        UserData.money = Math.Max(money, 0);
    }
    public void setLiver(int liver)
    {
        UserData.liver = liver;
    }
    #endregion


    #region 몬스터 데이터 getter/setter
    public void setMonster(Tuple<int, int> stage)
    {
        Pair<int, int> key_data = new Pair<int, int>(stage);
        UserData.monster.HP = UserData.monster.MaxHP = Monster_lists[key_data].First;
        UserData.monster.ATK = Monster_lists[key_data].Second;
        UserData.monster.last_Attack = 0;
    }
    public int getMonsterATK()
    {
        return UserData.monster.ATK;
    }
    public int getMonsterMaxHP()
    {
        return UserData.monster.MaxHP;
    }
    public int getMonsterHP()
    {
        return UserData.monster.HP;
    }
    public void setMonsterHP(int HP)
    {
        UserData.monster.HP = Math.Max(0, HP);
    }
    
    #endregion


    #region 전투 관련
    public bool canAttack(long last_date, double AttackSpeed)
    {
        long nowTime = utility.get_times();
        long speed_tmp = (int)(AttackSpeed * 10000000.0);
        if ((nowTime - last_date) >= speed_tmp)
            return true;
        return false;
    }
    public bool canPlayerAttack()
    {
        return canAttack(UserData.last_Attack, UserData.ATK_speed);
    }
    public bool canMonsterAttack()
    {
        return canAttack(UserData.monster.last_Attack, 3.0);
    }
    public void PlayerAttack() //공격했다는 것을 기록함
    {
        UserData.last_Attack = utility.get_times();
    }
    public void MonsterAttack() //공격했다는 것을 기록함
    {
        UserData.monster.last_Attack = utility.get_times();
    }

    #endregion


    #region 기타 등등
    //기타 등등
    public Tuple<int, int> getStage() //현재 플레이어가 어떤 스테이지에 있는지 튜플 형태로 반환합니다.
    {
        return UserData.stage.toTuple();
    }
    public void setStage(Tuple<int, int> stage) {
        UserData.stage = new Pair<int, int>(stage);
    }
    //AttackSpeed 수치에 대해 공격 가능한 타이밍인지 여부를 반환합니다.(미구현)

    #endregion

}

[Serializable]
public class UserData_type //세이브 및 로드할 데이터 json형태
{
    public int ATK, HP, MaxHP;
    public double ATK_speed, CritPercent;
    public List<int> status_levels = new List<int>(); //각 스탯 강화 레벨 기록
    public List<int> skill_level = new List<int>();
    public int money, liver; //돈과 간 수치
    public List<int> effects; //버프, 디버프 시간 저장
    public Pair<int, int> stage;
    public long last_Attack;
    public Monster monster;
}

public class Monster
{
    public int MaxHP, HP, ATK;
    public long last_Attack;
}

public class Pair<T, U>
{
    public T First { get; set; }
    public U Second { get; set; }

    public Pair(T first, U second)
    {
        this.First = first;
        this.Second = second;
    }
    public Pair(Tuple<T, U> tmp)
    {
        First = tmp.Item1;
        Second = tmp.Item2;
    }

    public override string ToString()
    {
        return $"({First}, {Second})";
    }


    public Tuple<T, U> toTuple()
    {
        return new Tuple<T, U>(this.First, this.Second);
    }

};