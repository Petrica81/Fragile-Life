using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextFadeDisperse : MonoBehaviour
{
    public enum MovementMode
    {
        Random,
        Targeted
    }

    [Header("Animation Settings")]
    public float duration = 2f;
    public float delayBetweenLetters = 0.05f;
    public float maxDistance = 10f;
    public bool loopAnimation;
    public MovementMode movementMode = MovementMode.Random;

    [Header("Random Movement Settings")]
    public float minDirectionAngle = 0f;
    public float maxDirectionAngle = 360f;
    public float minSpeedMultiplier = 0.8f;
    public float maxSpeedMultiplier = 1.2f;

    [Header("Targeted Movement Settings")]
    public Transform targetPosition;
    public float targetSpread = 0.5f; // How much letters spread around target
    public AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private TMP_Text _tmpText;
    private TMP_TextInfo _textInfo;
    private float[] _timers;
    private Vector3[][] _originalVertices;
    private Vector2[] _movementDirections;
    private float[] _speedMultipliers;
    private bool _isAnimating;
    private Vector3[] _targetPositions;

    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
        CacheOriginalVertices();
    }

    private void OnEnable()
    {
        InitializeMovementValues();
        ResetAnimation();
        _isAnimating = true;
    }

    private void OnDisable()
    {
        _isAnimating = false;
        ResetTextImmediately();
    }

    private void Update()
    {
        if (!_isAnimating) return;

        bool anyCharacterActive = false;
        _tmpText.ForceMeshUpdate();
        _textInfo = _tmpText.textInfo;

        for (int i = 0; i < _textInfo.characterCount; i++)
        {
            ref var charInfo = ref _textInfo.characterInfo[i];
            if (!charInfo.isVisible || charInfo.character == ' ') continue;

            _timers[i] += Time.deltaTime * _speedMultipliers[i];
            float t = Mathf.Clamp01(_timers[i] / duration);

            if (t < 1f) anyCharacterActive = true;

            UpdateCharacter(i, t);
        }

        UpdateMesh();

        if (!anyCharacterActive && !loopAnimation)
        {
            _isAnimating = false;
        }
        else if (!anyCharacterActive && loopAnimation)
        {
            ResetAnimation();
            InitializeMovementValues();
        }
    }

    private void InitializeMovementValues()
    {
        _tmpText.ForceMeshUpdate();
        _textInfo = _tmpText.textInfo;

        _movementDirections = new Vector2[_textInfo.characterCount];
        _speedMultipliers = new float[_textInfo.characterCount];
        _targetPositions = new Vector3[_textInfo.characterCount];

        if (movementMode == MovementMode.Random)
        {
            for (int i = 0; i < _textInfo.characterCount; i++)
            {
                float randomAngle = Random.Range(minDirectionAngle, maxDirectionAngle);
                _movementDirections[i] = new Vector2(
                    Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                    Mathf.Sin(randomAngle * Mathf.Deg2Rad)
                ).normalized;

                _speedMultipliers[i] = Random.Range(minSpeedMultiplier, maxSpeedMultiplier);
            }
        }
        else // Targeted movement
        {
            if (targetPosition == null)
            {
                Debug.LogWarning("No target position set for targeted movement. Using random fallback.");
                movementMode = MovementMode.Random;
                InitializeMovementValues();
                return;
            }

            // Calculate center positions of each character in world space
            Vector3[] charWorldPositions = new Vector3[_textInfo.characterCount];
            for (int i = 0; i < _textInfo.characterCount; i++)
            {
                ref var charInfo = ref _textInfo.characterInfo[i];
                if (!charInfo.isVisible || charInfo.character == ' ') continue;

                // Calculate character center in local space
                Vector3 charCenter = Vector3.zero;
                for (int j = 0; j < 4; j++)
                {
                    charCenter += _originalVertices[charInfo.materialReferenceIndex][charInfo.vertexIndex + j];
                }
                charCenter /= 4f;

                // Convert to world space
                charWorldPositions[i] = transform.TransformPoint(charCenter);

                // Calculate direction to target with some spread
                Vector3 toTarget = targetPosition.position - charWorldPositions[i];
                Vector3 spread = Random.insideUnitSphere * targetSpread;
                _targetPositions[i] = targetPosition.position + spread;

                // Normalized direction (not used directly, but stored for reference)
                _movementDirections[i] = (_targetPositions[i] - charWorldPositions[i]).normalized;

                _speedMultipliers[i] = Random.Range(minSpeedMultiplier, maxSpeedMultiplier);
            }
        }
    }

    private void UpdateCharacter(int charIndex, float progress)
    {
        ref var charInfo = ref _textInfo.characterInfo[charIndex];
        int matIndex = charInfo.materialReferenceIndex;
        int vertIndex = charInfo.vertexIndex;

        var vertices = _textInfo.meshInfo[matIndex].vertices;
        var colors = _textInfo.meshInfo[matIndex].colors32;

        // Reset to original position
        for (int j = 0; j < 4; j++)
        {
            vertices[vertIndex + j] = _originalVertices[matIndex][vertIndex + j];
        }

        // Calculate center of character in local space
        Vector3 charCenter = Vector3.zero;
        for (int j = 0; j < 4; j++)
        {
            charCenter += vertices[vertIndex + j];
        }
        charCenter /= 4f;

        // Apply movement based on mode
        Vector3 offset = Vector3.zero;
        if (movementMode == MovementMode.Random)
        {
            offset = new Vector3(_movementDirections[charIndex].x, _movementDirections[charIndex].y, 0) *
                    (movementCurve.Evaluate(progress) * maxDistance);
        }
        else // Targeted movement
        {
            Vector3 worldStart = transform.TransformPoint(charCenter);
            Vector3 worldEnd = _targetPositions[charIndex];
            Vector3 worldOffset = Vector3.Lerp(worldStart, worldEnd, movementCurve.Evaluate(progress)) - worldStart;
            offset = transform.InverseTransformVector(worldOffset);
        }

        byte alpha = (byte)(255 * (1 - progress));

        for (int j = 0; j < 4; j++)
        {
            vertices[vertIndex + j] += offset;
            colors[vertIndex + j].a = alpha;
        }
    }

    private void CacheOriginalVertices()
    {
        _tmpText.ForceMeshUpdate();
        _textInfo = _tmpText.textInfo;
        _originalVertices = new Vector3[_textInfo.meshInfo.Length][];

        for (int i = 0; i < _textInfo.meshInfo.Length; i++)
        {
            _originalVertices[i] = (Vector3[])_textInfo.meshInfo[i].vertices.Clone();
        }
    }

    private void ResetAnimation()
    {
        _tmpText.ForceMeshUpdate();
        _textInfo = _tmpText.textInfo;
        _timers = new float[_textInfo.characterCount];

        for (int i = 0; i < _timers.Length; i++)
        {
            _timers[i] = -i * delayBetweenLetters;
        }
    }

    private void UpdateMesh()
    {
        for (int i = 0; i < _textInfo.meshInfo.Length; i++)
        {
            _textInfo.meshInfo[i].mesh.vertices = _textInfo.meshInfo[i].vertices;
            _tmpText.UpdateGeometry(_textInfo.meshInfo[i].mesh, i);
        }
        _tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    private void ResetTextImmediately()
    {
        _tmpText.ForceMeshUpdate();
        _textInfo = _tmpText.textInfo;

        for (int i = 0; i < _textInfo.characterCount; i++)
        {
            ref var charInfo = ref _textInfo.characterInfo[i];
            if (!charInfo.isVisible || charInfo.character == ' ') continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;

            var vertices = _textInfo.meshInfo[matIndex].vertices;
            var colors = _textInfo.meshInfo[matIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                vertices[vertIndex + j] = _originalVertices[matIndex][vertIndex + j];
                colors[vertIndex + j].a = 255;
            }
        }

        UpdateMesh();
    }

    public void RestartAnimation()
    {
        InitializeMovementValues();
        ResetAnimation();
        _isAnimating = true;
    }
}