﻿using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
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
    private bool IsGameStart = false;
    private PlayerController playerController;

    // UI
    public Animator gameMenuAnim;
    public Animator menuAnim;
    public Text highScoreText, totalCoinText;
    public Text scoreText, modifierText, coinText, diamonText;
    private float score, modifierScore;
    private int coin, diamon, lastScore, totalCoin;
    // -- Death Menu
    public Animator deathMenuAnim;
    public Text deathScoreText, deathCoinText, deathDiamonText;
    // -- GPGS Menu
    public GameObject connectedGPGSBTN, disconnectedGPGSBTN;
    public Text gpgsText;

    void Awake()
    {
        Instance = this;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        score = 0;
        coin = 0;
        modifierScore = 1;
        IsGameStart = false;

        UpdateScore();
        UpdateModifier(0);
        UpdateCoin(false);
        UpdateDiamon(false);

        gameMenuAnim.SetTrigger("Hide");

        highScoreText.text = PlayerPrefs.GetInt("Highscore").ToString();

        // GPGS
        GooglePlayGames.BasicApi.PlayGamesClientConfiguration config = new GooglePlayGames.BasicApi.PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        OnConnectionResponse(PlayGamesPlatform.Instance.localUser.authenticated);
    }

    private void Update()
    {
        if (IsGameStart && !IsDead)
        {
            score += (Time.deltaTime * modifierScore);
            if (lastScore != (int)score)
            {
                lastScore = (int)score;
                UpdateScore();
            }
        }
    }

    public void StartGame()
    {
        //if (MobileInput.Instance.Tap && !isGameStart)
        //{
        IsGameStart = true;
        playerController.StartGame();
        //GlacierSpawner.Instance.IsScrolling = true;
        FindObjectOfType<GlacierSpawner>().IsScrolling = true;
        FindObjectOfType<CameraController>().IsMoving = true;
        gameMenuAnim.SetTrigger("Show");
        menuAnim.SetTrigger("Hide");
        //}
    }

    // Update Text
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
        coinText.text = coin.ToString("0");

        if (!isIncreaseScore) return;

        score += COIN_SCORE;
        UpdateScore();
    }
    public void UpdateDiamon(bool isIncreaseScore)
    {
        diamon++;
        diamonText.text = diamon.ToString("0");

        if (!isIncreaseScore) return;

        score += DIAMON_SCORE;
        UpdateScore();
    }

    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    public void OnDeath()
    {
        deathScoreText.text = scoreText.text;
        deathCoinText.text = coinText.text;
        deathDiamonText.text = diamonText.text;
        deathMenuAnim.SetTrigger("Dead");

        IsDead = true;

        gameMenuAnim.SetTrigger("Hide");

        //GlacierSpawner.Instance.IsScrolling = false;
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;

        // Check Highscore
        ReportScore((int)score);
        if (score > PlayerPrefs.GetInt("Highscore"))
        {
            float s = score % 1 == 0 ? score + 1 : score;

            PlayerPrefs.SetInt("Highscore", (int)s);
        }

        totalCoin += coin;
        OpenSave(true);
    }

    // Google Play Services
    public void OnClickConnectGPGS()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            print("Authen: " + success.ToString());
            OnConnectionResponse(success);
        });
    }
    private void OnConnectionResponse(bool authenticated)
    {
        print(authenticated);
        gpgsText.text = authenticated.ToString();
        if (authenticated)
        {
            UnlockAchievement(PR_GPGS.achievement_login);
            connectedGPGSBTN.SetActive(true);
            disconnectedGPGSBTN.SetActive(false);
            OpenSave(false);
        }
        else
        {
            connectedGPGSBTN.SetActive(false);
            disconnectedGPGSBTN.SetActive(true);
        }
    }

    private string GetSaveString()
    {
        string str = "";
        str += PlayerPrefs.GetInt("Highscore").ToString();
        str += "|";
        str += totalCoin.ToString();

        return str; // 100|80
    }

    private void LoadSaveString(string save)
    {
        // 100|80
        string[] data = save.Split('|');

        PlayerPrefs.SetInt("Highscore", int.Parse(data[0]));
        totalCoin = int.Parse(data[1]);

        totalCoinText.text = totalCoin.ToString();
    }

    // Leadboard
    public void OnClickLeadboard()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
    }
    public void ReportScore(int score)
    {
        Social.ReportScore(score, PR_GPGS.leaderboard_highscore, (bool succ) =>
        {
            Debug.Log("Report to Leaderboard: " + succ.ToString());
        });
    }

    // Achievement
    public void OnClickAchievement()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowAchievementsUI();
        }
    }
    public void UnlockAchievement(string achievement)
    {
        Social.ReportProgress(achievement, 100.0f, (bool succ) =>
        {
            Debug.Log("Achievement Unlock: " + succ.ToString());
        });
    }

    // Cloud Saving
    private bool isSaving = false;
    public void OpenSave(bool saving)
    {
        Debug.Log("Open Save");
        if (Social.localUser.authenticated)
        {
            isSaving = saving;
            ((PlayGamesPlatform)Social.Active).SavedGame
                .OpenWithAutomaticConflictResolution(
                    "PenguinRunner",
                    GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork,
                    GooglePlayGames.BasicApi.SavedGame.ConflictResolutionStrategy.UseLongestPlaytime, SaveGameOpened);
        }
    }
    private void SaveGameOpened(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        Debug.Log("SaveGameOpened");
        if (status == SavedGameRequestStatus.Success)
        {
            if (isSaving) // writing
            {
                byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(GetSaveString());
                SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription("Save at " + DateTime.Now.ToString()).Build();

                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(meta, update, data, SaveUpdate);
            }
            else // reading
            {
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(meta, SaveRead);
            }
        }
    }
    // Load
    private void SaveRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            string saveData = System.Text.ASCIIEncoding.ASCII.GetString(data);
            LoadSaveString(saveData);
        }
    }
    // Success Save
    private void SaveUpdate(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        throw new NotImplementedException();
    }
}
