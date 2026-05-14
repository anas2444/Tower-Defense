using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int startGold = 150;
    public int castleMaxHP = 100;
    public bool isGameOver = false;
    private int gold;
    private int castleHP;
    private int wavessurvived = 0;

    public AudioClip goldClip, hitClip, gameOverClip;
    private AudioSource audioSource;
    private bool fastMode = false;
    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        gold = startGold;
        castleHP = castleMaxHP;
    }
    void Start()
    {
        UIManager.instance.UpdateGold(gold);
        UIManager.instance.UpdateCastleHP(castleHP, castleMaxHP);
    }
    public void AddGold(int amount)
    {
        gold += amount;
        UIManager.instance.UpdateGold(gold);
        if (goldClip) audioSource.PlayOneShot(goldClip);
    }
    // Returns true if purchase succeeded, false if not enough gold
    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        UIManager.instance.UpdateGold(gold);
        return true;
    }
    public void DamageCastle(int dmg)
    {
        castleHP -= dmg;
        castleHP = Mathf.Max(castleHP, 0);
        UIManager.instance.UpdateCastleHP(castleHP, castleMaxHP);
        if (hitClip) audioSource.PlayOneShot(hitClip);
        if (castleHP <= 0) TriggerGameOver();
    }
    public void WaveSurvived()
    {
        wavessurvived++;
        int best = PlayerPrefs.GetInt("BestWave", 0);
        if (wavessurvived > best)
            PlayerPrefs.SetInt("BestWave", wavessurvived);
    }
    void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (gameOverClip) audioSource.PlayOneShot(gameOverClip);
        UIManager.instance.ShowGameOver(wavessurvived);
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ToggleSpeed()
    {
        fastMode = !fastMode;
        Time.timeScale = fastMode ? 2f : 1f;
        UIManager.instance.speedBtnText.text = fastMode ? "> 1x" : ">> 2x";
    }
}