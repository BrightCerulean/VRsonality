The complete GameManager scoring script:

```csharp
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Scores for each archetype
    private Dictionary<string, int> scores = new Dictionary<string, int>()
    {
        { "H", 0 },  // Hero
        { "C", 0 },  // Caregiver
        { "E", 0 },  // Explorer
        { "S", 0 }   // Sage
    };

    // Track which questions have been answered
    private int questionsAnswered = 0;
    private const int TOTAL_QUESTIONS = 4;

    void Awake()
    {
        // Singleton pattern - persist across scenes
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

    // Call this from OptionSelector when player picks an answer
    public void AddTrait(string trait)
    {
        if (scores.ContainsKey(trait))
        {
            scores[trait]++;
            questionsAnswered++;
            Debug.Log("[GameManager] Added trait: " + trait);
            Debug.Log("[GameManager] Current scores - H:" + scores["H"] 
                + " C:" + scores["C"] 
                + " E:" + scores["E"] 
                + " S:" + scores["S"]);
        }
        else
        {
            Debug.LogWarning("[GameManager] Unknown trait: " + trait);
        }
    }

    // Call this in results room to get final personality
    public string GetResult()
    {
        string dominant = GetDominantTrait();
        string secondary = GetSecondaryTrait();

        // Mixed results
        if (scores[dominant] == scores[secondary])
        {
            return GetMixedResult(dominant, secondary);
        }

        // Pure results
        switch (dominant)
        {
            case "H": return "The Hero";
            case "C": return "The Caregiver";
            case "E": return "The Explorer";
            case "S": return "The Sage";
            default: return "Unknown";
        }
    }

    // Get description for results room display
    public string GetDescription()
    {
        string dominant = GetDominantTrait();
        string secondary = GetSecondaryTrait();

        if (scores[dominant] == scores[secondary])
        {
            return GetMixedDescription(dominant, secondary);
        }

        switch (dominant)
        {
            case "H": 
                return "You face challenges head on. Courageous and determined, " +
                       "you act when others hesitate — but sometimes rush in without looking.";
            case "C": 
                return "You put others first, always. Warm and nurturing, your strength " +
                       "is in the people around you — at your best when someone needs you.";
            case "E": 
                return "You live for discovery. Independent and adventurous, no path feels " +
                       "too uncertain — roots have never come easy.";
            case "S": 
                return "You seek truth before action. Thoughtful and analytical, you " +
                       "understand the world deeply — but sometimes overthink before moving.";
            default: 
                return "";
        }
    }

    string GetDominantTrait()
    {
        string dominant = "H";
        foreach (var pair in scores)
        {
            if (pair.Value > scores[dominant])
                dominant = pair.Key;
        }
        return dominant;
    }

    string GetSecondaryTrait()
    {
        string dominant = GetDominantTrait();
        string secondary = "";
        int secondaryScore = -1;

        foreach (var pair in scores)
        {
            if (pair.Key != dominant && pair.Value > secondaryScore)
            {
                secondary = pair.Key;
                secondaryScore = pair.Value;
            }
        }
        return secondary;
    }

    string GetMixedResult(string a, string b)
    {
        // Sort so order doesn't matter
        string key = string.Compare(a, b) < 0 ? a + b : b + a;

        switch (key)
        {
            case "CH": return "The Guardian Hero";
            case "EH": return "The Pioneer";
            case "CS": return "The Wise Caregiver";
            case "ES": return "The Wandering Sage";
            case "CE": return "The Nurturing Explorer";
            case "HS": return "The Heroic Sage";
            default:   return "The Balanced Soul";
        }
    }

    string GetMixedDescription(string a, string b)
    {
        string key = string.Compare(a, b) < 0 ? a + b : b + a;

        switch (key)
        {
            case "CH": return "Brave enough to act, caring enough to bring others along.";
            case "EH": return "You don't just dream big — you go first and figure it out.";
            case "CS": return "You think deeply and love deeply — the quiet backbone of every group.";
            case "ES": return "Curious and restless — you seek wisdom through experience.";
            case "CE": return "You explore the world but always come back for the people you love.";
            case "HS": return "You act with both courage and wisdom — a rare combination.";
            default:   return "You are a balance of all things — adaptable and whole.";
        }
    }

    // Check if all questions answered
    public bool IsComplete()
    {
        return questionsAnswered >= TOTAL_QUESTIONS;
    }

    // Reset for restart
    public void ResetScores()
    {
        scores["H"] = 0;
        scores["C"] = 0;
        scores["E"] = 0;
        scores["S"] = 0;
        questionsAnswered = 0;
        Debug.Log("[GameManager] Scores reset");
    }

    // Get individual score for UI display
    public int GetScore(string trait)
    {
        return scores.ContainsKey(trait) ? scores[trait] : 0;
    }
}
```

---

**How to set it up:**

1. Create an empty GameObject called **GameManager**
2. Attach this script to it
3. It will persist across all scenes automatically

---

**How to connect to your option props:**

On each option cube's **OptionSelector.cs**, the `OnSelect()` already calls:
```csharp
GameManager.Instance.AddTrait(traitPoint);
```

Just make sure `traitPoint` is set correctly in Inspector:
- Option A → **H**
- Option B → **C**
- Option C → **S**
- Option D → **E**

---

**How to display results:**

In your results room, attach this to a TextMeshPro:

```csharp
using UnityEngine;
using TMPro;

public class ResultsDisplay : MonoBehaviour
{
    public TextMeshProUGUI personalityTitle;
    public TextMeshProUGUI personalityDescription;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            personalityTitle.text = GameManager.Instance.GetResult();
            personalityDescription.text = 
                GameManager.Instance.GetDescription();
        }
    }
}
```

---

**Also call ResetScores() on restart:**

In your `ActionRestart()` in SettingsMenuController:
```csharp
void ActionRestart()
{
    GameManager.Instance.ResetScores();
    CloseSettings();
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
```

Want me to now write the OptionSelector.cs that connects to this?