using UnityEngine;

/// <summary>
/// Persistent singleton (DontDestroyOnLoad) that stores the current score
/// and the player's entered name, so they survive scene changes and can be
/// read from the results scene at the end.
///
/// SETUP:
/// 1. Create an empty GameObject named "ScoreManager" in your FIRST scene
///    (the one that loads first).
/// 2. Attach this script to it. Don't place it in any other scene.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public float CurrentScore { get; private set; } = 0f;
    public string PlayerName { get; private set; } = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Adds the given amount to the current score.
    /// </summary>
    public void AddScore(float amount)
    {
        CurrentScore += amount;
    }

    /// <summary>
    /// Stores the player's entered name.
    /// </summary>
    public void SetPlayerName(string name)
    {
        PlayerName = name;
    }
}