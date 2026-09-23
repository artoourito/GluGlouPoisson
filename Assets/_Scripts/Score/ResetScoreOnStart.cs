using UnityEngine;

public class ResetScoreOnStart : MonoBehaviour
{
    void Start()
    {
        // Au tout début du jeu, on remet le score courant à 0
        PlayerPrefs.SetInt("CurrentGameScore", 0);
        PlayerPrefs.Save();
    }
}