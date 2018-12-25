using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    private bool isGameStart = false;
    private PlayerController playerController;

    // UI
    public Text scoreText, coinText, modifierText;
    private float score, coin, modifierScore;
    private int lastScore;

    private const int COINT_SCORE = 5;

    void Awake()
    {
        Instance = this;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        score = 0;
        coin = 0;
        modifierScore = 1;

        UpdateScore();
        UpdateCoin(false);
        UpdateModifier(0);
    }

    private void Update()
    {
        if (MobileInput.Instance.Tap && !isGameStart)
        {
            isGameStart = true;
            playerController.StartStopGame();
        }

        if (isGameStart)
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
    public void UpdateCoin(bool isIncreaseScore)
    {
        coin++;
        coinText.text = "Coin: " + coin.ToString("0");

        if (!isIncreaseScore) return;

        score += COINT_SCORE;
        UpdateScore();
    }
    public void UpdateModifier(float amount)
    {
        modifierScore += amount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }


}
