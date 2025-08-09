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
using Unity.Mathematics;
public class GameManager : MonoBehaviour
{
    public const long ONE_SECOND = 10000000;
    #region 데이터 선언
    private GameUtility utility = new GameUtility();
    public static GameManager Instance { get; set; }
    public bool isTesting = true; //나중에 이거 끄고 키는 것만 하면 로그 출력 제어할 수 있도록
    [Header("CSV 파일 상대 경로 (StreamingAssets 기준)")]
    public string UserData_FilePath = "UserData/UserData.json";
    private List<KeyValuePair<string, int>[]> csvData = new List<KeyValuePair<string, int>[]>();
    public List<List<int>> CostData = new List<List<int>>();
    public UserData_type UserData = new UserData_type(); //유저 데이터 저장 변수
    private List<int> ATK_levels_lists = new List<int>();
    private List<int> HP_levels_lists = new List<int>();
    private List<double> ATKspeed_levels_lists = new List<double>();
    private List<double> CRIpercent_levels_lists = new List<double>();
    private Dictionary<Tuple<int, int, int>, Tuple<int, int, int>> Monster_lists = new Dictionary<Tuple<int, int, int>, Tuple<int, int, int>>();
    private Pair<Tuple<int, bool>, List<Tuple<string, string>>> Scripts = new Pair<Tuple<int, bool>, List<Tuple<string, string>>>(new Tuple<int, bool>(0, true), new List<Tuple<string, string>>());
    public List<List<int>> Info_Alcohol = new List<List<int>>();
    public List<List<int>> Snacks = new List<List<int>>();
    public List<List<int>> Skill_Cost = new List<List<int>>();
    public List<Pair<List<int>, List<int>>> Skill_Info = new List<Pair<List<int>, List<int>>>();
    private long Last_Save = 0;
    public long Save_Frequency = 1000000;

    public enum Alcohol_index
    {
        Cass, Terra, Kelly, ChingTao, Asahi, Ale, LikeFirst, ChamIsle, Saro, Jinro, Red
    };

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
                Debug.Log($"자동 세이브 간격은 {(double)Save_Frequency / 10000000.0}초입니다.");
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
        result += Load_Alcohol_Info();
        result += Load_Snacks_Data();
        result += LoadSkillDatas();
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
        UserData.diamond = 0;
        UserData.stage = new StageType<int, int, int>(0, 0, 0);

        setMonster(new Tuple<int, int, int>(0, 0, 0));

        for (int i = 0; i < 4; i++)
        {
            UserData.status_levels.Add(0);
        }

        for (int i = 0; i < 8; i++)
        {
            UserData.skill_level.Add(0);
        }

        for (int i = 0; i < 12; i++) //11번째 인덱스는 소맥의 디버프를 기록하는 데에 사용됩니다.
        {
            UserData.effects.Add(0);
            UserData.effects_cool.Add(0);
        }

        for (int i = 0; i < 7; i++)
            UserData.snack_times.Add(0);
    }
    public void SaveUserData()
    {
        //Debug.Log("유저 데이터 세이브 시도");
        return;
        //string json = JsonUtility.ToJson(this.UserData, true); // true = 보기 좋게 정렬
        //string path = Path.Combine(Application.persistentDataPath, UserData_FilePath);
        //File.WriteAllText(path, json);
        //Debug.Log("저장 완료: " + path);
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
        string path = Path.Combine(Application.streamingAssetsPath, "Costs/Cost_State.csv");
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
            Debug.Log($"Cost 관련 CSV 로드 완료");

        //술 비용
        path = Path.Combine(Application.streamingAssetsPath, "Costs/Cost_Drink.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"Drink Cost CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }
        using (StreamReader sr = new StreamReader(path))
        {
            sr.ReadLine(); //첫 번째 행 index 패스
            index = 0;
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
                    Info_Alcohol.Add(new List<int>());
                    Info_Alcohol[Info_Alcohol.Count - 1].Add(result);
                }
            }
        }
        if (isTesting)
            Debug.Log($"Drink Cost 관련 CSV 로드 완료. {Info_Alcohol.Count}개");
        return 0;
    }

    private int Load_Alcohol_Info()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Drink.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"Drink CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }
        int index = 0;
        using (StreamReader sr = new StreamReader(path))
        {
            sr.ReadLine();
            //첫 번째 행 index 패스
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
                    Info_Alcohol[index].Add(result);
                }

                index++;
            }
        }
        if (isTesting)
            Debug.Log($"Drink info 관련 CSV 로드 완료");

        return 0;
    }

    private int Load_Snacks_Data()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Costs/Cost_Snack.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"Snack Cost CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }
        int index;
        using (StreamReader sr = new StreamReader(path))
        {
            sr.ReadLine(); //첫 번째 행 index 패스
            index = 0;
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
                    Snacks.Add(new List<int>());
                    Snacks[Snacks.Count - 1].Add(result);
                }
            }
        }
        if (isTesting)
            Debug.Log($"Snack Cost 관련 CSV 로드 완료. {Snacks.Count}개");

        path = Path.Combine(Application.streamingAssetsPath, "Snack.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"Snack CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }

        index = 0;
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
                    Snacks[index].Add(result);
                }

                index++;
            }
        }


        if (isTesting)
            Debug.Log($"Snack 관련 CSV 로드 완료");

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
        List<string> lines = new List<string>(File.ReadAllLines(path));
        path = Path.Combine(Application.streamingAssetsPath, "Monster/Boss_Monster.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"Boss Monster 레벨별 CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }
        List<string> tmp = new List<string>(File.ReadAllLines(path));
        tmp.RemoveAt(0);
        lines.AddRange(tmp);

        // 첫 줄은 헤더이므로 1부터 시작
        for (int i = 1; i < lines.Count; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            if (parts.Length < 4)
            {
                Debug.LogWarning($"잘못된 데이터 형식: {line}");
                continue;
            }

            // Stage 파싱 (예: "1_0_0")
            string[] stageParts = parts[0].Split('_');
            if (stageParts.Length != 3)
            {
                Debug.LogWarning($"잘못된 Stage 형식: {parts[0]}");
                continue;
            }

            int stageA = int.Parse(stageParts[0]);
            int stageB = int.Parse(stageParts[1]);
            int stageC = int.Parse(stageParts[2]);

            // Monster_H, Monster_D, Gold
            int monsterH = int.Parse(parts[1]);
            int monsterD = int.Parse(parts[2]);
            int gold = int.Parse(parts[3]);

            var stageKey = Tuple.Create(stageA, stageB, stageC);
            var value = Tuple.Create(monsterH, monsterD, gold);

            Monster_lists[stageKey] = value;
        }
        //튜토리얼 몬스
        for (int i = 0; i < 50; i++)
        {
            int stageA = 0;
            int stageB = 0;
            int stageC = i;

            // Monster_H, Monster_D, Gold
            int monsterH = i + 1;
            int monsterD = 1;
            int gold = i + 1;

            var stageKey = Tuple.Create(stageA, stageB, stageC);
            var value = Tuple.Create(monsterH, monsterD, gold);

            Monster_lists[stageKey] = value;
        }
        return 0;
    }

    private int LoadScriptsdatas(int MainStage, bool isStart)
    {
        string path;
        if (MainStage == 0) //튜토리얼
        {
            path = Path.Combine(Application.streamingAssetsPath, $"Tutorial/Tutorial_Script.csv");

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
                    if (values.Count >= 2)
                    {
                        Scripts.Second.Add(new Tuple<string, string>("김민승 선배", values[1]));
                    }
                }
            }
            if (isTesting)
                Debug.Log($"대화 스크립트 CSV 로드 완료: {Scripts.Second.Count}개");
            return 0;
        }
        path = Path.Combine(Application.streamingAssetsPath, $"Monster/Scripts/Boss_Stage{MainStage}_{(isStart ? "Start" : "End")}_script.csv");

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
        if (isTesting)
            Debug.Log($"대화 스크립트 CSV 로드 완료: {Scripts.Second.Count}개");
        return 0;
    }

    private int LoadSkillDatas()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Skill/Cost_Skill.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"Skill Cost CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }

        Skill_Cost.Clear();
        using (StreamReader sr = new StreamReader(path))
        {
            int index = 0;
            sr.ReadLine(); //첫 번째 행 index 패스
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');
                Skill_Cost.Add(new List<int>());
                foreach (string obj in values)
                {
                    if (!int.TryParse(obj, out int result))
                    {
                        // 변환 실패: int로 바꿀 수 없는 값이면 넘어감
                        continue;
                    }
                    Skill_Cost[index].Add(result);
                }

                index++;
            }
        }

        if (isTesting)
            Debug.Log($"Skill Cost 관련 CSV 로드 완료 :: {Skill_Cost[Skill_Cost.Count - 1][0]}개");

        path = Path.Combine(Application.streamingAssetsPath, "Skill/Skill.csv");
        if (!File.Exists(path))
        {
            Debug.LogError($"Skill CSV 파일을 찾을 수 없습니다: {path}\n");
            return 1;
        }

        Skill_Info.Clear();
        using (StreamReader sr = new StreamReader(path))
        {
            int index = 0;
            sr.ReadLine(); //첫 번째 행 index 패스
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');
                Skill_Info.Add(new Pair<List<int>, List<int>>(new List<int>(), new List<int>()));

                for (int i = 1; i <= 10; i++)
                {
                    if (!int.TryParse(values[i], out int result))
                    {
                        // 변환 실패: int로 바꿀 수 없는 값이면 넘어감
                        continue;
                    }
                    Skill_Info[index].First.Add(result);
                }

                for (int i = 11; i < 13; i++)
                {
                    if (!int.TryParse(values[i], out int result))
                    {
                        // 변환 실패: int로 바꿀 수 없는 값이면 넘어감
                        continue;
                    }
                    Skill_Info[index].Second.Add(result);
                }

                index++;
            }
        }
        if (isTesting)
            Debug.Log($"Skill info 관련 CSV 로드 완료::{Skill_Info.Count}개");

        // 술 신 강 림
        Skill_Cost.Add(new List<int>());
        Skill_Cost[Skill_Cost.Count - 1].Add(5000);
        Skill_Info.Add(new Pair<List<int>, List<int>>(new List<int>(), new List<int>()));
        Skill_Info[Skill_Info.Count - 1].First.Add(120);
        Skill_Info[Skill_Info.Count - 1].First.Add(240);  //임시 값 
        Skill_Info[Skill_Info.Count - 1].Second.Add(40);
        Skill_Info[Skill_Info.Count - 1].Second.Add(1);
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
    public int Load_Alcohol_Cost(Alcohol_index idx)
    {
        return Info_Alcohol[(int)idx][0];
    }
    public int Load_Skill_Cost(int index, int level)
    {
        if (level >= Skill_Cost[index].Count)
            return -1; //이용 불가
        return Skill_Cost[index][level];
    }
    public int Load_Skill_Damage(int index, int level)
    {
        if (level >= Skill_Info[index].First.Count)
        {
            Debug.LogError($"Skill Damage 인덱스 초과::{index},{level}");
            return -1;
        }

        return Skill_Info[index].First[level];
    }

    public int Load_Skill_HitCount(int index)
    {
        return Skill_Info[index].Second[1];
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
        int current_ATK = getCurrentATK();
        double Weight = 1.0;
        if (isEffectsOn(Alcohol_index.Cass))
            Weight += 0.5;
        if (isEffectsOn(Alcohol_index.ChingTao))
            Weight += 2.5;

        current_ATK = (int)((double)current_ATK * Weight);

        if (isEffectsOn(Alcohol_index.Ale))
            current_ATK = (int)((double)current_ATK * 1.15);

        return current_ATK;
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
        return Math.Min(UserData.HP, getMaxHP());
    }
    public int getMaxHP()
    {
        int current_MaxHP = getCurrentMaxHP();
        if (isEffectsOn(Alcohol_index.Terra))
            current_MaxHP = (int)((double)current_MaxHP * 1.35);
        return current_MaxHP;
    }
    public int getCurrentMaxHP()
    {
        return UserData.MaxHP;
    }
    public int getCurrentHP()
    {
        return Math.Min(UserData.HP, getMaxHP());
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
        double current_AS = getCurrentATK_speed();
        double Weight = 1.0;

        if (isEffectsOn(Alcohol_index.Kelly))
            Weight += 0.2;
        if (isEffectsOn(Alcohol_index.Asahi))
            Weight += 1.0;

        return current_AS * Weight;
    }
    public double getCurrentATK_speed()
    {
        return UserData.ATK_speed;
    }

    public int get_Skill_level(int index)
    {
        return UserData.skill_level[index];
    }

    public int getDiamond()
    {
        return UserData.diamond;
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
    public void setDiamond(int diamond)
    {
        UserData.diamond = Math.Max(diamond, 0);
    }
    public void setLiver(int liver)
    {
        UserData.liver = Math.Max(0, liver);
    }
    #endregion


    #region 몬스터 데이터 getter/setter
    public void setMonster(Tuple<int, int, int> stage)
    {
        if (Monster_lists == null)
        {
            Debug.LogError("monsterDictionary가 null입니다.");
            return;
        }

        if (!Monster_lists.ContainsKey(stage))
        {
            Debug.LogError($"monsterDictionary에 해당 키 ({stage.Item1}, {stage.Item2}, {stage.Item3})가 없습니다.");
            return;
        }

        if (Monster_lists.TryGetValue(stage, out Tuple<int, int, int> monsterData))
        {
            UserData.monster.HP = monsterData.Item1;
            UserData.monster.MaxHP = monsterData.Item1;
            UserData.monster.ATK = monsterData.Item2;
            UserData.monster.Gold = monsterData.Item3;
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

    //Main, Sub만 불러옵니다.
    public Tuple<int, int> getStage() //현재 플레이어가 어떤 스테이지에 있는지 튜플 형태로 반환합니다.
    {
        return UserData.stage.ToStageTuple();
    }

    //Main, Sub, index를 불러옵니다.
    public Tuple<int, int, int> getAllStage()
    {
        return UserData.stage.ToTuple();
    }
    public void setStage(Tuple<int, int> stage)
    {
        UserData.stage = new StageType<int, int, int>(stage);
        if (UserData.stage.Main > UserData.Max_stage.Main)
            UserData.Max_stage = UserData.stage;
        else if (UserData.stage.Main == UserData.Max_stage.Main && UserData.stage.Sub > UserData.Max_stage.Sub)
            UserData.Max_stage = UserData.stage;
    }
    public void setStage(Tuple<int, int, int> stage)
    {
        UserData.stage = new StageType<int, int, int>(stage);
        if (UserData.stage.Main > UserData.Max_stage.Main)
            UserData.Max_stage = UserData.stage;
        else if (UserData.stage.Main == UserData.Max_stage.Main && UserData.stage.Sub > UserData.Max_stage.Sub)
            UserData.Max_stage = UserData.stage;
    }
    public bool isVaildStage(Tuple<int, int, int> stage)
    {
        if (Monster_lists.TryGetValue(stage, out Tuple<int, int, int> monsterData))
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

    public bool isAlcoholCoolTimeEnd(Alcohol_index index)
    {
        long gap = utility.get_times() - (long)UserData.effects_cool[(int)index];
        if (gap / ONE_SECOND >= (long)Info_Alcohol[(int)index][3])
            return true;
        return false;
    }

    public bool isSnackCoolTimeEnd(int index)
    {
        long gap = utility.get_times() - (long)UserData.snack_times[index];
        if (gap / ONE_SECOND >= (long)Snacks[index][2])
            return true;
        return false;
    }

    public void effects_on(int index)
    {
        UserData.effects[index] = utility.get_times();
        UserData.effects_cool[index] = utility.get_times();
    }

    public bool isEffectsOn(Alcohol_index index)
    {
        long gap = utility.get_times() - (long)UserData.effects[(int)index];
        if (gap / ONE_SECOND >= (long)Info_Alcohol[(int)index][2])
            return false;
        return true;
    }

    public int getRestAlcoholCoolTime(int index)
    {
        long gap = utility.get_times() - (long)UserData.effects_cool[(int)index];
        return Math.Max(0, (int)(((long)Info_Alcohol[(int)index][3] - gap) / ONE_SECOND));
    }

    public void EarnMoney(int money)
    {
        double bonus = 1.0;
        if (isEffectsOn(Alcohol_index.LikeFirst))
            bonus += 0.5;
        if (isEffectsOn(Alcohol_index.Saro))
            bonus += 1.0;
        if (isEffectsOn(Alcohol_index.Red))
            bonus += 2.5;

        money = (int)((double)money * bonus);
        setMoney(getMoney() + money);
    }

    public void BeatMonster()
    {
        if (isBossMonster(getAllStage()))
        {
            UserData.diamond += UserData.monster.Gold;
        }
        else
        {
            EarnMoney(UserData.monster.Gold);
        }
    }

    public void EatSnack(int index)
    {
        UserData.snack_times[index] = utility.get_times();
    }

    public bool isBossMonster(Tuple<int, int, int> stage)
    {
        if (stage.Item2 == 6)
            return true;
        return false;
    }

    public int get_Skill_Cool(int index)
    {
        return Skill_Info[index].Second[0];
    }

    public void SetSoMacCool()
    {
        UserData.record.SoMacTime = utility.get_times();
    }

    public bool isSoMacCoolEnd()
    {
        double gap = (double)(utility.get_times() - UserData.record.SoMacTime) / (double)ONE_SECOND;

        if (gap >= 1)
            return true;

        return false;
    }

    public void Skill_Use(int index)
    {
        UserData.skill_cooltime[index] = utility.get_times();
    }

    public bool isSkill_CoolTimeEnd(int index)
    {
        long gap = utility.get_times() - (long)UserData.skill_cooltime[index];
        if (gap / ONE_SECOND >= (long)get_Skill_Cool(UserData.skill_set[index]))
            return true;
        return false;
    }
    #endregion

}

[Serializable]

public class Monster
{
    public int MaxHP, HP, ATK, Gold;
    public long last_Attack;
}
[System.Serializable]
public class UserData_type //세이브 및 로드할 데이터 json형태
{
    public int ATK, HP, MaxHP;
    public double ATK_speed, CritPercent;
    public List<int> status_levels = new List<int>(); //각 스탯 강화 레벨 기록
    public List<int> skill_level = new List<int>();
    public int money, liver, diamond = 0; //돈과 간 수치
    public List<long> effects = new List<long>(), effects_cool = new List<long>(); //술 버프, 디버프 시간 저장
    public List<long> snack_times = new List<long>();
    public StageType<int, int, int> stage;
    public StageType<int, int, int> Max_stage; //스테이지 최고 기록
    public long last_Attack;
    public UserDataRecord record = new UserDataRecord();
    public Monster monster = new Monster();
    public List<int> skill_set = new List<int> { 0, 1, 2, 3 };
    public List<long> skill_cooltime = new List<long>();
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

[System.Serializable]
public class UserDataRecord
{
    public int SoMac = 0;
    public long SoMacTime = 0;
    public int[] Soju_Record = { -1, 1, -1, 2, -1 };
}

[System.Serializable]
public class Pair<T, U>
{
    public T First { get; set; }
    public U Second { get; set; }

    public Pair()
    {
        
    }
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

[System.Serializable]
public class StageType<M, S, I>
{
    public M Main { get; set; }
    public S Sub { get; set; }
    public I Index { get; set; }

    public StageType(M main, S sub, I index)
    {
        Main = main;
        Sub = sub;
        Index = index;
    }

    public StageType(System.Tuple<M, S> tmp)
    {
        Main = tmp.Item1;
        Sub = tmp.Item2;
    }

    public StageType(System.Tuple<M, S, I> tmp)
    {
        Main = tmp.Item1;
        Sub = tmp.Item2;
        Index = tmp.Item3;
    }

    public override string ToString()
    {
        return $"({Main}, {Sub}, {Index})";
    }

    public Tuple<M, S, I> ToTuple()
    {
        return new Tuple<M, S, I>(Main, Sub, Index);
    }

    public Tuple<M, S> ToStageTuple()
    {
        return new Tuple<M, S>(Main, Sub);
    }

    public override bool Equals(object obj)
    {
        if (obj is StageType<M, S, I> other)
        {
            return EqualityComparer<M>.Default.Equals(this.Main, other.Main)
                && EqualityComparer<S>.Default.Equals(this.Sub, other.Sub)
                && EqualityComparer<I>.Default.Equals(this.Index, other.Index);
        }
        return false;
    }

    public override int GetHashCode()
    {
        int hash1 = Main == null ? 0 : EqualityComparer<M>.Default.GetHashCode(Main);
        int hash2 = Sub == null ? 0 : EqualityComparer<S>.Default.GetHashCode(Sub);
        int hash3 = Index == null ? 0 : EqualityComparer<I>.Default.GetHashCode(Index);
        return HashCode.Combine(hash1, hash2, hash3);
    }
}
