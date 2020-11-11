using System;
using System.Collections;
using System.Collections.Generic;
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

    public void DrawFieldOfView(float _totalAngleOfView, float _distance, float _currentRotationAngle)
    {
        var nbOfVertices = m_nbOfTriangles + 2;
        
        var mesh = new Mesh();

        var vertices = new Vector3[nbOfVertices];
        var uvs = new Vector2[nbOfVertices];
        var triangles = new int[m_nbOfTriangles * 3];
        
        vertices[0] = Vector3.zero;
        uvs[0] = Vector2.zero;
        
        var step = _totalAngleOfView / m_nbOfTriangles;

        for (var i = 1; i < vertices.Length; i++)
        {
            var direction =
                Quaternion.Euler(0, (-_totalAngleOfView * .5f) + (i * step), 0)
                * Quaternion.Euler(0, _currentRotationAngle, 0)
                * Vector3.forward;
            
            vertices[i] = direction * _distance;
            uvs[i] = new Vector2(1,0);
        }
        
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