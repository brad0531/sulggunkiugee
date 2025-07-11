using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Stage1Manager : MonoBehaviour
{
    public GameObject Monster;
    public GameObject Boss;

    private GameObject enemy;
    private GameObject enemies;
    private GameObject BossMon;

    private bool isRespawning = false;
    private int Stagelevel = 1;
    private List<GameObject> enemiesList = new List<GameObject>();

    void SpawnMonsters()
    {
        enemiesList.Clear();
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        enemy = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 500, playerPosition.y), Quaternion.identity);
        enemiesList.Add(enemy);
        for (int i = 1; i < 10; i++)
        {
            enemies = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 500 + 1200 * i, playerPosition.y), Quaternion.identity);
            enemiesList.Add(enemies);
        }
        Monster.SetActive(false);
        isRespawning = false;
    }

    void BossSpawn()
    {
        // 플레이어 리스폰 함수 호출 
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        BossMon = (GameObject)Instantiate(Boss, new Vector3(playerPosition.x + 500, playerPosition.y), Quaternion.identity);
        Boss.SetActive(false);
    }

    void Respawn()
    {
        //정재가 만든 플레이어 리스폰 함수 호출 
        // 혹시 몰라서 허락 받고 하려고 남겨둠
        SpawnMonsters();
    }



    void Start()
    {
        //1-0
        Respawn();
    }

    void Update()
    {

        //1-1 ~ 1-4
        if (Stagelevel <= 4)
        {
            if (enemiesList.All(e => e != null && !e.activeSelf) && !isRespawning)
            {
                isRespawning = true;
                Stagelevel++;
                Invoke("Respawn", 3f);
            }
        }
   
        

        else if (Stagelevel == 5) 
        {
            if (enemiesList.All(e => e != null && !e.activeSelf) && !isRespawning)
            {
                // 1-5
                isRespawning = true;
                Stagelevel++;
                BossSpawn();
            }
            
        }

        //Stage1 Clear
        else if (Stagelevel == 6)
        {
            if (!isRespawning && BossMon != null && !BossMon.activeSelf)
            {
                SceneManager.LoadScene("Stage2");
            }
        }
    }
}