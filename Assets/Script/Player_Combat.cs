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
    private Rigidbody2D rb;
    private PlayerCtrl playerCtrl;
    private PlayerHealth playerHealth;
    private PlayerEnergy playerEnergy;

    [SerializeField] private bool showHitbox = true;
    [Header("Combat Feel")]
    [SerializeField] private float normalHitStop = 0.035f;
    [SerializeField] private float parryHitStop = 0.06f;
    [SerializeField] private float parryInvincibleTime = 0.25f;
    [SerializeField] private float parryPlayerRecoil = 5f;
    [SerializeField] private float parryEnemyRecoil = 3f;
    [SerializeField] private float recoilDuration = 0.12f;

    public bool IsAttackActive => isAttacking;


    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponentInParent<Rigidbody2D>();
        playerCtrl = GetComponentInParent<PlayerCtrl>();
        playerHealth = GetComponentInParent<PlayerHealth>();
        playerEnergy = GetComponentInParent<PlayerEnergy>();
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

    public void ApplyCombatRecoil(Vector2 sourcePosition, float force, float duration)
    {
        if (rb == null) return;
        StartCoroutine(CombatRecoilCoroutine(sourcePosition, force, duration));
    }

    IEnumerator CombatRecoilCoroutine(Vector2 sourcePosition, float force, float duration)
    {
        if (playerCtrl != null)
            playerCtrl.isKnockedBack = true;

        Vector2 direction = ((Vector2)rb.transform.position - sourcePosition).normalized;
        if (Mathf.Abs(direction.x) < 0.1f)
            direction.x = spriteRenderer != null && spriteRenderer.flipX ? 1f : -1f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            rb.linearVelocity = new Vector2(direction.x * force, Mathf.Max(rb.linearVelocity.y, force * 0.35f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (playerCtrl != null)
            playerCtrl.isKnockedBack = false;
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

            SlimeCtrl slime = enemy.GetComponentInParent<SlimeCtrl>();
            if (slime != null && slime.IsAttackActive)
            {
                TriggerParry(slime);
                continue;
            }

            // apply damage to the enemy's health component
            health.ChangeHealth(-attackDamage, transform.position, GetPlayerEnergy());
            CombatFeedback.HitStop(this, normalHitStop);
            Debug.Log("Hit: " + enemy.name);
            }
        }
    }

    private void TriggerParry(SlimeCtrl slime)
    {
        PlayerEnergy energy = GetPlayerEnergy();
        if (energy != null)
            energy.AddEnergy(1);

        if (playerHealth != null)
            playerHealth.GrantBriefInvincible(parryInvincibleTime);

        ApplyCombatRecoil(slime.transform.position, parryPlayerRecoil, recoilDuration);
        slime.ApplyCombatRecoil(transform.position, parryEnemyRecoil, recoilDuration);
        slime.ResetAttackState();
        CombatFeedback.HitStop(this, parryHitStop);
        Debug.Log("Parry!");
    }

    private PlayerEnergy GetPlayerEnergy()
    {
        if (playerEnergy != null) return playerEnergy;

        playerEnergy = GetComponentInParent<PlayerEnergy>();
        if (playerEnergy == null && rb != null)
            playerEnergy = rb.gameObject.AddComponent<PlayerEnergy>();

        return playerEnergy;
    }
}
