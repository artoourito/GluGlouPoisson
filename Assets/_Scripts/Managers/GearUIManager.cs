using UnityEngine;
using UnityEngine.UI;

public class GearUIManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Gear Images")]
    [SerializeField] private Image gearR;
    [SerializeField] private Image gear1;
    [SerializeField] private Image gear2;

    [Header("Active Sprites")]
    [SerializeField] private Sprite rBig;
    [SerializeField] private Sprite oneBig;
    [SerializeField] private Sprite twoBig;

    [Header("Inactive Sprites")]
    [SerializeField] private Sprite rNormal;
    [SerializeField] private Sprite oneNormal;
    [SerializeField] private Sprite twoNormal;

    private int lastGear = 999;


    private void Start()
    {
        Debug.Log("========== GEAR UI START ==========");

        // Si aucune voiture n'est assignée manuellement,
        // on cherche automatiquement le PlayerController dans la scène.
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        // Vérification
        if (player == null)
        {
            Debug.LogError(
                "GearUIManager : impossible de trouver une voiture avec un PlayerController !"
            );

            return;
        }

        Debug.Log(
            "GearUI : voiture trouvée automatiquement = " +
            player.gameObject.name
        );

        // Affichage initial
        UpdateGearUI(player.CurrentGear);
    }


    private void Update()
    {
        if (player == null)
            return;

        int currentGear = player.CurrentGear;

        // On ne met à jour l'UI que lorsque la vitesse change.
        if (currentGear != lastGear)
        {
            UpdateGearUI(currentGear);
        }
    }


    private void UpdateGearUI(int gear)
    {
        lastGear = gear;

        Debug.Log("UI GEAR : " + gear);

        if (gearR == null ||
            gear1 == null ||
            gear2 == null)
        {
            Debug.LogError(
                "GearUIManager : une ou plusieurs Images ne sont pas assignées !"
            );

            return;
        }

        switch (gear)
        {
            // =========================
            // MARCHE ARRIÈRE
            // =========================

            case -1:

                gearR.sprite = rBig;
                gear1.sprite = oneNormal;
                gear2.sprite = twoNormal;

                break;


            // =========================
            // PREMIÈRE
            // =========================

            case 0:

                gearR.sprite = rNormal;
                gear1.sprite = oneBig;
                gear2.sprite = twoNormal;

                break;


            // =========================
            // DEUXIÈME
            // =========================

            case 1:

                gearR.sprite = rNormal;
                gear1.sprite = oneNormal;
                gear2.sprite = twoBig;

                break;


            default:

                Debug.LogWarning(
                    "GearUIManager : rapport inconnu : " + gear
                );

                break;
        }
    }
}