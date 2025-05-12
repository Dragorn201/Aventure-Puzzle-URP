using UnityEngine;
using System.Collections.Generic;

public class SpawnAleatoire : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject resourcePrefab;
    [Range(0f, 1f)] public float spawnChance = 0.2f;
    public int maxSpawnCount = 500;

    [Header("Raycast setup")]
    public float distanceBetweenCheck = 5f;   //espacement entre chaque spawn
    public float heightOfCheck = -18f;   // raycast vers le bas doit etre plus haut que mon sol
    public float rangeOfCheck = 30f;   // distance max que raycast peut parcourir
    public LayerMask layerMask;

    [Header("Spawn Zone")]
    public Vector2 positivePosition = new Vector2(50, 50);
    public Vector2 negativePosition = new Vector2(-50, -50);

    private void Start()
    {
        SpawnResources();
    }

    void SpawnResources()
    {
        
        if (resourcePrefab == null)
        {
            Debug.LogError("resourcePrefab is not assigné !");
            return;
        }

        if (distanceBetweenCheck <= 0f)
        {
            Debug.LogError("distanceBetweenCheck doit être > 0 !");
            return;
        }

        int spawnCount = 0;

        for (float x = negativePosition.x; x < positivePosition.x; x += distanceBetweenCheck)
        {
            for (float z = negativePosition.y; z < positivePosition.y; z += distanceBetweenCheck)
            {
                if (spawnCount >= maxSpawnCount)
                {
                    Debug.Log("Limite de spawn atteinte (" + maxSpawnCount + ")");
                    return;
                }

                Vector3 origin = new Vector3(x, heightOfCheck, z);
                RaycastHit hit;

                if (Physics.Raycast(origin, Vector3.down, out hit, rangeOfCheck, layerMask))
                {
                    if (Random.value < spawnChance)
                    {
                        Instantiate(
                            resourcePrefab,
                            hit.point,
                            Quaternion.Euler(0, Random.Range(0, 360), 0),
                            transform
                        );
                        spawnCount++;
                    }
                }
            }
        }

        Debug.Log("Nombre d'objets spawnés : " + spawnCount);
    }

    
    private void OnDrawGizmosSelected()  //pour checker ou notre script agis
    {
        float terrainY = -24f; //hauteur envirion du sol

        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Vector3 center = new Vector3(
            (positivePosition.x + negativePosition.x) / 2f,
            terrainY,
            (positivePosition.y + negativePosition.y) / 2f
        );
        Vector3 size = new Vector3(
            Mathf.Abs(positivePosition.x - negativePosition.x),
            0.1f,
            Mathf.Abs(positivePosition.y - negativePosition.y)
        );
        Gizmos.DrawCube(center, size);
    }

}
