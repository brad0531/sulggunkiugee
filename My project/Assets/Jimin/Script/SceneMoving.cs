using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoving : MonoBehaviour
{
    public Tuple<int, int> stage;

    private Scene currentScene;

    public static SceneMoving Instance { get; private set; }

    private void UnloadCurrentScene()
    {
        // 메인 씬은 항상 유지
        Scene active = SceneManager.GetActiveScene();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name != "Main" && scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene.name);
            }
        }
    }

    public void MoveTDiamondDungeon()
    {
        UnloadCurrentScene();
        SceneManager.LoadScene("DIamondDungeon", LoadSceneMode.Additive);
    }

    public void MoveToStage()
    {
        stage = GameManager.Instance.getStage();
        UnloadCurrentScene();
        SceneManager.LoadScene("Stage" + stage.Item1, LoadSceneMode.Additive);
    }

    public void MoveToTutorial()
    {
        UnloadCurrentScene();
        SceneManager.LoadScene("Tutorial", LoadSceneMode.Additive);
    }

    public void StartHappyEnding()
    {
        UnloadCurrentScene();
        SceneManager.LoadScene("HappyEnding", LoadSceneMode.Additive);
    }

    public void StartBadEnding()
    {
        UnloadCurrentScene();
        SceneManager.LoadScene("BadEnding", LoadSceneMode.Additive);
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
