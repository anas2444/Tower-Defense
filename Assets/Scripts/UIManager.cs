using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TMP_Text goldText, waveText, waveStatusText;
    public Slider castleHPBar;
    public TMP_Text messageText;
    public TMP_Text bossWarningText;
    public GameObject gameOverPanel, towerInfoPanel;
    public TMP_Text finalWaveText, bestWaveText;
    public TMP_Text towerNameText, towerDmgText, towerLvlText;
    public TMP_Text upgradeBtnText;
    public TextMeshProUGUI speedBtnText;
    void Awake() { instance = this; }
    public void UpdateGold(int g) { goldText.text = "Gold: " + g; }
    public void UpdateWave(int w) { waveText.text = "Wave " + w; }
    public void UpdateCastleHP(int hp, int max)
    {
        castleHPBar.value = (float)hp / max * 100f;
    }
    public void ShowMessage(string msg)
    {
        StartCoroutine(FlashMessage(msg));
    }
    IEnumerator FlashMessage(string msg)
    {
        messageText.text = msg;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        messageText.gameObject.SetActive(false);
    }
    public void ShowWaveComplete(int wave)
    {
        waveStatusText.text = "Wave " + wave + " complete! Press Start Wave";
        GameManager.instance.WaveSurvived();
    }
    public void ShowBossWarning()
    {
        StartCoroutine(BossWarningFlash());
    }
    IEnumerator BossWarningFlash()
    {
        bossWarningText.gameObject.SetActive(true);

        for (int i = 0; i < 6; i++)
        {
            bossWarningText.enabled = !bossWarningText.enabled;
            yield return new WaitForSeconds(0.25f);
        }
        bossWarningText.gameObject.SetActive(false);
    }
    public void ShowGameOver(int waves)
    {
        gameOverPanel.SetActive(true);
        finalWaveText.text = "Survived: Wave " + waves;
        bestWaveText.text = "Best: Wave " + PlayerPrefs.GetInt("BestWave", 0);
    }
    public void ShowTowerInfo(Tower t)
    {
        towerInfoPanel.SetActive(true);
        towerNameText.text = t.towerName;
        towerDmgText.text = "Damage: " + t.damage.ToString("F0");
        towerLvlText.text = "Level: " + (t.upgradeLevel + 1) + "/" + (t.maxUpgrade + 1);
        if (t.upgradeLevel < t.maxUpgrade)
            upgradeBtnText.text = "Upgrade " + t.upgradeCosts[t.upgradeLevel] + "g";
        else
            upgradeBtnText.text = "MAX";
    }
    public void HideTowerInfo() { towerInfoPanel.SetActive(false); }
}