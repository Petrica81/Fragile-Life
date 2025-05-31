using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class DoubleSidedCollider : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Mesh collisionMesh = new Mesh();

        // Duplică vertecșii și triunghiurile
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        int vertexCount = vertices.Length;
        int triangleCount = triangles.Length;

        Vector3[] newVertices = new Vector3[vertexCount * 2];
        Vector3[] newNormals = new Vector3[vertexCount * 2];
        int[] newTriangles = new int[triangleCount * 2];

        // Copiază vertecșii originali
        for (int i = 0; i < vertexCount; i++)
        {
            newVertices[i] = vertices[i];
            newNormals[i] = normals[i];
        }

        // Adaugă vertecșii inversați
        for (int i = 0; i < vertexCount; i++)
        {
            newVertices[vertexCount + i] = vertices[i];
            newNormals[vertexCount + i] = -normals[i];
        }

        // Copiază triunghiurile originale
        for (int i = 0; i < triangleCount; i += 3)
        {
            newTriangles[i] = triangles[i];
            newTriangles[i + 1] = triangles[i + 1];
            newTriangles[i + 2] = triangles[i + 2];
        }

        // Adaugă triunghiurile inversate
        for (int i = 0; i < triangleCount; i += 3)
        {
            newTriangles[triangleCount + i] = triangles[i + 2] + vertexCount;
            newTriangles[triangleCount + i + 1] = triangles[i + 1] + vertexCount;
            newTriangles[triangleCount + i + 2] = triangles[i] + vertexCount;
        }

        // Aplică noul mesh collider
        collisionMesh.vertices = newVertices;
        collisionMesh.normals = newNormals;
        collisionMesh.triangles = newTriangles;

        GetComponent<MeshCollider>().sharedMesh = collisionMesh;
    }
}