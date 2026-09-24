using UnityEngine;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentPanel; // Glisse l'objet "Content" de ta Scroll View ici !
    [SerializeField] private GameObject scoreRowPrefab; // Ton prefab de ligne

    void Start()
    {
        DisplayScores();
    }

    void DisplayScores()
    {
        // 1. Nettoie les anciennes lignes s'il y en a
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // 2. Affichage du Dernier Score (EN PREMIER)
        int lastScore = PlayerPrefs.GetInt("CurrentGameScore", 0);
        GameObject lastRow = Instantiate(scoreRowPrefab, contentPanel);
        TextMeshProUGUI lastRowText = lastRow.GetComponentInChildren<TextMeshProUGUI>();

        if (lastRowText != null)
        {
            lastRowText.text = "Dernier Score : " + lastScore + " pts";
        }

        // 3. Affichage du Meilleur Score (EN DEUXIÈME)
        int bestScore = PlayerPrefs.GetInt("HighScore", 0);
        GameObject bestRow = Instantiate(scoreRowPrefab, contentPanel);
        TextMeshProUGUI bestRowText = bestRow.GetComponentInChildren<TextMeshProUGUI>();

        if (bestRowText != null)
        {
            bestRowText.text = "Meilleur Score : " + bestScore + " pts";
        }
    }
}