using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineOfSight : MonoBehaviour
{
    public Transform m_lineStartPosition;
    public LayerMask m_layerMask;
    
    public Player m_player;
    
    public float m_progressSpeedInUnitPerSecond = 2f;
    public float m_retreatSpeedInUnitPerSecond = 3f;
    
    private LineRenderer m_lineRenderer;
    private Material m_lineRendererMaterial;
    
    private Enemy m_enemy;

    private void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRendererMaterial = m_lineRenderer.materials[0];
        m_enemy = GetComponent<Enemy>();
    }
    
    void Update()
    {
        if (PlayerIsInFieldOfView())
        {
            if (PlayerIsDetected())
            {
                HandleDetection();
                return;
            }
        }
        ResetDetection();
    }
    
    private bool PlayerIsInFieldOfView()
    {
        var fromEnemyToPlayerVector = m_player.transform.position - transform.position;
        
        var distanceBetweenEnemyAndPlayer = Vector3.Magnitude(fromEnemyToPlayerVector);
        var detectionAngle = Vector3.Angle(fromEnemyToPlayerVector, m_enemy.LookForwardDirection);
        
        var playerIsNear = distanceBetweenEnemyAndPlayer < m_enemy.m_secondaryFieldOfViewDistance;
        var playerIsInAngle = detectionAngle < m_enemy.m_totalViewAngleInDegree * .5f;
        
        return playerIsNear && playerIsInAngle;
    }

    private bool PlayerIsDetected()
    {
        var position = transform.position;
        var fromEnemyToPlayerVector = m_player.transform.position - position;
        
        var ray = new Ray(position,fromEnemyToPlayerVector);
        var raycastHits = Physics.RaycastAll(ray, 500, m_layerMask);
        if (raycastHits.Length == 0) return false;

        var semiCover = false;
   
        var orderedByDistanceRaycastHits = raycastHits.OrderBy(hit => hit.distance).ToArray();
        
        foreach (var hit in orderedByDistanceRaycastHits)
        {
            switch (hit.collider.gameObject.layer)
            {
                case 12:
                    return false;
                case 13:
                    semiCover = true;
                    continue;
            }

            if (semiCover) return !m_player.isCrouch;
            
            var playerDistance = Vector3.Magnitude(fromEnemyToPlayerVector);
            if (playerDistance > m_enemy.m_secondaryFieldOfViewDistance) return false;
            if (playerDistance > m_enemy.m_primaryFieldOfViewDistance) return !m_player.isCrouch;
            
            return true;
        }
        
        return false;
    }
    
    private void HandleDetection()
    {
        m_lineRenderer.enabled = true;
        Vector3[] linePoints = {
            m_lineStartPosition.position,
            m_player.m_headPosition.position
        };
        m_lineRenderer.positionCount = 2;
        m_lineRenderer.SetPositions(linePoints);
        
        var fromEnemyToPlayerVector = m_player.transform.position - transform.position;
        var distanceBetweenEnemyAndPlayer = Vector3.Magnitude(fromEnemyToPlayerVector);
        
        var timeToFill = distanceBetweenEnemyAndPlayer / m_progressSpeedInUnitPerSecond;
        var progressSpeed = 1 / timeToFill;
        
        var currentProgress = m_lineRendererMaterial.GetFloat("_Progress");
        currentProgress += progressSpeed*Time.deltaTime;
        
        currentProgress = Mathf.Clamp(currentProgress, 0, 1);
        m_lineRendererMaterial.SetFloat("_Progress",currentProgress);
    }
    
    private void ResetDetection()
    {
        m_lineRenderer.enabled = false;
        
        var fromEnemyToPlayerVector = m_player.transform.position - transform.position;
        var distanceBetweenEnemyAndPlayer = Vector3.Magnitude(fromEnemyToPlayerVector);

        var timeToEmpty = distanceBetweenEnemyAndPlayer / m_retreatSpeedInUnitPerSecond;
        var retreatSpeed = 1 / timeToEmpty;
        
        var currentProgress = m_lineRendererMaterial.GetFloat("_Progress");
        currentProgress -= retreatSpeed*Time.deltaTime;
        
        currentProgress = Mathf.Clamp(currentProgress, 0, 1);
        m_lineRendererMaterial.SetFloat("_Progress",currentProgress);
    }

    
    
    
    
    
    
    
    
    
    
    
    /*
    private void OnDrawGizmos()
    {
        var fromEnemyToPlayerVector = m_player.transform.position - transform.position;
        
        var distanceBetweenEnemyAndPlayer = Vector3.Magnitude(fromEnemyToPlayerVector);
        var detectionAngle = Vector3.Angle(fromEnemyToPlayerVector, m_enemy.LookForwardDirection);
        
        var playerIsNear = distanceBetweenEnemyAndPlayer < m_enemy.m_secondaryFieldOfViewDistance;
        var playerIsInAngle = detectionAngle < m_enemy.m_totalViewAngleInDegree * .5f;

        var p1 = transform.position;
        var p2 = m_player.transform.position;
        
        var thickness = 5;
        Handles.DrawBezier(p1,p2,p1,p2, playerIsInAngle?Color.cyan:Color.magenta,null,thickness);
    }*/
}
