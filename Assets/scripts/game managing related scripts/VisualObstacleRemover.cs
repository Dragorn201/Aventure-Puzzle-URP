using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualObstacleRemover : MonoBehaviour
{
    private Transform playerTransform;
    private RaycastHit[] obstacles;

    private Dictionary<Renderer, Coroutine> activeFades = new();
    private List<Renderer> currentTransparentRenderers = new();

    public float fadeDuration = 0.5f;
    public float targetAlpha = 0.2f;

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
            if (rend != null && !currentTransparentRenderers.Contains(rend))
            {
                StartFade(rend, targetAlpha);
                currentTransparentRenderers.Add(rend);
            }
        }
    }

    private void ResetTransparency()
    {
        foreach (Renderer rend in currentTransparentRenderers)
        {
            StartFade(rend, 1f);
        }
        currentTransparentRenderers.Clear();
    }

    private void StartFade(Renderer rend, float targetAlpha)
    {
        if (activeFades.TryGetValue(rend, out Coroutine current))
        {
            StopCoroutine(current);
        }
        activeFades[rend] = StartCoroutine(FadeMaterial(rend, targetAlpha));
    }

    private IEnumerator FadeMaterial(Renderer rend, float targetAlpha)
    {
        Material mat = rend.material; // Use instance of material
        Color color = mat.color;
        float startAlpha = color.a;

        for (float t = 0; t < fadeDuration; t += Time.fixedDeltaTime)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            mat.color = new Color(color.r, color.g, color.b, alpha);
            yield return new WaitForFixedUpdate();
        }

        mat.color = new Color(color.r, color.g, color.b, targetAlpha);
        activeFades.Remove(rend);
    }
}
