using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextSequentialFadeIn : MonoBehaviour
{
    [Header("Fade In Settings")]
    public float fadeInDuration = 0.5f;
    public float delayBetweenLetters = 0.1f;
    public float holdDurationAfterFadeIn = 1f; // Timpul de așteptare după ce toate literele sunt vizibile

    [Header("Disperse Settings")]
    public TextFadeDisperse disperseScript;
    public bool autoStartDisperse = true;

    private TMP_Text _tmpText;
    private float[] _letterTimers;
    private bool _allLettersVisible;
    private float _allVisibleTime;
    private bool _fadeInComplete;

    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();

        // Dacă nu avem referință la disperseScript, încercăm să o găsim
        if (disperseScript == null)
        {
            disperseScript = GetComponent<TextFadeDisperse>();
        }

        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
        _fadeInComplete = false;
        _allLettersVisible = false;
    }

    private void Initialize()
    {
        _tmpText.ForceMeshUpdate();
        int characterCount = _tmpText.textInfo.characterCount;

        _letterTimers = new float[characterCount];
        for (int i = 0; i < characterCount; i++)
        {
            _letterTimers[i] = -i * delayBetweenLetters;
        }

        // Setăm inițial toate literele invizibile
        SetAllLettersAlpha(0);
    }

    private void Update()
    {
        if (_fadeInComplete) return;

        _tmpText.ForceMeshUpdate();
        var textInfo = _tmpText.textInfo;
        bool allComplete = true;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            ref var charInfo = ref textInfo.characterInfo[i];
            if (!charInfo.isVisible || charInfo.character == ' ') continue;

            _letterTimers[i] += Time.deltaTime;
            float t = Mathf.Clamp01(_letterTimers[i] / fadeInDuration);

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;
            var colors = textInfo.meshInfo[matIndex].colors32;

            byte alpha = (byte)(255 * t);
            for (int j = 0; j < 4; j++)
            {
                colors[vertIndex + j].a = alpha;
            }

            if (t < 1f) allComplete = false;
        }

        // Actualizăm toate mesh-urile
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            _tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
        _tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        if (allComplete && !_allLettersVisible)
        {
            _allLettersVisible = true;
            _allVisibleTime = Time.time;
        }

        // După ce toate literele sunt vizibile și a trecut holdDuration
        if (_allLettersVisible && Time.time - _allVisibleTime >= holdDurationAfterFadeIn)
        {
            _fadeInComplete = true;
            OnFadeInComplete();
        }
    }

    private void OnFadeInComplete()
    {
        if (autoStartDisperse && disperseScript != null)
        {
            disperseScript.enabled = true;
            disperseScript.RestartAnimation();
        }
    }

    private void SetAllLettersAlpha(byte alpha)
    {
        _tmpText.ForceMeshUpdate();
        var textInfo = _tmpText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            ref var charInfo = ref textInfo.characterInfo[i];
            if (!charInfo.isVisible || charInfo.character == ' ') continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;
            var colors = textInfo.meshInfo[matIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                colors[vertIndex + j].a = alpha;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            _tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
        _tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    // Funcție publică pentru a declanșa manual efectul
    public void StartSequence()
    {
        Initialize();
        _fadeInComplete = false;
        _allLettersVisible = false;
        enabled = true;
    }
}