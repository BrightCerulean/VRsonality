using UnityEngine;

public class MuseumManager : MonoBehaviour
{
    [Header("Portals")]
    public GameObject portalToUpload;
    public GameObject portalToResults;

    [Header("Door Highlights")]
    public GameObject pastDoorHighlight;
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

        // Past portal always active
        if (portalToUpload != null)
            portalToUpload.SetActive(true);
        if (pastDoorHighlight != null)
            pastDoorHighlight.SetActive(true);

        // Results portal only after Q1 + Q2
        bool done = count >= 2;
        if (portalToResults != null)
            portalToResults.SetActive(done);
        if (resultsDoorHighlight != null)
            resultsDoorHighlight.SetActive(done);

        Debug.Log("[Museum] Count: " + count + 
            " | Results active: " + done);
    }
}