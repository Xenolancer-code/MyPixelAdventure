// PlayerManager.cs
using System.Collections;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    private GameObject gameManager;
    private GameObject sndManager;
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;

    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.12f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    private Vector2 movementVector;
    private int lifes;
    private bool isDead;
    private bool isPlayerReady;
    private bool isGrounded;

    // ── Setup ────────────────────────────────────────────────────────

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        sndManager = GameObject.FindGameObjectWithTag("SoundManager");
        gameManager = GameObject.FindGameObjectWithTag("GameManager");

        StartCoroutine(InitPlayer());
    }

    IEnumerator InitPlayer()
    {
        yield return new WaitForSeconds(0.75f);
        lifes = 3;
        isDead = false;
        isPlayerReady = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    // ── Update ───────────────────────────────────────────────────────

    void Update()
    {
        isGrounded = Physics2D.BoxCast(
            col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, jumpableGround);

        // Coyote time
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // Jump buffer
        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;

        TryJump();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!isPlayerReady) return;
        rb.linearVelocity = new Vector2(movementVector.x * speed, rb.linearVelocity.y);
    }

    // ── Métodos públicos — InputReceiver y botones UI los llaman ─────

    public void SetMovementVector(Vector2 input)
    {
        movementVector = input;
    }

    public void OnJump()
    {
        if (!isPlayerReady) return;
        jumpBufferCounter = jumpBufferTime;
    }

    // Botones móviles izquierda/derecha
    public void MoveLeft()   => SetMovementVector(Vector2.left);
    public void MoveRight()  => SetMovementVector(Vector2.right);
    public void StopMoving() => SetMovementVector(Vector2.zero);

    // ── Salto interno ────────────────────────────────────────────────

    private void TryJump()
    {
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            sndManager.GetComponent<SoundManager>().PlayFX(0);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    // ── Animación ────────────────────────────────────────────────────

    void UpdateAnimator()
    {
        // Movimiento horizontal y flip
        if (movementVector.x > 0f)
        {
            anim.SetBool("run", true);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (movementVector.x < 0f)
        {
            anim.SetBool("run", true);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            anim.SetBool("run", false);
        }

        // Salto y caída
        if (rb.linearVelocity.y > .1f)
        {
            anim.SetBool("jump", true);
            anim.SetBool("fall", false);
        }
        else if (rb.linearVelocity.y < -.1f)
        {
            anim.SetBool("jump", false);
            anim.SetBool("fall", true);
        }
        else
        {
            anim.SetBool("jump", false);
            anim.SetBool("fall", false);
        }
    }

    // ── Muerte y nivel ───────────────────────────────────────────────

    void KillPlayer()
    {
        if (isDead) return;
        isDead = true;
        isPlayerReady = false;
        lifes--;

        sndManager.GetComponent<SoundManager>().PlayFX(3);
        anim.SetTrigger("dead");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        StartCoroutine(lifes > 0 ? RestartLevel() : GameOver());
    }

    void CompleteLevel()
    {
        isPlayerReady = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        StartCoroutine(TriggerCompleteLevel());
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(2f);
        gameManager.GetComponent<GameManager>().RestartLevel();
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        gameManager.GetComponent<GameManager>().GameOver();
    }

    IEnumerator TriggerCompleteLevel()
    {
        yield return new WaitForSeconds(2f);
        gameManager.GetComponent<GameManager>().CompleteLevel();
    }

    // ── Colisiones ───────────────────────────────────────────────────

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Trap")  ||
            c.gameObject.CompareTag("Death") ||
            c.gameObject.CompareTag("Enemy"))
            KillPlayer();
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Finish"))
        {
            sndManager.GetComponent<SoundManager>().PlayFX(2);
            CompleteLevel();
        }
    }
}