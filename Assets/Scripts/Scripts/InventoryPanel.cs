using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryPanel : MonoBehaviour
{
    [Header("Inventory Panel Canvas")]
    public GameObject inventoryCanvas;

    [Header("Three Tile Slots")]
    public GameObject tile0;
    public GameObject tile1;
    public GameObject tile2;

    [Header("References")]
    public MenuController menuController;
    public SettingsMenuController settingsMenu;
    public Teleporting movementScript;
    public RayCast raycastScript;
    public Transform cameraTransform;
    public float grabOffset = 1.5f;

    private List<GameObject> tiles = new List<GameObject>();
    private int currentTileIndex = 0;
    private bool isOpen = false;
    private GameObject grabbedObject = null;
    private float joystickCooldown = 0f;
    private const float JOYSTICK_DELAY = 0.25f;

    void Start()
    {
        tiles.Add(tile0);
        tiles.Add(tile1);
        tiles.Add(tile2);

        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);
    }

    void Update()
    {
        openCooldown -= Time.deltaTime;
        // Handle destroyed object following camera
        if (grabbedObject != null)
        {
            grabbedObject.transform.position = cameraTransform.position
                + cameraTransform.forward * grabOffset;

            // A key / JoystickButton12 → release to ground
            if (Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("js11"))
                ReleaseGrabbedObject();

            return;
        }

        if (!isOpen) return;
        if (openCooldown > 0f) return;

        HandleNavigation();

        // M / B button → select tile
        if (Input.GetKeyDown(KeyCode.M) || Input.GetButtonDown("js10"))
            SelectCurrentTile();

        // Return → close inventory
        //if (Input.GetKeyDown(KeyCode.Return))
           // ClosePanel(reopenSettings: true);

        // Return → close inventory and reopen settings
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("js7"))
            ClosePanel(reopenSettings: true);
    }

    void HandleNavigation()
    {
        joystickCooldown -= Time.deltaTime;
        if (joystickCooldown > 0f) return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetButtonDown("js0"))
        {
            currentTileIndex = (currentTileIndex - 1 + tiles.Count) % tiles.Count;
            UpdateHighlight();
            joystickCooldown = JOYSTICK_DELAY;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetButtonDown("js3"))
        {
            currentTileIndex = (currentTileIndex + 1) % tiles.Count;
            UpdateHighlight();
            joystickCooldown = JOYSTICK_DELAY;
        }
    }

    private float openCooldown = 0f;
    private const float OPEN_DELAY = 0.2f;

    public void OpenPanel()
    {
        isOpen = true;
        currentTileIndex = 0;
        openCooldown = OPEN_DELAY;

        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(true);

        RefreshTiles();
        UpdateHighlight();
        Debug.Log("[Inventory] Panel opened");
    }

    void ClosePanel(bool reopenSettings)
    {
        isOpen = false;

        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);

        if (reopenSettings && settingsMenu != null)
            settingsMenu.OpenSettings();

        Debug.Log("[Inventory] Panel closed");
    }

 /* void RefreshTiles()   
     {
         List<GameObject> inv = menuController.GetInventory();

         for (int i = 0; i < tiles.Count; i++)
         {
             if (tiles[i] == null) continue;

             TextMeshProUGUI label = tiles[i].GetComponentInChildren<TextMeshProUGUI>();
             Image img = tiles[i].GetComponent<Image>();

             if (i < inv.Count && inv[i] != null)
             {
                 tiles[i].SetActive(true);
                 if (label != null) label.text = inv[i].name;
                 if (img != null) img.color = Color.white;
             }
             else
             {
                 tiles[i].SetActive(true);
                 if (label != null) label.text = "Empty";
                 if (img != null) img.color = new Color(0.3f, 0.3f, 0.3f);
             }
         }
     }*/

    void RefreshTiles()
    {
        List<GameObject> inv = menuController.GetInventory();

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] == null) continue;

            TextMeshProUGUI label = tiles[i].GetComponentInChildren<TextMeshProUGUI>();
            Image img = tiles[i].GetComponent<Image>();

            if (i < inv.Count && inv[i] != null)
            {
                tiles[i].SetActive(true);
                if (label != null) label.text = inv[i].name;

                // Get object's actual color and apply to tile
                if (img != null)
                {
                    Renderer rend = inv[i].GetComponent<Renderer>();
                    if (rend != null)
                        img.color = rend.material.color;
                    else
                        img.color = Color.white;
                }
            }
            else
            {
                tiles[i].SetActive(true);
                if (label != null) label.text = "Empty";
                if (img != null) img.color = new Color(0.3f, 0.3f, 0.3f);
            }
        }
    }


    void SelectCurrentTile()
    {
        List<GameObject> inv = menuController.GetInventory();

        if (currentTileIndex >= inv.Count || inv[currentTileIndex] == null)
        {
            Debug.Log("[Inventory] Empty tile selected");
            return;
        }

        grabbedObject = inv[currentTileIndex];
        menuController.RemoveFromInventory(currentTileIndex);

        // Close panel and settings
        ClosePanel(reopenSettings: false);
        if (settingsMenu != null)
            settingsMenu.CloseSettings();

        // Re-enable movement
        if (movementScript != null) movementScript.enabled = true;

        // Disable physics while held
        grabbedObject.SetActive(true);
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("[Inventory] Grabbed: " + grabbedObject.name);
    }

    void ReleaseGrabbedObject()
    {
        if (grabbedObject == null) return;

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        grabbedObject = null;

        if (raycastScript != null) raycastScript.enabled = true;

        Debug.Log("[Inventory] Object released");
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] == null) continue;
            Image img = tiles[i].GetComponent<Image>();
            if (img != null)
                img.color = (i == currentTileIndex) ? Color.yellow : Color.white;
        }
    }
}