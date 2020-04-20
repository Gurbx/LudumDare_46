using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUI : MonoBehaviour
{
    [SerializeField] private Animator cardAtMouseAnimator;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private CardHandler cardHandler;
    [SerializeField] private int cardsPlayedWhenDroppedAboveHeight;
    [SerializeField] private int cardsPlayedWhenDroppedBelowHeight;
    [SerializeField] Image cardImageAtMouse;

    private bool isCardSelected;

    private CardUIObject hoverCard;

    void Awake()
    {
        cardImageAtMouse.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isCardSelected) PlayCard();
            else SelectCard();
        }
        if (Input.GetMouseButtonDown(1))
        {
            cardImageAtMouse.enabled = false;
            DeselectCard();
        }

        if (isCardSelected)
        {
            cardImageAtMouse.transform.position = Input.mousePosition;
        }

    }

    private void PlayCard()
    {
        if (hoverCard == null || !isCardSelected) return;
        if (Input.mousePosition.y >= cardsPlayedWhenDroppedAboveHeight && Input.mousePosition.y <= cardsPlayedWhenDroppedBelowHeight)
        {
            cardHandler.PlayCard(hoverCard.card);
            cardAtMouseAnimator.SetTrigger("PlayCard");
            Invoke("HideCardImageAtMouse", 0.3f);
            DeselectCard();
        }
        else DeselectCard();
    }

    private void SelectCard()
    {
        if (hoverCard == null) return; //No card hovering over that is selectable
        cardImageAtMouse.enabled = true;
        hoverCard.Hide();
        isCardSelected = true;

        cardHandler.CardSelected(hoverCard.card);
    }


    private void DeselectCard()
    {
        //cardImageAtMouse.enabled = false;
        if (hoverCard != null) hoverCard.Show();
        isCardSelected = false;

        cardHandler.CardDeselected();
    }

    private void HideCardImageAtMouse()
    {
        cardImageAtMouse.enabled = false;
    }


    public void UpdateHandUI(List<Card> hand)
    {
        // Remove previous children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        // Add new gameobject children
        foreach (var c in hand)
        {
            var card = Instantiate(cardPrefab, transform);
            card.GetComponent<CardUIObject>().Initialize(c, this);
        }
    }

    public void SetHoveringCard(CardUIObject cardObj)
    {
        if (isCardSelected) return;
        if (hoverCard != null) hoverCard.Show();

        hoverCard = cardObj;
        if (cardObj != null)
        {
            cardImageAtMouse.sprite = cardObj.card.image;
        }
    }
}
