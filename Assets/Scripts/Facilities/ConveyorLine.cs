
using System.Collections.Generic;
using UnityEngine;

public class ConveyorLine
{
    public readonly List<Conveyor> Belts = new();
    public readonly List<ConveyorItem> Items = new();

    public float Speed { get; set; } = 1f;
    public float ItemSpacing { get; set; } = 0.8f;

    public int Capacity => Belts.Count;

    public float EndDistance =>
        Mathf.Max(0, Belts.Count - 1);

    public ConveyorLine(List<Conveyor> belts)
    {
        Belts.AddRange(belts);

        foreach (Conveyor belt in Belts)
            belt.SetLine(this);
    }

    public float GetDistanceAt(Conveyor conveyor)
    {
        return Belts.IndexOf(conveyor);
    }

    public bool TryAddItem(Item item, Conveyor conveyor)
    {
        if (item == null || conveyor == null)
            return false;

        if (Items.Count >= Capacity)
            return false;

        int index = Belts.IndexOf(conveyor);

        if (index < 0)
            return false;

        float distance = index;

        foreach (ConveyorItem existing in Items)
        {
            if (Mathf.Abs(existing.Distance - distance)
                < ItemSpacing)
                return false;
        }

        Items.Add(new ConveyorItem(item, distance));

        item.transform.position =
            GetWorldPosition(distance);

        return true;
    }

    public bool TryTakeItem(
        Conveyor conveyor,
        out Item item)
    {
        item = null;

        if (conveyor == null)
            return false;

        int index = Belts.IndexOf(conveyor);

        if (index < 0)
            return false;

        // Find the closest plate to this conveyor.
        ConveyorItem closest = null;
        float closestDistance = 0.5f;

        foreach (ConveyorItem existing in Items)
        {
            float distance = Mathf.Abs(
                existing.Distance - index
            );

            if (distance < closestDistance)
            {
                closest = existing;
                closestDistance = distance;
            }
        }

        if (closest == null)
            return false;

        Items.Remove(closest);
        item = closest.Item;

        return true;
    }

    public void Update(float deltaTime)
    {
        Items.Sort((a, b) =>
            b.Distance.CompareTo(a.Distance));

        float nextLimit = EndDistance;

        foreach (ConveyorItem conveyorItem in Items)
        {
            float desired =
                conveyorItem.Distance + Speed * deltaTime;

            conveyorItem.Distance =
                Mathf.Min(desired, nextLimit);

            conveyorItem.Item.transform.position =
                GetWorldPosition(conveyorItem.Distance);

            nextLimit =
                conveyorItem.Distance - ItemSpacing;
        }
    }

    public Vector3 GetWorldPosition(float distance)
    {
        if (Belts.Count == 0)
            return Vector3.zero;

        if (Belts.Count == 1)
            return Belts[0].transform.position;

        float clamped =
            Mathf.Clamp(distance, 0f, EndDistance);

        int index = Mathf.Min(
            Mathf.FloorToInt(clamped),
            Belts.Count - 2
        );

        float t = clamped - index;

        return Vector3.Lerp(
            Belts[index].transform.position,
            Belts[index + 1].transform.position,
            t
        );
    }
}
