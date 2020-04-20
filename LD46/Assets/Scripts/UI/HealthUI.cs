using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private GameObject hpIcon;

    public void SetHP(int amount)
    {
        Debug.Log("HP: " + amount);

        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < amount; i++)
        {
            Instantiate(hpIcon, transform);
        }
    }
}
