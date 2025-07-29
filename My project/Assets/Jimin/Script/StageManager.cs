using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

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

    private int mainStage = 0;
    private int substage = 0;

    private List<GameObject> enemiesList = new List<GameObject>();

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
        // 플레이어 리스폰 함수
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
        // 플레이어 리스폰 함수 호출 
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        BossMon = (GameObject)Instantiate(Boss, new Vector3(playerPosition.x + 1000, playerPosition.y), Quaternion.identity);
        isRespawning = false;
        //대화 스크립트 출력
    }

    void Respawn()
    {
        FadeInOut.Fade(fadeEffect);
        //플레이어 리스폰 함수 호출 
        SpawnMonsters();
    }


     void Playerdying()
    {
         //if(플레이어 죽음 판정 시)
        {
            stage = GameManager.Instance.getStage();
            if (stage.Item1 == 0) //튜토리얼
            {
                FadeInOut.Fade(fadeEffect);
                TutorialSpawnMonster();
            }
            if (stage.Item1 >= 1) 
            {
                stage = new Tuple<int, int>(0, 0);
                GameManager.Instance.setStage(stage);
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
                mainStage++;
                GameManager.Instance.setStage(new Tuple<int, int>(mainStage, 0));
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
                mainStage++;
                GameManager.Instance.setStage(new Tuple<int, int>(mainStage, 0));
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
    }
}