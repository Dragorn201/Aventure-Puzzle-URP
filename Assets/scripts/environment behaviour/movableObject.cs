using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    [HideInInspector]public bool obstacleHited = false;
    [HideInInspector]public bool isMoving = false;
    [HideInInspector] public float blocWallDistance;
    private RaycastHit hit;
    private RaycastHit hitback;
    [HideInInspector]public float selfVelocity;
    private CanDamageBoss canDamageBoss;
    private WallDestroy wallDestroy;

    void Awake()
    {
        canDamageBoss = GetComponent<CanDamageBoss>();
        WallDestroy wallDestroy = GetComponent<WallDestroy>();
    }
    
    
    public IEnumerator WaitUnitilCollision(Vector3 direction)
    {
        isMoving = true;
        Physics.BoxCast(transform.position, transform.localScale / 2, direction, out hit, transform.rotation);
        Physics.Raycast(hit.point, -direction, out hitback, Mathf.Infinity);
        while (Vector3.Distance(hitback.point, hit.point) > blocWallDistance && isMoving)
        {
            Physics.BoxCast(transform.position, transform.localScale / 2, direction, out hit, transform.rotation);
            Physics.Raycast(hit.point, -direction, out hitback, Mathf.Infinity);
            yield return new WaitForFixedUpdate(); 
        }
        obstacleHited = true;
        isMoving = false;
        TryDestroyObstacle(hit);
        TryDestroySelf();
        TryDamageBoss();
        selfVelocity = 0f;

    }

    void TryDestroyObstacle(RaycastHit hit)
    {
        if (hit.transform != null && hit.collider.GetComponent<WallDestroy>() != null)
        {
            WallDestroy wallDestroy = hit.collider.GetComponent<WallDestroy>();
            bool wallDestroyed = wallDestroy.TryDestroyWall(selfVelocity);
            if (wallDestroyed) Destroy(wallDestroy.transform.gameObject);
        }
        
    }

    void TryDestroySelf()
    {
        if (wallDestroy != null)
        {
            bool wallDestroyed = wallDestroy.TryDestroyWall(selfVelocity);
            if (wallDestroyed) Destroy(wallDestroy.transform.gameObject);
        }
    }

    void TryDamageBoss()
    {
        if(canDamageBoss != null)canDamageBoss.TryDamageBoss(hit , selfVelocity);
    }

    public void StopMoving()
    {
        obstacleHited = true;
        isMoving = false;
        selfVelocity = 0f;
    }
}

