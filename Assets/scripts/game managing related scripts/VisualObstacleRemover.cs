using UnityEngine;
using System.Collections.Generic;

public class VisualObstacleRemover : MonoBehaviour
{
    private Transform playerTransform;
    private RaycastHit[] obstacles;

    // On garde une liste des objets qu'on a rendus transparents
    private List<Renderer> transparentRenderers = new List<Renderer>();

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        ResetTransparency();
        Vector3 playerPosOffset = new Vector3(playerTransform.position.x, playerTransform.position.y + 0.5f, playerTransform.position.z);
        Vector3 direction = playerPosOffset - transform.position;
        obstacles = Physics.RaycastAll(transform.position, direction, Vector3.Distance(transform.position, playerTransform.position));

        foreach (RaycastHit hit in obstacles)
        {
            GameObject obj = hit.collider.gameObject;


            if (obj.CompareTag("Player")) continue;

            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                MakeTransparent(rend);
                transparentRenderers.Add(rend);
            }
        }
    }

    private void MakeTransparent(Renderer rend)
    {
        rend.enabled = false;
    }

    private void ResetTransparency()
    {
        foreach (Renderer rend in transparentRenderers)
        {
            rend.enabled = true;
        }

        transparentRenderers.Clear();
    }
}
