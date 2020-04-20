using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreHandler : MonoBehaviour
{
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private List<PowerUp> allPowerUps;
    [SerializeField] private Button button1, button2, button3;
    [SerializeField] private Text moneyText;

    private bool b1Pressed, b2Pressed, b3Pressed;

    private PowerUp[] storePowerups;

    private void Awake()
    {
        storePowerups = new PowerUp[3];
        for (int i = 0; i < 3; i++)
        {
            storePowerups[i] = allPowerUps[Random.Range(0, allPowerUps.Count)];
        }

        button1.image.sprite = storePowerups[0].sprite;
        button1.GetComponentInChildren<Text>().text = storePowerups[0].name + "\n" + storePowerups[0].cost;

        button2.image.sprite = storePowerups[1].sprite;
        button2.GetComponentInChildren<Text>().text = storePowerups[1].name + "\n" + storePowerups[1].cost;

        button3.image.sprite = storePowerups[2].sprite;
        button3.GetComponentInChildren<Text>().text = storePowerups[2].name + "\n" + storePowerups[2].cost;

        moneyText.text = "Money: " + GameData.instance.money;
    }

    public void Button1Pressed()
    {
        if (b1Pressed) return;
        if (storePowerups[0].cost > GameData.instance.money) return;
        b1Pressed = true;
        button1.GetComponent<Animator>().SetTrigger("Pressed");
        button1.enabled = false;
        GameData.instance.health += storePowerups[0].extraHealth;
        GameData.instance.numberOfUnits += storePowerups[0].extraUnits;
        buttonSound.Play();
        PurchaseMade(storePowerups[0].cost);
    }

    public void Button2Pressed()
    {
        if (b2Pressed) return;
        if (storePowerups[1].cost > GameData.instance.money) return;
        b2Pressed = true;
        button2.GetComponent<Animator>().SetTrigger("Pressed");
        button2.enabled = false;
        GameData.instance.health += storePowerups[1].extraHealth;
        GameData.instance.numberOfUnits += storePowerups[1].extraUnits;
        buttonSound.Play();
        PurchaseMade(storePowerups[1].cost);
    }

    public void Button3Pressed()
    {
        if (b3Pressed) return;
        if (storePowerups[2].cost > GameData.instance.money) return;
        b3Pressed = true;
        button3.GetComponent<Animator>().SetTrigger("Pressed");
        button3.enabled = false;
        GameData.instance.health += storePowerups[2].extraHealth;
        GameData.instance.numberOfUnits += storePowerups[2].extraUnits;
        buttonSound.Play();
        PurchaseMade(storePowerups[2].cost);
    }

    private void PurchaseMade(int cost)
    {
        GameData.instance.money -= cost;
        moneyText.text = "Money: " + GameData.instance.money;
    }
}
