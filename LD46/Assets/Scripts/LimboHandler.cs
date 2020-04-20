using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LimboHandler : MonoBehaviour
{
    [SerializeField] private Animator fade;
    [SerializeField] private AudioSource audio;
    private bool changinScene;

    private void Awake()
    {
        changinScene = false;
        if (GameData.instance.input != null)
        {
            if (GameData.instance.input.offerRandomReward)
            {
                HandleReward();
            } else
            {
                InstantSwitch();
            }
        }
        else
        {
            InstantSwitch();
        }
    }

    private void InstantSwitch()
    {
        GameData.instance.IncrementLevel();
        PlayScene();
    }

    private void HandleReward()
    {
        Debug.Log("HandleReward");
    }

    public void SwitchToPlay()
    {
        if (changinScene) return;
        changinScene = true;
        audio.Play();
        GameData.instance.IncrementLevel();
        Invoke("PlayScene", 2f);
        fade.SetTrigger("FadeOut");
    }


    public void PlayScene()
    {
        SceneManager.LoadScene("PlayScene", LoadSceneMode.Single);
    }

}
