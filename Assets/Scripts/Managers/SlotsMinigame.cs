using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotsMinigame : MonoBehaviour
{
    public Image[] slotWindows; // The 3 slot windows
    public Sprite[] slotSprites1; // The 3 Head colours
    public Sprite[] slotSprites2; // The 3 Torso colours
    public Sprite[] slotSprites3; // The 3 Pants colours
    public GameObject warningMessage; // Shows up when not enough Souls
    public GameObject winMessage; // Shows up when prize is won
    public GameObject loseMessage; // Shows up when there are no matches
    public TMP_Text costText;
    public int spinCost; // Cost to spin slots
    public int prize; // Prize for matching colors

    void Start()
    {
        costText.text = spinCost.ToString();
        // Hide all messages initially
        warningMessage.SetActive(false);
        winMessage.SetActive(false);
        loseMessage.SetActive(false);
    }

    public void PlaySlots()
    {
        if (SoulsManager.instance.SpendSouls(spinCost))
        {
            // Randomly assign images to slot windows from their respective pools
            int index1 = Random.Range(0, slotSprites1.Length);
            int index2 = Random.Range(0, slotSprites2.Length);
            int index3 = Random.Range(0, slotSprites3.Length);

            slotWindows[0].sprite = slotSprites1[index1];
            slotWindows[1].sprite = slotSprites2[index2];
            slotWindows[2].sprite = slotSprites3[index3];

            // Check for winning combinations
            CheckWin(index1, index2, index3);
        }
        else
        {
            if (!warningMessage.activeInHierarchy)
            {
                StartCoroutine(ShowMessage(warningMessage));
            }
        }
    }

    private void CheckWin(int index1, int index2, int index3)
    {
        if (index1 == index2 && index2 == index3)
        {
            SoulsManager.instance.AddSouls(prize);
            if (!winMessage.activeInHierarchy)
            {
                StartCoroutine(ShowMessage(winMessage));
            }
        }
        else
        {
            if (!loseMessage.activeInHierarchy)
            {
                StartCoroutine(ShowMessage(loseMessage));
            }
        }
    }

    IEnumerator ShowMessage(GameObject message)
    {
        message.SetActive(true);
        yield return new WaitForSeconds(2);
        message.SetActive(false);
    }
}
