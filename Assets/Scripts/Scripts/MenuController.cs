/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Raycast")]
    public float raycastDistance = 10f;
    public LayerMask interactableLayer;
    public LayerMask menuButtonLayer;
    public LayerMask groundLayer;

    [Header("Movement Script to disable when menu opens")]
    public MonoBehaviour movementScript;

    [Header("Inventory Full Message")]
    public TextMeshProUGUI inventoryFullText;

    private ObjectMenu currentOpenMenu;
    private GameObject hoveredMenuButton;
    private List<GameObject> inventory = new List<GameObject>();
    private const int MAX_INVENTORY = 3;
    private GameObject lastDestroyedObject;

    void Start()
    {
        if (inventoryFullText != null)
            inventoryFullText.gameObject.SetActive(false);
    }

    void Update()
    {
        DoRaycast();
        HandleInput();
    }

// Raycast forward to detect menus and buttons
    void DoRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (currentOpenMenu != null)
        {
            if (Physics.Raycast(ray, out hit, raycastDistance, menuButtonLayer))
            {
                GameObject hitObj = hit.collider.gameObject;
                hoveredMenuButton = hitObj;
                currentOpenMenu.HighlightButton(hitObj);
            }
            else
            {
                hoveredMenuButton = null;
                currentOpenMenu.ClearHighlight();
            }
        }
    }


// Handle input for opening menus and executing actions
    void HandleInput()
    {
        // X button → open menu
        if (Input.GetKeyDown(KeyCode.JoystickButton11))
        {
            Debug.Log("[MenuController] X button pressed");
            TryOpenMenu();
        }

        // O key → open menu (keyboard debug shortcut)
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("[MenuController] O key pressed");
            TryOpenMenu();
        }

        // B button → confirm action
        if (Input.GetKeyDown(KeyCode.JoystickButton10))
        {
            Debug.Log("[MenuController] B button pressed");
            TryExecuteAction();
        }

        // M key → confirm action (keyboard debug shortcut)
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("[MenuController] M key pressed");
            TryExecuteAction();
        }

        // A button → spawn last destroyed
        if (Input.GetKeyDown(KeyCode.JoystickButton12))
        {
            Debug.Log("[MenuController] A button pressed");
            TrySpawnLastDestroyed();
        }
        // N key → spawn last destroyed (keyboard debug shortcut)
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("[MenuController] N key pressed");
            TrySpawnLastDestroyed();
        }
    }

    void TryOpenMenu()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.Log("[MenuController] TryOpenMenu called, ray from: " + transform.position + " dir: " + transform.forward);

        if (!Physics.Raycast(ray, out hit, raycastDistance)) //interactableLayer | menuButtonLayer | groundLayer))
        {
            Debug.Log("[MenuController] Raycast missed — not pointing at Interactable layer");
            return;
        }

        Debug.Log("[MenuController] Raycast hit: " + hit.collider.gameObject.name);

        ObjectMenu hitMenu = hit.collider.GetComponent<ObjectMenu>();
        Debug.Log("[MenuController] ObjectMenu found: " + (hitMenu != null));

        if (hitMenu == null) return;

        // Close existing menu first
        if (currentOpenMenu != null && currentOpenMenu != hitMenu)
            CloseCurrentMenu(reenableMovement: false);

        currentOpenMenu = hitMenu;
        hitMenu.ShowMenu();
        DisableMovement();
    }

    void TryExecuteAction()
    {
        if (currentOpenMenu == null)
        {
            Debug.Log("[MenuController] No menu open");
            return;
        }
        if (hoveredMenuButton == null)
        {
            Debug.Log("[MenuController] No button hovered");
            return;
        }

        Debug.Log("[MenuController] Executing: " + hoveredMenuButton.name);

        switch (hoveredMenuButton.name)
        {
            case "DestroyButton": ActionDestroy(); break;
            case "StoreButton":   ActionStore();   break;
            case "ExitButton":    ActionExit();    break;
            default:
                Debug.LogWarning("[MenuController] Unknown button: " + hoveredMenuButton.name);
                break;
        }
    }

    void ActionDestroy()
    {
        GameObject target = currentOpenMenu.gameObject;
        lastDestroyedObject = target;
        CloseCurrentMenu(reenableMovement: true);
        target.SetActive(false);
        Debug.Log("[MenuController] Destroyed: " + target.name);
    }

    void ActionStore()
    {
        if (inventory.Count >= MAX_INVENTORY)
        {
            Debug.Log("[MenuController] Inventory full!");
            StartCoroutine(ShowInventoryFull());
            return;
        }

        GameObject target = currentOpenMenu.gameObject;
        inventory.Add(target);
        CloseCurrentMenu(reenableMovement: true);
        target.SetActive(false);
        Debug.Log("[MenuController] Stored: " + target.name + " | Count: " + inventory.Count);
    }

    void ActionExit()
    {
        CloseCurrentMenu(reenableMovement: true);
        Debug.Log("[MenuController] Exit pressed");
    }

    void TrySpawnLastDestroyed()
    {
        if (lastDestroyedObject == null)
        {
            Debug.Log("[MenuController] Nothing to spawn");
            return;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
    {
        // Reset rigidbody velocity before spawning
        Rigidbody rb = lastDestroyedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Spawn slightly above ground so it lands on top
        lastDestroyedObject.transform.position = hit.point + Vector3.up * 0.5f;
        lastDestroyedObject.SetActive(true);
        Debug.Log("[MenuController] Spawned: " + lastDestroyedObject.name);
        lastDestroyedObject = null;
    }
        else
        {
            Debug.Log("[MenuController] A pressed but raycast missed ground");
        }
    }


//Settings Menu / Inventory Panel 

    public void ForceCloseObjectMenu()
    {
        if (currentOpenMenu != null)
            CloseCurrentMenu(reenableMovement: false);
    }
 
    public void DisableMovementPublic()
    {
        if (movementScript != null) movementScript.enabled = false;
    }
 
    public void EnableMovementPublic()
    {
        if (movementScript != null) movementScript.enabled = true;
    }
 
    public void SetMovementSpeed(float speed)
    {
        // Try common speed property names on the movement script
        if (movementScript == null) return;
 
        System.Type t = movementScript.GetType();
 
        var speedProp = t.GetProperty("speed") ?? t.GetProperty("Speed") ?? t.GetProperty("moveSpeed") ?? t.GetProperty("MoveSpeed");
        if (speedProp != null) { speedProp.SetValue(movementScript, speed); return; }
 
        var speedField = t.GetField("speed") ?? t.GetField("Speed") ?? t.GetField("moveSpeed") ?? t.GetField("MoveSpeed");
        if (speedField != null) { speedField.SetValue(movementScript, speed); return; }
 
        Debug.LogWarning("[MenuController] Could not find speed property on movement script: " + t.Name);
    }
 
    public List<GameObject> GetInventory() => inventory;
 
    public void RemoveFromInventory(GameObject obj)
    {
        inventory.Remove(obj);
        Debug.Log("[MenuController] Removed from inventory: " + obj.name + " | Count: " + inventory.Count);
    }
    void CloseCurrentMenu(bool reenableMovement)
    {
        if (currentOpenMenu != null)
        {
            currentOpenMenu.HideMenu();
            currentOpenMenu = null;
        }
        hoveredMenuButton = null;
        if (reenableMovement) EnableMovement();
    }

    void DisableMovement()
    {
        if (movementScript != null) movementScript.enabled = false;
    }

    void EnableMovement()
    {
        if (movementScript != null) movementScript.enabled = true;
    }

    IEnumerator ShowInventoryFull()
    {
        if (inventoryFullText != null)
        {
            inventoryFullText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            inventoryFullText.gameObject.SetActive(false);
        }
    }
}*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Raycast")]
    public float raycastDistance = 50f;
    public LayerMask interactableLayer;
    public LayerMask menuButtonLayer;
    public LayerMask groundLayer;

    [Header("Movement Script to disable when menu opens")]
    public MonoBehaviour movementScript;

    [Header("Inventory Full Message")]
    public TextMeshProUGUI inventoryFullText;

    [Header("Settings Menu Reference")]
    public SettingsMenuController settingsMenuController;

    private ObjectMenu currentOpenMenu;
    private GameObject hoveredMenuButton;
    private List<GameObject> inventory = new List<GameObject>();
    private const int MAX_INVENTORY = 3;
    private GameObject lastDestroyedObject;


    void Start()
    {
        if (inventoryFullText != null)
            inventoryFullText.gameObject.SetActive(false);
    }

    void Update()
    {
        DoRaycast();
        HandleInput();
    }


    void DoRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (currentOpenMenu != null)
        {
            Debug.Log("[DoRaycast] Menu is open, casting ray...");
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                Debug.Log("[DoRaycast] Hit: " + hit.collider.gameObject.name);
                GameObject hitObj = hit.collider.gameObject;
                hoveredMenuButton = hitObj;
                currentOpenMenu.HighlightButton(hitObj);
            }
            else
            {
                Debug.Log("[DoRaycast] Hit nothing");
                hoveredMenuButton = null;
                currentOpenMenu.ClearHighlight();
            }
        }
    }


    /*void HandleInput()
    {
        // OK button → open menu
        if (Input.GetButtonDown("js7"))
        {
            Debug.Log("[MenuController] OK button pressed");
            TryOpenMenu();
        }

        // O key → open menu (keyboard debug shortcut)
        if (Input.GetKeyDown(KeyCode.O) || Input.GetKey(KeyCode.Return))
        {
            Debug.Log("[MenuController] O key pressed");
            TryOpenMenu();
        }

        // A button → confirm action
        if (Input.GetButtonDown("js11"))
        {
            Debug.Log("[MenuController] A button pressed");
            TryExecuteAction();
        }

        // M key → confirm action (keyboard debug shortcut)
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("[MenuController] M key pressed");
            TryExecuteAction();
        }

        // B button → spawn last destroyed
        if (Input.GetButtonDown("js10"))
        {
            Debug.Log("[MenuController] B button pressed");
            TrySpawnLastDestroyed();
        }

        // N key → spawn last destroyed (keyboard debug shortcut)
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("[MenuController] N key pressed");
            TrySpawnLastDestroyed();
        }
    }*/

    // Trying to use OK / Return as context-sensitive button for both menu and settings by checking raycast first for menu, then fallback to settings if no menu hit.
    /*void HandleInput()
    {
        // Y button / O key → context sensitive (Object Menu or Teleport)
        if (Input.GetButtonDown("js5") || Input.GetKeyDown(KeyCode.O))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                ObjectMenu hitMenu = hit.collider.GetComponent<ObjectMenu>();
                if (hitMenu != null)
                {
                    Debug.Log("[MenuController] Y → Object Menu");
                    TryOpenMenu();
                    return;
                }
            }
            // Not pointing at object → let Teleporting.cs handle teleport
        }

        // OK / Return → Settings Menu only
        if (Input.GetButtonDown("js7") )
        {
            Debug.Log("[MenuController] OK → Settings");
            if (settingsMenuController != null)
            {
                if (settingsMenuController.IsOpen()) settingsMenuController.CloseSettings();
                else settingsMenuController.OpenSettings();
            }
        }
*/
    /* // B / M → context sensitive (Open Object Menu or Confirm action)


    if (Input.GetButtonDown("js10") || Input.GetKeyDown(KeyCode.M))
    {
        if (currentOpenMenu != null)
        {
            Debug.Log("[MenuController] B → Confirm action");
            TryExecuteAction();
        }
        else
        {
            Debug.Log("[MenuController] B → Open Object Menu");
            TryOpenMenu();
        }
    }

    // B / M → confirm Object Menu action only
    if (Input.GetButtonDown("js10") || Input.GetKeyDown(KeyCode.M))
    {
        Debug.Log("[MenuController] B → Confirm action");
        TryExecuteAction();
    }

    // A / N → spawn last destroyed
    if (Input.GetButtonDown("js11") || Input.GetKeyDown(KeyCode.N))
        TrySpawnLastDestroyed();
}*/

    // OK / Return → toggle Settings Menu if not pointing at menu, otherwise open Object Menu
    void HandleInput()
    {
        // Y button / O key → context sensitive
        if (Input.GetButtonDown("js5") || Input.GetKeyDown(KeyCode.O))
         {
             Ray ray = new Ray(transform.position, transform.forward);
             RaycastHit hit;

             if (Physics.Raycast(ray, out hit, raycastDistance))
             {
                 ObjectMenu hitMenu = hit.collider.GetComponent<ObjectMenu>();
                 if (hitMenu != null)
                 {
                     Debug.Log("[MenuController] Y → Object Menu");
                     TryOpenMenu();
                     return;
                 }
             }
         }
        
        if (Input.GetButtonDown("js7") || Input.GetKeyDown(KeyCode.Return))
        {
            if (settingsMenuController != null)
            {
                if (settingsMenuController.IsOpen()) settingsMenuController.CloseSettings();
                else settingsMenuController.OpenSettings();
            }
        }

        // O key → open Object Menu (keyboard debug)
        /*if (Input.GetKeyDown(KeyCode.O))
            TryOpenMenu();*/

        // B / M → confirm Object Menu action
        if (Input.GetButtonDown("js10") || Input.GetKeyDown(KeyCode.M))
            TryExecuteAction();

        // A / N → spawn last destroyed
        if (Input.GetButtonDown("js11") || Input.GetKeyDown(KeyCode.N))
            TrySpawnLastDestroyed();
    }

    void TryOpenMenu()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.Log("[MenuController] TryOpenMenu called, ray from: " + transform.position + " dir: " + transform.forward);

        if (!Physics.Raycast(ray, out hit, raycastDistance))
        {
            Debug.Log("[MenuController] Raycast missed");
            return;
        }

        Debug.Log("[MenuController] Raycast hit: " + hit.collider.gameObject.name);

        ObjectMenu hitMenu = hit.collider.GetComponent<ObjectMenu>();
        Debug.Log("[MenuController] ObjectMenu found: " + (hitMenu != null));

        if (hitMenu == null) return;

        if (currentOpenMenu != null && currentOpenMenu != hitMenu)
            CloseCurrentMenu(reenableMovement: false);

        currentOpenMenu = hitMenu;
        hitMenu.ShowMenu();
        DisableMovement();
    }

    void TryExecuteAction()
    {
        if (currentOpenMenu == null)
        {
            Debug.Log("[MenuController] No menu open");
            return;
        }
        if (hoveredMenuButton == null)
        {
            Debug.Log("[MenuController] No button hovered");
            return;
        }

        Debug.Log("[MenuController] Executing: " + hoveredMenuButton.name);

        switch (hoveredMenuButton.name)
        {
            case "DestroyButton": ActionDestroy(); break;
            case "StoreButton": ActionStore(); break;
            case "ExitButton": ActionExit(); break;
            default:
                Debug.LogWarning("[MenuController] Unknown button: " + hoveredMenuButton.name);
                break;
        }
    }

    void ActionDestroy()
    {
        GameObject target = currentOpenMenu.gameObject;
        lastDestroyedObject = target;
        CloseCurrentMenu(reenableMovement: true);
        target.SetActive(false);
        Debug.Log("[MenuController] Destroyed: " + target.name);
    }

    void ActionStore()
    {
        if (inventory.Count >= MAX_INVENTORY)
        {
            Debug.Log("[MenuController] Inventory full!");
            StartCoroutine(ShowInventoryFull());
            return;
        }

        GameObject target = currentOpenMenu.gameObject;
        inventory.Add(target);
        //if (settingsMenuController != null)
        if (settingsMenuController != null) settingsMenuController.UpdateInventoryDisplay();
        {
            settingsMenuController.UpdateInventoryDisplay();
        }
        CloseCurrentMenu(reenableMovement: true);
        target.SetActive(false);
        Debug.Log("[MenuController] Stored: " + target.name + " | Count: " + inventory.Count);
    }

    void ActionExit()
    {
        CloseCurrentMenu(reenableMovement: true);
        Debug.Log("[MenuController] Exit pressed");
    }
    void TrySpawnLastDestroyed()
    {
        if (lastDestroyedObject == null)
        {
            Debug.Log("[MenuController] Nothing to spawn");
            return;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            Rigidbody rb = lastDestroyedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            lastDestroyedObject.transform.position = hit.point + Vector3.up * 0.5f;
            lastDestroyedObject.SetActive(true);
            Debug.Log("[MenuController] Spawned: " + lastDestroyedObject.name);
            lastDestroyedObject = null;
        }
        else
        {
            Debug.Log("[MenuController] Raycast missed ground");
        }
    }
    void CloseCurrentMenu(bool reenableMovement)
    {
        if (currentOpenMenu != null)
        {
            currentOpenMenu.HideMenu();
            currentOpenMenu = null;
        }
        hoveredMenuButton = null;
        if (reenableMovement) EnableMovement();
    }

    void DisableMovement()
    {
        if (movementScript != null) movementScript.enabled = false;
    }

    void EnableMovement()
    {
        if (movementScript != null) movementScript.enabled = true;
    }

    IEnumerator ShowInventoryFull()
    {
        if (inventoryFullText != null)
        {
            inventoryFullText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            inventoryFullText.gameObject.SetActive(false);
        }
    }

    public void ForceCloseMenu()
    {
        CloseCurrentMenu(reenableMovement: false);
    }

    public List<GameObject> GetInventory()
    {
        return inventory;
    }

    public void RemoveFromInventory(int index)
    {
        if (index >= 0 && index < inventory.Count)
        {
            Debug.Log("[MenuController] Removed from inventory: " + inventory[index].name);
            inventory.RemoveAt(index);
        }
    }
}
