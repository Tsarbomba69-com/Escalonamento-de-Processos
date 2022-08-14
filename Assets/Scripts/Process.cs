using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Process : MonoBehaviour
{
    private Rigidbody2D rb;
    public State state;
    private SpriteRenderer _renderer;
    private Camera mainCamera;
    private Color[] colors = new Color[] { Color.red, Color.green, Color.yellow };
    private float stateChangeCountdown;
    private float cooldownTime;
    private float elapsedTime = 0;
    private GameManager manager;
    [SerializeField] private float fadeAmplitude = 10;
    [SerializeField] private float fadeSpeed = 8;
    [SerializeField] private static Vector3 processDamage = new Vector3(0, 0.08f, 1);
    [SerializeField] private AudioClip stateChangeSound;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        state = new State(Random.Range(0.03f, 1), Random.Range(0.1f, 1), colors[Random.Range(0, 3)], Random.Range(1, 5));
        transform.localScale += new Vector3(0, state.size, 1);
        _renderer.color = state.color;
        stateChangeCountdown = 5;
        cooldownTime = 5;
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
        cooldownTime -= Time.deltaTime;

        if (cooldownTime > 0)
        {
            stateChangeCountdown = state.rate;
            return;
        }

        if (stateChangeCountdown < 0)
        {
            state.color = colors[Random.Range(0, 3)];
            _renderer.color = state.color;
            cooldownTime = state.rate;
        }
        else
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            stateChangeCountdown -= Time.deltaTime;
            elapsedTime += Time.fixedDeltaTime;
            float alpha = fadeAmplitude * Mathf.Sin(elapsedTime * fadeSpeed);
            _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);

            if (hit.transform == transform && Input.GetMouseButtonDown(0))
            {
                if (Color.green == state.color)
                {
                    transform.localScale -= processDamage;
                    manager.UpdateScore(15);
                    if (transform.localScale.y < 0.02f)
                    {
                        manager.UpdateScore((uint)(state.size + state.speed - state.rate) * 2);
                        Destroy(gameObject);
                    }
                }
                else if (Color.yellow == state.color)
                {
                    state.color = Color.green;
                    AudioSource.PlayClipAtPoint(stateChangeSound, transform.position, 1);
                    _renderer.color = state.color;
                }
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


