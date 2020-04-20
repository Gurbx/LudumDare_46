using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private AudioSource nextTurnButton;
    [SerializeField] private Animator endTurnButton;
    [SerializeField] private Animator infoPanel;
    [SerializeField] private Text infoText;
    [SerializeField] private UnitHandler unitHandler;
    [SerializeField] private CardHandler cardHandler;
    [SerializeField] private EnemyHandler enemyHandler;
    [SerializeField] private HutHandler hutHandler;

    public int turnNumber;

    private bool canEndTurn;

    private bool placementDone;

    private void Awake()
    {
        placementDone = false;
        canEndTurn = false;
    }


    void Start()
    {
        endTurnButton.SetBool("IsVisible", false);
        cardHandler.HideCards();

        List<string> tutorialTexts = GameData.instance.input.tutorialTexts;
        if (tutorialTexts == null || tutorialTexts.Count == 0)
        {
            StartPlacement();
        }
        else
        {
            TutorialTexts();
        }
    }

    private void StartPlacement()
    {
        SetInfoText("Place your leucoytes");
        unitHandler.HandleUnitPlacments(this);
    }

    int tutorialNr = 0;
    private void TutorialTexts()
    {
        List<string> tutorialTexts = GameData.instance.input.tutorialTexts;
        SetInfoText(tutorialTexts[tutorialNr], 4.5f);
        tutorialNr++;

        if (tutorialNr == tutorialTexts.Count)
        {
            Invoke("StartPlacement", 5.5f);
        }
        else
        {
            Invoke("TutorialTexts", 5.5f);
        }
    }


    public void PlacementDone()
    {
        HideInfoText();
        canEndTurn = true;
        Invoke("EndTurn", 1.5f);
    }

    public void EndTurn()
    {
        if (!canEndTurn) return;
        canEndTurn = false;
        endTurnButton.SetBool("IsVisible", false);
        cardHandler.HideCards();
        SetInfoText("The virus is on the move");
        Invoke("EnemyActions", 1f);
        nextTurnButton.Play();
    }

    private void EnemyActions()
    {
        float enemyTurnDuration = enemyHandler.DoEnemyActions();
        hutHandler.IncrementTurn();
        Invoke("NewTurn", enemyTurnDuration);
    }

    public void NewTurn()
    {
        HideInfoText();
        cardHandler.DrawCards(5, true);
        turnNumber++;
        canEndTurn = true;
        endTurnButton.SetBool("IsVisible", true);
    }


    // Info text
    private void SetInfoText(string text)
    {
        infoText.text = text;
        infoPanel.SetBool("IsVisible", true);
    }

    private void SetInfoText(string text, float duration)
    {
        infoText.text = text;
        infoPanel.SetBool("IsVisible", true);
        Invoke("HideInfoText", duration);
    }

    private void HideInfoText()
    {
        infoPanel.SetBool("IsVisible", false);
    }
}
