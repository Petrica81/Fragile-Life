using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextFadeDisperse : MonoBehaviour
{
    [Header("Animation Settings")]
    public float duration = 2f;
    public float delayBetweenLetters = 0.05f;
    public float maxDistance = 10f;
    public bool loopAnimation;

    [Header("Randomization")]
    public float minDirectionAngle = 0f;
    public float maxDirectionAngle = 360f;
    public float minSpeedMultiplier = 0.8f;
    public float maxSpeedMultiplier = 1.2f;

    private TMP_Text _tmpText;
    private TMP_TextInfo _textInfo;
    private float[] _timers;
    private Vector3[][] _originalVertices;
    private Vector2[] _randomDirections;
    private float[] _speedMultipliers;
    private bool _isAnimating;

    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
        CacheOriginalVertices();
    }

    private void OnEnable()
    {
        InitializeRandomValues();
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
            InitializeRandomValues(); // Reinitialize random values when looping
        }
    }

    private void InitializeRandomValues()
    {
        _tmpText.ForceMeshUpdate();
        _textInfo = _tmpText.textInfo;

        _randomDirections = new Vector2[_textInfo.characterCount];
        _speedMultipliers = new float[_textInfo.characterCount];

        for (int i = 0; i < _textInfo.characterCount; i++)
        {
            float randomAngle = Random.Range(minDirectionAngle, maxDirectionAngle);
            _randomDirections[i] = new Vector2(
                Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                Mathf.Sin(randomAngle * Mathf.Deg2Rad)
            ).normalized;

            _speedMultipliers[i] = Random.Range(minSpeedMultiplier, maxSpeedMultiplier);
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

        // Apply random direction movement
        Vector2 randomDir = _randomDirections[charIndex];
        Vector3 offset = new Vector3(randomDir.x, randomDir.y, 0) * (progress * maxDistance);
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
        InitializeRandomValues();
        ResetAnimation();
        _isAnimating = true;
    }
}