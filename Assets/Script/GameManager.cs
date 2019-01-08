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
    public Animator gameMenuAnim;
    public Animator menuAnim;
    public Text highScoreText;
    public Text scoreText, modifierText, coinText, diamonText;
    private float score, modifierScore, coin, diamon;
    private int lastScore;

    // Death Menu
    public Animator deathMenuAnim;
    public Text deathScoreText, deathCoinText, deathDiamonText;

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

        gameMenuAnim.SetTrigger("Hide");

        highScoreText.text = PlayerPrefs.GetInt("Highscore").ToString();
    }

    private void Update()
    {
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

    public void StartGame()
    {
        print("Click BTN PLAY");
        //if (MobileInput.Instance.Tap && !isGameStart)
        //{
        isGameStart = true;
        playerController.StartGame();
        //GlacierSpawner.Instance.IsScrolling = true;
        FindObjectOfType<GlacierSpawner>().IsScrolling = true;
        FindObjectOfType<CameraController>().IsMoving = true;
        gameMenuAnim.SetTrigger("Show");
        menuAnim.SetTrigger("Hide");
        //}
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
        if (score > PlayerPrefs.GetInt("Highscore"))
        {
            float s = score % 1 == 0 ? score + 1 : score;

            PlayerPrefs.SetInt("Highscore", (int)s);
        }
    }
}
