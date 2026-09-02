using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text LevelText;

    [SerializeField] private GameStats stats;


    private void OnEnable()
    {
        stats.OnScoreUp += UpdateScore;
       
    }

    private void OnDisable()
    {
        // Always unsubscribe to avoid memory leaks / null ref errors
        stats.OnScoreUp -= UpdateScore;
        
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
}
