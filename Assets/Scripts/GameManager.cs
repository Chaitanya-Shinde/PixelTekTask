using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public BallSpawner spawner;
    public CircleTarget targetCircle;

    [Header("UI")]
    public GameObject ResultTextObject;
    public Button goBtn;

    private TMP_Text resultText;
    public TMP_Text totalScoreText;
    public TMP_Text highScoreText;

    public Button nextLevelButton;
    public Button restartLevelButton;
    

    [Header("Radius Limits")]
    public float minRadius = 1f;
    public float maxRadius = 100f;

    public float targetRadius;
    private int totalScore = 0;
    private int highScore = 0;
    private const string HIGH_SCORE_KEY = "HIGH_SCORE";

    float minPlayableRadius;
    float maxPlayableRadius;

    private PlayerInputActions inputActions;

    void Start()
    {
        resultText = ResultTextObject.GetComponentInChildren<TMP_Text>();
        totalScoreText.text = $"Total Score:0";


        nextLevelButton.gameObject.SetActive(false);
        restartLevelButton.gameObject.SetActive(false);

        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0); //getting high score from player prefs

        ResultTextObject.SetActive(false);

        minPlayableRadius = spawner.CalculateRadius(1);
        maxPlayableRadius = spawner.CalculateRadius(100);

        StartNewLevel();
    }

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Quit.performed += _ => Quit();
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    public void StartNewLevel()
    {
        //reset all ui score elements
        resultText.text = "";
        totalScoreText.text = $"Total Score:{totalScore}";
        highScoreText.text = $"High Score:{highScore}";

        spawner.UnlockSpawner(); //let player spawn balls
        goBtn.interactable = true;

        ResultTextObject.SetActive(false);

        nextLevelButton.gameObject.SetActive(false);
        restartLevelButton.gameObject.SetActive(false);

        spawner.ClearBalls();

        targetRadius = Random.Range(minPlayableRadius, maxPlayableRadius);

        targetCircle.SetRadius(targetRadius);

    }

    // Called after balls settle
    public void EvaluateRound(float spawnedRadius)
    {
        float difference = Mathf.Abs(targetRadius - spawnedRadius);

        float accuracy = Mathf.Clamp01(1f - (difference / targetRadius));

        int score = Mathf.RoundToInt(accuracy * 100);

        if (spawnedRadius > targetRadius)
        {
            LevelFailed();
            return;
        }

        if (score < 50)
        {
            LevelFailed();
            return;
        }

        LevelPass(score);
    }

    public void LevelPass(int score)
    {
        totalScore += score;

        if (totalScore > highScore)//check if total score is higher than high score
        {
            highScore = totalScore;//update high score

            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore); //storing high score in player prefs
            PlayerPrefs.Save();

            UpdateHighScoreUI();
        }

        ResultTextObject.SetActive(true);
        nextLevelButton.gameObject.SetActive(true);
        restartLevelButton.gameObject.SetActive(false);

        //displaying result score and total score
        resultText.color = Color.green;
        resultText.text = $"PASS! Score:{score}";
        totalScoreText.text = $"Total Score:{totalScore}";

        spawner.LockSpawner(); //locking spawner script to stop player from spawning more balls after level is passed
        goBtn.interactable = false;

    }

    public void LevelFailed()
    {

        ResultTextObject.SetActive(true);
        resultText.color = Color.red;
        resultText.text = "FAILED!";

        totalScore = 0;
        totalScoreText.text = "Total Score:0";

        nextLevelButton.gameObject.SetActive(false);
        restartLevelButton.gameObject.SetActive(true);

        spawner.LockSpawner();
        goBtn.interactable = true;
    }

    void UpdateHighScoreUI()
    {
        highScoreText.text = $"High Score: {highScore}"; //update high score text in UI
    }

    public void Quit()
    {
        Debug.Log("Quitting Game");

        Application.Quit();

    }

}