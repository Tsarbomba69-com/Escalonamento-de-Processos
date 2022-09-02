using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Image imageComp;
    [SerializeField] private Text text;
    [SerializeField] private Text textNormal;


    public void StartGame()
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private IEnumerator LoadLevel(int levelIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            text.text = Math.Truncate(progress * 100) + "%";
            imageComp.fillAmount = progress;
            textNormal.text = "Carregando...";
            yield return null;
        }
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Exited game!");
    }
}
