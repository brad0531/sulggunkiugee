using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVManager : MonoBehaviour
{
    public static CSVManager Instance { get; private set; }

    [Header("CSV 파일 상대 경로 (StreamingAssets 기준)")]
    public string csvFileName = "test.csv"; // 예: data.csv 또는 folder/data.csv

    private List<string[]> csvData = new List<string[]>();
    public IReadOnlyList<string[]> Data => csvData; // 다른 스크립트에서 읽기 전용 접근

    private void Awake()
    {
        // 싱글턴 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경에도 유지
            LoadCSV();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadCSV()
    {
        string path = Path.Combine(Application.streamingAssetsPath, csvFileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: {path}");
            return;
        }

        csvData.Clear();
        using (StreamReader sr = new StreamReader(path))
        {
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');
                csvData.Add(values);
            }
        }

        Debug.Log($"CSV 로드 완료: {csvData.Count}줄");
    }

    // 외부에서 강제로 다시 불러올 수 있게 (getter)
    public void ReloadCSV()
    {
        LoadCSV();
    }
}
