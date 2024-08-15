using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play the hover sound effect when the button is hovered over
        SoundManager.instance.PlayHoverSound();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Play the click sound effect when the button is clicked
        SoundManager.instance.PlayClickSound();
    }
}
