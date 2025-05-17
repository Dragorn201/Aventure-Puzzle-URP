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
            transform.rotation
        );
        

        
        foreach (Rigidbody rb in fracturedInstance.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(explosionForce, (transform.position-player.position).normalized*0.1f , explosionRadius);
            Destroy(rb.transform.gameObject, Random.Range(2f,4f));
        }

        
        fracturedInstance.SetActive(true);

        
        Destroy(gameObject);
        
    }
}