
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSystem : MonoBehaviour
{
    [SerializeField] private GridOccupancy occupancy;

    private readonly List<ConveyorLine> lines = new();

    public IReadOnlyList<ConveyorLine> Lines => lines;

    public void RebuildLines()
    {
        // Clear previous line assignments.
        foreach (ConveyorLine line in lines)
        {
            foreach (Conveyor belt in line.Belts)
                if (belt != null)
                    belt.SetLine(null);
        }

        lines.Clear();

        Conveyor[] belts =
            FindObjectsByType<Conveyor>(
                FindObjectsSortMode.None
            );

        HashSet<Conveyor> visited = new();

        // Count incoming connections to find path starts.
        Dictionary<Conveyor, int> incoming = new();

        foreach (Conveyor belt in belts)
            incoming[belt] = 0;

        foreach (Conveyor belt in belts)
        {
            Conveyor next = GetNextBelt(belt);

            if (next != null)
                incoming[next]++;
        }

        foreach (Conveyor belt in belts)
        {
            if (visited.Contains(belt))
                continue;

            // Start with tiles that have no incoming belt.
            if (incoming[belt] != 0)
                continue;

            BuildLine(belt, visited, incoming);
        }

        // Handle isolated tiles and remaining paths.
        foreach (Conveyor belt in belts)
        {
            if (!visited.Contains(belt))
                BuildLine(belt, visited, incoming);
        }
        Debug.Log($"Built {lines.Count} conveyor lines.");

        for (int i = 0; i < lines.Count; i++)
        {
            ConveyorLine line = lines[i];

            Debug.Log(
                $"Line {i}: {line.Belts.Count} belts, " +
                $"Capacity: {line.Capacity}"
            );

            for (int j = 0; j < line.Belts.Count; j++)
            {
                Conveyor belt = line.Belts[j];

                Debug.Log(
                    $"  [{j}] Cell: {belt.GridPosition}, " +
                    $"Direction: {belt.Direction}"
                );
            }
        }
    }

    private void BuildLine(
        Conveyor start,
        HashSet<Conveyor> visited,
        Dictionary<Conveyor, int> incoming)
    {
        List<Conveyor> path = new();

        Conveyor current = start;

        while (current != null && !visited.Contains(current))
        {
            visited.Add(current);
            path.Add(current);

            Conveyor next = GetNextBelt(current);

            // Stop at a merge point.
            if (next != null && incoming[next] > 1)
                break;

            current = next;
        }

        if (path.Count > 0)
            lines.Add(new ConveyorLine(path));
    }

    private Conveyor GetNextBelt(Conveyor belt)
    {
        Vector3Int nextCell = belt.GetOutputCell();

        PlaceableFacility facility =
            occupancy.GetFacilityAt(nextCell);

        if (facility == null)
            return null;

        return facility.GetComponent<Conveyor>();
    }
    public bool TryAddItem(Conveyor conveyor, Item item)
    {
        if (conveyor == null || conveyor.Line == null)
            return false;

        return conveyor.Line.TryAddItem(item, conveyor);
    }
    private void Update()
    {
        foreach (ConveyorLine line in lines)
        {
            line.Update(Time.deltaTime);
        }
    }
}
