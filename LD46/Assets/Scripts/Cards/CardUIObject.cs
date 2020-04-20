using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUIObject : Image, IPointerEnterHandler, IPointerExitHandler
{
    private HandUI handUI;
    public Card card;

    public void Initialize(Card card, HandUI handUI)
    {
        this.handUI = handUI;
        this.card = card;
        this.sprite = card.image;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        handUI.SetHoveringCard(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        handUI.SetHoveringCard(null);
    }

    public void Hide()
    {
        color = new Color(color.r, color.g, color.b, 0f);
    }

    public void Show()
    {
        color = new Color(color.r, color.g, color.b, 1f);
    }
}
