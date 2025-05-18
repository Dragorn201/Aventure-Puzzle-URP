using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualObstacleRemover : MonoBehaviour
{
    private Transform playerTransform;
    private RaycastHit[] obstacles;

    private Dictionary<Renderer, Coroutine> activeFades = new();
    private HashSet<Renderer> currentlyTransparent = new();

    public float fadeDuration = 0.5f;
    public float targetAlpha = 0.2f;

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        HashSet<Renderer> detectedThisFrame = new();

        Vector3 playerPosOffset = playerTransform.position + Vector3.up * 0.5f;
        Vector3 direction = playerPosOffset - transform.position;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        obstacles = Physics.RaycastAll(transform.position, direction, distance);

        foreach (RaycastHit hit in obstacles)
        {
            GameObject obj = hit.collider.gameObject;
            if (obj.CompareTag("Player")) continue;

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                detectedThisFrame.Add(rend);

                if (!currentlyTransparent.Contains(rend))
                {
                    StartFade(rend, targetAlpha);
                    currentlyTransparent.Add(rend);
                }
            }
        }

        List<Renderer> toRemove = new();
        foreach (Renderer rend in currentlyTransparent)
        {
            if (!detectedThisFrame.Contains(rend))
            {
                StartFade(rend, 1f);
                toRemove.Add(rend);
            }
        }

        foreach (Renderer rend in toRemove)
        {
            currentlyTransparent.Remove(rend);
        }
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
        Material mat = rend.material;

        // Vérifie si le shader a bien la propriété "_Color"
        if (!mat.HasProperty("_Color"))
        {
            activeFades.Remove(rend);
            yield break;
        }

        Color color = mat.color;
        float startAlpha = color.a;

        if (targetAlpha < 1f)
            SetMaterialTransparent(mat);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            mat.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        mat.color = new Color(color.r, color.g, color.b, targetAlpha);

        if (targetAlpha >= 1f)
            SetMaterialOpaque(mat);

        activeFades.Remove(rend);
    }

    private void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Surface", 1);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    private void SetMaterialOpaque(Material mat)
    {
        mat.SetFloat("_Surface", 0);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;
    }
}
