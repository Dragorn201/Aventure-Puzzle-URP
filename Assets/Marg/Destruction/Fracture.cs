using UnityEngine;

public class Fracture : MonoBehaviour
{
    public GameObject fracturedPrefab;       // Le prefab avec les morceaux fracturés
    public string triggerTag = "Player";     // Tag qui déclenche la fracture

    public float explosionForce = 500f;
    public float explosionRadius = 2f;

    private bool hasFractured = false;

    private void Start()
    {
        // Pas besoin de désactiver le prefab dans la scène, il n'est pas encore instancié
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasFractured) return;

        if (collision.gameObject.CompareTag(triggerTag))
        {
            hasFractured = true;

            // Instancier le prefab à la bonne position, avec une rotation neutre
            GameObject fracturedInstance = Instantiate(
                fracturedPrefab,
                transform.position,
                Quaternion.identity // On n’applique PAS la rotation de l’objet actuel
            );

            // Appliquer une force d’explosion sur tous les rigidbodies enfants
            foreach (Rigidbody rb in fracturedInstance.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Activer le GameObject fracturé après la collision (initialement désactivé dans le prefab)
            fracturedInstance.SetActive(true);

            // Détruire l'objet d'origine (le mesh non fracturé)
            Destroy(gameObject);
        }
    }
}
