using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1Manager : MonoBehaviour
{
    public GameObject Monster;

    private GameObject enemy;
    private GameObject enemys;

    private bool isRespawning = false;

    void SpawnMonsters()
    {
        Monster.SetActive(true);
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        enemy = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 500, playerPosition.y), Quaternion.identity);
        for (int i = 1; i < 10; i++)
        {
            enemys = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 500 + 1200 * i, playerPosition.y), Quaternion.identity);
        }
        isRespawning = false;
    }

    void Respawn()
    {
        //정재가 만든 플레이어 리스폰 함수 호출 
        // 혹시 몰라서 허락 받고 하려고 남겨둠
        SpawnMonsters();
    }



    void Start()
    {
        Respawn();
    }

    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (enemy != null && enemys != null) {
                if (!enemy.activeSelf && !enemys.activeSelf)
                {
                    isRespawning = true;
                    Invoke("Respawn", 3f);
                }
            }
        }
        /* if () {
            SceneManager.LoadScene("Stage2"); 
        }
        */
    }
}