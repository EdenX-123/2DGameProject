using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public  bool isInvincible = false;
    public float invincibleTime = 1f;
    [Header("knockback settings")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    public Animator anim;
    private bool isDead = false;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage, bool ignoreInvincible = false)
    {
        if (isDead) return;
        if (isInvincible && !ignoreInvincible) return;

        currentHealth -= damage;
        Debug.Log("Player HP: " + currentHealth);

        anim.SetTrigger("isHurt");
        StartCoroutine(InvincibleCoroutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 被怪物攻击时击退
    public void TakeDamageWithKnockback(int damage, Vector2 attackerPosition)
    {
        if (isDead || isInvincible) return;

        TakeDamage(damage);

        // 计算击退方向（远离攻击者）
        Vector2 knockbackDir = ((Vector2)transform.position - attackerPosition).normalized;
        StartCoroutine(KnockbackCoroutine(knockbackDir));
    }

    IEnumerator KnockbackCoroutine(Vector2 direction)
    {
        PlayerCtrl ctrl = GetComponent<PlayerCtrl>();
        if (ctrl != null) ctrl.isKnockedBack = true;

        float timer = 0f;
        while (timer < knockbackDuration)
        {
            rb.linearVelocity = new Vector2(direction.x * knockbackForce, knockbackForce * 0.5f);
            timer += Time.deltaTime;
            yield return null;
        }

        if (ctrl != null) ctrl.isKnockedBack = false;
    }

    IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    void Die()
    {
        isDead = true;

        anim.SetTrigger("isDead"); // play death animation

        GetComponent<PlayerCtrl>().enabled = false; // disable movement
        
        Rigidbody2D rb = GetComponentInChildren<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;

        // 可选：冻结
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

}
