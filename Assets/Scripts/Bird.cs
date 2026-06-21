using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Bird : MonoBehaviour
{
    private UI_Manager UIm;
    private GameManager gm;
    private Coins_Manager cm;

    private Animator animate;
    private AudioSource sfx;
    private Touch touch;
    private Rigidbody2D rb;
    [HideInInspector] public bool IsDead;

    [SerializeField] private ParticleSystem feather,
                                            DeathEffect;
    public float JumpForce;

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void Awake()
    {
        cm = Coins_Manager.Instance;
        gm = GameManager.Ins;
        UIm = UI_Manager.Instance;
        animate = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sfx = GetComponent<AudioSource>();
        touch = new Touch();
    }

    //private void Start()
    //{
    //    touch.TouchControls.TouchPress.started += ctx => StartTouch(ctx);
    //}

    private void Update()
    {
        Check_Death();

        if (!gm.IsLevelStarted)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Move();
    }

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (!IsDead
                && !gm.IsPaused
                && gm.IsLevelStarted)
            {
                rb.AddForce(Vector3.up * JumpForce);

                animate.SetTrigger("fly");

                if (feather != null)
                {
                    feather.Play();
                }

                sfx.Play();
            }
        }
    }

    //private void StartTouch(InputAction.CallbackContext context)
    //{
    //    if (!IsDead
    //        && !gm.IsPaused
    //        && gm.IsLevelStarted)
    //    {
    //        rb.AddForce(Vector3.up * JumpForce);

    //        animate.SetTrigger("fly");

    //        if (feather != null)
    //        {
    //            feather.Play();
    //        }

    //        sfx.Play();
    //    }
    //}

    private void Check_Death()
    {
        if ((transform.position.y >= Camera.main.orthographicSize
            || transform.position.y <= -Camera.main.orthographicSize)
            && !IsDead)
        {
            Died();
        }
    }

    private void Died()
    {
        IsDead = true;

        gm.On_Death?.Invoke();

        DeathEffect.Play();

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("point")
            && !IsDead)
        {
            gm.On_ScoreIncrease?.Invoke();

            cm.Add_Coins(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Died();
    }

    //private void OnEnable()
    //{
    //    touch.Enable();
    //}

    //private void OnDisable()
    //{
    //    touch.Disable();
    //}
}