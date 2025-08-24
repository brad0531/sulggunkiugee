using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public GameObject Monster;
    public GameObject Boss;
    public FadeInOut fadeEffect;
    
    public Tuple<int, int> stage = new Tuple<int, int>(0, 0); //gamemanager에서 Stage 받아오기용

    private GameObject enemy;
    private GameObject enemies;
    private GameObject BossMon;

    private bool isRespawning = false;
    private bool tutorialClear = false;

    private int _currentStage = 0;
    private int subStage = 0;
    private int index = 0;

    public float delay = 0.5f;

    private List<GameObject> enemiesList = new List<GameObject>();

    public static StageManager Instance { get; private set; }


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

    void Start()
    {
        GameManager.Instance.setStage(stage);
        stage = GameManager.Instance.getStage();

        if (stage.Item1 == 0 && stage.Item2 == 0) //튜토리얼 스테이지가 0 - 0 이라고 할때
        {
            TutorialSpawnMonster();
        }
        else
        {
            SpawnMonsters();
        }
    }

    void FixedUpdate()
    {

        //Playerdying();

        // 튜토리얼
        if (stage.Item1 == 0 && stage.Item2 == 0)
        {

            if (!tutorialClear && enemiesList.All(e => e != null && !e.activeSelf))
            {
                enemiesList.Clear();
                FadeInOut.Fade(fadeEffect);
                TutorialSpawnMonster();
            }

            if (tutorialClear)
            {
                GameObject.FindWithTag("enemy").SetActive(false);
                enemiesList.Clear();
                GameManager.Instance.setStage(new Tuple<int, int>(1, 0));
                SceneMoving.Instance.MoveToStage();
                Respawn();
            }
        }

        //n-0 ~ n-4
        if (stage.Item1 >= 1 && stage.Item2 <= 4)
        {
            if (enemiesList.All(e => e != null && !e.activeSelf) && !isRespawning) //몬스터가 비활성화 될 시
            {
                enemiesList.Clear();
                subStage++;
                GameManager.Instance.setStage(new Tuple<int, int>(_currentStage, subStage));
                Respawn();
            }
        }



        else if (stage.Item1 <= 6 && stage.Item2 == 5)
        {
            if (enemiesList.All(e => e != null && !e.activeSelf) && !isRespawning)
            {
                // n-5
                FadeInOut.Fade(fadeEffect);
                GameObject.FindWithTag("enemy").SetActive(false);
                enemiesList.Clear();

                PlayerRespawnDelay(delay);
                BossSpawn();
                if (!isRespawning && enemiesList.All(e => e != null && !e.activeSelf) && BossMon != null && !BossMon.activeSelf)
                {
                    Dialogue.Instance.turn = false;
                    StartCoroutine(Dialogue.Instance.MainDialogue(index, stage.Item1, Dialogue.Instance.turn));
                    if (index > GameManager.Instance.Scripts.Second.Count)
                    {
                        _currentStage++;
                        GameManager.Instance.setStage(new Tuple<int, int>(_currentStage, 0));

                        if (_currentStage != 7)
                        {
                            SceneMoving.Instance.MoveToStage();
                        }
                    }
                }
            }

        }
    }

    private IEnumerator PlayerRespawnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //_currentStage = Mathf.Max(1, _currentStage - 1);
        //GameManager.Instance.setStage(new Tuple<int, int>(_currentStage, 0));
        PlayerController.Instance.HandlePlayerRespawn();
    }

    void SpawnMonsters()
    {
        enemiesList.Clear();
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        for (int i = 0; i < 10; i++)
        {
            enemies = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 1000 + 2000 * i, playerPosition.y), Quaternion.identity);
            enemiesList.Add(enemies);
        }
        isRespawning = false;
        PlayerController.Instance.RegisterMonsters();
    }

    void TutorialSpawnMonster() 
    {
        PlayerRespawnDelay(delay);
        StartCoroutine(Dialogue.Instance.TutorialDialogue(index));
        enemiesList.Clear();
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        for (int i = 0; i < 50; i++)
        {
            enemies = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 1000 + 1200 * i, playerPosition.y), Quaternion.identity);
            enemiesList.Add(enemies);
        }
        isRespawning = false;
    }

    void BossSpawn()
    {
        FadeInOut.Fade(fadeEffect);
        PlayerRespawnDelay(delay);
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        for (int i = 0; i < 10; i++)
        {
            enemies = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 1000 + 2000 * i, playerPosition.y), Quaternion.identity);
            enemiesList.Add(enemies);
        }
        BossMon = (GameObject)Instantiate(Boss, new Vector3(playerPosition.x + 1000, playerPosition.y), Quaternion.identity);
        isRespawning = false;

        stage = GameManager.Instance.getStage();
        StartCoroutine(Dialogue.Instance.MainDialogue(index, stage.Item1, Dialogue.Instance.turn));
    }

    void Respawn()
    {
        FadeInOut.Fade(fadeEffect);
        PlayerRespawnDelay(delay);
        SpawnMonsters();
    }

     void Playerdying()
    {
         //if(플레이어 죽음 판정 시)
        {
            stage = GameManager.Instance.getStage();

            //튜토리얼
            if (stage.Item1 == 0) 
            {
                FadeInOut.Fade(fadeEffect);
                PlayerRespawnDelay(delay);
                TutorialSpawnMonster();
            }

            // 튜토리얼, 1-0 제외 n-0
            if (stage.Item1 > 1 && stage.Item2 == 0)
            {
                GameObject.FindWithTag("enemy").SetActive(false);
                enemiesList.Clear();
                _currentStage = stage.Item1 - 1;
                GameManager.Instance.setStage(new Tuple<int, int>(_currentStage, 5));
                SceneMoving.Instance.MoveToStage();
            }

            // 1-0
            if (stage.Item1 == 1 && stage.Item2 == 0)
            {
                GameObject.FindWithTag("enemy").SetActive(false);
                enemiesList.Clear();
            } 

            // 튜토리얼 제외 n-1 ~ n-5
            if (stage.Item1 >= 1 && stage.Item2 >= 1 && stage.Item2 <= 5) 
            {
                GameObject.FindWithTag("enemy").SetActive(false);
                enemiesList.Clear();
                subStage = stage.Item2 - 1;
                GameManager.Instance.setStage(new Tuple<int, int>(_currentStage, subStage));
            }
        }
    } 
}