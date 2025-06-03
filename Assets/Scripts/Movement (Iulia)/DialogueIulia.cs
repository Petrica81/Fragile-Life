using UnityEngine;
using TMPro;

public class DialogueIulia : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _mainCamera;
    [SerializeField] private Transform _player;
    [SerializeField] private TMP_Text _textComponent;

    [Header("Settings")]
    [SerializeField]
    [Tooltip("Minimum distance to trigger dialogue camera focus")]
    private float _minDistance = 10f;

    private void Start()
    {
        // Validate references
        if (_textComponent == null)
        {
            _textComponent = GetComponentInChildren<TMP_Text>(true);
            if (_textComponent == null)
            {
                Debug.LogError("No TMP_Text component found in children!", this);
                enabled = false;
                return;
            }
        }

        // Disable text at start if it's not already
        _textComponent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_mainCamera == null || _player == null || _textComponent == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, _player.position);
        if (distance < _minDistance)
        {
            if (!_textComponent.gameObject.activeSelf)
            {
                _textComponent.gameObject.SetActive(true);
                Destroy(gameObject, 30f);
            }

            // Make the text face the camera
            transform.LookAt(_mainCamera);
        }
    }
}