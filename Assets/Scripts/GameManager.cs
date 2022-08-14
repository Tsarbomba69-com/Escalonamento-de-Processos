using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    private GameObject[] spawnPoints;
    [SerializeField] private float spawnRate = 3;
    [SerializeField] private float spawnCooldown = 3;
    [SerializeField] private Transform processPrefab;
    [SerializeField] private float loseTimer = 1;
    [SerializeField] private ContactFilter2D filter;
    [SerializeField] private GameObject panel;
    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private TextMeshProUGUI scoreTxt;
    private Collider2D killBar;
    private List<Collider2D> processesToKill = new List<Collider2D>(13);
    private float spawnTimer;
    private Camera mainCamera;
    private bool waiting = false;
    private static Color backgroundColor = new Color32(5, 118, 121, 0);
    private AudioSource audioSource;
    private uint score = 0;
    public delegate void EventHandler();
    public event EventHandler gameOverEvent;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = spawnRate;
        mainCamera = Camera.main;
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        killBar = GameObject.FindGameObjectWithTag("Finish").GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        Instantiate(processPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
    }

    enum Sounds
    {
        Alert = 0,
        Lost = 1,
        Score = 2,
        BigScore = 3,
    }

    IEnumerator SpawnProcess()
    {
        waiting = true;

        foreach (var sp in spawnPoints)
        {
            Instantiate(processPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnCooldown);
        }

        waiting = false;
        yield break;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (waiting) return;

        if (spawnTimer < 0)
        {
            StartCoroutine(SpawnProcess());
            spawnTimer = spawnRate;
        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (loseTimer < 0)
        {
            panel.SetActive(true);
            audioSource.PlayOneShot(sounds[(int)Sounds.Lost]);
            Time.timeScale = 0;
            gameOverEvent?.Invoke();
        }

        if (Physics2D.OverlapCollider(killBar, filter, processesToKill) > 0)
        {
            foreach (var p in processesToKill)
            {
                Process process = p.GetComponent<Process>();
                loseTimer -= process.state.inStack ? Time.deltaTime : 0;
                mainCamera.backgroundColor = process.state.inStack ? Color.Lerp(backgroundColor, Color.red, Mathf.PingPong(Time.time, 1)) : backgroundColor;
                if (process.state.inStack && !audioSource.isPlaying) audioSource.PlayOneShot(sounds[(int)Sounds.Alert], 0.5f);
            }
        }
        else
        {
            mainCamera.backgroundColor = backgroundColor;
            loseTimer = 5;
        }
    }

    public void UpdateScore(uint value)
    {
        score += value;
        audioSource.PlayOneShot(value > 15 ? sounds[(int)Sounds.BigScore] : sounds[(int)Sounds.Score]);
    }

    void OnGUI()
    {
        scoreTxt.SetText($"Pontuação: {score}");
    }
}
