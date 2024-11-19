using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI controlText;
    [SerializeField] private Button playButton;
    [SerializeField] private Button instructionButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;
    public void OnPlayButton(){
        SceneManager.LoadScene(1);
    }
    public void OnQuitButton(){
        Application.Quit();
    }

    public void OnInstructionsButton(){
        //swap to the instruction menu
        titleText.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(true);
        controlText.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
        instructionButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
    }

    public void OnBackButton(){
        //swap to the title menu
        titleText.gameObject.SetActive(true);
        instructionText.gameObject.SetActive(false);
        controlText.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);
        instructionButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
    }
}
