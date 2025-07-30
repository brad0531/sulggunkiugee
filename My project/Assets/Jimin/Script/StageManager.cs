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
    private PlayerController playercontroller;
    public Tuple<int, int> stage = new Tuple<int, int>(0, 0); //gamemanager에서 Stage 받아오기용

    private GameObject enemy;
    private GameObject enemies;
    private GameObject BossMon;

    private bool isRespawning = false;
    private bool tutorialClear = false;

    private int _currentStage = 0;
    private int subStage = 0;

    public float delay = 0.5f;

    private List<GameObject> enemiesList = new List<GameObject>();

    private IEnumerator PlayerRespawnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentStage = Mathf.Max(1, _currentStage - 1);
        GameManager.Instance.setStage(new Tuple<int, int>(_currentStage, 0));
        playercontroller.HandlePlayerRespawn();
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
    }

    void TutorialSpawnMonster() 
    {
        PlayerRespawnDelay(delay);
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
        //대화 스크립트 출력
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
                TutorialSpawnMonster();
            }

            // 튜토리얼, 1-0 제외 n-0
            if (stage.Item1 > 1 && stage.Item2 == 0)
            {
                _currentStage = stage.Item1 - 1;
                subStage = 5;
                GameManager.Instance.setStage(new Tuple<int, int>(_currentStage, subStage));
            }

            // 1-0
            if (stage.Item1 == 1 && stage.Item2 == 0)
            {
                enemiesList.Clear();
                Respawn();
            } 

            // 튜토리얼 제외 n-1 ~ n-5
            if (stage.Item1 >= 1 && stage.Item2 >= 1 && stage.Item2 <= 5) 
            {
                subStage = stage.Item2 - 1;
                GameManager.Instance.setStage(new Tuple<int, int>(_currentStage, subStage));
            }

        }
    } 



    void Start()
    {
        GameManager.Instance.setStage(stage);
        stage = GameManager.Instance.getStage();
        if (stage.Item1 == 0 && stage.Item2 == 0) //튜토리얼 스테이지가 0 - 0 이라고 할때
        {
            SceneManager.LoadScene("Tutorial");
            TutorialSpawnMonster();
        }
        else
        {
          SpawnMonsters();
        }
    }

    void Update()
    {

        //Playerdying();

        // 튜토리얼
        if (stage.Item1 == 0 && stage.Item2 == 0)
        {

            if (tutorialClear == false && enemiesList.All(e => e != null && !e.activeSelf))
            {
                FadeInOut.Fade(fadeEffect);
                // 플레이어 리스폰 함수 호출 
                TutorialSpawnMonster();
            }

            if(tutorialClear == true)
            {
                enemiesList.Clear();
                _currentStage++;
                GameManager.Instance.setStage(new Tuple<int, int>(_currentStage, 0));
            }
        }

        //n-0 ~ n-4
        if (stage.Item1 >= 1 && stage.Item2 <= 4)
        {
            if (enemiesList.All(e => e != null && !e.activeSelf) && !isRespawning) //몬스터가 비활성화 될 시
            {
                isRespawning = true;
                //GameManager.Instance.setStage(new Tuple<int, int>(stage+1, 0));
                Respawn();
            }
        }
   
        

        else if (stage.Item2 == 5) 
        {
            if (enemiesList.All(e => e != null && !e.activeSelf) && !isRespawning)
            {
                // 1-5
                isRespawning = true;
                BossSpawn();
                _currentStage++;
                GameManager.Instance.setStage(new Tuple<int, int>(_currentStage, 0));
            }
            
        }

        //BossStage Clear
        /* else if (Stagelevel == 6)
        {
            if (!isRespawning && BossMon != null && !BossMon.activeSelf)
            {
                FadeInOut.Fade(fadeEffect);
                SceneManager.LoadScene("Stage2");
            }
        } */

        // 해피엔딩
        if (stage.Item1 == 7 && stage.Item2 == 0)
        {
            SceneManager.LoadScene("HappyEnding");
        }

        // 배드 엔딩
        int liver = GameManager.Instance.getLiver();
        if (liver >= 10000)
        {
            SceneManager.LoadScene("BadEnding");
        }
    }
}