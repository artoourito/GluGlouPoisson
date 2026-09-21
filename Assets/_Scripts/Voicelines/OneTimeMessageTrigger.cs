using System.Collections;
using UnityEngine;

/// <summary>
/// Affiche un message texte une seule fois lorsque le joueur touche cet objet
/// (ex: une boîte spécifique). Ne se redéclenche plus ensuite, même si le
/// joueur touche à nouveau la boîte.
///
/// MISE EN PLACE :
/// 1. Attachez ce script à votre boîte, qui doit avoir un Collider (2D ou 3D)
///    avec "Is Trigger" coché.
/// 2. Assurez-vous qu'un GameObject avec le script MessageDisplay existe dans la scène.
/// 3. Renseignez le texte à afficher dans "message".
/// 4. Le joueur doit avoir le Tag renseigné dans "requiredTag" (par défaut "Player").
/// </summary>

[RequireComponent(typeof(AudioSource))]

public class OneTimeMessageTrigger : MonoBehaviour
{
    #region Variables
    [Tooltip("Audio joué lors du premier contact avec le joueur.")]
    [SerializeField] private AudioClip voiceline;

    [Tooltip("Tag requis sur l'objet qui déclenche le trigger.")]
    [SerializeField] private string requiredTag = "Player";

    private bool _hasTriggered = false;
    private AudioSource _audioSource;
    #endregion

    #region Built-in Methods
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(requiredTag)) 
        {
            TryTrigger();
        }
    }
    #endregion

    private void TryTrigger()
    {
        if (_hasTriggered) return;

        StartCoroutine(StartVoiceline());
    }

    IEnumerator StartVoiceline()
    {
        _hasTriggered = true;
        _audioSource.clip = voiceline;
        _audioSource.Play();
        yield return new WaitForSeconds(15f);
        gameObject.SetActive(false);
    }
}
