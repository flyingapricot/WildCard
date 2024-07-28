using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextChanger : MonoBehaviour
{

    private float dialogRate = 5f; // Seconds between each shot
    private float nextDialog = 0f;
    public GameObject LeftSide;
    public GameObject RightSide;
    public GameObject LeftImage;
    public GameObject RightImage;
    public Sprite WASD;
    public Sprite ESC;
    public Sprite Timer;
    public Sprite pickup;
    public Sprite weapon;
    public Sprite health;
    static int count = 0;
    //Rightside is leftside
    //leftside is rightside
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Time.time >= nextDialog)
        {
            count++;
            nextDialog = Time.time + dialogRate;
            UpdateDialog(count);
        }

    }

    void UpdateDialog(int count)
    {
        if(count < 5)
        {
            switch(count)
            {
                case 1:
                    LeftSide.GetComponent<TMP_Text>().text = "Press the Esc Key \r\nto Pause the game & \r\nsee your Current Stats";
                    RightImage.GetComponent<Image>().sprite = WASD;
                    RightSide.GetComponent<TMP_Text>().text = "Use WASD or Arrow Keys\r\nto move around";
                    LeftImage.GetComponent<Image>().sprite = ESC;
                    break;
                case 2:
                    LeftSide.GetComponent<TMP_Text>().text = "InGame Timer tells \r\nyou how long \r\nyou have survived.";
                    RightImage.GetComponent<Image>().sprite = pickup;
                    RightSide.GetComponent<TMP_Text>().text = "Enemies drop XP Points \r\nwhen they are killed which can be collected.";
                    LeftImage.GetComponent<Image>().sprite = Timer;
                    break;
                case 3:
                    LeftSide.GetComponent<TMP_Text>().text = "Choose extra weapons/items to equip when levelling up";
                    RightImage.GetComponent<Image>().sprite = health;
                    RightSide.GetComponent<TMP_Text>().text = "When health reaches 0, game ends.";
                    LeftImage.GetComponent<Image>().sprite = weapon;
                    break;
                case 4:
                    //Activate return to main button/return to main menu
                    break;
            }
            
        }
    }
}
