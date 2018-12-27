using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    // Const
    private const int COIN_SCORE = 5;
    private const int DIAMON_SCORE = 10;

    // Gameplay
    public bool IsDead { get; set; }
    private bool isGameStart = false;    
    private PlayerController playerController;

    // UI
    public Text scoreText, modifierText, coinText, diamonText;
    private float score, modifierScore, coin, diamon;
    private int lastScore;

    void Awake()
    {
        Instance = this;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        score = 0;
        coin = 0;
        modifierScore = 1;

        UpdateScore();
        UpdateModifier(0);
        UpdateCoin(false);
        UpdateDiamon(false);
    }

    private void Update()
    {
        if (MobileInput.Instance.Tap && !isGameStart)
        {
            isGameStart = true;
            playerController.StartStopGame();
        }

        if (isGameStart && !IsDead)
        {
            score += (Time.deltaTime * modifierScore);
            if (lastScore != (int)score)
            {
                lastScore = (int)score;
                UpdateScore();
            }
        }
    }

    public void UpdateScore()
    {
        scoreText.text = "Score: " + score.ToString("0");
    }
    public void UpdateModifier(float amount)
    {
        modifierScore += amount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }
    public void UpdateCoin(bool isIncreaseScore)
    {
        coin++;
        coinText.text = "Coin: " + coin.ToString("0");

        if (!isIncreaseScore) return;

        score += COIN_SCORE;
        UpdateScore();
    }
    public void UpdateDiamon(bool isIncreaseScore)
    {
        diamon++;
        diamonText.text = "Diamon: " + diamon.ToString("0");

        if (!isIncreaseScore) return;

        score += DIAMON_SCORE;
        UpdateScore();
    }
}
