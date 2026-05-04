using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Color playerColor = Color.white;
    public string playerColorName = "None";
    public int currentQuestionSet = 1; // 1 = past, 2 = present

    public static readonly string Scene1 = "toddlerbedroom";
    public static readonly string Scene2 = "playground";
    public static readonly string Scene3 = "partyroom";
    public static readonly string Scene4 = "adultbedroom";

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

    private string GetDominantLetter(List<string> letters)
    {
        Dictionary<string, int> counts = new Dictionary<string, int>()
    {
        { "H", 0 }, { "C", 0 }, { "E", 0 }, { "S", 0 }
    };

        foreach (string letter in letters)
            if (counts.ContainsKey(letter))
                counts[letter]++;

        int highest = 0;
        foreach (var pair in counts)
            if (pair.Value > highest)
                highest = pair.Value;

        string[] priority = { "C", "H", "E", "S" };

        foreach (string letter in priority)
            if (counts[letter] == highest)
                return letter;

        return "H";
    }

    private string GetPastLetter()
    {
        List<string> past = new List<string>();

        if (selections.ContainsKey(Scene1))
            past.Add(selections[Scene1]);

        if (selections.ContainsKey(Scene2))
            past.Add(selections[Scene2]);

        return GetDominantLetter(past);
    }

    private string GetPresentLetter()
    {
        List<string> present = new List<string>();

        if (selections.ContainsKey(Scene3))
            present.Add(selections[Scene3]);

        if (selections.ContainsKey(Scene4))
            present.Add(selections[Scene4]);

        return GetDominantLetter(present);
    }

    private string LetterToArchetype(string letter)
    {
        switch (letter)
        {
            case "H": return "The Hero";
            case "C": return "The Caregiver";
            case "E": return "The Explorer";
            case "S": return "The Sage";
            default: return "The Hero";
        }
    }

    public string GetResult()
    {
        string past = GetPastLetter();
        string present = GetPresentLetter();

        if (past == present)
        {
            return LetterToArchetype(present);
        }

        return GetEvolvedArchetype(past, present);
    }

    private string GetEvolvedArchetype(string past, string present)
    {
        if (past == "H" && present == "C") return "The Protector";
        if (past == "H" && present == "E") return "The Pathfinder";
        if (past == "H" && present == "S") return "The Tactician";

        if (past == "C" && present == "H") return "The Advocate";
        if (past == "C" && present == "E") return "The Wayfarer";
        if (past == "C" && present == "S") return "The Empath";

        if (past == "E" && present == "H") return "The Trailblazer";
        if (past == "E" && present == "C") return "The Companion";
        if (past == "E" && present == "S") return "The Seeker";

        if (past == "S" && present == "H") return "The Visionary";
        if (past == "S" && present == "C") return "The Guide";
        if (past == "S" && present == "E") return "The Nomad";

        return LetterToArchetype(present);
    }

    public string GetDescription()
    {
        switch (GetResult())
        {
            //H->H
            case "The Hero":
                return "You face challenges head on. Courageous and " +
                       "determined, you act when others hesitate.";
            //C->C              
            case "The Caregiver":
                return "You put others first, always. Warm and nurturing, " +
                       "your strength is in the people around you.";
            //E->E
            case "The Explorer":
                return "You live for discovery. Independent and adventurous, " +
                       "no path feels too uncertain.";
            //S->S
            case "The Sage":
                return "You seek truth before action. Thoughtful and " +
                       "analytical, you understand the world deeply.";
            //H->C
            case "The Protector":
                return "You’ve shifted from acting boldly to caring deeply. Your " +
                    "strength now lies in defending and supporting those around you.";
            //H->E
            case "The Pathfinder":
                return "Your courage now drives you toward new paths. Instead of " +
                    "fighting what’s in front of you, you seek what lies beyond.";
            //H->S
            case "The Tactician":
                return "You’ve learned to think before you act. Your strength now " +
                    "comes from strategy, timing, and understanding the bigger picture.";
            //C->H
            case "The Advocate":
                return "Your courage gives you the strength to act on behalf of others. " +
                    "You find purpose in standing up for everyone, not just yourself.";
            //C->E
            case "The Wayfarer":
                return "You’ve stepped away from others to better understand yourself. " +
                    "Your growth now comes from independence and self-discovery.";
            //C->S
            case "The Empath":
                return "Your care has deepened into true understanding. " +
                    "You sense not just how people feel, but why they feel it.";
            //E->H
            case "The Trailblazer":
                return "You’ve turned independence into leadership. " + 
                    "Your path now inspires others to follow.";
            //E->C
            case "The Companion":
                return "You’ve opened your journey to others. Where you once " + 
                    "walked alone, you now find strength in connection.";
            //E->S
            case "The Seeker":
                return "Your curiosity has grown into reflection. " +
                    "You now search for meaning beyond your past experiences.";
            //S->H
            case "The Visionary":
                return "You’ve turned knowledge into action. You lead the future " + 
                    "with your ideas now shaping the world around you.";
            //S->C
            case "The Guide":
                return "You’ve chosen to share your understanding with others. " + 
                    "Your knowledge now helps people find their own way.";
            //S->E
            case "The Nomad":
                return "You’ve stepped beyond thought and into experience. " + 
                    "Your wisdom now grows through living, not just observing.";
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
    public string GetSelectionForScene(string sceneName)
    {
        if (selections.ContainsKey(sceneName))
            return selections[sceneName];

        return null;
    }
}