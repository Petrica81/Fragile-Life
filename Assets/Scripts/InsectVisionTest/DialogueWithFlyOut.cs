using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueWithFlyOut : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private float _minDistance = 10f;
    [SerializeField] private float _displayDuration = 30f;

    [Header("Fly Out Settings")]
    public Transform targetPoint;
    public float flySpeed = 3f;
    public float letterDelay = 0.1f;
    public float fadeStartDistance = 1f;

    private Transform _mainCamera;
    private Transform _player;
    private TMP_Text _textComponent;
    private bool _isAnimating;
    private Vector3[] _originalVertices;
    private Vector3[] _currentVertices;

    private void Awake()
    {
        _textComponent = GetComponentInChildren<TMP_Text>(true);
        _mainCamera = Camera.main.transform;
        _player = GameObject.FindAnyObjectByType<Player>().transform;

        if (targetPoint == null)
        {
            CreateDefaultTarget();
        }
    }

    private void CreateDefaultTarget()
    {
        targetPoint = new GameObject("FlyOutTarget").transform;
        targetPoint.position = transform.position + _mainCamera.forward * 3f;
    }

    private void Update()
    {
        if (_mainCamera == null || _player == null) return;

        // Dialogue activation logic
        if (Vector3.Distance(transform.position, _player.position) < _minDistance)
        {
            if (!_textComponent.gameObject.activeSelf)
            {
                _textComponent.gameObject.SetActive(true);
                StartCoroutine(DisplayAndFlyOut());
            }
            transform.LookAt(_mainCamera);
        }
    }

    private IEnumerator DisplayAndFlyOut()
    {
        // Wait for display duration
        yield return new WaitForSeconds(_displayDuration);

        // Initialize fly out animation
        _textComponent.ForceMeshUpdate();
        InitializeVertexData();
        _isAnimating = true;

        // Animate from last to first character
        for (int i = _textComponent.textInfo.characterCount - 1; i >= 0; i--)
        {
            if (!_textComponent.textInfo.characterInfo[i].isVisible) continue;

            StartCoroutine(AnimateCharacter(i));
            yield return new WaitForSeconds(letterDelay);
        }

        _isAnimating = false;
        Destroy(gameObject, 1f); // Destroy after animation completes
    }

    private void InitializeVertexData()
    {
        TMP_TextInfo textInfo = _textComponent.textInfo;
        _originalVertices = new Vector3[textInfo.meshInfo[0].vertices.Length];
        _currentVertices = new Vector3[textInfo.meshInfo[0].vertices.Length];

        System.Array.Copy(textInfo.meshInfo[0].vertices, _originalVertices, _originalVertices.Length);
        System.Array.Copy(_originalVertices, _currentVertices, _currentVertices.Length);
    }

    private IEnumerator AnimateCharacter(int charIndex)
    {
        TMP_CharacterInfo charInfo = _textComponent.textInfo.characterInfo[charIndex];
        if (!charInfo.isVisible) yield break;

        int materialIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;

        Vector3 charCenter = (_currentVertices[vertexIndex] + _currentVertices[vertexIndex + 2]) / 2f;
        Vector3 worldCenter = transform.TransformPoint(charCenter);
        Vector3 targetWorldPos = targetPoint.position;
        Vector3 direction = (targetWorldPos - worldCenter).normalized;

        float distance = Vector3.Distance(worldCenter, targetWorldPos);
        float startDistance = distance;

        while (distance > 0.1f)
        {
            // Move vertices
            float moveAmount = flySpeed * Time.deltaTime;
            distance = Vector3.Distance(worldCenter, targetWorldPos);

            for (int i = 0; i < 4; i++)
            {
                Vector3 worldOffset = direction * moveAmount;
                Vector3 localOffset = transform.InverseTransformVector(worldOffset);
                _currentVertices[vertexIndex + i] += localOffset;
            }

            // Update alpha
            byte alpha = (byte)(255 * Mathf.Clamp01(distance / fadeStartDistance));
            for (int i = 0; i < 4; i++)
            {
                _textComponent.textInfo.meshInfo[materialIndex].colors32[vertexIndex + i].a = alpha;
            }

            // Update mesh
            UpdateMeshVertices();
            _textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            // Recalculate position
            charCenter = (_currentVertices[vertexIndex] + _currentVertices[vertexIndex + 2]) / 2f;
            worldCenter = transform.TransformPoint(charCenter);

            yield return null;
        }

        // Hide character when arrived
        for (int i = 0; i < 4; i++)
        {
            _textComponent.textInfo.meshInfo[materialIndex].colors32[vertexIndex + i].a = 0;
        }
        _textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void UpdateMeshVertices()
    {
        for (int i = 0; i < _currentVertices.Length; i++)
        {
            _textComponent.textInfo.meshInfo[0].vertices[i] = _currentVertices[i];
        }
    }
}