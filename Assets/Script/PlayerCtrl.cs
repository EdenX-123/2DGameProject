using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;

public class PlayerCtrl : MonoBehaviour
{
    public Player_Combat player_combat;
    public PlayerHealth PlayerHealth;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float deathY = -10f;
    [SerializeField] private Transform respawnPoint;

    // Ground check variables
    [SerializeField] Transform groundCheck;
    [SerializeField] float checkRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] private CapsuleCollider2D hitbox;
    [SerializeField] private bool showHitbox = true;

    
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    Rigidbody2D rb;
    public Animator anim;

    float moveX;
    bool isGrounded;
     public bool isKnockedBack = false; // 被击退时 FixedUpdate 不干扰

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        player_combat = GetComponentInChildren<Player_Combat>();
        PlayerHealth = GetComponent<PlayerHealth>();
    
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDroppingDead && !isKnockedBack)
        {
            HandleMovement();
            CheckGround();

            if (Keyboard.current.xKey.wasPressedThisFrame)
                player_combat.Attack();
        }

        if (transform.position.y < deathY && !isDroppingDead)
        {
            StartCoroutine(FallDeath());
        }

    }
    
    private bool isDroppingDead = false;
    IEnumerator FallDeath()
    {
        isDroppingDead = true;
        isKnockedBack = true;

        anim.SetBool("isGrounded", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isJumping", true);

        // 先弹一下再下落
        rb.gravityScale = 1f; // 确保重力正常
        rb.linearVelocity = new Vector2(0f, 4f);

        yield return new WaitForSeconds(0.3f);

        // 加速下落
        rb.gravityScale = 4f;

        yield return new WaitForSeconds(0.8f);

        // 全部重置后再传送
        rb.gravityScale = 3f;
        rb.linearVelocity = Vector2.zero;

        yield return null; // 等一帧确保物理更新完成

        transform.position = respawnPoint.position;

        yield return null; // 再等一帧

        isDroppingDead = false;
        isKnockedBack = false;

        anim.SetBool("isJumping", false);

        // 强制扣血
        PlayerHealth.TakeDamage(1, ignoreInvincible: true);
    }

    // Handle player movement and jumping
    void HandleMovement()
    {
        moveX = 0;

        if (Keyboard.current.leftArrowKey.isPressed)
            moveX = -1;

        if (Keyboard.current.rightArrowKey.isPressed)
            moveX = 1;

        if ((Keyboard.current.zKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isJumping", rb.linearVelocity.y > 0.1f);
        anim.SetBool("isRunning", moveX != 0);

        if (moveX != 0)
            spriteRenderer.flipX = moveX < 0;
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    void FixedUpdate()
    {
        // 掉坑或击退中不覆盖速度
        if (isDroppingDead || isKnockedBack) return;

        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }

    private void OnDrawGizmos()
    {
        if (!showHitbox) return;
        if (hitbox != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(hitbox.offset, hitbox.size);
        }
    }

}
