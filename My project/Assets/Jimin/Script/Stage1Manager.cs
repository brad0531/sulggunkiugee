using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1Manager : MonoBehaviour
{
    public GameObject Monster;
    public Vector3 PlayerRespawnPoint;

    void SpawnMonster()
    {
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        GameObject enemy = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 1000, playerPosition.y), Quaternion.identity);
        for (int i = 1; i < 10; i++) {
            GameObject enemys = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x +1500 + 1200*i, playerPosition.y), Quaternion.identity);
        }
    }

    void Respawn()
    {
        //if() //스테이지 이동
        {
            GameObject.FindWithTag("Player").transform.position = PlayerRespawnPoint;
        }
         
    }

    void Start()
    {
        Respawn();
        Invoke("SpawnMonster", 1f);
    }

    void Update()
    {
        /* if () {
            SceneManager.LoadScene("Stage2"); 
        }
        */
    }
}
