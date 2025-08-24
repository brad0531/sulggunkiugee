using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoving : MonoBehaviour
{
    public Tuple<int, int> stage;

    private Scene currentScene;

    public static SceneMoving Instance { get; private set; }
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

    private void Start()
    {
        SceneManager.LoadScene("HJM", LoadSceneMode.Additive);
        MoveToTutorial();
    }

    private void Update()
    {
        stage = GameManager.Instance.getStage();

        // 해피엔딩
        if (stage.Item1 == 7 && stage.Item2 == 0)
        {
            int Mainstage = stage.Item1;
            Mainstage++;
            GameManager.Instance.setStage(new Tuple<int, int>(Mainstage, stage.Item2));
            StartHappyEnding();
        }

        // 배드 엔딩
        int liver = GameManager.Instance.getLiver();
        if (liver >= 10000)
        {
            GameManager.Instance.setLiver(0);
            StartBadEnding();
        }
    }

    private void UnloadCurrentScene()
    {
        // 메인 씬, 준민이 UI씬은 항상 유지
        Scene active = SceneManager.GetActiveScene();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name != "Main" && scene.name != "HJM" && scene.isLoaded)
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
        Diamond.Instance.inDungeon = false;

        if (stage.Item1 == 0)
        {
            SceneManager.LoadScene("Tutorial", LoadSceneMode.Additive);
        }

        else if(stage.Item1 != 0 && stage.Item1 <= 6) 
        {
            SceneManager.LoadScene("Stage" + stage.Item1, LoadSceneMode.Additive);
        }
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
}

