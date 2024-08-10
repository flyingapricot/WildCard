using System.Text;
using System.Reflection;
using System.Linq;
using UnityEngine;
using TMPro;

public class UIStatsDisplay : MonoBehaviour
{
    public PlayerStats player; // The player that this stat display is rendering stats for.
    public bool updateInEditor = false;
    TextMeshProUGUI mainValues, extraValues; //statNames

    // Define the order of main and extra stats
    private readonly string[] mainStatNames = { "maxHealth", "might", "armour", "moveSpeed", "recovery", "magnet" };
    private readonly string[] extraStatNames = { "area", "amount", "duration", "cooldown", "projSpeed", "luck", "greed", "curse", "growth", "revive" };

    // Update this stat display whenever it is set to be active.
    void OnEnable()
    {
        UpdateStatFields();
    }

    void OnDrawGizmosSelected()
    {
        if(updateInEditor) UpdateStatFields();
    }

    public void UpdateStatFields()
    {
        if (!player) return;

        // Get a reference to both Text objects to render stat names and stat values.
        //if (!statNames) statNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!mainValues) mainValues = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        if (!extraValues) extraValues = transform.GetChild(4).GetComponent<TextMeshProUGUI>();

        // Render all stat names and values.
        // Use StringBuilders so that the string manipulation runs faster.
        //StringBuilder names = new();
        StringBuilder main = new();
        StringBuilder extra = new();

        // Get the fields of the stats object
        FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);

        // Helper method to get the value of a stat
        float GetStatValue(string statName)
        {
            foreach (FieldInfo field in fields)
            {
                // Render stat names.
                //names.AppendLine(field.Name);

                if (field.Name == statName)
                {
                    // Get the stat value.
                    object val = field.GetValue(player.Stats);
                    return val is int v ? v : (float)val;
                }
            }
            return 0;
        }

        // Helper method to format the stat value
        string FormatStatValue(FieldInfo field, float fval)
        {
            PropertyAttribute attribute = (PropertyAttribute)field.GetCustomAttribute(typeof(PropertyAttribute));
            if (attribute != null && field.FieldType == typeof(float))
            {
                float percentage = Mathf.Round(fval * 100 - 100);

                // If the stat value is 0, just put a dash.
                if (Mathf.Approximately(percentage, 0))
                {
                    return "-\n";
                }
                else
                {
                    StringBuilder value = new StringBuilder();
                    if (percentage > 0)
                        value.Append('+');
                    value.Append(percentage).Append('%').Append('\n');
                    return value.ToString();
                }
            }
            else
            {
                return fval + "\n";
            }
        }

        // Add current health display
        float currentHealth = Mathf.RoundToInt(player.CurrentHealth);
        float maxHealth = GetStatValue("maxHealth");
        main.Append(currentHealth).Append(" / ").Append(maxHealth).Append("\n");

        // Append main stats in the defined order
        foreach (string statName in mainStatNames)
        {
            // Skip maxHealth as it is already handled in the current health display
            if (statName == "maxHealth") continue;

            float statValue = GetStatValue(statName);
            FieldInfo field = fields.First(f => f.Name == statName);
            main.Append(FormatStatValue(field, statValue));
        }

        // Append extra stats in the defined order
        foreach (string statName in extraStatNames)
        {
            float statValue = GetStatValue(statName);
            FieldInfo field = fields.First(f => f.Name == statName);
            extra.Append(FormatStatValue(field, statValue));
        }

        // Updates the fields with the strings we built.
        //statNames.text = PrettifyNames(names);
        mainValues.text = main.ToString();
        extraValues.text = extra.ToString();
    }

    public static string PrettifyNames(StringBuilder input)
    {
        // Return an empty string if StringBuilder is empty.
        if (input.Length <= 0) return string.Empty;

        StringBuilder result = new StringBuilder();
        char last = '\0';
        for(int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // Check when to uppercase or add spaces to a character.
            if(last == '\0' || char.IsWhiteSpace(last))
            {
                c = char.ToUpper(c);
            } else if (char.IsUpper(c))
            {
                result.Append(' '); // Insert space before capital letter
            }
            result.Append(c);

            last = c;
        }
        
        return result.ToString();
    }

    void Reset()
    {
        player = FindObjectOfType<PlayerStats>();
    }
}