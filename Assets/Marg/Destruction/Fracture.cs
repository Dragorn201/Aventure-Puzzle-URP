using UnityEngine;

public class Fracture : MonoBehaviour
{
    public GameObject fracturedPrefab;       
    public string triggerTag = "Player";     

    public float explosionForce = 500f;
    public float explosionRadius = 2f;

    private bool hasFractured = false;

    private void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasFractured) return;

        if (collision.gameObject.CompareTag(triggerTag))
        {
            hasFractured = true;

            
            GameObject fracturedInstance = Instantiate
            (
                fracturedPrefab,
                transform.position,
                Quaternion.identity 
            );
            

            
            foreach (Rigidbody rb in fracturedInstance.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            
            fracturedInstance.SetActive(true);

            
            Destroy(gameObject);
        }
    }
}