using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject[] spawnPoints;
    [SerializeField] private float spawnRate = 3;
    [SerializeField] private Transform processPrefab;
    private float spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = spawnRate;
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer < 0)
        {
            foreach (var sp in spawnPoints)
            {
                Instantiate(processPrefab, sp.transform.position, Quaternion.identity);
            }

            spawnTimer = spawnRate;
        }

        spawnTimer -= Time.deltaTime;
    }
}
