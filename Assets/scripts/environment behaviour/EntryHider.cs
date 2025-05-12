using UnityEngine;

public class EntryHider : MonoBehaviour
{
    public GameObject entry;

    void OnTriggerEnter(Collider other)
    {
        entry.SetActive(!entry.activeSelf);
    }
}
