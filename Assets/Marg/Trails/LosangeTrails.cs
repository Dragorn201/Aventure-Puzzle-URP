using System.Collections.Generic;
using UnityEngine;

public class LosangeTrails : MonoBehaviour
{
    [Header("Losanges")]
    public GameObject losangePrefab;
    public int losangeCount = 3;
    public float trailLength = 5f;
    public float spawnDistance = 2f;

    [Header("Fade")]
    public float fadeDuration = 1f;

    [Header("Rotation")]
    public bool followCharacterRotation = false;

    [Header("Taille")]
    public float minScale = 0.5f;
    public float maxScale = 1.5f;

    private List<Vector3> trailPositions = new List<Vector3>();
    private List<Diamond> diamonds = new List<Diamond>();
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;

        // Crée les losanges
        for (int i = 0; i < losangeCount; i++)
        {
            GameObject diamondObj = Instantiate(losangePrefab, transform.position, Quaternion.identity);
            diamondObj.SetActive(false);
            Diamond d = new Diamond(diamondObj, this);
            diamonds.Add(d);
        }
    }

    void Update()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved >= spawnDistance)
        {
            trailPositions.Add(transform.position);
            lastPosition = transform.position;

            // Limite la mémoire du trail
            float totalTrailLength = 0f;
            for (int i = trailPositions.Count - 1; i > 0; i--)
            {
                totalTrailLength += Vector3.Distance(trailPositions[i], trailPositions[i - 1]);
                if (totalTrailLength > trailLength)
                {
                    trailPositions.RemoveRange(0, i - 1);
                    break;
                }
            }

            UpdateDiamonds();
        }

        // Met à jour les fades
        foreach (var diamond in diamonds)
        {
            diamond.UpdateFade(fadeDuration);
        }
    }

    void UpdateDiamonds()
    {
        if (trailPositions.Count < 2) return;

        for (int i = 0; i < diamonds.Count; i++)
        {
            float t = (float)i / (diamonds.Count - 1);
            Vector3 position = GetPositionAtT(t);
            diamonds[i].SetPositionAndRotation(position);
        }
    }

    Vector3 GetPositionAtT(float t)
    {
        float totalLength = 0f;
        for (int i = 1; i < trailPositions.Count; i++)
        {
            totalLength += Vector3.Distance(trailPositions[i - 1], trailPositions[i]);
        }

        float targetLength = t * totalLength;
        float accumulatedLength = 0f;

        for (int i = 1; i < trailPositions.Count; i++)
        {
            float segmentLength = Vector3.Distance(trailPositions[i - 1], trailPositions[i]);
            if (accumulatedLength + segmentLength >= targetLength)
            {
                float segmentT = (targetLength - accumulatedLength) / segmentLength;
                return Vector3.Lerp(trailPositions[i - 1], trailPositions[i], segmentT);
            }
            accumulatedLength += segmentLength;
        }

        return trailPositions[trailPositions.Count - 1];
    }

    private class Diamond
    {
        public GameObject obj;
        private SpriteRenderer renderer;
        private LosangeTrails manager;
        private float lifeTimer = 0f;
        private bool isActive = false;
        private Vector3 initialScale;

        public Diamond(GameObject obj, LosangeTrails manager)
        {
            this.obj = obj;
            this.manager = manager;
            renderer = obj.GetComponent<SpriteRenderer>();
        }

        public void SetPositionAndRotation(Vector3 position)
        {
            if (!isActive)
            {
                obj.SetActive(true);
                lifeTimer = 0f;
                isActive = true;

                // Donne une taille aléatoire au moment de l'activation
                float scale = Random.Range(manager.minScale, manager.maxScale);
                initialScale = new Vector3(scale, scale, scale);
                obj.transform.localScale = initialScale;
            }

            obj.transform.position = position;

            if (manager.followCharacterRotation)
            {
                obj.transform.rotation = manager.transform.rotation;
            }
            else
            {
                obj.transform.rotation = Quaternion.identity;
            }
        }

        public void UpdateFade(float fadeDuration)
        {
            if (!isActive) return;

            lifeTimer += Time.deltaTime;
            if (renderer != null)
            {
                float alpha = Mathf.Clamp01(1f - (lifeTimer / fadeDuration));
                Color color = renderer.color;
                color.a = alpha;
                renderer.color = color;
            }

            // Désactive quand totalement transparent
            if (lifeTimer >= fadeDuration)
            {
                obj.SetActive(false);
                isActive = false;
            }
        }
    }
}
