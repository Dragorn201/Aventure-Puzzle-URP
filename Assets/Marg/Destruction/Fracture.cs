using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

public class Fracture : MonoBehaviour
{
    public GameObject fractured;
    public string triggerTag = "Projectile";

    private bool hasFractured = false;

    void OnCollisionEnter(Collision collision)
    {
        if (hasFractured) return;


        if (collision.gameObject.CompareTag(triggerTag))
        {
            hasFractured = true;
            Instantiate(fractured, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
