using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Image targetImage; 
    public float rotationSpeed = 1f; 
    private float currentHue = 0f;

    public GameObject MainMenu;
    public GameObject PlayMenu;
    public GameObject LevelsScreen;

    void Update()
    {
        if (targetImage == null) return;

        currentHue += rotationSpeed * Time.deltaTime;
        currentHue %= 360f;

        Color currentColor = targetImage.color;
        float h, s, v;
        ColorConverting.RGBToHSV(currentColor, out h, out s, out v);

        h = currentHue / 360f;
        Color newColor = ColorConverting.HSVToRGB(h, s, v);
        targetImage.color = newColor;
    }

    public void PlayButton()
    {
        MainMenu.SetActive(false);
        PlayMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        PlayMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void StartSwobodny()
    {
        SceneManager.LoadScene("Cenwis", LoadSceneMode.Single);
    }

    public void StartFabula()
    {
        PlayMenu.SetActive(false);
        LevelsScreen.SetActive(true);
    }

    public void BackToGamemode()
    {
        LevelsScreen.SetActive(false);
        PlayMenu.SetActive(true);
    }

    public void StartLevel(int lvl)
    {
        SceneManager.LoadScene("Level" + lvl, LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
