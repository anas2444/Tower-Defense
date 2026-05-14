using UnityEngine;
public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 20f;
    public bool isSlow = false; // true for Slow tower bullets
    public float slowAmt = 0.5f; // multiplier (0.5 = half speed)
    public float slowTime = 2f;
    private Transform target; // the enemy this bullet chases
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 4f); // self-destruct if it misses
    }
    // Tower.cs calls this right after spawning the bullet
    public void SetTarget(Transform t)
    {
        target = t;
    }
    void FixedUpdate()
    {
        if (target == null)
        {
            // Target was killed before bullet arrived — fly straight
            Destroy(gameObject);
            return;
        }
        // Calculate direction toward the target and move
        Vector2 dir = ((target.position - transform.position)).normalized;
        rb.velocity = dir * speed;
        // Rotate bullet to face movement direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        Enemy e = other.GetComponent<Enemy>();
        e.TakeDamage(damage);
        if (isSlow) e.ApplySlow(slowAmt, slowTime);
        Destroy(gameObject);
    }
}