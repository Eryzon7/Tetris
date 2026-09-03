using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameStats : MonoBehaviour
{
    private int Score = 0;
    private int LineValue;
    private int Level = 1;
    private int LevelProgress = 0;
    private int BlocksToNextLevel = 10;
    private int GameSpeed;

    [SerializeField] private Board board;

    public event Action<int> OnScoreUp;
    public event Action<int, int, int> OnProgressUP;

    private void OnEnable()
    {
        board.OnLineBreak += ScoreUp;
        board.OnBlockLocked += LevelUp;
    }

    private void OnDisable()
    {
        // Always unsubscribe to avoid memory leaks / null ref errors
        board.OnLineBreak -= ScoreUp;
        board.OnBlockLocked -= LevelUp;
    }

    private void ScoreUp(int BrokenLines)
    {
        Score = (100 * BrokenLines) + Score;
        OnScoreUp?.Invoke(Score);
    }

    private void LevelUp()
    {
        LevelProgress++;
        if (LevelProgress >= BlocksToNextLevel)
        {
            LevelProgress = 0;
            Level++;
        }
        OnProgressUP?.Invoke(LevelProgress, Level, BlocksToNextLevel);
    }
}
