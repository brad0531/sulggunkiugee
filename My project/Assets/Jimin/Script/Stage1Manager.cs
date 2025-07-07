using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1Manager : MonoBehaviour
{
    public GameObject Monster;

    void SpawnMonster()
    {
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        GameObject enemy = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x + 1000, playerPosition.y), Quaternion.identity);
        for (int i = 1; i < 10; i++) {
            GameObject enemys = (GameObject)Instantiate(Monster, new Vector3(playerPosition.x +1000 + 500*i, playerPosition.y), Quaternion.identity);
        }
    }

    void Start()
    {
        Invoke("SpawnMonster", 3f);
    }

    void Update()
    {
        /* if () {
            SceneManager.LoadScene("Stage2"); 
        }
        */
    }
}
