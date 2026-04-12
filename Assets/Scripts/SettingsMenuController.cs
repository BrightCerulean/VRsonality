using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Settings Menu Canvas")]
    public GameObject settingsMenuCanvas;

    [Header("Menu Items (assign in order)")]
    public GameObject resumeItem;
    //public GameObject raycastItem;
    //public GameObject inventoryItem;
    //public GameObject speedItem;
    public GameObject restartItem;
    public GameObject quitItem;
    public InventoryPanel inventoryPanel;
    public JoystickMovement joystickMovement;

    [Header("Dynamic Labels")]
    public TextMeshProUGUI raycastLabel;
    public TextMeshProUGUI speedLabel;

    [Header("References")]
    public MenuController menuController;
    public Teleporting movementScript;
    public RayCast raycastScript;
    [Header("Inventory Display")]
    public TextMeshProUGUI inventoryDisplayText;

    // Navigation
    private List<GameObject> menuItems = new List<GameObject>();
    private int currentIndex = 0;

    // Raycast length cycling: 1m, 10m, 50m
    private float[] raycastLengths = { 1f, 10f, 50f };
    private string[] raycastLabelNames = { "1m", "10m", "50m" };
    private int raycastIndex = 1; // default 10m

    // Speed cycling: High, Medium, Low
    private float[] speeds = { 20f, 10f, 5f };
    private string[] speedNames = { "High", "Medium", "Low" };
    private int speedIndex = 1; // default Medium

    private bool isOpen = false;
    private float joystickCooldown = 0f;
    private const float JOYSTICK_DELAY = 0.25f;

    public static string XButton;

    void Start()
    {
        menuItems.Add(resumeItem);
        //menuItems.Add(raycastItem);
        //menuItems.Add(inventoryItem);
        //menuItems.Add(speedItem);
        menuItems.Add(restartItem);
        menuItems.Add(quitItem);

        if (settingsMenuCanvas != null)
            settingsMenuCanvas.SetActive(false);

        UpdateLabels();
    }
    private void Awake()
    {
        if (Application.isEditor)//PC
        {
            XButton = "js1";
        }
        else//Android
        {
            XButton = "js2";
        }
    }

    /*void Update()
    {
        // Return / OK button → toggle settings menu
        if (Input.GetKey(KeyCode.Return) || Input.GetButtonDown("js7"))
        {
            if (isOpen) CloseSettings();
            else OpenSettings();
            return;
        }

        if (!isOpen) return;

        HandleNavigation();

        // M / A button → select current item
        if (Input.GetKey(KeyCode.M) || Input.GetButtonDown("js11"))
            SelectCurrentItem();
    }*/

    /*void Update()
    {
        // OK / Return → toggle Settings
        if (Input.GetButtonDown("js7") || Input.GetKeyDown(KeyCode.Return))
        {
            if (isOpen) CloseSettings();
            else OpenSettings();
            return;
        }

        if (!isOpen) return;

        HandleNavigation();

        // B / M → select current item
        if (Input.GetButtonDown("js10") || Input.GetKeyDown(KeyCode.M))
            SelectCurrentItem();
    }*/
    void Update()
    {
        if (!isOpen) return;
        if (Input.GetKeyDown(KeyCode.M) || Input.GetButtonDown("js7") || Input.GetButtonDown(XButton))
        {
            if (isOpen) CloseSettings();
            else OpenSettings();
            return;
        }

        HandleNavigation();

        if (Input.GetKeyDown(KeyCode.M))
            //if (Input.GetKeyDown(KeyCode.M) || Input.GetButtonDown("js10"))
            SelectCurrentItem();
    }


    void HandleNavigation()
    {
        joystickCooldown -= Time.deltaTime;
        if (joystickCooldown > 0f) return;

        // UpArrow or joystick up → move up
        if (Input.GetKeyDown(KeyCode.UpArrow))
        //if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetButtonDown("js0"))
        {
            currentIndex = (currentIndex - 1 + menuItems.Count) % menuItems.Count;
            UpdateHighlight();
            joystickCooldown = JOYSTICK_DELAY;
        }
        // DownArrow or joystick down → move down
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        //else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetButtonDown("js3"))
        {
            currentIndex = (currentIndex + 1) % menuItems.Count;
            UpdateHighlight();
            joystickCooldown = JOYSTICK_DELAY;
        }
    }

    public void OpenSettings()
    {
        isOpen = true;
        currentIndex = 0;

        // Close any open object menu
        if (menuController != null)
            menuController.ForceCloseMenu();

        // Disable movement and raycast
        if (movementScript != null) movementScript.enabled = false;
        if (raycastScript != null) raycastScript.enabled = false;
        if (joystickMovement != null) joystickMovement.enabled = false;

        settingsMenuCanvas.SetActive(true);
        UpdateHighlight();
        UpdateInventoryDisplay();
        Debug.Log("[Settings] Opened");
    }

    public void CloseSettings()
    {
        isOpen = false;
        settingsMenuCanvas.SetActive(false);

        // Re-enable movement and raycast
        if (movementScript != null) movementScript.enabled = true;
        if (raycastScript != null) raycastScript.enabled = true;
        if (joystickMovement != null) joystickMovement.enabled = true;

        Debug.Log("[Settings] Closed");
    }

    void SelectCurrentItem()
    {
        string item = menuItems[currentIndex].name;
        Debug.Log("[Settings] Selected: " + item);

        switch (item)
        {
            case "ResumeItem": ActionResume(); break;
            case "RaycastItem": ActionRaycast(); break;
            case "RestartItem": ActionRestart(); break;
            case "InventoryItem": ActionInventory(); break;
            case "SpeedItem": ActionSpeed(); break;
            case "QuitItem": ActionQuit(); break;
            default:
                Debug.LogWarning("[Settings] Unknown item: " + item);
                break;
        }
    }

    void ActionResume()
    {
        CloseSettings();
    }

    void ActionRaycast()
    {
        raycastIndex = (raycastIndex + 1) % raycastLengths.Length;
        if (menuController != null)
            menuController.raycastDistance = raycastLengths[raycastIndex];
        UpdateLabels();
        Debug.Log("[Settings] Raycast set to: " + raycastLengths[raycastIndex]);
    }

    /*void ActionInventory()
    {
        UpdateInventoryDisplay();

        TextMeshProUGUI label = inventoryItem.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
            label.text = "Inventory: " + menuController.GetInventory().Count + "/3";

        Debug.Log("[Settings] Inventory updated");

    }*/

    void ActionRestart()
    {
        Debug.Log("[Settings] Restart");
        if (GameManager.Instance != null)
            GameManager.Instance.ResetSelections();
        CloseSettings();
        SceneManager.LoadScene("gamestart");
    }

    void ActionInventory()
    {
        settingsMenuCanvas.SetActive(false);
        if (inventoryPanel != null)
            inventoryPanel.OpenPanel();
        else
            Debug.LogWarning("[Settings] InventoryPanel not assigned!");
        Debug.Log("[Settings] Opening inventory panel");
    }

    public void UpdateInventoryDisplay()
    {
        if (inventoryDisplayText == null) return;

        List<GameObject> inv = menuController.GetInventory();

        string display = "Inventory\n";
        for (int i = 0; i < 3; i++)
        {
            if (i < inv.Count && inv[i] != null)
                display += (i + 1) + ". " + inv[i].name + "\n";
            else
                display += (i + 1) + ". \n";
        }
        display += "Total: " + inv.Count + "/3";

        inventoryDisplayText.text = display;
    }

    /*void ActionSpeed()
    {
        speedIndex = (speedIndex + 1) % speeds.Length;

        // Try to set speed field/property on movement script via reflection
        if (movementScript != null)
        {
            var field = movementScript.GetType().GetField("speed");
            if (field != null) field.SetValue(movementScript, speeds[speedIndex]);

            var prop = movementScript.GetType().GetProperty("speed");
            if (prop != null) prop.SetValue(movementScript, speeds[speedIndex]);
        }

        UpdateLabels();
        Debug.Log("[Settings] Speed set to: " + speedNames[speedIndex]);
    }*/

    void ActionSpeed()
    {
        speedIndex = (speedIndex + 1) % speeds.Length;

        JoystickMovement movement = FindFirstObjectByType<JoystickMovement>();
        if (movement != null)
            movement.speed = speeds[speedIndex];

        UpdateLabels();
        Debug.Log("[Settings] Speed set to: " + speedNames[speedIndex]);
    }
    void ActionQuit()
    {
        Debug.Log("[Settings] Quit");
        Application.Quit();
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (menuItems[i] == null) continue;
            Image img = menuItems[i].GetComponent<Image>();
            if (img != null)
                img.color = (i == currentIndex) ? Color.yellow : Color.white;
        }
        if (inventoryDisplayText != null)
        {
            GameObject panel = inventoryDisplayText.transform.parent.gameObject;
            //panel.SetActive(menuItems[currentIndex] == inventoryItem);
        }
    }

    void UpdateLabels()
    {
        if (raycastLabel != null)
            raycastLabel.text = "Raycast Length: " + raycastLabelNames[raycastIndex];
        if (speedLabel != null)
            speedLabel.text = "Speed: " + speedNames[speedIndex];
    }

    public bool IsOpen() => isOpen;

}