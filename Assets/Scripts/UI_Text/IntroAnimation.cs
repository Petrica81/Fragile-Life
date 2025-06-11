using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class IntroAnimation : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI[] tmpLines;
    public float[] fadeInDurations = { 2f, 2f, 1.5f };
    public float[] displayDurations = { 3f, 2f, 4f };
    public float delayBetweenLines = 0.5f;
    public float finalFadeOutDuration = 2f;

    [Header("Panel Reference")]
    public Image blackPanel;

    [Header("Audio Settings")]
    public AudioClip[] voiceOverClips; // Assign voice-over clips for each line
    public AudioSource audioSource; // Assign an AudioSource component
    [Range(0, 1)] public float voiceOverVolume = 0.5f;

    void Start()
    {
        // Initialize audio source if not assigned
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = voiceOverVolume;
        }

        InitializeAlpha();
        StartCoroutine(PlaySequence());
    }

    void InitializeAlpha()
    {
        foreach (TextMeshProUGUI tmp in tmpLines)
        {
            Color color = tmp.color;
            color.a = 0;
            tmp.color = color;
        }

        blackPanel.color = Color.black;
    }

    IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(0.5f); // Initial pause

        for (int i = 0; i < tmpLines.Length; i++)
        {
            // Play voice-over if available
            if (voiceOverClips != null && i < voiceOverClips.Length && voiceOverClips[i] != null)
            {
                audioSource.PlayOneShot(voiceOverClips[i]);
            }

            yield return StartCoroutine(FadeTMP(tmpLines[i], 0, 1, fadeInDurations[i]));
            yield return new WaitForSeconds(displayDurations[i]);

            if (i < tmpLines.Length - 1)
            {
                yield return StartCoroutine(FadeTMP(tmpLines[i], 1, 0, fadeInDurations[i]));
                yield return new WaitForSeconds(delayBetweenLines);
            }
        }

        StartCoroutine(FadeTMP(tmpLines[tmpLines.Length - 1], 1, 0, finalFadeOutDuration));
        StartCoroutine(FadePanel(blackPanel, 1, 0, finalFadeOutDuration));

        yield return new WaitForSeconds(finalFadeOutDuration);
        blackPanel.gameObject.SetActive(false);
    }

    IEnumerator FadeTMP(TextMeshProUGUI tmp, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0;
        Color color = tmp.color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            tmp.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        color.a = endAlpha;
        tmp.color = color;
    }

    IEnumerator FadePanel(Image panel, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0;
        Color color = panel.color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            panel.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        color.a = endAlpha;
        panel.color = color;
    }
}