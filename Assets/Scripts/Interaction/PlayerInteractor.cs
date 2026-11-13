
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1.2f;
    [SerializeField] private LayerMask interactableLayer;

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            interactionRadius,
            interactableLayer
        );

        IInteractable closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            IInteractable interactable =
                hit.GetComponentInParent<IInteractable>();

            if (interactable == null)
                continue;

            float distance = hit.Distance(
                GetComponent<Collider2D>()
            ).distance;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }

        if (closest != null)
        {
            closest.Interact(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            interactionRadius
        );
    }
}
