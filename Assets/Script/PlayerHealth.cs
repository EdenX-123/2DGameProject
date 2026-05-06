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
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();

        anim.SetTrigger("isDead");
        
        PlayerCtrl ctrl = GetComponent<PlayerCtrl>();
        if (ctrl != null)
        {
            ctrl.enabled = false;
            ctrl.isKnockedBack = false;
            ctrl.isDroppingDead = false;
        }

        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll; // ✅ 防止滑行

        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(3f); // 等死亡动画

        GameManager.instance.PlayerDied();
        transform.position = GameManager.instance.GetDefaultRespawnPos();

        // 重置状态
        isDead = false;
        currentHealth = maxHealth;
        isInvincible = true; // ✅ 先开无敌

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocity = Vector2.zero;

        // 重启移动但先锁住输入
        PlayerCtrl ctrl = GetComponent<PlayerCtrl>();
        if (ctrl != null)
        {
            ctrl.enabled = true;
            ctrl.isKnockedBack = true;  // ✅ 用isKnockedBack锁住输入
            ctrl.isDroppingDead = false;
        }

        anim.Rebind();
        anim.Update(0f);

        // ✅ 等相机跟上（等几秒无敌时间）
        yield return new WaitForSeconds(1f);

        // ✅ 解除锁定，可以行动了
        isInvincible = false;
        if (ctrl != null)
            ctrl.isKnockedBack = false;

        Debug.Log("Player respawned and ready!");
    }

}
