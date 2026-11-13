using UnityEngine;

public class PlatePileVisuals : MonoBehaviour
{
    [SerializeField] private GameObject platePrefab;
    [SerializeField] private Transform visualsParent;
    [SerializeField] private PolygonCollider2D spawnArea;
    [SerializeField] private int plateCount = 40;

    private void Start()
    {
        GeneratePile();
    }

    private void GeneratePile()
    {
        Bounds bounds = spawnArea.bounds;
        int spawned = 0;
        int attempts = 0;

        while (spawned < plateCount && attempts < plateCount * 100)
        {
            attempts++;

            Vector2 position = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            if (!spawnArea.OverlapPoint(position))
                continue;

            GameObject plate = Instantiate(
                platePrefab,
                position,
                Quaternion.Euler(0, 0, Random.Range(0f, 360f)),
                visualsParent
            );

            SpriteRenderer sr = plate.GetComponent<SpriteRenderer>();

            if (sr != null)
                sr.sortingOrder = spawned;

            spawned++;
        }
    }
}
