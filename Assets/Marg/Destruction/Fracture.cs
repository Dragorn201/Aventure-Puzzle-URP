using UnityEngine;

public class Fracture : MonoBehaviour
{
    public GameObject fracturedPrefab;
    public string triggerTag = "Player";

    public float explosionForce = 500f;
    public float explosionRadius = 2f;

    private bool hasFractured = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasFractured) return;

        if (collision.gameObject.CompareTag(triggerTag))
        {
            hasFractured = true;

            // Créer les morceaux
            GameObject fracturedInstance = Instantiate(fracturedPrefab, transform.position, transform.rotation);
            fracturedInstance.SetActive(true);

            // Explosion au point de contact
            Vector3 explosionPoint = collision.contacts[0].point;

            foreach (Rigidbody rb in fracturedInstance.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, explosionPoint, explosionRadius);
            }

            // Détruire l'objet original (non fracturé)
            Destroy(gameObject);
        }
    }
}
