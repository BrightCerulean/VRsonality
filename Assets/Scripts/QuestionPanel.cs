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
    private Coroutine autoRoutine;

    void Start()
    {
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
        //Question appears on start then disappears after some time
        isVisible = true;
        gameObject.SetActive(true);

        float timer = 0f;
        while (timer < displayTime)
        {
            if (isToggledOpen) yield break; // stop auto-hide if player took control
            timer += Time.deltaTime;
            yield return null;
        }

        if (!isToggledOpen)
        {
            HidePanel();
        }
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

    void ShowPanelManual()
    {
        isToggledOpen = true;
        isVisible = true;

        if (autoRoutine != null)
            StopCoroutine(autoRoutine);

        gameObject.SetActive(true);
    }

    void HidePanel()
    {
        isToggledOpen = false;
        isVisible = false;

        gameObject.SetActive(false);
    }
}