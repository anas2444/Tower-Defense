using UnityEngine;

public class TowerInfo : MonoBehaviour
{
    private Tower tower;
    private static TowerInfo selectedTower;

    void Start()
    {
        tower = GetComponent<Tower>();
    }

    void OnMouseDown()
    {
        // Deselect previous
        if (selectedTower != null && selectedTower != this)
            selectedTower.Deselect();

        selectedTower = this;

        UIManager.instance.ShowTowerInfo(tower);
        TileManager.instance.ExitBuildMode();

        tower.ShowRange(true); // ✅ FIXED
    }

    void Deselect()
    {
        UIManager.instance.HideTowerInfo();

        tower.ShowRange(false); // ✅ FIXED
    }

    public void OnUpgradeClick()
    {
        if (selectedTower == null) return;

        bool ok = selectedTower.tower.TryUpgrade();

        if (ok)
            UIManager.instance.ShowTowerInfo(selectedTower.tower);
        else
            UIManager.instance.ShowMessage("Cannot upgrade!");
    }

    public void OnSellClick()
    {
        if (selectedTower == null) return;

        UIManager.instance.HideTowerInfo();
        selectedTower.tower.Sell();
        selectedTower = null;
    }
}