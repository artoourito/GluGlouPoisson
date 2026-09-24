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
        // Nettoie les anciennes lignes s'il y en a
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Exemple : Si tu affiches plusieurs scores sauvegardés
        int bestScore = PlayerPrefs.GetInt("HighScore", 0);

        // Instancie la ligne dans le Content
        GameObject newRow = Instantiate(scoreRowPrefab, contentPanel);
        TextMeshProUGUI rowText = newRow.GetComponentInChildren<TextMeshProUGUI>();

        if (rowText != null)
        {
            rowText.text = "1. Meilleur Joueur : " + bestScore + " pts";
        }

        // Si tu as une liste de plusieurs scores, tu fais une boucle ici 
        // et chaque Instantiate ira se ranger tout seul grâce au Vertical Layout Group !
    }
}