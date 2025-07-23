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
    public Tuple<int, int> stage = new Tuple<int, int>(0, 0); //gamemanager에서 Stage 받아오기 

    private GameObject enemy;
    private GameObject enemies;
    private GameObject BossMon;

    private bool isRespawning = false;
    private bool tutorialClear = false;
    private int Stagelevel = 0;
    private List<GameObject> enemiesList = new List<GameObject>();

    void SpawnMonsters()
    {
        enemiesList.Clear();
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        enemy = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 1000, playerPosition.y), Quaternion.identity);
        enemiesList.Add(enemy);
        for (int i = 1; i < 10; i++)
        {
            enemies = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 1000 + 2000 * i, playerPosition.y), Quaternion.identity);
            enemiesList.Add(enemies);
        }
        isRespawning = false;
    }

    void TutorialSpawnMonster() 
    {
        enemiesList.Clear();
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        enemy = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 500, playerPosition.y), Quaternion.identity);
        enemiesList.Add(enemy);
        for (int i = 1; i < 50; i++)
        {
            enemies = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 500 + 1200 * i, playerPosition.y), Quaternion.identity);
            enemiesList.Add(enemies);
        }
        isRespawning = false;
    }

    void BossSpawn()
    {
        FadeInOut.Fade(fadeEffect);
        // 플레이어 리스폰 함수 호출 
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        BossMon = (GameObject)Instantiate(Boss, new Vector3(playerPosition.x + 500, playerPosition.y), Quaternion.identity);
        //대화 스크립트 출력
    }

    void Respawn()
    {
        FadeInOut.Fade(fadeEffect);
        //정재가 만든 플레이어 리스폰 함수 호출 
        // 혹시 몰라서 허락 받고 하려고 남겨둠
        SpawnMonsters();
    }


    /* void Playerdying()
    {
         //if(플레이어 죽음 판정 시)
        {
            stage = GameManager.Instance.getStage();
            if (stage.Item1 <= 0) //튜토리얼
            { 
            }
            foreach (var e in enemiesList)
            {
                e.SetActive(false);
            }

        }
    } */



    void Start()
    {
        Debug.Log(GameManager.Instance);
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
                GameManager.Instance.setStage(new Tuple<int, int>(1, 0));
            }
        }
        //1-0 ~ 1-4
        if (Stagelevel <= 4)
        {
            if (enemiesList.All(e => e != null && !e.activeSelf) && !isRespawning) //몬스터가 비활성화 될 시
            {
                isRespawning = true;
                Stagelevel++;
                Respawn();
            }
        }
   
        

        else if (Stagelevel == 5) 
        {
            if (enemiesList.All(e => e != null && !e.activeSelf) && !isRespawning)
            {
                // 1-5
                isRespawning = true;
                BossSpawn();
                Stagelevel++;
            }
            
        }

        //Stage1 Clear
        else if (Stagelevel == 6)
        {
            if (!isRespawning && BossMon != null && !BossMon.activeSelf)
            {
                FadeInOut.Fade(fadeEffect);
                SceneManager.LoadScene("Stage2");
            }
        }
    }
}