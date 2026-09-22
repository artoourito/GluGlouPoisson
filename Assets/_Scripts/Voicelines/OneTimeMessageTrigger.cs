using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
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
