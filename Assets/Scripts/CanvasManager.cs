// Created by Austin Patel on 5/26/16 at 6:51 PM

using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {
    private const int HEART_WIDTH = 19, HEART_HEIGHT = 18;
    private GameObject heart1;
    private GameObject heart2;
    private GameObject heart3;
    private Text scoreObj;
    private Text xpObj;
    private int score, xp;
    private GameObject settings;
    private bool settingsEnabled, isMuted, helpEnabled;
    private GameObject startButton, title, muteButton, helpButton, helpContent;
    public AudioSource backgroundMusic;

    void Start()
    {
        heart1 = GameObject.Find("Heart 1");
        heart2 = GameObject.Find("Heart 2");
        heart3 = GameObject.Find("Heart 3");
        scoreObj = GameObject.Find("Score").GetComponent<Text>();
        xpObj = GameObject.Find("XP").GetComponent<Text>();
        settings = GameObject.Find("Settings");
        startButton = GameObject.Find("Start Game");
        title = GameObject.Find("Title");
        helpButton = GameObject.Find("Help");
        muteButton = GameObject.Find("Mute");
        muteButton.SetActive(false);
        helpContent = GameObject.Find("HelpContent");
        helpContent.SetActive(false);

        backgroundMusic.Pause();

        if (PlayerPrefs.GetInt("muted") == 0)
        {
            isMuted = false;
            backgroundMusic.UnPause();
        }
        else
        {
            isMuted = true;
            muteButton.transform.Find("Text").GetComponent<Text>().text = "Unmute";
        }
    }

    public void Help()
    {
        if (helpEnabled)
        {
            helpEnabled = false;
            helpButton.transform.Find("Text").GetComponent<Text>().text = "Help";
            helpContent.SetActive(false);
            startButton.SetActive(true);
            settings.SetActive(true);
            title.GetComponent<Text>().text = "Aberrations";
        }
        else
        {
            helpEnabled = true;
            helpButton.transform.Find("Text").GetComponent<Text>().text = "Back";
            helpContent.SetActive(true);
            startButton.SetActive(false);
            settings.SetActive(false);
            title.GetComponent<Text>().text = "Help";
        }
    }

    public void Settings()
    {
        if (settingsEnabled)
        {
            settings.transform.Find("Text").GetComponent<Text>().text = "Settings";
            startButton.SetActive(true);
            title.GetComponent<Text>().text = "Aberrations";
            muteButton.SetActive(false);
            settingsEnabled = false;
            helpButton.SetActive(true);
        }
        else
        {
            settingsEnabled = true;
            startButton.SetActive(false);
            settings.transform.Find("Text").GetComponent<Text>().text = "Back";
            title.GetComponent<Text>().text = "Settings";
            muteButton.SetActive(true);
            helpButton.SetActive(false);
        }
    }

    public bool getIsMuted()
    {
        return isMuted;
    }

    public void Mute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            backgroundMusic.Pause();
            PlayerPrefs.SetInt("muted", 1);
            muteButton.transform.Find("Text").GetComponent<Text>().text = "Unmute";
        }
        else
        {
            backgroundMusic.UnPause();
            PlayerPrefs.SetInt("muted", 0);
            muteButton.transform.Find("Text").GetComponent<Text>().text = "Mute";
        }
    }

    public void StartGame()
    {
        settings.SetActive(false);
        helpButton.SetActive(false);
    }

    public void updatePlayerLives(int lives)
    {
        switch (lives)
        {
            case 0:
                Destroy(heart1);
                Destroy(heart2);
                Destroy(heart3);
                Destroy(xpObj);
                Destroy(scoreObj);
                break;
            case 1:
                heart1.SetActive(true);
                heart2.SetActive(false);
                heart3.SetActive(false);
                break;

            case 2:
                heart1.SetActive(true);
                heart2.SetActive(true);
                heart3.SetActive(false);
                break;

            case 3:
                heart1.SetActive(true);
                heart2.SetActive(true);
                heart3.SetActive(true);
                break;
        }
    }

    public void updateScore(int increment)
    {
        score += increment;

        scoreObj.text = "Score: " + score.ToString();
    }

    public void updateXP(int increment)
    {
        xp += increment;

        xpObj.text = "XP: " + xp.ToString();
    }

    public int getScore() {
        return score;
    }
}
