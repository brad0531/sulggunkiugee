using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool isTesting = true;
    [Header("CSV 파일 상대 경로 (StreamingAssets 기준)")]
    public string costFileName = "Costs/Cost_CSV.csv"; //코스트 데이터
    public string UserData_FilePath = "UserData/UserData.json";
    private List<KeyValuePair<string, int>[]> csvData = new List<KeyValuePair<string, int>[]>();
    public List<List<int>> CostData = new List<List<int>>();
    public UserData_type UserData = new UserData_type(); //유저 데이터 저장 변수
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
    public void LoadData_all()
    {
        int result = 0;
        result += LoadCost();
        result += LoadATKdatas();
        result += LoadATKspeeddatas();
        result += LoadCRIpercentdatas();
        result += LoadHPdatas();
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

    // 초기화
    public void ReloadCSV()
    {

    }

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
    //강화 레벨별 스탯 반환함수
    private List<int> ATK_levels_lists = new List<int>();
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
    public int LoadATK_Per_Level(int level)
    {
        if (level >= ATK_levels_lists.Count)
            return -1;
        return ATK_levels_lists[level];
    }

    private List<int> HP_levels_lists = new List<int>();
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
    public int LoadHP_Per_Level(int level)
    {
        if (level >= HP_levels_lists.Count)
            return -1;
        return HP_levels_lists[level];
    }

    private List<double> ATKspeed_levels_lists = new List<double>();
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
    public double LoadAttackSpeed_Per_Level(int level)
    {
        if (level >= ATKspeed_levels_lists.Count)
            return -1;
        return ATKspeed_levels_lists[level];
    }

    private List<double> CRIpercent_levels_lists = new List<double>();
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
    public double LoadCritPercent_Per_Level(int level)
    {
        if (level >= CRIpercent_levels_lists.Count)
            return -1;
        return CRIpercent_levels_lists[level];
    }
    //스탯 레벨 반환
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
    //현재 최종(버프, 디버프가 적용된) 스탯을 반환합니다.
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
    //스탯 setter
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
    } public void setCRIpercent(double percent)
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
}

[Serializable]
public class UserData_type //세이브 및 로드할 데이터 json형태
{
    public int ATK, HP, MaxHP;
    public double ATK_speed, CritPercent;
    public List<int> status_levels = new List<int>(); //각 스탯 강화 레벨 기록
    public List<int> skill_level = new List<int>();
    public int money, liver; //돈과 간 수치

    public List<int> effects;
}