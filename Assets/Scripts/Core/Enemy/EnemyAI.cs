using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target; // player
    public Transform firePoint; // จุดยิงกระสุน
    public GameObject bulletPrefab;
    public float speed = 2f;
    public float fireRate = 1f;
    public float fireDistance = 10f;
    public LayerMask obstructionMask; // Wall, Player

    private float fireCooldown;

    void Update()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;

        // Move toward the player
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Rotate to face player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Fire cooldown
        fireCooldown -= Time.deltaTime;

        // Raycast to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, fireDistance, obstructionMask);
        if (hit.collider != null && fireCooldown <= 0f)
        {
            fireCooldown = 1f / fireRate;

            // Fire bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * 10f;
        }
    }
}
