using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Serializable]
    public class Sound
    {
        public string id;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;

        [Range(0.1f, 3f)]
        public float pitch = 1f;

        public bool loop = false;
    }

    [Header("Sounds")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();

    [Header("Audio Sources")]
    [Tooltip("Used for one-shot sounds (Play). Can play several overlapping sounds.")]
    [SerializeField] private AudioSource oneShotAudioSource;

    [Tooltip("Dedicated to the single looping sound (PlayLoop/StopLoop).")]
    [SerializeField] private AudioSource loopAudioSource;

    [Header("Ambient (always looping)")]
    [Tooltip("Separate AudioSource for a sound that loops constantly, independent from PlayLoop.")]
    [SerializeField] private AudioSource ambientAudioSource;
    [SerializeField] private AudioClip ambientClip;

    private Dictionary<string, Sound> soundDictionary;

    private void Awake()
    {
        if (oneShotAudioSource == null)
            Debug.LogError("[SoundManager] 'oneShotAudioSource' non assigné !");

        if (loopAudioSource == null)
            Debug.LogError("[SoundManager] 'loopAudioSource' non assigné !");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (ambientAudioSource != null && ambientClip != null)
        {
            ambientAudioSource.clip = ambientClip;
            ambientAudioSource.loop = true;
            ambientAudioSource.Play();
        }

        soundDictionary = new Dictionary<string, Sound>();

        foreach (Sound sound in sounds)
        {
            if (string.IsNullOrEmpty(sound.id))
                continue;

            if (soundDictionary.ContainsKey(sound.id))
            {
                Debug.LogWarning("Duplicate sound ID: " + sound.id);
                continue;
            }

            soundDictionary.Add(sound.id, sound);
            Instance.SetAmbientVolume(0.2f);
        }
    }

    // Durée du clip associé à un identifiant (0 si inconnu ou sans clip)
    public float GetClipLength(string id)
    {
        if (string.IsNullOrEmpty(id) || soundDictionary == null)
            return 0f;

        return soundDictionary.TryGetValue(id, out Sound sound) && sound.clip != null
            ? sound.clip.length
            : 0f;
    }

    // Plays a sound once
    public void Play(string id)
    {
        if (!soundDictionary.TryGetValue(id, out Sound sound))
        {
            Debug.LogWarning("Sound not found: " + id);
            return;
        }

        if (sound.clip == null)
        {
            Debug.LogWarning("Sound has no AudioClip: " + id);
            return;
        }

        oneShotAudioSource.pitch = sound.pitch;
        oneShotAudioSource.PlayOneShot(sound.clip, sound.volume);
    }

    // Starts the looping sound
    public void PlayLoop(string id)
    {
        if (!soundDictionary.TryGetValue(id, out Sound sound))
        {
            Debug.LogWarning("Sound not found: " + id);
            return;
        }

        if (sound.clip == null)
        {
            Debug.LogWarning("Sound has no AudioClip: " + id);
            return;
        }

        loopAudioSource.clip = sound.clip;
        loopAudioSource.volume = sound.volume;
        loopAudioSource.pitch = sound.pitch;
        loopAudioSource.loop = true;

        loopAudioSource.Play();
    }

    // Stops the currently playing loop
    public void StopLoop()
    {
        loopAudioSource.Stop();
        loopAudioSource.clip = null;
        loopAudioSource.loop = false;
    }

    // Sets the ambient sound's volume (0 to 1)
    public void SetAmbientVolume(float volume)
    {
        if (ambientAudioSource != null)
        {
            ambientAudioSource.volume = Mathf.Clamp01(volume);
        }
    }
}