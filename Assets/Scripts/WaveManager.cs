using UnityEngine;
using System.Collections;
public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public GameObject[] enemyPrefabs; // 0=Normal, 1=Fast, 2=Tank, 3=Boss
    public Transform[] waypoints; // drag all WP objects here
    public Transform spawnPoint; // drag WP0 (the start) here
    public float spawnDelay = 0.6f; // time between each enemy spawn
    public float waveBreak = 8f; // seconds between waves
    private int currentWave = 0;
    private int enemiesAlive = 0;
    private bool waveActive = false;
    private bool waitingToStart = true;
    public GameObject spawnIndicator;
    void Awake() { instance = this; }
    // Called by the "Start Wave" button in the UI
    public void StartWave()
    {
        StartCoroutine(FlashIndicator());
        if (waveActive || GameManager.instance.isGameOver) return;
        StartCoroutine(SpawnWave());
    }
    public void EnemyRemoved()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0 && waveActive)
        {
            waveActive = false;
            UIManager.instance.ShowWaveComplete(currentWave);
        }
    }
    IEnumerator SpawnWave()
    {
        waveActive = true;
        currentWave++;
        UIManager.instance.UpdateWave(currentWave);
        bool isBossWave = (currentWave % 5 == 0);
        if (isBossWave) UIManager.instance.ShowBossWarning();
        yield return new WaitForSeconds(2f); // dramatic pause
                                             // Boss wave: one huge boss + fewer normal enemies
        if (isBossWave)
        {
            SpawnEnemy(3); // Boss
            yield return new WaitForSeconds(3f);
        }
        // Regular enemies — count grows with wave number
        int count = 5 + currentWave * 2;
        if (isBossWave) count = currentWave; // fewer with boss
        for (int i = 0; i < count; i++)
        {
            // Mix enemy types based on wave
            int type = 0;
            if (currentWave >= 3 && i % 3 == 1) type = 1; // Fast
            else if (currentWave >= 5 && i % 4 == 3) type = 2; // Tank
            SpawnEnemy(type);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    void SpawnEnemy(int typeIdx)
    {
        GameObject go = Instantiate(
        enemyPrefabs[typeIdx], spawnPoint.position, Quaternion.identity);
        Enemy e = go.GetComponent<Enemy>();
        e.waypoints = waypoints;
        // Enemies get tougher each wave
        e.maxHealth *= 1f + currentWave * 0.1f;
        e.moveSpeed *= 1f + currentWave * 0.03f;
        enemiesAlive++;
    }
    IEnumerator FlashIndicator()
    {
        spawnIndicator.SetActive(true);
        for (int i = 0; i < 6; i++)
        {
            spawnIndicator.GetComponent<SpriteRenderer>().enabled = (i % 2 == 0);
            yield return new WaitForSeconds(0.3f);
        }
        spawnIndicator.SetActive(false);
    }
}