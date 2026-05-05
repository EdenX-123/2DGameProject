using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SlimeCtrl : MonoBehaviour
{

   private Rigidbody2D rb;
    private Animator anim;

     [Header("movement settings")]
    public float moveSpeed = 2f;
    public bool usePatrolPoints = false; // true=用指定点, false=用平台边缘检测
    [SerializeField] private float deathY = -10f; // 掉出地图死亡Y值

     // 方式一：指定巡逻范围
    [SerializeField] private Transform patrolPointLeft;
    [SerializeField] private Transform patrolPointRight;

    // 方式二：平台边缘检测
    [SerializeField] private Transform edgeCheckLeft;
    [SerializeField] private Transform edgeCheckRight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float edgeCheckRadius = 0.1f;

    // 检测墙壁
    [SerializeField] private Transform wallCheckFront;
    [SerializeField] private float wallCheckRadius = 0.1f;
    [SerializeField] private LayerMask wallLayer;
    private bool movingRight = true;
    private SpriteRenderer spriteRenderer;


    [Header("attack settings")]
    public int attackDamage = 1;
    public int touchDamage = 1; // 接触伤害
    public float attackCooldown = 2f;      // 攻击后摇/冷却时间
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool canAttack = true;

    [Header("attack hitbox settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 attackSize = new Vector2(0.8f, 0.6f);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Collider2D hitbox;
    [SerializeField] private bool showHitbox = true;
    private float patrolLeftX;
    private float patrolRightX;
    public bool isKnockedBack = false;
    private float wallCheckCooldown = 0f; // 新增

    private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 游戏开始时保存世界坐标，之后巡逻点跟着动也没关系
        if (patrolPointLeft != null)  patrolLeftX  = patrolPointLeft.position.x;
        if (patrolPointRight != null) patrolRightX = patrolPointRight.position.x;

    }

    // Update is called once per frame
    void Update()
    {

        if (attackTimer > 0)
        attackTimer -= Time.deltaTime;

        if (wallCheckCooldown > 0)
        wallCheckCooldown -= Time.deltaTime; // ✅ 新增
        // 每帧检测玩家是否进入范围 → 触发攻击动画
        if (canAttack && attackTimer <= 0)
        {
            CheckIfPlayerInRange();
        }

        // 攻击动画进行中 → 持续检测攻击箱
        if (isAttacking)
        {
            DoHitboxCheck();
        }

        // falldead
        if (transform.position.y < deathY)
        {
            GetComponent<Enemy_Health>().Die();
        }
    }

    void FixedUpdate()
    {
        // 攻击中、击退中不移动
        if (isAttacking || !canAttack || isKnockedBack ||isFlipping) return;

        CheckWallAndFlip();
        Patrol();
    }

      // 检测前方墙壁自动翻转
    void CheckWallAndFlip()
    {
        if (wallCheckFront == null) return;
        if (isKnockedBack || isFlipping) return;
        if (wallCheckCooldown > 0) return; // ✅ 冷却中不检测

        bool hitWall = Physics2D.OverlapCircle(wallCheckFront.position, wallCheckRadius, wallLayer);
        if (hitWall)
        {
            StartCoroutine(FlipWithDelay());
        }
    }
    

    private bool isFlipping = false;
    IEnumerator FlipWithDelay()
    {
        isFlipping = true;
        rb.linearVelocity = Vector2.zero;
        Flip();

        float elapsed = 0f;
        while (elapsed < 0.2f)
        {
            if (isKnockedBack) // ✅ 击退时立刻退出协程
            {
                isFlipping = false;
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        isFlipping = false;
    }
    public void ForceStopFlip()
    {
        isFlipping = false;
        wallCheckCooldown = 0.5f; // ✅ 击退结束后0.5秒内不检测墙壁
    }

    //movement logic
    void Patrol()
    {
        if (usePatrolPoints)
            PatrolWithPoints();
        else
            PatrolWithEdgeCheck();
    }

    // 方式一：在两个指定点之间来回
    void PatrolWithPoints()
    {
        if (patrolPointLeft == null || patrolPointRight == null) return;

        if (movingRight)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            if (transform.position.x >= patrolRightX)
            {
                transform.position = new Vector3(patrolRightX, transform.position.y, transform.position.z);
                movingRight = false;
                Flip(); // ✅ 统一用Flip()
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            if (transform.position.x <= patrolLeftX)
            {
                transform.position = new Vector3(patrolLeftX, transform.position.y, transform.position.z);
                movingRight = true;
                Flip(); // ✅ 统一用Flip()
            }
        }
    }

    // 方式二：检测平台边缘自动折返
    void PatrolWithEdgeCheck()
    {
        if (movingRight)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

            // 检测右边是否到边缘（地面消失）
            bool rightEdge = edgeCheckRight != null &&
                !Physics2D.OverlapCircle(edgeCheckRight.position, edgeCheckRadius, groundLayer);

            if (rightEdge) Flip();
        }
        else
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);

            bool leftEdge = edgeCheckLeft != null &&
                !Physics2D.OverlapCircle(edgeCheckLeft.position, edgeCheckRadius, groundLayer);

            if (leftEdge) Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        // 用Scale翻转，所有子物体自动跟着镜像 ✅
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamageWithKnockback(touchDamage, transform.position);
            }
        }
    }

    //attack logic
    //check if player in attack range
    void CheckIfPlayerInRange()
    {
        if (attackPoint == null) return;

        Collider2D hit = Physics2D.OverlapBox(attackPoint.position,attackSize,0f,playerLayer);

        if (hit != null)
        {
            TriggerAttack();
        }
    }
    void TriggerAttack()
    {
        canAttack = false;
        rb.linearVelocity = Vector2.zero; // 攻击时停下
        anim.SetTrigger("isAttack"); // 播放攻击动画
    }

    // ========== Animation Event ==========

    // 动画帧：攻击开始（开始检测攻击箱）
    public void StartAttack()
    {
        isAttacking = true;
        hitTargets.Clear(); // 每次新攻击清空记录
    }

    // 动画帧：攻击结束（停止检测攻击箱）
    public void EndAttack()
    {
        isAttacking = false;
        hitTargets.Clear();
        attackTimer = attackCooldown; // 开始后摇冷却
        canAttack = true;
    }

    // 实际伤害检测（每帧调用）
    void DoHitboxCheck()
    {
        if (attackPoint == null) return;

        Collider2D hit = Physics2D.OverlapBox(attackPoint.position,attackSize,0f,playerLayer);

        if (hit != null && !hitTargets.Contains(hit))
        {
            hitTargets.Add(hit); // 将命中的目标加入集合
            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                // 传入自身位置，让玩家被击退
                player.TakeDamageWithKnockback(attackDamage, transform.position);
                Debug.Log("Slime attacked player!");
               
            }
        }
    }

    public void ResetAttackState()
    {
        isAttacking = false;
        canAttack = true;
        attackTimer = 0.3f;
    }

    private void OnDrawGizmos()
    {
        if (!showHitbox) return;
        // 每次画之前重置矩阵！
        Gizmos.matrix = Matrix4x4.identity;

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackPoint.position, attackSize);
        }

        if (hitbox != null)
        {
            Gizmos.color = Color.yellow;
            // 局部坐标用 localToWorldMatrix
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(hitbox.offset, hitbox.bounds.size);
            // 画完立刻重置！
            Gizmos.matrix = Matrix4x4.identity;
        }

        // 巡逻点：用世界坐标，matrix 必须是 identity
        if (patrolPointLeft != null && patrolPointRight != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(patrolPointLeft.position, patrolPointRight.position);
            Gizmos.DrawSphere(patrolPointLeft.position, 0.15f);
            Gizmos.DrawSphere(patrolPointRight.position, 0.15f);
        }

        // 边缘检测点
        if (edgeCheckLeft != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(edgeCheckLeft.position, edgeCheckRadius);
        }
        if (edgeCheckRight != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(edgeCheckRight.position, edgeCheckRadius);
        }
         // 墙壁检测点
        if (wallCheckFront != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(wallCheckFront.position, wallCheckRadius);
        }
    }

}
