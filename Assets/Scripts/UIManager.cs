using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text LevelText;
    [SerializeField] private TMP_Text GameOverText;

    [SerializeField] private GameStats stats;
    [SerializeField] private Board board;


    private void OnEnable()
    {
        stats.OnScoreUp += UpdateScore;
        board.OnGameOver += GameOver;
        stats.OnProgressUP += UpdateLevelProgress;


    }

    private void OnDisable()
    {
        // Always unsubscribe to avoid memory leaks / null ref errors
        stats.OnScoreUp -= UpdateScore;
        board.OnGameOver -= GameOver;
        stats.OnProgressUP -= UpdateLevelProgress;

    }

    private void UpdateLevelProgress(int LevelProgress, int Level, int BlocksToNextLevel)
    {
        slider.maxValue = BlocksToNextLevel;
        slider.value = LevelProgress;
        LevelText.text = "Level: " + Level.ToString(); 

    }

    private void UpdateScore(int Score)
    {
        ScoreText.text = "Score: " + Score.ToString();
    }

    void Start()
    {
        slider.value = 0;
        ScoreText.text = "Score: 0";
        LevelText.text = "Level: 1";
    }

    void GameOver()
    {
        GameOverText.gameObject.SetActive(true);
    }
}
