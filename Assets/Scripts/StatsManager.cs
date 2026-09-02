using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameStats : MonoBehaviour
{
    private int Score;
    private int LineValue;
    private int GameSpeed;

    [SerializeField] private Board board;

    public event Action<int> OnScoreUp;

    private void OnEnable()
    {
        board.OnLineBreak += ScoreUp;

    }

    private void OnDisable()
    {
        // Always unsubscribe to avoid memory leaks / null ref errors
        board.OnLineBreak -= ScoreUp;

    }

    private void ScoreUp(int BrokenLines)
    {
        Score = 100 * BrokenLines;
        OnScoreUp?.Invoke(Score);
    }
}
