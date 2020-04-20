using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class MainMenu : MonoBehaviour
{
    private bool playPressed;
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField] private Animator fade;
    [SerializeField] private AudioSource audio;

    private void Awake()
    {
        playPressed = false;
    }


    public void PlayPressed()
    {
        if (playPressed) return;
        camera.Priority = 0;
        playPressed = true;
        Invoke("StartGame", 1.5f);
        fade.SetTrigger("FadeOut");
        audio.Play();
    }

    private void StartGame()
    {
        GameData.instance.ResetData();
        SceneManager.LoadScene("LimboScene", LoadSceneMode.Single);
    }
}
