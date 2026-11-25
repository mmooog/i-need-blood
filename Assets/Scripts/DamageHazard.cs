using UnityEngine;

public class DamageHazard : MonoBehaviour
{
    [Header("Hazard Damage Settings")]
    public int damagePerTick = 15;
    public float damageInterval = 1.0f;

    private float nextDamageTime = 0f;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // False = flashes red and drains HP, but does NOT push the player backward
                playerHealth.TakeDamage(damagePerTick, false);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
}
