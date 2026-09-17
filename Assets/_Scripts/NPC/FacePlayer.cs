using UnityEngine;

/// <summary>
/// Fait toujours pivoter cet objet (ex: une image / un Sprite / un Quad UI dans le monde)
/// pour qu'il fasse face au joueur (ou à la caméra). Effet "billboard" classique,
/// utile pour les icônes de vie, panneaux, marqueurs, etc.
///
/// MISE EN PLACE :
/// 1. Attachez ce script à l'objet contenant l'image (ex: un Sprite, un Quad avec une
///    texture, ou un Canvas en World Space).
/// 2. Si "target" est laissé vide, le script utilisera automatiquement la Main Camera.
/// 3. Choisissez le mode qui convient à votre jeu (voir "lockYAxis" ci-dessous).
/// </summary>
public class FacePlayer : MonoBehaviour
{
    [Tooltip("Transform à regarder. Laissez vide pour utiliser automatiquement la caméra principale.")]
    [SerializeField] private Transform target;

    [Tooltip("Si coché, l'objet ne pivote que sur l'axe Y (reste droit, ne bascule pas vers le haut/bas). " +
             "Idéal pour des sprites 2D dans un monde 3D ou des panneaux qui doivent rester verticaux.")]
    [SerializeField] private bool lockYAxis = true;

    [Tooltip("Inverse la face qui regarde le joueur (utile si l'image apparaît à l'envers).")]
    [SerializeField] private bool flip180 = false;

    private void Start()
    {
        if (target == null && Camera.main != null)
        {
            target = Camera.main.transform;
        }

        if (target == null)
        {
            Debug.LogWarning("[FacePlayer] Aucune cible assignée et aucune Main Camera trouvée dans la scène.");
        }
    }

    // LateUpdate est utilisé pour s'assurer que la caméra/le joueur a déjà fini
    // de bouger ce frame avant qu'on ne s'oriente vers lui.
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 directionToTarget = target.position - transform.position;

        if (lockYAxis)
        {
            directionToTarget.y = 0f;
        }

        if (directionToTarget.sqrMagnitude < 0.0001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

        if (flip180)
        {
            lookRotation *= Quaternion.Euler(0f, 180f, 0f);
        }

        transform.rotation = lookRotation;
    }
}
