using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool keepPlayerBetweenScenes = true;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Animator fadeAnimator;

    [Header("Trigger Settings")]
    [SerializeField] private Collider transitionTrigger;

    public GameObject player;
    private bool isTransitioning = false;

    void Start()
    {

        Debug.Log($"Scene Transition Manager initialized. Next scene: {nextSceneName}");
        Debug.Log($"Player found: {player != null}");
        Debug.Log($"Trigger found: {transitionTrigger != null}");

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.gameObject.name}");

        if (!isTransitioning && other.gameObject.tag == "Player")
        {
            Debug.Log("Starting scene transition");
            StartCoroutine(TransitionToScene());
        }
    }

    private System.Collections.IEnumerator TransitionToScene()
    {
        isTransitioning = true;
        Debug.Log("Transition started");

        // Dezactivează controlul jucătorului
        if (player.TryGetComponent(out PlayerMovement playerMovement))
        {
            playerMovement.enabled = false;
            Debug.Log("Player movement disabled");
        }

        // Fade out
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger("FadeOut");
            Debug.Log("FadeOut animation triggered");
        }
        else if (fadeImage != null)
        {
            Debug.Log("Starting manual fade out");
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("No fade effect configured");
            yield return new WaitForSeconds(fadeDuration);
        }

        Debug.Log($"Attempting to load scene: {nextSceneName}");

        // Verifică dacă scena există în Build Settings
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            // Păstrează jucătorul dacă este necesar
            if (keepPlayerBetweenScenes && player != null)
            {
                Debug.Log("Preserving player object");
                DontDestroyOnLoad(player);
            }

            Debug.Log("Loading new scene");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError($"Scene '{nextSceneName}' not found in Build Settings!");
            isTransitioning = false;

            // Reactivează controlul jucătorului dacă scena nu s-a încărcat
            if (player != null && player.TryGetComponent(out PlayerMovement pm))
            {
                pm.enabled = true;
            }
        }
    }
}