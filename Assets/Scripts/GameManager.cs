using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Color playerColor = Color.white;
    public string playerColorName = "None";
    // 1 = past, 2 = present
    public int currentQuestionSet = 1; 


    private Dictionary<string, string> selections = 
        new Dictionary<string, string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddSelection(string sceneName, string letter)
    {
        if (!selections.ContainsKey(sceneName))
        {
            selections[sceneName] = letter;
            Debug.Log("[GameManager] Scene: " + sceneName + 
                " → Letter: " + letter);
        }
    }

    public string GetResult()
    {
        Dictionary<string, int> counts = new Dictionary<string, int>()
        {
            { "H", 0 }, { "C", 0 }, { "E", 0 }, { "S", 0 }
        };

        foreach (var pair in selections)
            if (counts.ContainsKey(pair.Value))
                counts[pair.Value]++;

        string dominant = "H";
        int highest = 0;
        foreach (var pair in counts)
        {
            if (pair.Value > highest)
            {
                highest = pair.Value;
                dominant = pair.Key;
            }
        }

        switch (dominant)
        {
            case "H": return "The Hero";
            case "C": return "The Caregiver";
            case "E": return "The Explorer";
            case "S": return "The Sage";
            default: return "The Hero";
        }
    }

    public string GetDescription()
    {
        switch (GetResult())
        {
            case "The Hero":
                return "You face challenges head on. Courageous and " +
                       "determined, you act when others hesitate.";
            case "The Caregiver":
                return "You put others first, always. Warm and nurturing, " +
                       "your strength is in the people around you.";
            case "The Explorer":
                return "You live for discovery. Independent and adventurous, " +
                       "no path feels too uncertain.";
            case "The Sage":
                return "You seek truth before action. Thoughtful and " +
                       "analytical, you understand the world deeply.";
            default: return "";
        }
    }

    public string GetSelectionsString()
    {
        string result = "";
        foreach (var pair in selections)
            result += pair.Key + ":" + pair.Value + " | ";
        return result;
    }

    public int GetSelectionCount()
    {
        return selections.Count;
    }

    public bool IsSceneAnswered(string sceneName)
    {
        return selections.ContainsKey(sceneName);
    }

    public void ResetSelections()
    {
        selections.Clear();
        Debug.Log("[GameManager] Reset");
    }
    public void SetPlayerColor(Color color, string colorName)
    {
        playerColor = color;
        playerColorName = colorName;
        Debug.Log($"Player color set to: {colorName}");
    }
}