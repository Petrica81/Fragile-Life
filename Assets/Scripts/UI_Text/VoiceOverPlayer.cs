using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class VoiceOverPlayer : MonoBehaviour
{
    [Header("Voice Over")]
    public AudioClip voiceOverClip;
    [Range(0.1f, 3f)] public float pitchVariation = 0.1f;

    private AudioSource _audioSource;
    private TMP_Text _text;
    private bool _hasPlayed = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;

        // Get the text component from children
        _text = GetComponentInChildren<TMP_Text>(true);
    }

    private void Update()
    {
        if (_hasPlayed || _text == null) return;

        // Play when text becomes active
        if (_text.gameObject.activeInHierarchy)
        {
            PlayVoiceOver();
            _hasPlayed = true;
        }
    }

    private void PlayVoiceOver()
    {
        if (voiceOverClip == null) return;

        _audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        _audioSource.PlayOneShot(voiceOverClip);
    }

    // Call this if you want to reset and allow replay
    public void Reset()
    {
        _hasPlayed = false;
    }
}