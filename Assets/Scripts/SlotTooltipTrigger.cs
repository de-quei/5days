using UnityEngine;
using UnityEngine.EventSystems;

public class SlotTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite tooltipSprite; 

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipSprite != null && TooltipController.instance != null)
        {
            TooltipController.instance.ShowTooltip(tooltipSprite);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipController.instance != null)
        {
            TooltipController.instance.HideTooltip();
        }
    }
}