using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class TextFadeToPoint : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform targetPoint;
    public float arrivalDistance = 0.1f;

    [Header("Animation Settings")]
    public float flySpeed = 3f;
    public float delayBetweenLetters = 0.1f;
    public float fadeStartDistance = 1f;

    private TMP_Text _textComponent;
    private Coroutine _animationCoroutine;
    private Vector3[] _originalVertices;
    private Vector3[] _currentVertices;
    private bool _isAnimating;

    private void Awake()
    {
        _textComponent = GetComponent<TMP_Text>();
        if (targetPoint == null)
        {
            CreateDefaultTarget();
        }
    }

    private void CreateDefaultTarget()
    {
        targetPoint = new GameObject("FlyOutTarget").transform;
        targetPoint.position = transform.position + transform.forward * 3f;
    }

    public void StartAnimation()
    {
        if (_isAnimating) return;

        _textComponent.ForceMeshUpdate();
        InitializeVertexData();
        _isAnimating = true;
        _animationCoroutine = StartCoroutine(RunFlyOutAnimation());
    }

    private void InitializeVertexData()
    {
        TMP_TextInfo textInfo = _textComponent.textInfo;
        _originalVertices = new Vector3[textInfo.meshInfo[0].vertices.Length];
        _currentVertices = new Vector3[textInfo.meshInfo[0].vertices.Length];

        // Store original vertex positions
        System.Array.Copy(textInfo.meshInfo[0].vertices, _originalVertices, _originalVertices.Length);
        System.Array.Copy(_originalVertices, _currentVertices, _currentVertices.Length);
    }

    private IEnumerator RunFlyOutAnimation()
    {
        // Animate from last to first character
        for (int i = _textComponent.textInfo.characterCount - 1; i >= 0; i--)
        {
            if (!_textComponent.textInfo.characterInfo[i].isVisible) continue;

            StartCoroutine(AnimateCharacter(i));
            yield return new WaitForSeconds(delayBetweenLetters);
        }
        _isAnimating = false;
    }

    private IEnumerator AnimateCharacter(int charIndex)
    {
        TMP_CharacterInfo charInfo = _textComponent.textInfo.characterInfo[charIndex];
        if (!charInfo.isVisible) yield break;

        int materialIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;

        // Calculate center position of this character
        Vector3 charCenter = (_currentVertices[vertexIndex] + _currentVertices[vertexIndex + 2]) / 2f;
        Vector3 worldCenter = transform.TransformPoint(charCenter);
        Vector3 targetWorldPos = targetPoint.position;
        Vector3 direction = (targetWorldPos - worldCenter).normalized;

        float distance = Vector3.Distance(worldCenter, targetWorldPos);
        float startDistance = distance;

        while (distance > arrivalDistance)
        {
            // Calculate movement
            float moveAmount = flySpeed * Time.deltaTime;
            distance = Vector3.Distance(worldCenter, targetWorldPos);
            float progress = 1f - (distance / startDistance);

            // Update all 4 vertices of this character
            for (int i = 0; i < 4; i++)
            {
                Vector3 originalPos = _originalVertices[vertexIndex + i];
                Vector3 offset = transform.TransformDirection(direction) * moveAmount;
                _currentVertices[vertexIndex + i] += offset;
            }

            // Update alpha based on distance
            byte alpha = (byte)(255 * Mathf.Clamp01(distance / fadeStartDistance));
            for (int i = 0; i < 4; i++)
            {
                _textComponent.textInfo.meshInfo[materialIndex].colors32[vertexIndex + i].a = alpha;
            }

            // Update mesh
            UpdateMeshVertices();
            _textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            // Recalculate world position for next frame
            charCenter = (_currentVertices[vertexIndex] + _currentVertices[vertexIndex + 2]) / 2f;
            worldCenter = transform.TransformPoint(charCenter);

            yield return null;
        }

        // Hide when arrived
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

    public void TriggerAnimation()
    {
        if (!_isAnimating)
        {
            StartAnimation();
        }
    }

    private void OnEnable()
    {
        if (targetPoint == null)
        {
            CreateDefaultTarget();
        }
    }
}