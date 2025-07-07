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
    public List<List<int>> CostData;
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

        if (result > 0)
            Debug.LogError($"----------------------------------------\n데이터 불러오는 중 오류 발생 :: {result}개\n");
    }
    public void SaveUserData()
    {
        string json = JsonUtility.ToJson(this.UserData, true); // true = 보기 좋게 정렬
        string path = Path.Combine(Application.persistentDataPath, UserData_FilePath);
        File.WriteAllText(path, json);
        Debug.Log("저장 완료: " + path);
    }
    public int LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, UserData_FilePath);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            UserData = JsonUtility.FromJson<UserData_type>(json);
            if(isTesting)
                Debug.Log("불러오기 완료");
            return 0;
        }
        else
        {
            Debug.LogWarning("LoadData:: 저장된 파일이 없습니다.");
            return 1;
        }
    }
    public int LoadCost()
    {
        string path = Path.Combine(Application.streamingAssetsPath, costFileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"Cost CSV 파일을 찾을 수 없습니다: {path}\n");
            return 0;
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

                foreach (string obj in values)
                {
                    CostData[index].Add(int.Parse(obj));
                }

                index++;
            }
        }

        Debug.Log($"CSV 로드 완료: {csvData.Count}줄\n");
        return 1;
    }

    // 초기화
    public void ReloadCSV()
    {

    }

    //강화 비용 관련 함수들입니다.
    public List<int> LoadATKcosts()
    {
        return CostData[0];
    }
    public List<int> LoadHPcosts()
    {
        return CostData[1];
    }
    public List<int> LoadCRIpercentcosts()
    {
        return CostData[3];
    }
    public List<int> LoadATKSpeedcosts()
    {
        return CostData[2];
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
    
    public int getHP()
    {
        return UserData.HP;
    }
    public int getMaxHP()
    {
        return UserData.MaxHP;
    }
    public double getCRIpercent()
    {
        return UserData.CritPercent;
    }
    public double getATKspeed()
    {
        return UserData.ATK_speed;
    }
    public void setHP(int HP)
    {
        UserData.HP = Math.Min(HP, getMaxHP()); //에이 설마 maxHP를 넘는 HP 데이터를 주겠어?? 대응
    }
}

[Serializable]
public class UserData_type //세이브 및 로드할 데이터 json형태
{
    public int ATK, HP, MaxHP;
    public double ATK_speed, CritPercent;
    public List<int> levels = new List<int>();
}