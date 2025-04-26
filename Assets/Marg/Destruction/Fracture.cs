using UnityEngine;

public class Fracture : MonoBehaviour
{
    public GameObject fracturedPrefab;       

    public float explosionForce = 500f;
    public float explosionRadius = 2f;

    private bool hasFractured = false;
    private Transform player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    public void Explode()
    {
        if (hasFractured) return;

        
        hasFractured = true;

        
        GameObject fracturedInstance = Instantiate
        (
            fracturedPrefab,
            transform.position,
            Quaternion.identity 
        );
        

        
        foreach (Rigidbody rb in fracturedInstance.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(explosionForce, player.position, explosionRadius);
            Destroy(rb.transform.gameObject, Random.Range(2f,4f));
        }

        
        fracturedInstance.SetActive(true);

        
        Destroy(gameObject);
        
    }
}