using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class tester : MonoBehaviour
{
    public string filename;
    public void ReadFileFromStreamingAssets(string fileName)
    {
        StartCoroutine(LoadStreamingAsset(fileName));
    }

    private IEnumerator LoadStreamingAsset(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

#if UNITY_ANDROID
        UnityWebRequest request = UnityWebRequest.Get(filePath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"파일 읽기 실패: {request.error}");
        }
        else
        {
            Debug.Log($"파일 내용:\n{request.downloadHandler.text}");
        }
#else
        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            Debug.Log($"파일 내용:\n{content}");
        }
        else
        {
            Debug.LogError("파일이 존재하지 않습니다: " + filePath);
        }
        yield return null;
#endif
    }

    void Start()
    {
        ReadFileFromStreamingAssets(filename);
    }
}
