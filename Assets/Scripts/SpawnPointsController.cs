using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsController : MonoBehaviour
{
    public GameObject zombie;
    GameObject[] spawnPoints;
    float tick = 0;

    private void Start()
    {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            children.Add(this.transform.GetChild(i).gameObject);
        }
        this.spawnPoints = children.ToArray();
        Debug.Log(this.spawnPoints.Length);
    }

    int SelectRandomSpawnPoint()
    {
        return Random.Range(0, this.spawnPoints.Length);
    }

    void MakeZombie(int idx)
    {
        var newZombie = Instantiate(this.zombie, this.spawnPoints[idx].transform.position, Quaternion.identity);
        newZombie.GetComponent<ZombieController>().goal = PlayerController.Instance.gameObject;
    }

    private void Update()
    {
        this.tick += Time.deltaTime;
        if (this.tick >= 2)
        {
            this.tick = 0;
            int idx = SelectRandomSpawnPoint();
            MakeZombie(idx);
        }
    }
}
