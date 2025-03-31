using UnityEngine;

public class PlayerSpawnBehaviour : MonoBehaviour
{
    public Transform SpawnPosition;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        gameObject.transform.position = SpawnPosition.position;
    }

}
