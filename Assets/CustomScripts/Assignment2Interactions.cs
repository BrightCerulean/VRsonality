using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Assignment2Interactions : NetworkBehaviour
{
    private Camera _cam;
    [Header("Ray Settings")]
    public float rayDistance = 10f;
    public LayerMask interactableLayer;
    public LayerMask floorLayer;
    private LineRenderer rayLine;

    [Header("Character Movement")]
    public CharacterMovement characterMovement;
    public float rotateSpeed = 120f;
    public float teleportHeightOffset = 1.2f;

    [Header("Interactions Menu")]
    public GameObject objectMenuPrefab;
    private GameObject currentMenu;
    private GameObject menuTargetObject;

    [Header("Settings Menu")]
    public GameObject settingsMenuPrefab;
    private GameObject currentSettingsMenu;
    private int settingsMenuIndex = 0;


    private string[] raycastOptions = { "1m", "10m", "50m" };
    private int raycastOptionIndex = 1;
    private string[] speedOptions = { "Low", "Medium", "High" };
    private int speedOptionIndex = 1;

    private GameObject currentHit;
    private GameObject lastDisabledObject;

    private bool movementDisabled = false;
    private EventSystem eventSystem;

    private float menuNavCooldown = 0.2f;
    private float lastMenuNavTime = 0f;
    private float inventoryOpenTime;
    private GameObject lastHitObject = null;

    void Start()
    {
        eventSystem = EventSystem.current;
        _cam = GetComponentInChildren<Camera>();
        if (rayLine == null)
        {
            GameObject rayObj = new GameObject("RayLine");
            Material mat = new Material(Shader.Find("Sprites/Default"));
            rayObj.transform.SetParent(_cam.transform);
            rayLine = rayObj.AddComponent<LineRenderer>();
            rayLine.positionCount = 2;
            rayLine.startWidth = 0.01f;
            rayLine.endWidth = 0.01f;
            mat.color = Color.white;
            rayLine.material = mat;
            rayLine.useWorldSpace = true;
            rayLine.enabled = true;
        }
    }

    void Raycast_Handler()
    {
        if (_cam == null)
        {
            return;
        }

        Vector3 start = _cam.transform.position;
        Vector3 forward = _cam.transform.forward;
        Ray ray = new Ray(start, forward);
        RaycastHit hit;
        Vector3 end = start + forward * rayDistance;
        currentHit = null;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            currentHit = hit.collider.gameObject;
            HandleRaycastHighlight(currentHit);
            /*
            if (X_Held() && currentMenu == null && IsInteractable(currentHit))
            {
                OpenMenu(currentHit);
                return;
            }
            */

            if (currentMenu != null)
            {
                HandleMenuInteraction();
                return;
            }
        }
        else
        {
            HandleRaycastHighlight(null);
            currentHit = null;
        }

        if (rayLine != null)
        {
            rayLine.SetPosition(0, start);
            rayLine.SetPosition(1, end);
        }

        if (currentSettingsMenu == null && currentHit != null)
        {

            if (currentMenu != null)
            {
                HandleMenuInteraction();
                return;
            }
            if (!movementDisabled)
            {
                //Interact_Translation();
                //Interact_Rotation(); 
                //Interact_Destroy_Spawn(hit);
                InteractTeleport(hit);
            }
        }
    }
    void Update()
    {
      
        HandleSettingsMenuNavigation();
        //HandleInventoryPanelNavigation();
        //HandleHeldObject();
    }
    bool X_Held()
    {
#if UNITY_EDITOR
        return Input.GetKey(KeyCode.X);
#else
                return Input.GetKey(KeyCode.JoystickButton2);
#endif
    }
    bool Y_Pressed()
    {
#if UNITY_EDITOR
        return Input.GetKeyDown(KeyCode.Y);
#else
                return Input.GetKeyDown(KeyCode.JoystickButton3);
#endif
    }
    bool A_Pressed()
    {
#if UNITY_EDITOR
        return Input.GetKeyDown(KeyCode.A);
#else
                return Input.GetKeyDown(KeyCode.JoystickButton10);
#endif
    }

    bool B_Pressed()
    {
#if UNITY_EDITOR
        return Input.GetKeyDown(KeyCode.B);
#else
                return Input.GetKeyDown(KeyCode.JoystickButton5);
#endif
    }
    bool OK_Pressed()
    {
#if UNITY_EDITOR
        return Input.GetKeyDown(KeyCode.Return);
#else
                return Input.GetKeyDown(KeyCode.JoystickButton0);
#endif
    }
    bool IsInteractable(GameObject obj)
    {
        return obj.layer == LayerMask.NameToLayer("Interactable");
    }
    /* 
    void Interact_Translation()
    {
        if (currentHit != null && currentHit.name == "Cube1")
        {
            Rigidbody rb = currentHit.GetComponent<Rigidbody>();
            if (rb != null)


                rb.linearVelocity = X_Held() ? Vector3.right * characterMovement.speed : Vector3.zero;
        }
    }

    void Interact_Rotation()
    {
        if (currentHit != null && currentHit.name == "Cube2" && X_Held())
        {
            currentHit.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    } 
    void Interact_Destroy_Spawn(RaycastHit hit)
    {
        if (!A_Pressed())
        {
            return;
        }
        if (((1 << hit.collider.gameObject.layer) & floorLayer) != 0 && lastDisabledObject != null)
        {
            Vector3 spawnPos = hit.point + Vector3.up * 0.5f;
            GameObject newObj = Instantiate(lastDisabledObject, spawnPos, lastDisabledObject.transform.rotation);
            newObj.SetActive(true);

            Rigidbody rb = newObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
    */
    void InteractTeleport(RaycastHit hit)
    {
        if (!Y_Pressed()) return;

        if (((1 << hit.collider.gameObject.layer) & floorLayer) != 0)
        {
            Vector3 targetTP = hit.point;
            Vector3 headOffset = transform.position - transform.root.position;
            headOffset.y = 0f;
            Vector3 finalTP = targetTP - headOffset;

            NetworkObject netObj = transform.root.GetComponent<NetworkObject>();
            if (netObj != null)
                TeleportRpc(netObj.NetworkObjectId, finalTP);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    void TeleportRpc(ulong networkObjectId, Vector3 finalTP)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects
            .TryGetValue(networkObjectId, out NetworkObject target))
        {
            CharacterController controller = target.GetComponent<CharacterController>();
            controller.enabled = false;
            target.transform.position = finalTP;
            controller.enabled = true;
        }
    }
    /*
    void OpenMenu(GameObject target)
    {
        if (currentMenu != null) CloseMenu();

        currentMenu = Instantiate(objectMenuPrefab);
        menuTargetObject = target;

        currentMenu.transform.position = target.transform.position + Vector3.up * 1.5f;
        currentMenu.transform.forward = (currentMenu.transform.position - _cam.transform.position).normalized;

        DisableMovement(true);
    }
    */
    void CloseMenu()
    {
        if (currentMenu != null) Destroy(currentMenu);
        currentMenu = null;
        menuTargetObject = null;
        DisableMovement(false);
    }


    void HandleMenuInteraction()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            GameObject hitObj = hit.collider.gameObject;
            HighlightButton(hitObj);

            if (B_Pressed())
            {
                //if (hitObj.name.Contains("Destroy")) DestroyObject();
                //else if (hitObj.name.Contains("Store")) StoreObject();
                if (hitObj.name.Contains("Exit")) CloseMenu();
            }
        }
    }
    void HighlightButton(GameObject button)
    {
        if (currentMenu == null)
        {
            return;
        }

        foreach (Transform child in currentMenu.transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
                img.color = (child.gameObject == button) ? Color.yellow : Color.white;
        }
    }
    /*
void DestroyObject()
{
    if (menuTargetObject == null)
    {
        return;
    }
    lastDisabledObject = menuTargetObject;
    menuTargetObject.SetActive(false);
    CloseMenu();
}

void StoreObject()
{
    if (menuTargetObject == null)
    {
        return;
    }
    if (inventory.Count >= maxInventory)
    {
        ShowInventoryFullMessage();
        return;
    }

    inventory.Add(menuTargetObject);
    menuTargetObject.SetActive(false);
    CloseMenu();
}

void ShowInventoryFullMessage()
{
    if (currentInventoryFull != null)
    {
        Destroy(currentInventoryFull);
    }
    currentInventoryFull = Instantiate(inventoryFullPrefab);
    currentInventoryFull.transform.position = transform.position + transform.forward * 2f + Vector3.up * 0.3f;
    currentInventoryFull.transform.forward = (currentInventoryFull.transform.position - _cam.transform.position).normalized;

    StartCoroutine(HideInventoryFullAfterSeconds(2f));
}

IEnumerator HideInventoryFullAfterSeconds(float seconds)
{
    yield return new WaitForSeconds(seconds);
    if (currentInventoryFull != null)
    {
        Destroy(currentInventoryFull);
    }
}
*/

    void DisableMovement(bool disable)
    {
        movementDisabled = disable;
        if (characterMovement != null)
            characterMovement.enabled = !disable;
    }

    void OpenSettingsMenu()
    {
        if (currentMenu != null)
        {
            CloseMenu();
        }
        if (currentSettingsMenu != null)
        {
            Destroy(currentSettingsMenu);
        }
        currentSettingsMenu = Instantiate(settingsMenuPrefab);
        currentSettingsMenu.transform.position = transform.position + transform.forward * 2f + Vector3.up * 0.3f;
        currentSettingsMenu.transform.forward = (currentSettingsMenu.transform.position - _cam.transform.position).normalized;
        DisableMovement(true);
        settingsMenuIndex = 0;
        RefreshSettingsMenuText();
        HighlightSettingsButton(settingsMenuIndex);
    }

    void CloseSettingsMenu()
    {
        if (currentSettingsMenu != null)
        {
            Destroy(currentSettingsMenu);
        }
        currentSettingsMenu = null;
        DisableMovement(false);
    }
    void HighlightSettingsButton(int index)
    {
        if (currentSettingsMenu == null)
        {
            return;
        }
        for (int i = 0; i < currentSettingsMenu.transform.childCount; i++)
        {
            Transform child = currentSettingsMenu.transform.GetChild(i);
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                img.color = (i == index) ? Color.yellow : Color.white;
            }
        }
    }
    void SelectSettingsMenuItem(int index)
    {
        string itemName = currentSettingsMenu.transform.GetChild(index).name;
        switch (itemName)
        {
            case "Resume":
                CloseSettingsMenu();
                break;
            case "RaycastLength":
                raycastOptionIndex = (raycastOptionIndex + 1) % raycastOptions.Length;
                RefreshSettingsMenuText();
                rayDistance = (raycastOptionIndex == 0) ? 1f : (raycastOptionIndex == 1) ? 10f : 50f;
                break;
            //case "Inventory":
                //OpenInventoryPanel();
                //break;
            case "Speed":
                speedOptionIndex = (speedOptionIndex + 1) % speedOptions.Length;
                RefreshSettingsMenuText();
                if (characterMovement != null)
                {
                    switch (speedOptionIndex)
                    {
                        case 0:
                            characterMovement.speed = 5f;
                            break;
                        case 1:
                            characterMovement.speed = 10f;
                            break;
                        case 2:
                            characterMovement.speed = 20f;
                            break;
                    }
                }
                break;
            case "Quit":
#if UNITY_ANDROID
                Application.Quit();
#endif
                break;
        }
    }

    void HandleSettingsMenuNavigation()
    {
        if (currentSettingsMenu == null) return;

        float verticalInput = Input.GetAxis("Vertical");
        Debug.Log("Vertical Input: " + verticalInput);

        if (Time.time - lastMenuNavTime > menuNavCooldown)
        {
            if (verticalInput > 0.5f)
            {
                settingsMenuIndex = Mathf.Max(0, settingsMenuIndex - 1);
                HighlightSettingsButton(settingsMenuIndex);
                lastMenuNavTime = Time.time;
            }
            else if (verticalInput < -0.5f)
            {
                settingsMenuIndex = Mathf.Min(currentSettingsMenu.transform.childCount - 1, settingsMenuIndex + 1);
                HighlightSettingsButton(settingsMenuIndex);
                lastMenuNavTime = Time.time;
            }
        }
        if (B_Pressed())
        {
            SelectSettingsMenuItem(settingsMenuIndex);
        }
    }
    /*    void OpenInventoryPanel()
        {
            if (currentSettingsMenu != null)
            {
                Destroy(currentSettingsMenu);
                currentSettingsMenu = null;
            }

            if (currentInventoryPanel != null) Destroy(currentInventoryPanel);

            currentInventoryPanel = Instantiate(inventoryPanelPrefab);
            currentInventoryPanel.transform.position = transform.position + transform.forward * 2f + Vector3.up * 0.5f;
            currentInventoryPanel.transform.forward = (currentInventoryPanel.transform.position - _cam.transform.position).normalized;

            DisableMovement(true);
            inventoryOpenTime = Time.time;

            for (int i = 0; i < currentInventoryPanel.transform.childCount; i++)
            {
                Image img = currentInventoryPanel.transform.GetChild(i).GetComponent<Image>();
                if (i < inventory.Count)
                {
                    Sprite thumb = inventory[i].GetComponent<Thumbnail4Inventory>().thumbnail;
                    if (thumb != null)
                    {
                        img.sprite = thumb;
                        img.color = Color.white;
                    }
                    else img.color = Color.gray;
                }
                else img.color = Color.clear;
            }

            inventoryPanelIndex = 0;
            HighlightInventorySlot(inventoryPanelIndex);
        }
        void HighlightInventorySlot(int index)
        {
            for (int i = 0; i < currentInventoryPanel.transform.childCount; i++)
            {
                Image img = currentInventoryPanel.transform.GetChild(i).GetComponent<Image>();
                if (img != null) img.color = (i == index) ? Color.yellow : Color.white;
            }
        }
        void HandleInventoryPanelNavigation()
        {
            if (currentInventoryPanel == null)
            {
                return;
            }
            if (Time.time - inventoryOpenTime < 0.2f)
            {
                return;
            }

            float verticalInput = Input.GetAxis("Vertical");
            if (Time.time - lastMenuNavTime > menuNavCooldown)
            {
                if (verticalInput > 0.5f)
                {
                    inventoryPanelIndex = Mathf.Max(0, inventoryPanelIndex - 1);
                    HighlightInventorySlot(inventoryPanelIndex);
                    lastMenuNavTime = Time.time;
                }
                else if (verticalInput < -0.5f)
                {
                    int maxIndex = currentInventoryPanel.transform.childCount - 1;
                    do
                    {
                        inventoryPanelIndex = Mathf.Min(maxIndex, inventoryPanelIndex + 1);
                    }
                    while (inventoryPanelIndex < inventory.Count == false &&
                           !currentInventoryPanel.transform.GetChild(inventoryPanelIndex).name.Contains("Exit")); HighlightInventorySlot(inventoryPanelIndex);
                    lastMenuNavTime = Time.time;
                }
            }
            if (B_Pressed())
            {
                GrabInventoryObject(inventoryPanelIndex);
            }
        }
        void CloseInventoryPanel()
        {
            if (currentInventoryPanel != null)
            {
                Destroy(currentInventoryPanel);
            }
            currentInventoryPanel = null;
            DisableMovement(false);
        }
        void GrabInventoryObject(int index)
        {
            string itemName = currentInventoryPanel.transform.GetChild(index).name;
            if (itemName.Contains("Exit"))
            {
                CloseInventoryPanel();
                return;
            }
            if (index >= inventory.Count)
            {
                return;
            }
            heldObject = inventory[index];
            inventory.RemoveAt(index);
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            heldObject.transform.SetParent(_cam.transform);
            heldObject.transform.localPosition = new Vector3(0f, -0.3f, 2f);
            heldObject.transform.localRotation = Quaternion.identity;
            heldObject.SetActive(true);
            CloseInventoryPanel();
        }
         void HandleHeldObject()
        {
            if (heldObject == null) return;
            if (A_Pressed())
            {
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                heldObject.transform.SetParent(null);
                heldObject.transform.position = _cam.transform.position + _cam.transform.forward * 2f;
                heldObject = null;
            }
        } */
    void RefreshSettingsMenuText()
    {
        for (int i = 0; i < currentSettingsMenu.transform.childCount; i++)
        {
            Transform child = currentSettingsMenu.transform.GetChild(i);
            string itemName = child.name;
            TextMeshProUGUI txt = child.GetComponentInChildren<TextMeshProUGUI>();
            if (txt == null)
            {
                continue;
            }
            if (itemName.Contains("Raycast"))
            {
                txt.text = "Raycast Length: " + raycastOptions[raycastOptionIndex];
            }
            else if (itemName.Contains("Speed"))
            {
                txt.text = "Speed: " + speedOptions[speedOptionIndex];
            }
            else if (itemName.Contains("Resume"))
            {
                txt.text = "Resume";
            }
            else if (itemName.Contains("Inventory"))
            {
                txt.text = "Inventory";
            }
            else if (itemName.Contains("Quit"))
            {
                txt.text = "Quit";
            }
        }
    }
    void HandleRaycastHighlight(GameObject hitObj)
    {
        if (lastHitObject != null && lastHitObject != hitObj)
        {
            Outline lastOutline = lastHitObject.GetComponent<Outline>();
            if (lastOutline != null) lastOutline.enabled = false;
            lastHitObject = null;
        }

        if (hitObj != null && IsInteractable(hitObj))
        {
            Outline outline = hitObj.GetComponent<Outline>();
            if (outline != null && outline.enabled == false)
            {
                outline.enabled = true;
                lastHitObject = hitObj;
            }
        }
    }
}