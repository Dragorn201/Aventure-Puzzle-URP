using System.Collections.Generic;
using UnityEngine;

public class LosangeTrails : MonoBehaviour
{
    [Header("Losanges")]
    public GameObject losangePrefab;           // Prefab du losange
    public float trailLength = 5f;             // Longueur maximum du trail
    public float spawnDistance = 1.5f;        // Distance entre chaque losange

    [Header("Fade")]
    public float fadeDuration = 1f;            // Temps avant disparition des losanges

    [Header("Rotation")]
    public bool suivreRotationPersonnage = false; // Si true, les losanges suivent la rotation du personnage

    [Header("Taille")]
    public float minScale = 0.5f;
    public float maxScale = 1.5f;

    private List<Vector3> trailPositions = new List<Vector3>();
    private List<LosangeInstance> losanges = new List<LosangeInstance>();
    private Vector3 lastPosition;
    private float distanceParcourue = 0f;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // Calcul de la distance parcourue
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        distanceParcourue += distanceMoved;

        // Si la distance parcourue est suffisante, instancier les losanges
        if (distanceParcourue >= spawnDistance)
        {
            trailPositions.Add(transform.position);
            lastPosition = transform.position;

            // Supprimer les anciennes positions si le trail devient trop long
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

            MettreAJourLesLosanges();

            // Réinitialiser la distance parcourue après avoir généré les losanges
            distanceParcourue = 0f;
        }

        // Fade progressif des losanges
        foreach (var losange in losanges)
        {
            losange.MettreAJourFade(fadeDuration);
        }
    }

    void MettreAJourLesLosanges()
    {
        // Calcul de la distance totale parcourue sur le trail
        float longueur = GetLongueurTotale();

        // Calcul du nombre de losanges en fonction de la distance parcourue
        int losangeRequis = Mathf.FloorToInt(longueur / spawnDistance);

        // Limiter le nombre de losanges à afficher (maximum de 4)
        losangeRequis = Mathf.Clamp(losangeRequis, 1, 4);

        // Instancier des losanges si nécessaire
        while (losanges.Count < losangeRequis)
        {
            GameObject obj = Instantiate(losangePrefab, transform.position, Quaternion.identity);
            obj.SetActive(false);  // Désactiver les losanges au départ
            losanges.Add(new LosangeInstance(obj, this));
        }

        // Mettre à jour la position et l'activation des losanges
        for (int i = 0; i < losanges.Count; i++)
        {
            if (i < losangeRequis)
            {
                float t = (float)i / (losangeRequis - 1);
                Vector3 position = GetPositionAtT(t);
                losanges[i].MettrePosition(position);
            }
            else
            {
                losanges[i].Desactiver();
            }
        }
    }

    float GetLongueurTotale()
    {
        float total = 0f;
        for (int i = 1; i < trailPositions.Count; i++)
        {
            total += Vector3.Distance(trailPositions[i - 1], trailPositions[i]);
        }
        return total;
    }

    Vector3 GetPositionAtT(float t)
    {
        float totalLength = GetLongueurTotale();
        float targetLength = t * totalLength;
        float accumulated = 0f;

        for (int i = 1; i < trailPositions.Count; i++)
        {
            float segment = Vector3.Distance(trailPositions[i - 1], trailPositions[i]);
            if (accumulated + segment >= targetLength)
            {
                float localT = (targetLength - accumulated) / segment;
                return Vector3.Lerp(trailPositions[i - 1], trailPositions[i], localT);
            }
            accumulated += segment;
        }

        return trailPositions[trailPositions.Count - 1];
    }

    private class LosangeInstance
    {
        public GameObject obj;
        private SpriteRenderer renderer;
        private LosangeTrails manager;
        private float lifeTimer = 0f;
        private bool actif = false;
        private Vector3 echelle;

        public LosangeInstance(GameObject obj, LosangeTrails manager)
        {
            this.obj = obj;
            this.manager = manager;
            renderer = obj.GetComponent<SpriteRenderer>();
        }

        public void MettrePosition(Vector3 position)
        {
            if (!actif)
            {
                obj.SetActive(true);
                actif = true;
                lifeTimer = 0f;

                // Appliquer une échelle aléatoire au losange
                float scale = Random.Range(manager.minScale, manager.maxScale);
                echelle = new Vector3(scale, scale, scale);
                obj.transform.localScale = echelle;
            }

            obj.transform.position = position;

            // Orienter les losanges face à la caméra ou selon la rotation du joueur
            if (manager.suivreRotationPersonnage)
                obj.transform.rotation = manager.transform.rotation;
            else
                obj.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        public void MettreAJourFade(float fadeDuration)
        {
            if (!actif) return;

            lifeTimer += Time.deltaTime;

            if (renderer != null)
            {
                float alpha = Mathf.Clamp01(1f - (lifeTimer / fadeDuration));
                Color c = renderer.color;
                c.a = alpha;
                renderer.color = c;
            }

            if (lifeTimer > fadeDuration)
                Desactiver();
        }

        public void Desactiver()
        {
            if (obj != null)
                obj.SetActive(false);
            actif = false;
        }
    }
}
