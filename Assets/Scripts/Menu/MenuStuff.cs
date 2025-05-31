using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuStuff : MonoBehaviour
{
    public Image ImageScreenSettings;
    public Button toggleButton;
    public float fadeDuration = 0.3f;

    private Color fullVisibleColor = new Color(137f / 255f, 185f / 255f, 179f / 255f, 27f / 255f);
    private bool isTransitioning = false;
    private Coroutine currentFadeRoutine;

    private void Start()
    {
        ImageScreenSettings.color = new Color(fullVisibleColor.r, fullVisibleColor.g, fullVisibleColor.b, 0);
        ImageScreenSettings.gameObject.SetActive(true);
        toggleButton.onClick.AddListener(ToggleImageVisibility);
    }

    private void ToggleImageVisibility()
    {
        if (isTransitioning) return;

        bool shouldFadeIn = ImageScreenSettings.color.a < fullVisibleColor.a / 2f;

        if (currentFadeRoutine != null)
        {
            StopCoroutine(currentFadeRoutine);
        }

        currentFadeRoutine = StartCoroutine(shouldFadeIn ? FadeIn() : FadeOut());
    }

    private IEnumerator FadeIn()
    {
        isTransitioning = true;
        float elapsed = 0f;
        Color startColor = ImageScreenSettings.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            ImageScreenSettings.color = Color.Lerp(startColor, fullVisibleColor, t);
            yield return null;
        }

        isTransitioning = false;
    }

    private IEnumerator FadeOut()
    {
        isTransitioning = true;
        float elapsed = 0f;
        Color startColor = ImageScreenSettings.color;
        Color targetColor = new Color(fullVisibleColor.r, fullVisibleColor.g, fullVisibleColor.b, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            ImageScreenSettings.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        isTransitioning = false;
    }

    private void OnDestroy()
    {
        toggleButton.onClick.RemoveListener(ToggleImageVisibility);
    }
}