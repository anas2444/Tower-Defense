using UnityEngine;

public class TowerSelector : MonoBehaviour
{
    public static TowerSelector instance;

    private Tower selectedTower;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0;

            Collider2D hit = Physics2D.OverlapPoint(world);

            // Clicked on tower
            if (hit != null && hit.CompareTag("Tower"))
            {
                Tower t = hit.GetComponent<Tower>();
                SelectTower(t);
            }
            else
            {
                Deselect();
            }
        }
    }

    void SelectTower(Tower t)
    {
        if (selectedTower != null)
            selectedTower.ShowRange(false);

        selectedTower = t;
        selectedTower.ShowRange(true);
    }

    void Deselect()
    {
        if (selectedTower != null)
        {
            selectedTower.ShowRange(false);
            selectedTower = null;
        }
    }
}