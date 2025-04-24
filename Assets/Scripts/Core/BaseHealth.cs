using UnityEngine;
using UnityEngine.SceneManagement; // สำหรับจบเกม

public class BaseHealth : MonoBehaviour
{
    public enum BaseType { Player, Enemy }
    public BaseType baseType;

    public float maxHealth = 50f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (baseType == BaseType.Player)
        {
            Debug.Log("YOU LOSE!");
            // โหลด scene แพ้ หรือ popup
            //SceneManager.LoadScene("LoseScene"); 
        }
        else if (baseType == BaseType.Enemy)
        {
            Debug.Log("YOU WIN!");
            //SceneManager.LoadScene("WinScene");
        }

        Destroy(gameObject);
    }
}
