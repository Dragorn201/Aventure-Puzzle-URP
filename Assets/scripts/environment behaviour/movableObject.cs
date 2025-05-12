using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovableObject : MonoBehaviour
{
    [HideInInspector]public bool obstacleHited = false;
    [HideInInspector]public bool isMoving = false;
    [HideInInspector] public float blocWallDistance;
    private RaycastHit hit;
    private RaycastHit hitback;
    [HideInInspector]public float selfVelocity;
    private CanDamageBoss canDamageBoss;

    private Vector3 basicPosition;
    private float levitationElapsedTime;
    private float levitationSpeed;
    [SerializeField]private float levitationIntensity = 0.5f;

    private Vector3 point;

    void Awake()
    {
        levitationElapsedTime = Random.Range(0f, 3.1415f);
        levitationSpeed = Random.Range(2.5f, 3f);
        basicPosition = transform.position;
        canDamageBoss = GetComponent<CanDamageBoss>();
    }
    
    
    public bool DetectCollision(Vector3 direction)
    {
        isMoving = true;
        Physics.BoxCast(transform.position, transform.localScale / 2 - transform.localScale * 0.1f, direction, out hit, transform.rotation);
        Physics.Raycast(hit.point , -direction, out hitback, 1f);
        bool collision = (Vector3.Distance(hitback.point, hit.point) < blocWallDistance);
        
        point = hit.point;
        if (collision)
        {
            CollisionDetected();
        }
        
        return collision;
    }

    void FixedUpdate()
    {
        levitationElapsedTime += Time.fixedDeltaTime;
        Vector3 levitationPosition = new Vector3(transform.position.x,basicPosition.y + Mathf.Sin(levitationElapsedTime * levitationSpeed) * levitationIntensity, transform.position.z);
        transform.position = levitationPosition;
        if (levitationElapsedTime >= 10 * Mathf.PI) levitationElapsedTime = 0f;
    }

    public void CollisionDetected()
    {
        
        obstacleHited = true;
        if (hit.collider.tag != "Player")
        {
            TryDestroyObstacle(hit);
        }
        
        TryDestroySelf();
        TryDamageBoss();
        StopMoving();
    }

    void TryDestroyObstacle(RaycastHit hit)
    {
        WallDestroy wallDestroy = hit.collider.GetComponent<WallDestroy>();
        if (wallDestroy != null)
        {
            wallDestroy.TryDestroyWall(selfVelocity);
        }
    }

    void TryDestroySelf()
    {
        WallDestroy wallDestroy = GetComponent<WallDestroy>();
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
        isMoving = false;
        selfVelocity = 0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(point, .5f);
    }
}

