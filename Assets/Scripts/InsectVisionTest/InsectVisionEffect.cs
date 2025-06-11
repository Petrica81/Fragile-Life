using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class InsectVisionEffect : MonoBehaviour
{
    [Range(1, 100)]
    public float hexDensity = 30f;

    private Material material;

    void OnEnable()
    {
        material = new Material(Shader.Find("Custom/InsectVision"));
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        material.SetFloat("_HexSize", hexDensity);
        Graphics.Blit(src, dest, material);
    }
}