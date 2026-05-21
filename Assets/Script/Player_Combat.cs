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
        //when the attack button is pressed and cooldown timer is 0 or less,
        //trigger attack animation and reset timer
        if (timer <= 0)
        {
            //attack animation will call StartAttack and EndAttack events to set isAttacking flag
            anim.SetTrigger("Attack");
            timer = cooldown;
            AudioManager.instance.PlayAttack();
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

    //the hit box check method, called during attack animation when isAttacking is true
    void DoHitboxCheck()
    {
        //calculate hitbox position based on attack point, direction, and distance
        float direction = spriteRenderer.flipX ? -1f : 1f;
        Vector2 hitboxPosition = (Vector2)attackPoint.position + new Vector2(direction * attackDistance, 0);
        //check for enemies in the hitbox area using OverlapBoxAll
        Collider2D[] enemies = Physics2D.OverlapBoxAll(hitboxPosition, attackSize, 0f, enemyLayers);

        //for each enemy hit, apply damage and add to 
        // hitEnemies set to avoid hitting the same enemy multiple times in one attack
        foreach (Collider2D enemy in enemies)
        {
            //if enemy is not already hit in this attack, apply damage
            if (!hitEnemies.Contains(enemy))
            {   
                // add enemy to hit set to prevent multiple hits in one attack
                hitEnemies.Add(enemy);

            // try to get Enemy_Health component from the enemy or its parent
            Enemy_Health health = enemy.GetComponentInParent<Enemy_Health>();
            if (health == null) continue;

            // apply damage to the enemy's health component
            health.ChangeHealth(-attackDamage, transform.position);
            Debug.Log("Hit: " + enemy.name);
            }
        }
    }
}