using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameInputManager : MonoBehaviour
{
    public TMP_Text headerText;
    public TMP_Text nameText;
    public TMP_Text instructionText;
    public GameObject upperCasePanel;
    public GameObject lowerCasePanel;
    public Color normalColor;
    public Color highlightColor;

    private string currentString = "";
    private int maxStringLength = 10;
    private int stringPositionX = -1;
    private int stringPositionY = -1;
    private List<List<Button>> asciiButtons = new List<List<Button>>();
    private string[] confirmationText = { "No", "Yes" };
    private int confirmationIndex = 1;
    private Button lastSelectedButton;

    private enum NameStates { INPUT_NAME, PRE_CONFIRMATION, CONFIRMATION }
    private NameStates nameState = NameStates.INPUT_NAME;

    void Start()
    {
        InitializeCharacterGrid();
        nameText.text = "hello";
        headerText.text = "Name the Challenger.";
        instructionText.text = "[ENTER or SPACE] - Confirm          [ARROW KEYS] - Move          [BACKSPACE] - Remove Character";
    }

    void InitializeCharacterGrid()
    {
        List<Button> buttons = new List<Button>();

        foreach (Transform child in upperCasePanel.transform)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                buttons.Add(button);
                button.onClick.AddListener(() => OnCharacterButtonClicked(button));
            }
        }

        foreach (Transform child in lowerCasePanel.transform)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                buttons.Add(button);
                button.onClick.AddListener(() => OnCharacterButtonClicked(button));
            }
        }

        for (int i = 0; i < 8; i++)
        {
            asciiButtons.Add(new List<Button>());
            for (int j = 0; j < 7; j++)
            {
                int index = i * 7 + j;
                if (index < buttons.Count)
                {
                    asciiButtons[i].Add(buttons[index]);
                }
            }
        }
    }

    void Update()
    {
        switch (nameState)
        {
            case NameStates.INPUT_NAME:
                HandleInputName();
                break;
            case NameStates.PRE_CONFIRMATION:
                CheckNameTaken();
                break;
            case NameStates.CONFIRMATION:
                HandleConfirmation();
                break;
        }

        DrawUI();
    }

    void updateUsername()
    {
        
    }

     void HandleInputName()
    {
        bool moved = false;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) { MoveCursor(-1, 0); moved = true; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { MoveCursor(1, 0); moved = true; }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { MoveCursor(0, -1); moved = true; }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { MoveCursor(0, 1); moved = true; }

        if (moved && stringPositionX == -1 && stringPositionY == -1)
        {
            stringPositionX = 0;
            stringPositionY = 0;
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && currentString.Length > 0)
        {
            currentString = currentString.Substring(0, currentString.Length - 1);
        }

        // Handle direct keyboard input for alphabets
        foreach (char c in Input.inputString)
        {
            if (char.IsLetter(c) && currentString.Length < maxStringLength)
            {
                currentString += c;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (stringPositionY >= 0 && stringPositionX >= 0 && stringPositionY < asciiButtons.Count && stringPositionX < asciiButtons[stringPositionY].Count)
            {
                string buttonText = asciiButtons[stringPositionY][stringPositionX].GetComponentInChildren<TMP_Text>().text;
                if (buttonText == "Quit")
                {
                    Quit();
                }
                else if (buttonText == "Backspace" && currentString.Length > 0)
                {
                    currentString = currentString.Substring(0, currentString.Length - 1);
                }
                else if (buttonText == "Done" && currentString.Length > 0)
                {
                    nameState = NameStates.PRE_CONFIRMATION;
                }
                else if (currentString.Length < maxStringLength)
                {
                    currentString += buttonText;
                }
            }
        }
    }

    void MoveCursor(int x, int y)
    {
        int newX = stringPositionX + x;
        int newY = stringPositionY + y;

        if (newX >= 0 && newY >= 0 && newY < asciiButtons.Count && newX < asciiButtons[newY].Count)
        {
            stringPositionX = newX;
            stringPositionY = newY;
        }
    }

    void CheckNameTaken()
    {
        // Implement the logic to check if the name is taken
        nameState = NameStates.CONFIRMATION;
    }

    void HandleConfirmation()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) confirmationIndex = Mathf.Max(0, confirmationIndex - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) confirmationIndex = Mathf.Min(confirmationText.Length - 1, confirmationIndex + 1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (confirmationIndex == 0) // No
            {
                nameState = NameStates.INPUT_NAME;
            }
            else if (confirmationIndex == 1) // Yes
            {
                // Save the name or perform any other necessary action
                nameState = NameStates.INPUT_NAME;
            }
        }
    }

    void DrawUI()
    {
        //currentString = PlayerInfo.getInstance().getUsername();
        nameText.text = currentString;
        

        for (int i = 0; i < asciiButtons.Count; i++)
        {
            for (int j = 0; j < asciiButtons[i].Count; j++)
            {
                TMP_Text buttonText = asciiButtons[i][j].GetComponentInChildren<TMP_Text>();
                buttonText.color = (i == stringPositionY && j == stringPositionX) ? highlightColor : normalColor;
            }
        }
    }

    void OnCharacterButtonClicked(Button button)
    {
        if (currentString.Length < maxStringLength)
        {
            currentString += button.GetComponentInChildren<TMP_Text>().text;
        }

        // Deselect last selected button
        if (lastSelectedButton != null)
        {
            lastSelectedButton.GetComponentInChildren<TMP_Text>().color = normalColor;
        }

        lastSelectedButton = button;
        stringPositionX = -1;
        stringPositionY = -1;
    }

    void Quit()
    {
        Debug.Log("Player has quit.");
        // Implement quit functionality
    }
}
