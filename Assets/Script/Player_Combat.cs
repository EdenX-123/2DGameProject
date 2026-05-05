using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public SpriteRenderer spriteRenderer;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 attackSize = new Vector2(1.2f, 0.6f);
    [SerializeField] private float attackDistance = 1f;

    public LayerMask enemyLayers;
    public int attackDamage = 1;

    public float cooldown = 0.5f;
    private float timer;
    bool isAttacking = false;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    [SerializeField] private bool showHitbox = true;


    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        
        if (timer > 0)
            timer -= Time.deltaTime;

        if (isAttacking)
        {
            DoHitboxCheck();
        }
    }

    public void Attack()
    {
        
        if (timer <= 0)
        {
            anim.SetTrigger("Attack");
            timer = cooldown;
        }
    }

    // show hitbox in editor
    private void OnDrawGizmos()
    {
        if (!showHitbox) return;
        // ========= 攻击框 =========
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;

            float direction = 1f;
            if (spriteRenderer != null)
                direction = spriteRenderer.flipX ? -1f : 1f;

            Vector2 hitboxPosition = (Vector2)attackPoint.position + new Vector2(direction * attackDistance, 0);

            Gizmos.DrawWireCube(hitboxPosition, attackSize);
        }

    }

    //  Animation Event 
        public void StartAttack()
    {
        isAttacking = true;
        hitEnemies.Clear();
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    void DoHitboxCheck()
    {
        float direction = spriteRenderer.flipX ? -1f : 1f;
        Vector2 hitboxPosition = (Vector2)attackPoint.position + new Vector2(direction * attackDistance, 0);

        Collider2D[] enemies = Physics2D.OverlapBoxAll(hitboxPosition, attackSize, 0f, enemyLayers);

        foreach (Collider2D enemy in enemies)
        {
            if (!hitEnemies.Contains(enemy))
            {
                hitEnemies.Add(enemy);
                
            // ✅ 加这个检查，找不到就跳过
            Enemy_Health health = enemy.GetComponentInParent<Enemy_Health>();
            if (health == null) continue;

            health.ChangeHealth(-attackDamage, transform.position);
            Debug.Log("Hit: " + enemy.name);
            }
        }
    }
}