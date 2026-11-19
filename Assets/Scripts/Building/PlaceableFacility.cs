
using UnityEngine;

public class PlaceableFacility : MonoBehaviour
{
    [SerializeField] private Vector2Int footprint = Vector2Int.one;

    public Vector2Int Footprint => footprint;

    // The bottom-left grid cell occupied by this facility.
    public Vector3Int GridPosition { get; private set; }

    public void SetGridPosition(Vector3Int cell)
    {
        GridPosition = cell;
    }

    private void OnValidate()
    {
        footprint.x = Mathf.Max(1, footprint.x);
        footprint.y = Mathf.Max(1, footprint.y);
    }
}
