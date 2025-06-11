using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI; // Required for Image/Panel fade

public class IntroAnimation : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI[] tmpLines;
    public float[] fadeInDurations = { 2f, 2f, 1.5f };
    public float[] displayDurations = { 3f, 2f, 4f };
    public float delayBetweenLines = 0.5f;
    public float finalFadeOutDuration = 2f;

    [Header("Panel Reference")]
    public Image blackPanel; // Assign your black background panel here

    void Start()
    {
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

        // Ensure panel is initially visible
        blackPanel.color = Color.black;
    }

    IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(0.5f); // Initial pause

        // Play all text sequences
        for (int i = 0; i < tmpLines.Length; i++)
        {
            yield return StartCoroutine(FadeTMP(tmpLines[i], 0, 1, fadeInDurations[i]));
            yield return new WaitForSeconds(displayDurations[i]);

            // Don't fade out the last line yet
            if (i < tmpLines.Length - 1)
            {
                yield return StartCoroutine(FadeTMP(tmpLines[i], 1, 0, fadeInDurations[i]));
                yield return new WaitForSeconds(delayBetweenLines);
            }
        }

        // Simultaneous fade-out of last text AND panel
        StartCoroutine(FadeTMP(tmpLines[tmpLines.Length - 1], 1, 0, finalFadeOutDuration));
        StartCoroutine(FadePanel(blackPanel, 1, 0, finalFadeOutDuration));

        // Disable elements when invisible
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