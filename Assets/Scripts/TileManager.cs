using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;

    // The 4 tower prefabs — assign in Inspector
    public GameObject[] towerPrefabs; // 0=Basic, 1=Rapid, 2=Sniper, 3=Slow

    private int selectedTowerIdx = 0;
    private bool buildMode = false;

    // Track which tiles are occupied
    private Dictionary<Vector2, GameObject> placed =
        new Dictionary<Vector2, GameObject>();

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // Right-click to cancel build mode
        if (Input.GetMouseButtonDown(1))
            ExitBuildMode();

        if (!buildMode) return;

        if (Input.GetMouseButtonDown(0))
        {
            // Convert mouse screen position to world position
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0;

            // 🔥 ONLY detect Default layer (Grass tiles)
            int mask = LayerMask.GetMask("Default");
            RaycastHit2D hit = Physics2D.Raycast(
                new Vector2(world.x, world.y),
                Vector2.zero,
                Mathf.Infinity,
                mask
            );

            if (hit.collider != null && hit.collider.CompareTag("Grass"))
            {
                TryPlaceTower(hit.collider.transform);
            }
        }
    }

    public void SelectTower(int idx)
    {
        selectedTowerIdx = idx;
        buildMode = true;
    }

    public void ExitBuildMode()
    {
        buildMode = false;
    }

    void TryPlaceTower(Transform tile)
    {
        Vector2 key = tile.position;

        // Already occupied
        if (placed.ContainsKey(key)) return;

        GameObject prefab = towerPrefabs[selectedTowerIdx];
        Tower tower = prefab.GetComponent<Tower>();

        // Not enough gold
        if (!GameManager.instance.SpendGold(tower.cost))
        {
            UIManager.instance.ShowMessage("Not enough gold!");
            return;
        }

        // Place tower
        GameObject t = Instantiate(prefab, tile.position, Quaternion.identity);

        // Optional (only if your Tower has this method)
        if (t.GetComponent<Tower>() != null && WaveManager.instance != null)
        {
            t.GetComponent<Tower>().SetWaypoints(WaveManager.instance.waypoints);
        }

        placed[key] = t;

        // Darken tile to show it's occupied
        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(0.2f, 0.4f, 0.2f);
        }
    }

    // Called when tower is sold
    public void RemoveTower(Vector2 pos)
    {
        placed.Remove(pos);
    }
}