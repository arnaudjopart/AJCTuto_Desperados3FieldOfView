using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverSystem : MonoBehaviour
{
    public int m_numberOfRaycast = 15;

    public LayerMask m_fullCoverLayerMask;
    public LayerMask m_semiCoverLayerMask;

    [HideInInspector]
    public int m_levelOfPrecision = 10;

    public List<MeshData> GenerateMaskMeshData(MeshData.CoverType _coverType, float _fovTotalAngle, float _fovLength, float _currentRotation)
    {
        var previousRaycastResult = new RaycastResult();
        var listOfMeshToDraw = new List<MeshData>();

        LayerMask raycastLayerMask;

        switch (_coverType)
        {
            case MeshData.CoverType.FULL:
                raycastLayerMask = m_fullCoverLayerMask;
                break;
            case MeshData.CoverType.SEMI:
                raycastLayerMask = m_semiCoverLayerMask;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_coverType), _coverType, null);
        }
        
        var currentMeshData = new MeshData(_coverType);

        var raycastAngleStep = _fovTotalAngle / (m_numberOfRaycast - 1);

        for (var i = 0; i < m_numberOfRaycast; i++)
        {
            var currentRaycastAngle = -_fovTotalAngle * .5f + i * raycastAngleStep;
            var raycastDirection = Quaternion.Euler(0, currentRaycastAngle, 0)
                                   * Quaternion.Euler(0, _currentRotation, 0)
                                   * transform.forward;
            
            var ray = new Ray(transform.position, raycastDirection);
            
            var currentRaycastResult = new RaycastResult();

            if (!Physics.Raycast(ray, out var hit, _fovLength, raycastLayerMask))
            {
                currentRaycastResult.m_hasHitObstacle = false;

                if (previousRaycastResult.m_hasHitObstacle)
                {
                    
                    var possibleBetterEdge = FindClosingEdge(
                        currentRaycastAngle,
                        raycastAngleStep,
                        _fovLength,
                        _currentRotation,
                        raycastLayerMask);
                        
                    if (possibleBetterEdge.m_hasHitObstacle)
                    {
                        currentMeshData.m_data.Add(possibleBetterEdge);
                    }
                        
                    listOfMeshToDraw.Add(currentMeshData);
                }

                previousRaycastResult = currentRaycastResult;
                continue;
            }

            currentRaycastResult.m_hasHitObstacle = true;
            currentRaycastResult.m_startPoint = hit.point;
            currentRaycastResult.m_endPoint = ray.GetPoint(_fovLength);

            if (previousRaycastResult.m_hasHitObstacle)
            {
                currentMeshData.m_data.Add(currentRaycastResult);
                if (i == m_numberOfRaycast - 1)
                {
                    if(currentMeshData.m_data.Count>0) listOfMeshToDraw.Add(currentMeshData);
                }
            }
            else
            {
                currentMeshData = new MeshData(_coverType);
                
                var possibleBetterEdge = FindEnteringEdge(
                    currentRaycastAngle,
                    raycastAngleStep,
                    _fovLength,
                    _currentRotation,
                    raycastLayerMask);

                if (possibleBetterEdge.m_hasHitObstacle)
                {
                    currentMeshData.m_data.Add(possibleBetterEdge);
                }
                
                currentMeshData.m_data.Add(currentRaycastResult);

                if (i == m_numberOfRaycast - 1)
                {
                    listOfMeshToDraw.Add(currentMeshData);
                }
            }

            previousRaycastResult = currentRaycastResult;

        }
        return listOfMeshToDraw;
    }

    private RaycastResult FindClosingEdge(float _startAngle, float _angle,float _length, float _currentRotation, 
    LayerMask _layerMask)
    {
        var result = new RaycastResult();
        
        var progression=0f;
        
        var hasFoundBetterEdge = false;
        
        for (var i = 0; i < m_levelOfPrecision; i++)
        {
            var directionOfMove = hasFoundBetterEdge ? 1 : -1;
            progression += directionOfMove / (Mathf.Pow(2, i+1));
            var currentAngle = _startAngle + _angle * progression;
            
            var raycastDirection = Quaternion.Euler(0, currentAngle, 0)
                                   * Quaternion.Euler(0, _currentRotation, 0)
                                   * transform.forward;
            var ray = new Ray(transform.position, raycastDirection);

            if (Physics.Raycast(ray, out var hit, _length, _layerMask))
            {
                hasFoundBetterEdge = true;
                result.m_hasHitObstacle = true;
                result.m_startPoint = hit.point;
                result.m_endPoint = ray.GetPoint(_length);
            }
            else
            {
                hasFoundBetterEdge = false;
            }
            
        }
        
        return result;
    }
    
    private RaycastResult FindEnteringEdge(float _startAngle, float _angle,float _length, float _currentRotation, 
        LayerMask _layerMask)
    {
        var result = new RaycastResult();
        
        var progression=0f;
        
        var hasFoundBetterEdge = true;
        
        for (var i = 0; i < m_levelOfPrecision; i++)
        {
            var directionOfMove = hasFoundBetterEdge ? -1 : 1;
            progression += directionOfMove / (Mathf.Pow(2, i+1));
            var currentAngle = _startAngle + _angle * progression;
            
            var raycastDirection = Quaternion.Euler(0, currentAngle, 0)
                                   * Quaternion.Euler(0, _currentRotation, 0)
                                   * transform.forward;
            var ray = new Ray(transform.position, raycastDirection);

            if (Physics.Raycast(ray, out var hit, _length, _layerMask))
            {
                hasFoundBetterEdge = true;
                result.m_hasHitObstacle = true;
                result.m_startPoint = hit.point;
                result.m_endPoint = ray.GetPoint(_length);
            }
            else
            {
                hasFoundBetterEdge = false;
            }
            
        }
        
        return result;
    }
}


public class MeshData
{
    public readonly List<RaycastResult> m_data;

    public enum CoverType
    {
        FULL,
        SEMI
    };

    public readonly CoverType m_coverType;

    public MeshData(CoverType _type)
    {
        m_data = new List<RaycastResult>();
        m_coverType = _type;
    }
}

public struct RaycastResult
{
    public Vector3 m_startPoint;
    public Vector3 m_endPoint;

    public bool m_hasHitObstacle;
}