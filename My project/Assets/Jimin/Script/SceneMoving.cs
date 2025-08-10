using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoving : MonoBehaviour
{
    public Tuple<int, int> stage;

    private Scene currentScene;

    public static SceneMoving Instance { get; private set; }

    void MoveTDiamondDungeon()
    {
        currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentScene);
        SceneManager.LoadScene("DIamondDungeon", LoadSceneMode.Additive);
    }

    void MoveToStage()
    {
        stage = GameManager.Instance.getStage();
        currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentScene);
        SceneManager.LoadScene("Stage" + stage.Item1, LoadSceneMode.Additive);
    }

    void MoveToTutorial()
    {
        currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentScene);
        SceneManager.LoadScene("Tutorial");
    }

    void StartHappyEnding()
    {
        currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentScene);
        SceneManager.LoadScene("HappyEnding");
    }

    void StartBadEnding()
    {
        currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentScene);
        SceneManager.LoadScene("BadEnding");
    }

    private void Awake()
    {
        // 싱글턴(오브젝트가 중복되지 않고 하나만 존재하도록) 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
