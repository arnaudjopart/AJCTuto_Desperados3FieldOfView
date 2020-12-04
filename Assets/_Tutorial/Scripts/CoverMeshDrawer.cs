using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverMeshDrawer : MonoBehaviour
{

    public MeshFilter[] m_meshFilters;

    public void DrawMesh(List<MeshData> _maskMeshDataCollection)
    {
        var nbOfMeshToDraw = _maskMeshDataCollection.Count;

        for (var i = 0; i < nbOfMeshToDraw; i++)
        {
            var mesh = DrawMeshFromData(_maskMeshDataCollection[i]);
            m_meshFilters[i].mesh = mesh;
            if (_maskMeshDataCollection[i].m_coverType == MeshData.CoverType.FULL)
            {
                m_meshFilters[i].gameObject.layer = 10;
            }
            if (_maskMeshDataCollection[i].m_coverType == MeshData.CoverType.SEMI)
            {
                m_meshFilters[i].gameObject.layer = 11;
            }
        }

        for (var i = 0; i < m_meshFilters.Length; i++)
        {
            m_meshFilters[i].gameObject.SetActive(i<nbOfMeshToDraw);
        }
    }

    private Mesh DrawMeshFromData(MeshData _meshData)
    {
        var mesh = new Mesh();

        if (_meshData.m_data.Count < 2) return null;
        var nbOfTrianles = (_meshData.m_data.Count - 1) * 2;

        var nbOfVertices = nbOfTrianles * 3;
        var vertices = new Vector3[nbOfVertices];

        for (var i = 0; i < _meshData.m_data.Count - 1; i++)
        {
            vertices[i * 6] = transform.InverseTransformPoint(_meshData.m_data[i].m_startPoint);
            vertices[i * 6+1] = transform.InverseTransformPoint(_meshData.m_data[i].m_endPoint);
            vertices[i * 6+2] = transform.InverseTransformPoint(_meshData.m_data[i+1].m_startPoint);
            vertices[i * 6+3] = transform.InverseTransformPoint(_meshData.m_data[i].m_endPoint);
            vertices[i * 6+4] = transform.InverseTransformPoint(_meshData.m_data[i+1].m_endPoint);
            vertices[i * 6+5] = transform.InverseTransformPoint(_meshData.m_data[i+1].m_startPoint);
        }

        var triangles = new int[nbOfTrianles * 3];

        for (var i = 0; i < triangles.Length; i++)
        {
            triangles[i] = i;
        }
        
        var uvs = new Vector2[nbOfVertices];

        for (var i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(0,0);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        
        return mesh;












    }
}
