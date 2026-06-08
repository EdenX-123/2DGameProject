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
    private Coroutine invincibleCoroutine;

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

    //when player takes damage, 
    //reduce current health by damage amount and trigger hurt animation
    public void TakeDamage(int damage, bool ignoreInvincible = false)
    {
        //check if player is already dead 
        // or currently invincible 
        // (unless ignoreInvincible is true(ignoreInvincible is the damage from falling, which should always apply))
        if (isDead) return;
        if (isInvincible && !ignoreInvincible) return;

        //reduce current health by damage amount
        currentHealth -= damage;
        Debug.Log("Player HP: " + currentHealth);

        //trigger hurt animation and play sound
        anim.SetTrigger("isHurt");
        AudioManager.instance.PlayTakeDamage();
        StartInvincible(invincibleTime);

        //if health drops to 0 or below, trigger death
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

    public bool Heal(int amount)
    {
        if (isDead || amount <= 0 || currentHealth >= maxHealth) return false;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log("Player HP: " + currentHealth);
        return true;
    }

    public void GrantBriefInvincible(float duration)
    {
        if (isDead || duration <= 0f) return;
        StartInvincible(duration);
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

    IEnumerator InvincibleCoroutine(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        invincibleCoroutine = null;
    }

    private void StartInvincible(float duration)
    {
        if (invincibleCoroutine != null)
            StopCoroutine(invincibleCoroutine);

        invincibleCoroutine = StartCoroutine(InvincibleCoroutine(duration));
    }

    //player death logic: trigger death animation, disable player control, and respawn after delay
    void Die()
    {
        //check if already dead to prevent multiple death triggers
        if (isDead) return;
        isDead = true;
        PlayerEnergy energy = GetComponent<PlayerEnergy>();
        if (energy != null)
            energy.ResetEnergy();

        //stop all movement and actions
        StopAllCoroutines();
        invincibleCoroutine = null;

        //deadth animation 
        anim.SetTrigger("isDead");
        
        //disable player control and movement
        PlayerCtrl ctrl = GetComponent<PlayerCtrl>();
        if (ctrl != null)
        {
            ctrl.enabled = false;
            ctrl.isKnockedBack = false;
            ctrl.isDroppingDead = false;
        }

        //freeze player in place
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll; // no movement after death

        //respawn after delay
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
        PlayerEnergy energy = GetComponent<PlayerEnergy>();
        if (energy != null)
            energy.ResetEnergy();

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
