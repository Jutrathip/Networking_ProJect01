using UnityEngine;

public class DestructibleWall2D : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            Destroy(gameObject); // พังกำแพง
        }
    }
}
