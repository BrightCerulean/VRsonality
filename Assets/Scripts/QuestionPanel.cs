using System.Collections;
using UnityEngine;

public class QuestionPanel : MonoBehaviour
{
    public Transform playerCamera;
    public float distance = 2f;
    public float heightOffset = 0.1f;
    public float displayTime = 3f;
    private bool isVisible = false;
    private bool isToggledOpen = false;
    public RayCast raycast;
    private Coroutine autoRoutine;

    void Start()
    {
        if (playerCamera == null)
        {
            if (Camera.main != null)
                playerCamera = Camera.main.transform;
            else
                Debug.LogWarning("[QuestionPanel] No camera found.");
        }

        autoRoutine = StartCoroutine(AutoShowRoutine());
    }
    void LateUpdate()
    {
        //Autoface player
        if (!isVisible || playerCamera == null) return;

        transform.position =
            playerCamera.position +
            playerCamera.forward * distance +
            Vector3.up * heightOffset;

        transform.rotation = Quaternion.LookRotation(
            transform.position - playerCamera.position
        );
    }

    IEnumerator AutoShowRoutine()
    {
        isVisible = true;
        gameObject.SetActive(true);
        if (raycast != null) raycast.raycastEnabled = false;

        float timer = 0f;
        while (timer < displayTime)
        {
            if (isToggledOpen) yield break;
            timer += Time.deltaTime;
            yield return null;
        }

        if (!isToggledOpen)
            HidePanel();
    }

    public void TogglePanel()
    {
        if (isToggledOpen)
        {
            HidePanel();
        }
        else
        {
            ShowPanelManual();
        }
    }

    void HidePanel()
    {
        isToggledOpen = false;
        isVisible = false;
        gameObject.SetActive(false);
        if (raycast != null) raycast.raycastEnabled = true;
    }

    void ShowPanelManual()
    {
        isToggledOpen = true;
        isVisible = true;
        if (autoRoutine != null)
            StopCoroutine(autoRoutine);
        gameObject.SetActive(true);
        if (raycast != null) raycast.raycastEnabled = false;
    }
}