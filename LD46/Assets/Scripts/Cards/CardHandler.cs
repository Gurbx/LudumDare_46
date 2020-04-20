using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandler : MonoBehaviour
{
    [SerializeField] private Animator handAnimator;
    [SerializeField] private UnitHandler unitHandler;
    [SerializeField] private HandUI handUI;
    [SerializeField] private List<Card> deck; //Temp possibly

    private const int MAX_HAND_SIZE = 10;

    private List<Card> hand;
    private List<Card> discarded;

    void Awake()
    {
        hand = new List<Card>();
        discarded = new List<Card>();
    }

    public void HideCards()
    {
        handAnimator.SetBool("Hidden", true);
    }

    public void DrawCards(int amount, bool discardHand)
    {
        handAnimator.SetBool("Hidden", false);
        if (discardHand)
        {
            discarded.AddRange(hand);
            hand.Clear();
        }

        for (int i = 0; i < amount; i++)
        {
            if (hand.Count >= MAX_HAND_SIZE) break;
            if (deck.Count <= 0)
            {
                ReshuffleDeck();
            }
            int selectedIndex = Random.Range(0, deck.Count - 1);

            hand.Add(deck[selectedIndex]);
            deck.Remove(deck[selectedIndex]);
        }

        handUI.UpdateHandUI(hand);
    }

    private void ReshuffleDeck()
    {
        deck.AddRange(discarded);
        discarded.Clear();
        Debug.Log("Reshuffled Deck");
    }

    public void CardSelected(Card card)
    {
        unitHandler.DisplayActionForAllUnits(card.unitAction);
    }

    public void CardDeselected()
    {
        unitHandler.HideActionMarkersForUnits();
    }

    public void PlayCard(Card card)
    {
        hand.Remove(card);
        discarded.Add(card);
        handUI.UpdateHandUI(hand);

        unitHandler.DoActionWithAllUnits(card.unitAction);
    }

}
