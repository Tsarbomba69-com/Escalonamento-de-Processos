using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Process : MonoBehaviour
{
    private Rigidbody2D rb;
    public State state;
    private SpriteRenderer renderer;
    private Camera mainCamera;
    private Color[] colors = new Color[] { Color.red, Color.yellow, Color.green };
    private float stateChangeCountdown;
    private float damageCooldown = 0.9f;
    private float elapsedTime = 0;
    private GameManager manager;
    [SerializeField] private float fadeAmplitude = 10;
    [SerializeField] private float fadeSpeed = 8;
    [SerializeField] private static Vector3 processDamage = new Vector3(0, 0.10f);
    [SerializeField] private AudioClip stateChangeSound;
    [SerializeField] private Sprite[] logos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        SpriteRenderer logoRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
        logoRender.sprite = logos[Random.Range(0, logos.Length)];
        state = new State(Random.Range(0.03f, 1), Random.Range(0.1f, 1), colors[Random.Range(0, 2)], Random.Range(5, 10));
        transform.localScale += new Vector3(0, state.size);
        renderer.color = state.color;
        stateChangeCountdown = state.rate;
        rb.gravityScale = state.speed;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        state.inStack = other.gameObject.layer == 3;
    }

    private void OnEnable()
    {
        mainCamera = Camera.main;
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        manager.gameOverEvent += Disable;
    }

    void OnDisable()
    {
        manager.gameOverEvent -= Disable;
    }

    void Disable()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        damageCooldown -= Time.deltaTime;

        if (stateChangeCountdown < 0)
        {
            state.color = colors[state.color == Color.red ? 1 : 0];
            renderer.color = state.color;
            stateChangeCountdown = state.rate;
            return;
        }

        if (stateChangeCountdown < 2)
        {
            elapsedTime += Time.fixedDeltaTime;
            float alpha = fadeAmplitude * Mathf.Sin(elapsedTime * fadeSpeed);
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
        }

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        stateChangeCountdown -= Time.deltaTime;


        if (hit.transform == transform && Input.GetMouseButtonDown(0))
        {
            if (Color.yellow == state.color)
            {
                Process prev = manager.prevProcess;

                if (prev != null)
                {
                    prev.state.color = Color.yellow;
                    prev.renderer.color = Color.yellow;
                }

                manager.prevProcess = this;
                state.color = Color.green;
                AudioSource.PlayClipAtPoint(stateChangeSound, transform.position, 1);
                renderer.color = state.color;
            }
        }

        if (Color.green == state.color && damageCooldown < 0)
        {
            transform.localScale -= processDamage;
            damageCooldown = 0.9f;
            manager.UpdateScore(15);
            if (transform.localScale.y < 0.02f)
            {
                manager.UpdateScore((uint)(state.size + state.speed - state.rate) * 2);
                Destroy(gameObject);
            }
        }

    }
}


public struct State
{
    public float size;
    public float speed;
    public float rate;
    public Color color;
    public bool inStack;

    public State(float size, float speed, Color color, float rate)
    {
        this.size = size;
        this.speed = speed;
        this.color = color;
        this.rate = rate;
        inStack = false;
    }
}


