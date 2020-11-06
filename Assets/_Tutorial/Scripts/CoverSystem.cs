using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverSystem : MonoBehaviour
{

    public int m_numberOfRaycast = 15;

    private RaycastResult m_previousRaycastResult;

    public LayerMask m_fullCoverLayerMask;
    public LayerMask m_semiCoverLayerMask;

    public int m_levelOfPrecision=10;
    // Start is called before the first frame update
    void Start()
    {
        m_previousRaycastResult = new RaycastResult();
    }

    public List<MaskMeshData> generateMaskMeshData(MaskMeshData.CoverType _coverType, float _totalAngle, float
        _length, float _currentRotation)
    {
        var result = new List<MaskMeshData>();

        LayerMask raycastLayerMask;

        switch (_coverType)
        {
            case MaskMeshData.CoverType.FULL:
                raycastLayerMask = m_fullCoverLayerMask;
                break;
            case MaskMeshData.CoverType.SEMI:
                raycastLayerMask = m_semiCoverLayerMask;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_coverType), _coverType, null);
        }
        
        var currentMaskMeshData = new MaskMeshData(_coverType);

        var raycastAngleStep = _totalAngle / (m_numberOfRaycast - 1);

        for (var i = 0; i < m_numberOfRaycast; i++)
        {
            var angle = -_totalAngle * .5f + i * raycastAngleStep;
            var raycastDirection = Quaternion.Euler(0, angle, 0)
                                   * Quaternion.Euler(0, _currentRotation, 0)
                                   * transform.forward;
            var ray = new Ray(transform.position, raycastDirection);
            
            var currentRaycastResult = new RaycastResult();

            if (!Physics.Raycast(ray, out var hit, _length, raycastLayerMask))
            {
                currentRaycastResult.m_hasHitObstacle = false;

                if (m_previousRaycastResult.m_hasHitObstacle)
                {
                    if (currentMaskMeshData.m_data.Count > 0)
                    {
                        var possibleBetterEdge = FindClosingEdge(
                            angle,
                            raycastAngleStep,
                            _length,
                            _currentRotation,
                            raycastLayerMask);
                        
                        if (possibleBetterEdge.m_hasHitObstacle)
                        {
                            currentMaskMeshData.m_data.Add(possibleBetterEdge);
                        }
                        
                        result.Add(currentMaskMeshData);
                    }
                }

                m_previousRaycastResult = currentRaycastResult;
                continue;
            }

            currentRaycastResult.m_hasHitObstacle = true;
            currentRaycastResult.m_startPoint = hit.point;
            currentRaycastResult.m_endPoint = ray.GetPoint(_length);

            if (m_previousRaycastResult.m_hasHitObstacle)
            {
                currentMaskMeshData.m_data.Add(currentRaycastResult);
                if (i == m_numberOfRaycast - 1)
                {
                    if(currentMaskMeshData.m_data.Count>0) result.Add(currentMaskMeshData);
                }
            }
            else
            {
                currentMaskMeshData = new MaskMeshData(_coverType);
                
                
                var possibleBetterEdge = FindEnteringEdge(
                    angle,
                    raycastAngleStep,
                    _length,
                    _currentRotation,
                    raycastLayerMask);

                if (possibleBetterEdge.m_hasHitObstacle)
                {
                    currentMaskMeshData.m_data.Add(possibleBetterEdge);
                }
                
                currentMaskMeshData.m_data.Add(currentRaycastResult);

                if (i == m_numberOfRaycast - 1)
                {
                    result.Add(currentMaskMeshData);
                }

            }

            m_previousRaycastResult = currentRaycastResult;

        }
        return result;
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


public class MaskMeshData
{
    public List<RaycastResult> m_data;

    public enum CoverType
    {
        FULL,
        SEMI
    };

    public CoverType m_coverType;

    public MaskMeshData(CoverType _type)
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