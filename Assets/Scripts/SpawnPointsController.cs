using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsController : MonoBehaviour
{
    public GameObject zombie;
    public GameObject cyberZombie;
    GameObject[] spawnPoints;
    float timeMakeZombie = 4.7f;
    float h = 0.9996f;
    float tick = 0;

    private void Start()
    {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            children.Add(this.transform.GetChild(i).gameObject);
        }
        this.spawnPoints = children.ToArray();
    }

    int SelectRandomSpawnPoint()
    {
        return Random.Range(0, this.spawnPoints.Length);
    }

    GameObject MakeZombie(int idx, bool isMutation)
    {
        var newZombie = Instantiate(this.zombie, this.spawnPoints[idx].transform.position, Quaternion.identity);
        var zombieControler = newZombie.GetComponent<ZombieController>();
        zombieControler.goal = PlayerController.Instance.gameObject;

        if (isMutation)
        {
            zombieControler.UpgradeToSuperState();
        }
        return newZombie;
    }

    GameObject MakeCyberZombie(int idx)
    {
        var newZombie = Instantiate(this.cyberZombie, this.spawnPoints[idx].transform.position, Quaternion.identity);
        newZombie.GetComponent<CyberZombieController>().goal = PlayerController.Instance.gameObject;
        return newZombie;
    }

    private void Update()
    {
        this.tick += Time.deltaTime;
        if (this.tick >= timeMakeZombie)
        {
            timeMakeZombie *= h;
            this.tick = 0;
            int idx = SelectRandomSpawnPoint();
            int randValue = Random.Range(0, 10);

            if (randValue <= 7)
            {
                bool isMutation = Random.Range(0, 40) <= 3;
                MakeZombie(idx, isMutation);
            }
            else
            {
                MakeCyberZombie(idx);
            }
        }
    }
}
