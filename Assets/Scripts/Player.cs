// Created by Austin Patel on 5/15/16 at 8:13 PM

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.Text;
using System.IO;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    private bool atHome = true, actAndroid = false;

    private const float SPEED = 0.2f, BULLET_SPEED = 20, BULLET_POWER_DURATION = 300, FRAME_DURATION = 10;
    private const int LOWEST_SHOOT_DELAY = 5, INITAL_SHOOT_DELAY = 20;
    private string SCHOOL_DATA_PATH = "T:\\Aberrations Public\\Aberrations_Data_Save\\SaveData.txt";
    private string HOME_DATA_PATH = "C:\\Users\\Austin Patel\\Documents\\Programming\\Unity\\Aberrations\\Sava Data\\data.txt";
    private string DATA_PATH;
    public GameObject[] children;
    public GameObject bullet;
    private Camera cam;
    private GameObject texture;
    private Vector3 posDelta;
    private Rigidbody2D rigidBody2D;
    private Vector3 newBulletLocation;
    private GameObject bullets;
    private float rotation;
    private Vector2 noVelocity;
    private Transform spawn;
    private int lives;
    private CanvasManager canvasManager;
    private int curShootDelay;
    private int shootDelay = INITAL_SHOOT_DELAY;
    private int bulletPowerUpCurTime;
    private bool fastShoot;
    public Sprite[] walkingAnim;
    private SpriteRenderer spriteRenderer;
    private int curFrameTime, curFrameIndex;
    private int xp;
    private float deltaX, deltaY;
    private GameObject restartButton, leaderBoardTitle, leaderboardContent, finalScore, startButton, joysticks, quitGameButton, pausedTitle;
    private GameObject mobileScore, mobileHighScore;
    private AudioSource audioSource;
    private GameObject heart1;
    private GameObject heart2;
    private GameObject heart3;
    private GameObject scoreObj;
    private GameObject xpObj;
    private GameObject pauseButton;
    private int numberOfBullets = 1;
    public GameObject buttonSound, startGameSound;
    private RuntimePlatform platform;
    private Vector3 quitGameStartPosition;

    void Start () {
        mobileScore = GameObject.Find("CurScoreMobile");
        mobileHighScore = GameObject.Find("HighScoreMobile");

        canvasManager = GameObject.Find("Canvas").GetComponent<CanvasManager>();

        platform = Application.platform;
        if (actAndroid) platform = RuntimePlatform.Android;

        if (platform != RuntimePlatform.Android)
        {
            Destroy(GameObject.Find("Joysticks"));
            Destroy(mobileScore);
            Destroy(mobileHighScore);
        }
        else
        {
            joysticks = GameObject.Find("Joysticks");
            GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize = 8;         
            mobileScore.SetActive(false);
            mobileHighScore.SetActive(false);
        }

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        texture = gameObject.transform.Find("Texture").gameObject;
        spriteRenderer = texture.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = walkingAnim[0];
        posDelta = new Vector3(0,0,0);
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        bullets = GameObject.Find("Bullets");
        noVelocity = new Vector2();
        spawn = GameObject.Find("Spawn").transform;
        transform.position = spawn.transform.position;
        lives = 3;

        restartButton = GameObject.Find("Restart");
        restartButton.SetActive(false);

        leaderBoardTitle = GameObject.Find("Leaderboard Title");
        leaderBoardTitle.SetActive(false);

        leaderboardContent = GameObject.Find("Leaderboard");
        leaderboardContent.SetActive(false);

        finalScore = GameObject.Find("Final Score");
        finalScore.SetActive(false);

        heart1 = GameObject.Find("Heart 1");
        heart2 = GameObject.Find("Heart 2");
        heart3 = GameObject.Find("Heart 3");
        scoreObj = GameObject.Find("Score");
        xpObj = GameObject.Find("XP");
        pauseButton = GameObject.Find("Pause");
        quitGameButton = GameObject.Find("Quit Game");
        pausedTitle = GameObject.Find("Paused");
        heart1.SetActive(false);
        heart2.SetActive(false);
        heart3.SetActive(false);
        scoreObj.SetActive(false);
        xpObj.SetActive(false);
        pauseButton.SetActive(false);
        pausedTitle.SetActive(false);

        audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();

        startButton = GameObject.Find("Start Game");
        startButton.SetActive(false);
        Time.timeScale = 0;
        startButton.SetActive(true);

        if (atHome)
            DATA_PATH = HOME_DATA_PATH;
        else
            DATA_PATH = SCHOOL_DATA_PATH;

        quitGameStartPosition = quitGameButton.GetComponent<RectTransform>().position;
        quitGameButton.SetActive(true);
    }

    public void StartGame()
    {
        canvasManager.StartGame();
        if (!canvasManager.getIsMuted())
        {
            GameObject newSound = (GameObject)Instantiate(startGameSound, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            newSound.GetComponent<AudioSource>().Play();
        }
        buttonClick();
        startButton.SetActive(false);
        Time.timeScale = 1;

        heart1.SetActive(true);
        heart2.SetActive(true);
        heart3.SetActive(true);
        scoreObj.SetActive(true);
        xpObj.SetActive(true);
        pauseButton.SetActive(true);
        GameObject.Find("Title").SetActive(false);
        //if (platform == RuntimePlatform.Android) joysticks.SetActive(true);
        quitGameButton.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
        quitGameButton.SetActive(false);
        pausedTitle.SetActive(false);
    }

    public void buttonPause()
    {
        buttonClick();
        Pause();
    }

    public void Pause()
    {
        if (leaderboardContent.activeSelf) return;
        if (startButton.activeSelf) return;
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;

            if (!canvasManager.getIsMuted()) audioSource.UnPause();
            quitGameButton.SetActive(false);
            pausedTitle.SetActive(false);
            pauseButton.transform.FindChild("Text").GetComponent<Text>().text = "pause";
        }
        else
        {
            Time.timeScale = 0;
            audioSource.Pause();
            quitGameButton.SetActive(true);
            pausedTitle.SetActive(true);
            pauseButton.transform.FindChild("Text").GetComponent<Text>().text = "Unpause";
        }
    }

    void FixedUpdate() {
        HandleInput();
        PointTowardsMouse();
        UpdateChildren();

        rigidBody2D.velocity = noVelocity;

        curShootDelay++;
        if (curShootDelay == shootDelay)
        {
            Shoot();
            curShootDelay = 0;
        }

        if (fastShoot)
        {
            bulletPowerUpCurTime++;
            if (bulletPowerUpCurTime == BULLET_POWER_DURATION)
            {
                shootDelay += 10;
                curShootDelay = 0;
                fastShoot = false;
            }
        }
    }

    void WalkAnim()
    {
        curFrameTime++;
        if (curFrameTime == FRAME_DURATION)
        {
            curFrameTime = 0;
            curFrameIndex++;
            if (curFrameIndex == walkingAnim.Length)
                curFrameIndex = 0;
            spriteRenderer.sprite = walkingAnim[curFrameIndex];
        }
    }

    void HandleInput()
    {
        // Player movement
        posDelta.Set(0, 0, 0);

        if (platform != RuntimePlatform.Android)
            posDelta.Set(Input.GetAxis("Horizontal") * SPEED, Input.GetAxis("Vertical") * SPEED, 0);
        else
        {
            posDelta.Set(CrossPlatformInputManager.GetAxis("Horizontal") * SPEED, CrossPlatformInputManager.GetAxis("Vertical") * SPEED, 0);
        }

        if (posDelta.x != 0 || posDelta.y != 0)
            WalkAnim();
        else
        {
            spriteRenderer.sprite = walkingAnim[0];
            curFrameIndex = 0;
            curFrameTime = 0;
        }

        rigidBody2D.transform.Translate(posDelta);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            Pause();
    }

    private void buttonClick()
    {
        if (canvasManager.getIsMuted()) return;
        GameObject newSound = (GameObject) Instantiate(buttonSound, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        newSound.GetComponent<AudioSource>().Play();
    }

    void Shoot()
    {
        float angle = 30;
        float curShootRotation = rotation - angle / 2 * (numberOfBullets - 1);
        for (int i = 0; i < numberOfBullets; i++)
        {
            newBulletLocation = new Vector3(transform.position.x, transform.position.y, bullet.transform.position.z);
            GameObject newBullet = (GameObject)Instantiate(bullet, newBulletLocation, Quaternion.Euler(0, 0, curShootRotation));

            float velX = BULLET_SPEED * Mathf.Cos(curShootRotation * Mathf.Deg2Rad);
            float velY = BULLET_SPEED * Mathf.Sin(curShootRotation * Mathf.Deg2Rad);

            newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(velX, velY);
            newBullet.transform.parent = bullets.transform; // Put all the bullets under a single parent
            Physics2D.IgnoreCollision(newBullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            curShootRotation += angle;
        }
    }

    // Points the player towards the mouse
    void PointTowardsMouse() {
        if (platform != RuntimePlatform.Android)
        {
            Vector2 mousePosition = Input.mousePosition;
            float mouseX = mousePosition.x;
            float mouseY = mousePosition.y;

            Vector2 playerPosition = cam.WorldToScreenPoint(gameObject.transform.position);
            float playerX = playerPosition.x;
            float playerY = playerPosition.y;

            deltaX = mouseX - playerX;
            deltaY = mouseY - playerY;       
        } else if (platform == RuntimePlatform.Android)
        {
            float lastDeltaX = deltaX;
            float lastDeltaY = deltaY;
            
            deltaX = CrossPlatformInputManager.GetAxis("LookX");
            deltaY = CrossPlatformInputManager.GetAxis("LookY");

            if (deltaX == 0 && deltaY == 0)
            {
                deltaX = lastDeltaX;
                deltaY = lastDeltaY;
            }
        }
        rotation = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;
        
        texture.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotation));
    }

    // Keeps all the children at the same location as the player, but preserving their z location
    void UpdateChildren()
    {
        foreach (GameObject obj in children)
        {
            Vector3 pos = gameObject.transform.position;
            pos.Set(pos.x, pos.y, obj.transform.position.z);
            obj.transform.position = pos;
        }
    }

    void saveScore()
    {
        int playerScore = canvasManager.getScore();

        if (platform == RuntimePlatform.Android)
        {
            //Destroy(joysticks);
            Destroy(finalScore);

            if (playerScore > PlayerPrefs.GetInt("highScore"))
                PlayerPrefs.SetInt("highScore", playerScore);

            mobileHighScore.SetActive(true);
            mobileHighScore.GetComponent<Text>().text = "High Score: " + PlayerPrefs.GetInt("highScore");

            mobileScore.SetActive(true);
            mobileScore.GetComponent<Text>().text = "Last Score: " + playerScore;

            return;
        }
        string playerName = Environment.UserName;

        ArrayList scoreData = ReadFile(DATA_PATH);

        int index = 0;
        bool success = false;
        foreach (string item in scoreData)
        {
            string curName = item.Substring(0, item.IndexOf(":"));
            if (curName.Equals(playerName))
            {
                success = true;
                int curScore = 0;
                int.TryParse(item.Substring(item.IndexOf(":") + 2), out curScore);
                if (playerScore > curScore)
                {
                    finalScore.GetComponent<Text>().text = "New Highscore: ";
                    scoreData.RemoveAt(index);
                    addScoreInOrder(scoreData, playerScore, playerName, true);
                    break;
                }
            }
            index++;
        }

        if (!success)
            addScoreInOrder(scoreData, playerScore, playerName, false);

        string dataString = "";
        foreach (string item in scoreData)
            dataString += item + Environment.NewLine;

        if (scoreData.Count == 0)
            dataString = playerName + ": " + playerScore;

        File.WriteAllText(DATA_PATH, dataString);

        // Update the leaderboard
        string topTen = "";
        for (int i = 0; i < scoreData.Count; i++)
        {
            if (i == 10) break;
            topTen += scoreData[i].ToString() + Environment.NewLine;
        }

        leaderboardContent.GetComponent<Text>().text = topTen;
    }

    void addScoreInOrder(ArrayList scoreData, int playerScore, string playerName, bool nameAlreadyIn)
    {
        int[] scores = new int[scoreData.Count];
        int index2 = 0;
        foreach (string item in scoreData)
        {
            int value;
            int.TryParse(item.Substring(item.IndexOf(":") + 1), out value);
            print(value);
            scores[index2] = value;
            index2++;
        }

        int playerScoreIndex = 0;
        bool show = false;
        for (int i = 0; i < scores.Length; i++)
        {
            if (playerScore > scores[i] && !show)
            {
                playerScoreIndex = i;
                show = true;
            }
            else if (i == scores.Length - 1 && !show)
            {
                playerScoreIndex = scores.Length;
            }
        }
        scoreData.Insert(playerScoreIndex, playerName + ": " + playerScore);
    }

    private ArrayList ReadFile(string filePath)
    {
        ArrayList data = new ArrayList();
        try
        {
            string line;
            StreamReader theReader = new StreamReader(filePath, Encoding.Default);

            using (theReader)
            {
                do
                {
                    line = theReader.ReadLine();

                    if (line != null)
                        data.Add(line);
                } while (line != null);
                theReader.Close();
                return data;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new ArrayList();
        }
    }

    public void loseLife()
    {
        lives -= 1;

        if (lives == 0)
        {
            GameObject.Find("Player Light").SetActive(false);
            GameObject outsideLight = GameObject.Find("Outside Light");
            outsideLight.SetActive(true);
            outsideLight.GetComponent<Light>().intensity = 1.0f;

            saveScore();
            
            Pause();
            pausedTitle.SetActive(false);
            quitGameButton.GetComponent<RectTransform>().position = quitGameStartPosition;
            Destroy(GameObject.Find("Pause"));
            restartButton.SetActive(true);
            leaderBoardTitle.SetActive(true);
            leaderboardContent.SetActive(true);
            finalScore.SetActive(true);
            finalScore.GetComponent<Text>().text += canvasManager.getScore();
            if (!canvasManager.getIsMuted()) audioSource.UnPause();
        }
        canvasManager.updatePlayerLives(lives);
    }

    public void restart()
    {
        PlayerPrefs.Save();
        buttonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int getLives()
    {
        return lives;
    }

    public void quit()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag.Equals("Powerup"))
        {
            string name = coll.gameObject.name;
            if (name.Contains("XpPowerUp"))
            {
                canvasManager.updateXP(1);
                xp++;
                
                if (xp % 5 == 0)
                    numberOfBullets++;
            }
            else if (name.Contains("BulletPowerUp"))
            {
                if (fastShoot)
                    shootDelay += 10;

                bulletPowerUpCurTime = 0;
                shootDelay -= 10;
                if (shootDelay < LOWEST_SHOOT_DELAY) shootDelay = LOWEST_SHOOT_DELAY;
                fastShoot = true;
                curShootDelay = 0;
            }
            else if (name.Contains("HeartPowerUp") && lives < 3)
            {
                lives++;
                canvasManager.updatePlayerLives(lives);
            }
            
            Destroy(coll.gameObject);           
        }
    }
}
