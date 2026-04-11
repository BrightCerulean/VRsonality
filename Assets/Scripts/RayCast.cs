using UnityEngine;

public class RayCast : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    public float maxDistance = 50f;
    public GameObject hitDot;
    public MenuController menuController;
    private HoverHighlight currentHighlight;

    void Start()
    {
        mainCamera = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
        }
        else
        {
            Debug.LogError("LineRenderer missing!");
        }
    }

    void Update()
    {
        if (menuController != null)
            maxDistance = menuController.raycastDistance;

        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = mainCamera.transform.forward;

        if (lineRenderer != null && GameManager.Instance != null)
        {
            Color pc = GameManager.Instance.playerColor;
            lineRenderer.startColor = pc;
            lineRenderer.endColor = pc;
        }

        int layerMask = ~LayerMask.GetMask("Character");
        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, maxDistance, layerMask))
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);

            if (hitDot != null)
            {
                hitDot.SetActive(true);
                hitDot.transform.position = hit.point;
            }

            // Debug
            Debug.Log("[RayCast] Hitting: " + hit.collider.gameObject.name);

            // Hover highlight
            HoverHighlight newHighlight = hit.collider.GetComponentInParent<HoverHighlight>();
            Debug.Log("[RayCast] HoverHighlight found: " + (newHighlight != null ? newHighlight.gameObject.name : "NULL"));

            if (currentHighlight != newHighlight)
            {
                Debug.Log("[RayCast] Switching highlight");
                if (currentHighlight != null) currentHighlight.OnHoverExit();
                if (newHighlight != null) newHighlight.OnHoverEnter();
                currentHighlight = newHighlight;
            }

            // Button press
            bool buttonPressed = Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.JoystickButton10);
            Debug.Log("[RayCast] ButtonPressed: " + buttonPressed);

            if (buttonPressed)
            {
                ColorSelector colorSelector = hit.collider.GetComponentInParent<ColorSelector>();
                Debug.Log("[RayCast] ColorSelector found: " + (colorSelector != null ? colorSelector.gameObject.name : "NULL"));
                if (colorSelector != null)
                {
                    colorSelector.OnSelect();
                    return;
                }

                SelectableObject selectable = hit.collider.GetComponentInParent<SelectableObject>();
                if (selectable != null)
                {
                    selectable.OnSelect();
                    return;
                }

                ImageUpload upload = hit.collider.GetComponent<ImageUpload>();
                if (upload != null)
                {
                    upload.OnSelect();
                    return;
                }
            }
        }
        else
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * maxDistance);

            if (hitDot != null) hitDot.SetActive(false);

            if (currentHighlight != null)
            {
                currentHighlight.OnHoverExit();
                currentHighlight = null;
            }
        }

        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown((KeyCode)(360 + i)))
                Debug.Log("JoystickButton" + i + " pressed");
        }
    }
}