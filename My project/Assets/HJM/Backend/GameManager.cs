using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using System;
using System.Data;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Playables;

public class GameManager : MonoBehaviour
{

    #region 데이터 선언
    private GameUtility utility = new GameUtility();
    public static GameManager Instance { get; private set; }
    public bool isTesting = true; //나중에 이거 끄고 키는 것만 하면 로그 출력 제어할 수 있도록
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
    private Dictionary<Tuple<int, int>, Tuple<int, int>> Monster_lists = new Dictionary<Tuple<int, int>, Tuple<int, int>>();
    private Pair<Tuple<int, bool>, List<Tuple<string, string>>> Scripts = new Pair<Tuple<int, bool>, List<Tuple<string, string>>>(new Tuple<int, bool>(0, true), new List<Tuple<string, string>>());
    private long Last_Save = 0;
    public long Save_Frequency = 1000000;
    #endregion


    private void Awake()
    {
        // 싱글턴(오브젝트가 중복되지 않고 하나만 존재하도록) 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경에도 유지
            LoadData_all();
            Last_Save = utility.get_times();
            if (isTesting)
                Debug.Log($"자동 세이브 간격은 {Save_Frequency / 10000000.0}초입니다.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if (utility.get_times() - Last_Save >= Save_Frequency)
        {
            Last_Save = utility.get_times();
            SaveUserData();
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
        UserData.stage = new Pair<int, int>(1, 0);

        setMonster(new Tuple<int, int>(1, 0));

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
        //Debug.Log("유저 데이터 세이브 시도");
        return;
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
            List<Tuple<int, int>> index = new List<Tuple<int, int>>();
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

                index.Add(new Tuple<int, int>(F, S));
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
                Monster_lists.Add(index[tmp], new Tuple<int, int>(result, 0));
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
                Monster_lists[index[tmp]] = new Tuple<int, int>(Monster_lists[index[tmp]].Item1, result);
                tmp++;
            }
        }
        if (isTesting)
        {
            Debug.Log($"Normal Monster CSV 로드 완료: {Monster_lists.Count}개\n");
            Debug.Log("=== Monster_lists 전체 출력 ===");
            foreach (var kvp in Monster_lists)
            {
                Debug.Log($"Stage {kvp.Key} -> HP: {Monster_lists[kvp.Key].Item1}, ATK: {Monster_lists[kvp.Key].Item2}");
            }
        }
        return 0;
    }

    private int LoadScriptsdatas(int MainStage, bool isStart)
    {
        string path = Path.Combine(Application.streamingAssetsPath, $"Monster/Scripts/Boss_Stage{MainStage}_{(isStart ? "Start" : "End")}_script.csv");

        if (!File.Exists(path))
        {
            Debug.LogError($"대화 스크립트 CSV 파일을 찾을 수 없습니다: {path}");
            return 1;
        }

        Scripts.First = new Tuple<int, bool>(MainStage, isStart);
        Scripts.Second = new List<Tuple<string, string>>();

        using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("euc-kr")))
        {
            sr.ReadLine(); // 첫 줄은 헤더이므로 건너뜀

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                var values = utility.SplitCsvLine(line);
                if (values.Count >= 3)
                {
                    Scripts.Second.Add(new Tuple<string, string>(values[1], values[2]));
                }
            }
        }

        Debug.Log($"대화 스크립트 CSV 로드 완료: {Scripts.Second.Count}개");
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
    public int getCurrentATK()
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
    public int getCurrentMaxHP()
    {
        return UserData.MaxHP;
    }
    public int getCurrentHP()
    {
        return UserData.HP;
    }
    public double getCritPercent()
    {
        return UserData.CritPercent;
    }
    public double getCurrentCritPercent()
    {
        return UserData.CritPercent;
    }
    public double getATK_speed()
    {
        return UserData.ATK_speed;
    }
    public double getCurrentATK_speed()
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
    public void setAttackSpeed(double speed)
    {
        UserData.ATK_speed = speed;
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
        if (Monster_lists == null)
        {
            Debug.LogError("monsterDictionary가 null입니다.");
            return;
        }

        if (!Monster_lists.ContainsKey(stage))
        {
            Debug.LogError($"monsterDictionary에 해당 키 ({stage.Item1}, {stage.Item2})가 없습니다.");
            return;
        }

        if (Monster_lists.TryGetValue(stage, out Tuple<int, int> monsterData))
        {
            UserData.monster.HP = monsterData.Item1;
            UserData.monster.MaxHP = monsterData.Item1;
            UserData.monster.ATK = monsterData.Item2;
            UserData.monster.last_Attack = 0;
        }
        else
        {
            Debug.LogError($"오류 발생:: {stage.Item1}, {stage.Item2}");
        }

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
    public void setStage(Tuple<int, int> stage)
    {
        UserData.stage = new Pair<int, int>(stage);
    }
    public bool isVaildStage(Tuple<int, int> stage)
    {
        if (Monster_lists.TryGetValue(stage, out Tuple<int, int> monsterData))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //(player, dialogue)
    public Tuple<string, string> getScript(int Mainstage, bool isStart, int index)
    {
        if (Scripts.First.Item1 != Mainstage || Scripts.First.Item2 != isStart) //새로 불러오기
        {
            LoadScriptsdatas(Mainstage, isStart);
        }
        if (index >= Scripts.Second.Count)
            return new Tuple<string, string>("오류 발생:: index 초과", "님 바보에요??");
        return Scripts.Second[index];
    }
    
    #endregion

}

[Serializable]

public class Monster
{
    public int MaxHP, HP, ATK;
    public long last_Attack;
}
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
    public Monster monster = new Monster();
}


#region 게임 유틸리티
public class GameUtility
{
    public long get_times() //utc 시간 (표준 시간)
    {
        return DateTime.UtcNow.Ticks;
    }
    public List<string> SplitCsvLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        StringBuilder field = new StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // "" → " 로 처리
                    field.Append('"');
                    i++; // skip next quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(c);
            }
        }

        result.Add(field.ToString()); // 마지막 필드 추가
        return result;
    }

}
#endregion
public class Pair<T, U>
{
    public T First { get; set; }
    public U Second { get; set; }

    public Pair(T first, U second)
    {
        First = first;
        Second = second;
    }

    public Pair(System.Tuple<T, U> tmp)
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
        return new Tuple<T, U>(First, Second);
    }

    public override bool Equals(object obj)
    {
        if (obj is Pair<T, U> other)
        {
            return EqualityComparer<T>.Default.Equals(this.First, other.First)
                && EqualityComparer<U>.Default.Equals(this.Second, other.Second);
        }
        return false;
    }

    public override int GetHashCode()
    {
        int hash1 = First == null ? 0 : EqualityComparer<T>.Default.GetHashCode(First);
        int hash2 = Second == null ? 0 : EqualityComparer<U>.Default.GetHashCode(Second);
        return HashCode.Combine(hash1, hash2); // .NET Core 2.1+ / .NET Standard 2.1+ / .NET Framework 4.7.2+
    }
}
