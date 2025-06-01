using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AutoCameraSetup : MonoBehaviour
{
    [Header("Texture Settings")]
    public int textureWidth = 512;
    public int textureHeight = 512;
    public bool useScreenSize = false;
    public int depth = 24;
    public RenderTextureFormat format = RenderTextureFormat.ARGB32;

    [Header("Material Settings")]
    public string materialName = "CameraMaterial";
    public Shader shaderToUse; // Lăsat null pentru shader-ul standard

    private RenderTexture _renderTexture;
    private Material _generatedMaterial;

    void Start()
    {
        SetupCameraSystem();
    }

    void OnDestroy()
    {
        CleanupResources();
    }

    void SetupCameraSystem()
    {
        // 1. Crează Render Texture
        CreateRenderTexture();

        // 2. Configurează camera
        GetComponent<Camera>().targetTexture = _renderTexture;

        // 3. Crează materialul
        CreateMaterial();

        // 4. Aplică materialul pe părinte
        ApplyToParent();
    }

    void CreateRenderTexture()
    {
        if (useScreenSize)
        {
            textureWidth = Screen.width;
            textureHeight = Screen.height;
        }

        _renderTexture = new RenderTexture(textureWidth, textureHeight, depth, format)
        {
            name = $"RT_{gameObject.name}",
            antiAliasing = Mathf.Max(QualitySettings.antiAliasing, 1)
        };

        _renderTexture.Create();
    }

    void CreateMaterial()
    {
        // Folosește shader-ul specificat sau Standard shader dacă nu e setat
        Shader targetShader = shaderToUse != null ? shaderToUse : Shader.Find("Standard");

        _generatedMaterial = new Material(targetShader)
        {
            name = materialName,
            mainTexture = _renderTexture
        };
    }

    void ApplyToParent()
    {
        if (transform.parent != null)
        {
            Renderer parentRenderer = transform.parent.GetComponent<Renderer>();

            if (parentRenderer != null)
            {
                parentRenderer.material = _generatedMaterial;
            }
            else
            {
                Debug.LogWarning($"Parent object {transform.parent.name} has no Renderer component");
            }
        }
        else
        {
            Debug.LogWarning("Camera has no parent object to apply material to");
        }
    }

    void CleanupResources()
    {
        if (_renderTexture != null)
        {
            _renderTexture.Release();
            Destroy(_renderTexture);
        }

        if (_generatedMaterial != null)
        {
            Destroy(_generatedMaterial);
        }
    }

    // Actualizează la schimbarea dimensiunilor ecranului
    void Update()
    {
        if (useScreenSize &&
            (_renderTexture.width != Screen.width || _renderTexture.height != Screen.height))
        {
            CleanupResources();
            SetupCameraSystem();
        }
    }
}