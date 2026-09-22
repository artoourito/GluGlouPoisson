using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class RandomMessageOnTouch : MonoBehaviour
{
    #region Variables
    [Tooltip("Liste des voiceline possibles. Un est choisi au hasard à chaque contact.")]
    [SerializeField] private List<AudioClip> randomVoicelines = new List<AudioClip>();

    [Tooltip("Tag requis sur l'objet qui déclenche le trigger.")]
    [SerializeField] private string requiredTag = "Player";

    [Tooltip("Délai minimum entre deux déclenchements, pour éviter le spam si le joueur reste dans le collider.")]
    [SerializeField] private float cooldown = 2f;

    private bool __hasTriggered = false;
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
            TryTrigger(other.gameObject);
        }
    }
    #endregion


    private void TryTrigger(GameObject other)
    {
        if (__hasTriggered) return;
        StartCoroutine(StartRandomVoiceline());
    }

    IEnumerator StartRandomVoiceline()
    {
        __hasTriggered = true;
        int randomVoicelineNb = Random.Range(0, randomVoicelines.Count);
        _audioSource.clip = randomVoicelines[randomVoicelineNb];
        _audioSource.Play();
        yield return new WaitForSeconds(cooldown);
        __hasTriggered = false;
    }
}
