using UnityEngine;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour
{
    public static TooltipController instance;

    private Image tooltipImage;
    private RectTransform rectTransform;

    void Awake()
    {
        instance = this;
        tooltipImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (tooltipImage.gameObject.activeSelf)
        {
            Vector2 movePos = Input.mousePosition;
            movePos.x += 15f;
            movePos.y -= 15f;

            transform.position = movePos;
        }
    }

    public void ShowTooltip(Sprite sprite)
    {
        tooltipImage.sprite = sprite;
        tooltipImage.SetNativeSize(); 
        tooltipImage.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipImage.gameObject.SetActive(false);
    }
}