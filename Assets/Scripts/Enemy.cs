using UnityEngine;
using System.Collections.Generic;
public class Enemy : MonoBehaviour
{
    // Stats — set different values per enemy type in their prefab
    public float maxHealth = 100f;
    public float moveSpeed = 2.5f;
    public int goldReward = 10;
    public int castleDamage = 10; // damage dealt to castle on arrival
                                  // Waypoints — set via WaveManager before spawning
    public Transform[] waypoints;
    private int wpIndex = 0;
    private float currentHealth;
    // Health bar (child SpriteRenderer for the red bar)
    public Transform healthBarFill;
    // Slow effect (set by Slow tower)
    private float slowMultiplier = 1f;
    public GameObject explosionPrefab;
    void Start()
    {
        currentHealth = maxHealth;
        if (waypoints.Length > 0)
            transform.position = waypoints[0].position;
    }
    void Update()
    {
        if (wpIndex >= waypoints.Length) return;
        Transform target = waypoints[wpIndex];
        float step = moveSpeed * slowMultiplier * Time.deltaTime;
        // Move toward the next waypoint
        transform.position = Vector3.MoveTowards(
        transform.position, target.position, step);
        // Arrived at waypoint? Go to next one
        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            wpIndex++;
            // Reached the end — damage castle
            if (wpIndex >= waypoints.Length)
            {
                GameManager.instance.DamageCastle(castleDamage);
                WaveManager.instance.EnemyRemoved();
                Destroy(gameObject);
            }
        }
    }
    // Called by Bullet.cs when a tower bullet hits
    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        UpdateHealthBar();
        if (currentHealth <= 0) Die();
    }
    void UpdateHealthBar()
    {
        if (healthBarFill == null) return;
        float pct = currentHealth / maxHealth;
        healthBarFill.localScale = new Vector3(pct, 1, 1);
    }
    void Die()
    {
        GameManager.instance.AddGold(goldReward);
        WaveManager.instance.EnemyRemoved();
        Destroy(gameObject);
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
    // Called by slow tower bullet
    public void ApplySlow(float mult, float duration)
    {
        StartCoroutine(SlowCoroutine(mult, duration));
    }
    System.Collections.IEnumerator SlowCoroutine(float mult, float dur)
    {
        slowMultiplier = mult;
        yield return new WaitForSeconds(dur);
        slowMultiplier = 1f;
    }
    // How far along the path is this enemy? Used by Tower to pick targets
    public float GetProgress()
    {
        return wpIndex + (Vector3.Distance(transform.position,

        waypoints[Mathf.Min(wpIndex, waypoints.Length - 1)].position));
    }
}