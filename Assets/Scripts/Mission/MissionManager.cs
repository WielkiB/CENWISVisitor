using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    [System.Serializable]
    public enum GoalValue
    {
        Greater, NotEqual
    }

    [System.Serializable]
    public struct MissionStruct
    {
        public PartManager part;
        public GoalValue goalValue;
        public int condition;
    }

    public TextMeshProUGUI numberText;
    public Image missionImage;
    public TextMeshProUGUI missionTitle;
    public TextMeshProUGUI missionDescription;
    public GameObject missionWindow;
    public GameObject winScreen;
    public PlayerController playerControllerRef;
    public PlayerAiming playerAimingRef;
    public int levelID;

    public GameObject looseScreen;
    public TextMeshProUGUI timerText;
    public float timeRemaining = 180;
    private bool timerIsRunning = false;

    [SerializeField]
    public List<MissionStruct> missionStruct = new();

    public PauseController pauseControllerRef;

    void Start()
    {
        ChangeFreezeState(true);

        missionWindow.SetActive(true);
        GetComponent<AnimateText>().ShowStoryBar();
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI(timeRemaining);
            }
            else
            {
                timerIsRunning = false;
                timerText.gameObject.SetActive(false);

                ChangeFreezeState(true);
                looseScreen.SetActive(true);
            }
        }
    }

    private void UpdateTimerUI(float timeToDisplay)
    {
        timeToDisplay = Mathf.Max(0, timeToDisplay); // Zapewnia, ¿e czas nie bêdzie ujemny
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("Pozosta³y czas: {0:00}:{1:00}", minutes, seconds);
    }

    private void ChangeFreezeState(bool newState)
    {
        if (newState)
        {
            pauseControllerRef.isLocked = true;
            playerAimingRef.enabled = false;
            playerControllerRef.enabled = false;

            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            pauseControllerRef.isLocked = false;
            playerAimingRef.enabled = true;
            playerControllerRef.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void CheckWinState()
    {
        foreach (var task in missionStruct)
        {
            if (!task.part.isMounted)
                return;

            if (task.goalValue == GoalValue.NotEqual)
            {
                if (task.part.partCondition == task.condition)
                    return;
            }
            else
            {
                if (task.part.partCondition <= task.condition)
                    return;
            }
        }

        timerIsRunning = false;
        timerText.gameObject.SetActive(false);

        ChangeFreezeState(true);
        winScreen.SetActive(true);
    }

    public void StartMission()
    {
        ChangeFreezeState(false);
        missionWindow.SetActive(false);

        timerText.gameObject.SetActive(true);
        timerIsRunning = true;
    }

    public void LoadNextLevel()
    {
        if (levelID < 4)
            SceneManager.LoadScene("Level" + (levelID + 2), LoadSceneMode.Single);
    }

    public void ExitToMenu()
    {
        ChangeFreezeState(false);
        winScreen.SetActive(false);
        missionWindow.SetActive(false);

        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
    }
}
