using UnityEngine;
using UnityEngine.UI;

public class ObjectMenu : MonoBehaviour
{
    public Canvas menuCanvas;
    
    public GameObject destroyButton;
    public GameObject storeButton;
    public GameObject exitButton;
    private GameObject currentHighlighted;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        menuCanvas.gameObject.SetActive(false);
    }

    /*void Update()
    {
        if (Input.GetKey(KeyCode.O))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    menuCanvas.gameObject.SetActive(true);
                    menuCanvas.transform.position = hit.point;
                    menuCanvas.transform.LookAt(playerCamera.transform);
                }
                else
                {
                    menuCanvas.gameObject.SetActive(false);
                }
            }
        }
    }*/

    void LateUpdate()
    {
        if (menuCanvas != null && menuCanvas.gameObject.activeSelf && playerCamera != null)
        {
            Vector3 direction = menuCanvas.transform.position - playerCamera.transform.position;
            menuCanvas.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void ShowMenu()
    {
        if (menuCanvas != null)
            menuCanvas.gameObject.SetActive(true);
    }
 
    public void HideMenu()
    {
        if (menuCanvas != null)
            menuCanvas.gameObject.SetActive(false);
        ClearHighlight();
    }
 
    public bool IsOpen() => menuCanvas != null && menuCanvas.gameObject.activeSelf;
 
    // highlight the given button yellow; un-highlight the previous one
    public void HighlightButton(GameObject btn)
    {
        if (btn == currentHighlighted) return;
        ClearHighlight();
        currentHighlighted = btn;
        SetButtonColor(btn, Color.yellow);
    }
 
    public void ClearHighlight()
    {
        if (currentHighlighted != null)
        {
            SetButtonColor(currentHighlighted, Color.white);
            currentHighlighted = null;
        }
    }
 
    public GameObject GetHighlightedButton() => currentHighlighted;

 
    private void SetButtonColor(GameObject btn, Color color)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        if (img != null) img.color = color;
    }


}
