using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 1;
    public float damageInterval = 1f; // tempo entre danos

    private float damageTimer;
    private PlayerHealth playerHealth;
    private bool playerInRange;

    void Update()
    {
        if (playerInRange && playerHealth != null)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                playerHealth.TakeDamage(damage);
                damageTimer = 0f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entrou na área de ataque do lobo");

            playerHealth = other.GetComponent<PlayerHealth>();
            playerInRange = true;
            damageTimer = 0f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerHealth = null;
            damageTimer = 0f;
        }
    }
}