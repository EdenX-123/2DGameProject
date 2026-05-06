using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Health : MonoBehaviour
{
    public Animator anim;

    public int currentHealth;
    public int maxHealth;
    bool isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int amount, Vector2 attackerPosition = default)
    {
        if (isDead) return;

        anim.SetTrigger("isDamaged");

        SlimeCtrl slime = GetComponent<SlimeCtrl>();
        if (slime != null)
            slime.ResetAttackState();

        // 击退：短促后跳，用协程快速衰减
        StartCoroutine(Knockback(attackerPosition));

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        else if (currentHealth <= 0)
        {
            Debug.Log("Enemy died");
            Die();
        }
    }

    IEnumerator Knockback(Vector2 attackerPosition)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        SlimeCtrl slime = GetComponent<SlimeCtrl>();

        // 击退时暂停移动
        if (slime != null) slime.isKnockedBack = true;

        Vector2 knockbackDir = ((Vector2)transform.position - attackerPosition).normalized;
        rb.linearVelocity = new Vector2(knockbackDir.x * 2.5f, 2f);

        yield return new WaitForSeconds(0.15f);

        rb.linearVelocity = Vector2.zero;

        if (slime != null)
        {
            slime.isKnockedBack = false;
            slime.ForceStopFlip(); // ✅ 强制停止翻转状态
        }
    }


    // Update is called once per frame
    public void Die()
    {   
        GetComponent<SlimeCtrl>().enabled = false;
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        isDead = true;

        anim.SetTrigger("isDead");
        
        // 停止移动
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        // 可选：冻结
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Destroy(gameObject, 5f);

    }
    void Update()
    {
        
    }
}
