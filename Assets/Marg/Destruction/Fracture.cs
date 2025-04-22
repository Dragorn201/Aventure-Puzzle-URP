using UnityEngine;

public class Fracture : MonoBehaviour
{
    public GameObject fracturedPrefab;       // Le prefab avec les morceaux fractur�s
    public string triggerTag = "Player";     // Tag qui d�clenche la fracture

    public float explosionForce = 500f;
    public float explosionRadius = 2f;

    private bool hasFractured = false;

    private void Start()
    {
        // Pas besoin de d�sactiver le prefab dans la sc�ne, il n'est pas encore instanci�
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasFractured) return;

        if (collision.gameObject.CompareTag(triggerTag))
        {
            hasFractured = true;

            // Instancier le prefab � la bonne position, avec une rotation neutre
            GameObject fracturedInstance = Instantiate(
                fracturedPrefab,
                transform.position,
                Quaternion.identity // On n�applique PAS la rotation de l�objet actuel
            );

            // Appliquer une force d�explosion sur tous les rigidbodies enfants
            foreach (Rigidbody rb in fracturedInstance.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Activer le GameObject fractur� apr�s la collision (initialement d�sactiv� dans le prefab)
            fracturedInstance.SetActive(true);

            // D�truire l'objet d'origine (le mesh non fractur�)
            Destroy(gameObject);
        }
    }
}
