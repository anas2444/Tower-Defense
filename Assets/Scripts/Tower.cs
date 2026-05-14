using UnityEngine;
using System.Collections.Generic;
public class Tower : MonoBehaviour
{
    // Stats — override in each prefab variant
    public string towerName = "Basic Tower";
    public float damage = 20f;
    public float fireRate = 1f;
    public float range = 2.5f;
    public int cost = 50;
    public bool isSlow = false;
    public GameObject bulletPrefab;
    public Transform towerHead; // the part that rotates
    public Transform firePoint;
    // Upgrade system
    public int upgradeLevel = 0;
    public int maxUpgrade = 3;
    public int[] upgradeCosts = { 60, 90, 130 };
    public float[] upgradeDamage = { 1.5f, 2.2f, 3f }; // multipliers
    private float nextFireTime = 0f;
    private Transform currentTarget;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    public AudioClip shootClip;
    private AudioSource audioSrc;
    public GameObject rangeObject;

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }
    void Update()
    {
        FindTarget();
        if (currentTarget != null)
        {
            RotateToTarget();
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + (1f / fireRate);
            }
        }
    }
    void FindTarget()
    {
        // Remove nulls (dead enemies) from list
        enemiesInRange.RemoveAll(e => e == null);
        if (enemiesInRange.Count == 0) { currentTarget = null; return; }
        // Target the enemy furthest along the path
        Enemy best = null;
        float maxProgress = -1f;
        foreach (var e in enemiesInRange)
        {
            float prog = e.GetProgress();
            if (prog > maxProgress) { maxProgress = prog; best = e; }
        }
        currentTarget = best?.transform;
    }
    void RotateToTarget()
    {
        if (towerHead == null) return;
        Vector2 dir = currentTarget.position - towerHead.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        towerHead.rotation = Quaternion.Euler(0, 0, angle);
    }
    void Shoot()
    {
        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bScript = b.GetComponent<Bullet>();
        bScript.SetTarget(currentTarget);
        bScript.damage = damage;
        bScript.isSlow = isSlow;
        if (shootClip) audioSrc.PlayOneShot(shootClip, 0.4f);
    }
    // Called when an enemy enters the range circle
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy e = other.GetComponent<Enemy>();
            if (!enemiesInRange.Contains(e)) enemiesInRange.Add(e);
        }
    }
    // Called when an enemy leaves the range circle
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy e = other.GetComponent<Enemy>();
            enemiesInRange.Remove(e);
        }
    }
    // Upgrade: increases damage and deducts gold
    public bool TryUpgrade()
    {
        if (upgradeLevel >= maxUpgrade) return false;
        int cost = upgradeCosts[upgradeLevel];
        if (!GameManager.instance.SpendGold(cost)) return false;
        damage *= upgradeDamage[upgradeLevel];
        upgradeLevel++;
        UpdateVisual(); // show upgrade visually
        return true;
    }
    void UpdateVisual()
    {
        // Make tower brighter with each upgrade level
        Color c = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(
        c.r + 0.1f * upgradeLevel,
        c.g + 0.1f * upgradeLevel,
        c.b, c.a);
    }
    // Sell: return 60% of total spent
    public void Sell()
    {
        int refund = Mathf.RoundToInt(cost * 0.6f);
        GameManager.instance.AddGold(refund);
        Destroy(gameObject);
    }

    // Add this method to the Tower class
    public void SetWaypoints(Transform[] waypoints)
    {
        // Implement logic as needed, or leave empty if not required
    }
    public void ShowRange(bool show)
    {
        if (rangeObject != null)
            rangeObject.SetActive(show);
    }
}