using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject[] spawnPoints;
    [SerializeField] private float spawnRate = 3;
    [SerializeField] private float spawnCooldown = 3;
    [SerializeField] private Transform processPrefab;
    [SerializeField] public float loseTimer = 1;
    [SerializeField] private ContactFilter2D filter;
    private Collider2D killBar;
    private List<Collider2D> processesToKill = new List<Collider2D>(13);
    private float spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = spawnRate;
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        killBar = GameObject.FindGameObjectWithTag("Finish").GetComponent<Collider2D>();
        Instantiate(processPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position + Vector3.up * Random.value, Quaternion.identity);
    }

    IEnumerator SpawnProcess()
    {
        foreach (var sp in spawnPoints)
        {
            Instantiate(processPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnCooldown);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer < 0)
        {
            StartCoroutine(SpawnProcess());
            spawnTimer = spawnRate;
        }

        spawnTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (loseTimer < 0)
        {
            Debug.Log("Perdeu!");
        }

        if (Physics2D.OverlapCollider(killBar, filter, processesToKill) > 0)
        {
            foreach (var p in processesToKill)
            {
                loseTimer -= p.GetComponent<Process>().state.inStack ? Time.deltaTime : 0;
            }
        }
        else
        {
            loseTimer = 5;
        }
    }
}
