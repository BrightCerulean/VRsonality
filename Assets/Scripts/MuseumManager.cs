using UnityEngine;

public class MuseumManager : MonoBehaviour
{
    [Header("Portals")]
    public GameObject portalToUpload;
    public GameObject portalToPresent;
    public GameObject portalToResults;git stash show stash@{0} --stat

    public GameObject pastPortalTrigger;
    public GameObject presentPortalTrigger;

    public GameObject pastPortalTrigger;
    public GameObject presentPortalTrigger;

    [Header("Door Highlights")]
    public GameObject pastDoorHighlight;
    public GameObject presentDoorHighlight;
    public GameObject resultsDoorHighlight;

    void Start()
    {
        UpdateDoors();
    }

    // Call this every time museum loads
    void OnEnable()
    {
        UpdateDoors();
    }

    public void UpdateDoors()
    {
        if (GameManager.Instance == null) return;
        int count = GameManager.Instance.GetSelectionCount();

        // Past — always active
        if (portalToUpload != null) portalToUpload.SetActive(true);
        if (pastDoorHighlight != null) pastDoorHighlight.SetActive(true);

        // Present — after Q1 + Q2
        bool presentDone = count >= 2;
        if(presentDone)
        {
            if (portalToPresent != null) portalToPresent.SetActive(true);
            if (presentDoorHighlight != null) presentDoorHighlight.SetActive(true);
            if (pastPortalTrigger != null) pastPortalTrigger.SetActive(false);
        }    
        //if (portalToPresent != null) portalToPresent.SetActive(presentDone);
        //if (presentDoorHighlight != null) presentDoorHighlight.SetActive(presentDone);

        // Results — after all 4
        bool allDone = count >= 4;
        if (allDone)
        {
            if (portalToResults != null) portalToResults.SetActive(true);
            if (resultsDoorHighlight != null) resultsDoorHighlight.SetActive(true);
            if (presentPortalTrigger != null) presentPortalTrigger.SetActive(false);
        }
        //if (portalToResults != null) portalToResults.SetActive(allDone);
        //if (resultsDoorHighlight != null) resultsDoorHighlight.SetActive(allDone);

        Debug.Log("[Museum] Count: " + count + " | Present: " + presentDone + " | Results: " + allDone);
    }
}