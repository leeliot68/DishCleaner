
using UnityEngine;
using UnityEngine.InputSystem;

public class GridPlacementManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GridOccupancy occupancy;
    [SerializeField] private PlaceableFacility facilityToMove;
    [SerializeField] private LayerMask facilityLayer;
    [SerializeField] private ConveyorSystem conveyorSystem;

    [Header("Buildable Facilities")]
    [SerializeField] private PlaceableFacility sinkPrefab;
    [SerializeField] private PlaceableFacility sellingStationPrefab;

    [SerializeField] private PlaceableFacility conveyorPrefab;

    private bool buildMode = false;
    private bool isMoving = false;
    private bool isConstructing = false;
    private PlaceableFacility selectedPrefab;

    private SpriteRenderer previewRenderer;
    private GameObject previewObject;
    private Vector3Int previewCell;
    private ConveyorDirection previewDirection;
    private Quaternion previewBaseRotation;


    private void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        // Toggle Build Mode
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            if (isConstructing)
                CancelConstruction();

            if (isMoving)
                CancelMove();

            buildMode = !buildMode;
            Debug.Log("Build Mode: " + buildMode);
        }

        if (!buildMode)
            return;

        // Select facility to construct
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            StartConstruction(sinkPrefab);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            StartConstruction(sellingStationPrefab);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            StartConstruction(conveyorPrefab);

        if ((isConstructing || isMoving) && Keyboard.current.rKey.wasPressedThisFrame)
        {
            RotatePreview();
        }

        // Constructing a new facility
        if (isConstructing)
        {
            UpdatePreview();

            if (Mouse.current.leftButton.wasPressedThisFrame)
                ConfirmConstruction();

            if (Mouse.current.rightButton.wasPressedThisFrame)
                CancelConstruction();

            return;
        }

        // Moving an existing facility
        if (isMoving)
        {
            UpdatePreview();

            if (Mouse.current.leftButton.wasPressedThisFrame)
                ConfirmMove();

            if (Mouse.current.rightButton.wasPressedThisFrame)
                CancelMove();

            return;
        }

        // Select an existing facility
        if (Mouse.current.leftButton.wasPressedThisFrame)
            TrySelectFacility();
    }


    private Vector3Int GetMouseCell()
    {
        Vector2 screen = Mouse.current.position.ReadValue();

        Vector3 world = mainCamera.ScreenToWorldPoint(
            new Vector3(
                screen.x,
                screen.y,
                -mainCamera.transform.position.z
            )
        );

        return grid.WorldToCell(world);
    }

    private void TrySelectFacility()
    {
        Vector2 screen = Mouse.current.position.ReadValue();

        Vector3 world = mainCamera.ScreenToWorldPoint(
            new Vector3(
                screen.x,
                screen.y,
                -mainCamera.transform.position.z
            )
        );

        // Find all facility colliders under the mouse
        Collider2D[] hits = Physics2D.OverlapPointAll(
            world,
            facilityLayer
        );

        foreach (Collider2D hit in hits)
        {
            PlaceableFacility facility =
                hit.GetComponentInParent<PlaceableFacility>();

            if (facility == null)
                continue;

            // Check that the facility has a visual
            SpriteRenderer source =
                facility.GetComponentInChildren<SpriteRenderer>();

            if (source == null)
                continue;

            facilityToMove = facility;
            isMoving = true;

            // Reuse the preview creation method
            CreatePreview(facility);

            Conveyor conveyor = facility.GetComponent<Conveyor>();

            if (conveyor != null)
            {
                previewDirection = conveyor.Direction;
                UpdatePreviewRotation();
            }

            Debug.Log($"Selected facility: {facility.name}");

            return;
        }
    }
    private void StartConstruction(PlaceableFacility prefab)
    {
        if (prefab == null)
            return;

        if (isMoving)
            CancelMove();

        if (isConstructing)
            CancelConstruction();

        selectedPrefab = prefab;
        isConstructing = true;

        CreatePreview(prefab);

        Conveyor conveyor = prefab.GetComponent<Conveyor>();

        if (conveyor != null)
        {
            previewDirection = conveyor.Direction;
            UpdatePreviewRotation();
        }
    }

    private void CreatePreview(PlaceableFacility facility)
    {
        if (previewObject != null)
            Destroy(previewObject);

        SpriteRenderer source =
            facility.GetComponentInChildren<SpriteRenderer>();

        if (source == null)
        {
            Debug.LogWarning("Facility has no SpriteRenderer!");
            return;
        }

        previewObject = new GameObject("PlacementPreview");

        previewRenderer =
            previewObject.AddComponent<SpriteRenderer>();

        // Copy sprite properties
        previewRenderer.sprite = source.sprite;
        previewRenderer.sortingLayerID = source.sortingLayerID;
        previewRenderer.sortingOrder = source.sortingOrder + 10;

        previewRenderer.flipX = source.flipX;
        previewRenderer.flipY = source.flipY;
        previewRenderer.drawMode = source.drawMode;

        // Preserve original visual dimensions
        if (source.drawMode != SpriteDrawMode.Simple)
            previewRenderer.size = source.size;

        // Preserve effective world scale
        previewObject.transform.localScale = source.transform.lossyScale;
    }



    private void ConfirmConstruction()
    {
        if (selectedPrefab == null)
            return;

        if (!occupancy.CanPlace(selectedPrefab, previewCell))
        {
            Debug.Log("Invalid construction position!");
            return;
        }

        PlaceableFacility newFacility =
            Instantiate(
                selectedPrefab,
                previewObject.transform.position,
                Quaternion.identity
            );

        Conveyor conveyor = newFacility.GetComponent<Conveyor>();

        if (conveyor != null)
        {
            conveyor.SetDirection(previewDirection);
        }

        occupancy.Register(newFacility, previewCell);
        if (newFacility.TryGetComponent<Conveyor>(out _))
            conveyorSystem.RebuildLines();

        Debug.Log($"Constructed {newFacility.name}");

        // Keep construction mode active for repeated placement.
    }
    private void UpdatePreview()
    {
        PlaceableFacility facility =
            isConstructing ? selectedPrefab : facilityToMove;

        if (facility == null || previewObject == null)
            return;

        previewCell = GetMouseCell();

        Vector3 center = grid.GetCellCenterWorld(previewCell);

        Vector2Int size = facility.Footprint;

        Vector3 offset = new Vector3(
            (size.x - 1) * grid.cellSize.x * 0.5f,
            (size.y - 1) * grid.cellSize.y * 0.5f,
            0f
        );

        previewObject.transform.position = center + offset;

        bool valid = occupancy.CanPlace(facility, previewCell);

        previewRenderer.color = valid
            ? new Color(0f, 1f, 0f, 0.5f)
            : new Color(1f, 0f, 0f, 0.5f);
    }



    private void ConfirmMove()
    {
        if (facilityToMove == null || previewObject == null)
            return;

        if (!occupancy.CanPlace(facilityToMove, previewCell))
            return;

        occupancy.Unregister(facilityToMove);

        facilityToMove.transform.position =
            previewObject.transform.position;

        Conveyor conveyor =
            facilityToMove.GetComponent<Conveyor>();

        if (conveyor != null)
            conveyor.SetDirection(previewDirection);

        occupancy.Register(facilityToMove, previewCell);

        if (conveyor != null)
            conveyorSystem.RebuildLines();

        FinishMove();
    }


    private void CancelMove()
    {
        FinishMove();
    }


    private void CancelConstruction()
    {
        isConstructing = false;
        selectedPrefab = null;

        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
        previewRenderer = null;
    }


    private void FinishMove()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
        previewRenderer = null;
        facilityToMove = null;
        isMoving = false;
    }

    private void RotatePreview()
    {
        PlaceableFacility facility =
            isConstructing ? selectedPrefab : facilityToMove;

        if (facility == null)
            return;

        Conveyor conveyor =
            facility.GetComponent<Conveyor>();

        if (conveyor == null)
            return;

        previewDirection =
            Conveyor.GetNextDirection(previewDirection);

        UpdatePreviewRotation();
    }

    private void UpdatePreviewRotation()
    {
        if (previewObject == null)
            return;

        float angle = previewDirection switch
        {
            ConveyorDirection.Right => 0f,
            ConveyorDirection.Down => 270f,
            ConveyorDirection.Left => 180f,
            ConveyorDirection.Up => 90f,
            _ => 0f
        };

        previewObject.transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

}
