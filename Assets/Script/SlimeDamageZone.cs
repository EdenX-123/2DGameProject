using UnityEngine;

public class SlimeDamageZone : MonoBehaviour
{
    private SlimeCtrl slimeCtrl;

    void Start()
    {
        slimeCtrl = GetComponentInParent<SlimeCtrl>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamageWithKnockback(slimeCtrl.touchDamage, slimeCtrl.transform.position);
            Debug.Log("DamageBox hit player!");
        }
    }
}
