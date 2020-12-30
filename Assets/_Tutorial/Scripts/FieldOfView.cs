using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class FieldOfView : MonoBehaviour
{

    [Range(1,20)]
    public int m_nbOfTriangles = 10;

    private MeshFilter m_meshFilter;

    private void Awake()
    {
        m_meshFilter = GetComponent<MeshFilter>();
    }

    public void DrawFieldOfView(float _totalAngleOfView, float _radius, float _currentRotationAngle)
    {
        var nbOfVertices = m_nbOfTriangles + 2;
        
        var mesh = new Mesh();

        var vertices = new Vector3[nbOfVertices];
        var uvs = new Vector2[nbOfVertices];
        var trianglesIndexes = new int[m_nbOfTriangles * 3];
        
        vertices[0] = Vector3.zero;
        uvs[0] = Vector2.zero;
        
        var angleStep = _totalAngleOfView / m_nbOfTriangles;

        for (var i = 1; i < vertices.Length; i++)
        {
            var direction =
                Quaternion.Euler(0, (-_totalAngleOfView * .5f) + ((i-1) * angleStep), 0)
                * Quaternion.Euler(0, _currentRotationAngle, 0)
                * Vector3.forward;
            
            vertices[i] = direction * _radius;
            uvs[i] = new Vector2(1,0);
        }
        
        for (var i = 0; i < trianglesIndexes.Length; i += 3)
        {
            trianglesIndexes[i] = 0;
            trianglesIndexes[i + 1] = i / 3 + 1;
            trianglesIndexes[i + 2] = i / 3 + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = trianglesIndexes;
        mesh.uv = uvs;

        m_meshFilter.mesh = mesh;

    }
}