using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

public class Fracture : MonoBehaviour
{
    public GameObject fractured;
    public string triggerTag = "Projectile";

    public float explosionForce = 500f;
    public float explosionRadius = 2f;

    private bool hasFractured = false;

    private void Start()
    {
        if (fractured != null)
        {
            fractured.SetActive(false);
        }

    }





    void OnCollisionEnter(Collision collision)
    {
        if (hasFractured) return;


        if (collision.gameObject.CompareTag(triggerTag))
        {
            hasFractured = true;

            GameObject fractureObj = Instantiate(fractured, transform.position, transform.rotation);

            foreach (Rigidbody rb in fractureObj.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }


            fractured.SetActive(true);
            Destroy(gameObject);
        }
    }
}
