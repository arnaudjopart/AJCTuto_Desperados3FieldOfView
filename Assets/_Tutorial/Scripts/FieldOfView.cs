using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public int m_nbOfTriangles = 10;

    private MeshFilter m_meshFilter;

    private void Awake()
    {
        m_meshFilter = GetComponent<MeshFilter>();
    }

    public void DrawFieldOfView(float _totalAngleOfView, float _distance, float _currentRotationAngle)
    {
        m_nbOfTriangles = Mathf.Max(1, m_nbOfTriangles);
        var endPoints = new Vector3[m_nbOfTriangles + 1];
        var step = m_nbOfTriangles == 1 ? _totalAngleOfView : _totalAngleOfView / m_nbOfTriangles;

        for (var i = 0; i < endPoints.Length; i++)
        {
            var endPointDirection =
                Quaternion.Euler(0, (-_totalAngleOfView * .5f) + (i * step), 0)
                *Quaternion.Euler(0,_currentRotationAngle,0)
                * transform.forward;
            endPoints[i] = endPointDirection * _distance;
        }
        
        var mesh = new Mesh();

        var nbOfVertices = m_nbOfTriangles + 2;
        var vertices = new Vector3[nbOfVertices];
        vertices[0]=Vector3.zero;
        
        var uvs = new Vector2[nbOfVertices];
        uvs[0] = Vector2.zero;

        for (var i = 1; i < vertices.Length; i++)
        {
            vertices[i] = endPoints[i - 1];
            uvs[i] = new Vector2(1,0);
        }

        var triangles = new int[m_nbOfTriangles * 3];
        for (var i = 0; i < triangles.Length; i += 3)
        {
            triangles[i] = 0;
            triangles[i + 1] = i / 3 + 1;
            triangles[i + 2] = i / 3 + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        m_meshFilter.mesh = mesh;

    }
}
