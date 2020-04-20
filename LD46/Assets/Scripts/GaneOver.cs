using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GaneOver : MonoBehaviour
{
    [SerializeField] private Animator fade;

    private void Awake()
    {
        
    }

    public void MenuButtonPressed()
    {
        Invoke("ToMenuScene", 2f);
        fade.SetTrigger("FadeOut");
    }

    private void ToMenuScene()
    {
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }
}
