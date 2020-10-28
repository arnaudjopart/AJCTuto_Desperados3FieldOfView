using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverMeshDrawer : MonoBehaviour
{

    public MeshFilter[] m_meshFilters;

    public void DrawMesh(List<MaskMeshData> _maskMeshDataCollection)
    {
        var nbOfMeshToDraw = _maskMeshDataCollection.Count;

        for (var i = 0; i < nbOfMeshToDraw; i++)
        {
            var mesh = DrawMeshFromData(_maskMeshDataCollection[i]);
            m_meshFilters[i].mesh = mesh;
            if (_maskMeshDataCollection[i].m_coverType == MaskMeshData.CoverType.FULL)
            {
                m_meshFilters[i].gameObject.layer = 10;
            }
            if (_maskMeshDataCollection[i].m_coverType == MaskMeshData.CoverType.SEMI)
            {
                m_meshFilters[i].gameObject.layer = 11;
            }
        }

        for (var i = 0; i < m_meshFilters.Length; i++)
        {
            m_meshFilters[i].gameObject.SetActive(i<nbOfMeshToDraw);
        }
    }

    private Mesh DrawMeshFromData(MaskMeshData _maskMeshData)
    {
        var mesh = new Mesh();

        if (_maskMeshData.m_data.Count < 2) return null;
        var nbOfTrianles = (_maskMeshData.m_data.Count - 1) * 2;

        var nbOfVertices = nbOfTrianles * 3;
        var vertices = new Vector3[nbOfVertices];
        
        
        return mesh;












    }
}
