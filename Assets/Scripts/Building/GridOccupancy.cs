
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridOccupancy : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collideTilemap;

    private Dictionary<Vector3Int, PlaceableFacility> occupiedCells =
        new Dictionary<Vector3Int, PlaceableFacility>();

    private void Start()
    {
        // Register facilities already present in the scene.
        PlaceableFacility[] facilities =
            FindObjectsByType<PlaceableFacility>(FindObjectsSortMode.None);

        foreach (PlaceableFacility facility in facilities)
        {
            Vector3Int cell = grid.WorldToCell(facility.transform.position);

            facility.SetGridPosition(cell);
            Register(facility, cell);
        }
    }

    public bool CanPlace(
        PlaceableFacility facility,
        Vector3Int origin)
    {
        Vector2Int size = facility.Footprint;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = origin + new Vector3Int(x, y, 0);

                // Must be on the factory floor.
                if (!groundTilemap.HasTile(cell))
                    return false;

                // Cannot overlap wall tiles.
                if (collideTilemap.HasTile(cell))
                    return false;

                // Cannot overlap other facilities.
                if (occupiedCells.TryGetValue(cell, out var occupant)
                    && occupant != facility)
                    return false;
            }
        }

        return true;
    }

    public void Register(
        PlaceableFacility facility,
        Vector3Int origin)
    {
        for (int x = 0; x < facility.Footprint.x; x++)
        {
            for (int y = 0; y < facility.Footprint.y; y++)
            {
                Vector3Int cell = origin + new Vector3Int(x, y, 0);

                occupiedCells[cell] = facility;
            }
        }

        facility.SetGridPosition(origin);
    }

    public PlaceableFacility GetFacilityAt(Vector3Int cell)
{
    occupiedCells.TryGetValue(cell, out var facility);
    return facility;
}

    public void Unregister(PlaceableFacility facility)
    {
        List<Vector3Int> toRemove = new List<Vector3Int>();

        foreach (var pair in occupiedCells)
        {
            if (pair.Value == facility)
                toRemove.Add(pair.Key);
        }

        foreach (Vector3Int cell in toRemove)
            occupiedCells.Remove(cell);
    }
}
