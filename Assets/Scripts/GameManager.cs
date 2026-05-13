/* Name: Ross A. Metcalf
 * Email: pg09ross@vfs.com
 * Date: 20170219
 * Summary: GameManager switches Canvases when for pause and death
 *          menus. Tracks scores and can reload GameScene or
 *          MenuScene.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static int playerHealth;
    public static int maxPlayerHealth = 100;
    public static int ammoCount;
    public static int maxPlayerAmmo = 10;
    public static int deathYPos = -50; // Anything below here is Destroyed.

    // UI Overlay texts.
    public Text healthText;
    public Text ammoText;

    // Pause & Death menu coutners, texts & canvases.
    public Text ovenantDestroyedText;
    public Text cheatMealsText;
    public Text mealsServedText;
    public Canvas scoreCanvas;
    public Canvas pausedCanvas;
    public Canvas deadCanvas;
    public static int ovenantDestroyedCounter; // Enemies killed.
    public static int cheatMealsCounter; // Health packs picked up.
    public static int mealsServedCounter; // Shots fired.

    public static bool gameOver;
    public static bool gamePaused;

    private Player playerObject;
    private int layerOffset = 2; // Used when layering menu Canvases.

    void Start () {
        playerObject = FindObjectOfType<Player>();
        // Reset static variables.
        gameOver = false;
        gamePaused = false;
        Time.timeScale = 1;
        playerHealth = maxPlayerHealth;
        ammoCount = maxPlayerAmmo;
        ovenantDestroyedCounter = 0;
        cheatMealsCounter = 0;
        mealsServedCounter = 0;
	}
	
    // Main game loop gets stopped, paused, or resumed inside here.
	void Update () {
        // If game is over, display death Canvas, check for input & return. 
        if (gameOver)
        {
            DisplayMenu(deadCanvas);
            if (Input.GetKeyDown(KeyCode.Escape))
                Restart();
            else if (Input.GetKeyDown(KeyCode.Return))
                MainMenu();
            return;
        }
 
        // If game is paused, display pause Canvas check for input & return.
        if (gamePaused)
        {
            DisplayMenu(pausedCanvas);
            if (Input.GetKeyDown(KeyCode.Escape))
                UnPause();
            else if (Input.GetKeyDown(KeyCode.Return))
                MainMenu();
            return;
        }

        ////// Below is the MAIN, unpaused, game loop. //////

        // Perform cursor lock in Update() so if player switches
        // windows and returns it will still be hidden.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Check if player paused the game.
        if (Input.GetKeyDown(KeyCode.Escape))
            gamePaused = !gamePaused;

        // Check if player should die because of low health.
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            playerObject.Die("MasterChef"); // Calls Die() in Unit.
        }

        // Update gameplay UI text fields.
        healthText.text = "Health: " + playerHealth.ToString();
        ammoText.text = "Ammo: " + ammoCount.ToString() + "/10";
    }

    // Called in death menu to restart game.
    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Called in death & pause menu to return to main menu.
    public void MainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    // Called in pause menu to unpause the game.
    public void UnPause()
    {
        gamePaused = false;
        pausedCanvas.sortingOrder = -layerOffset;
        scoreCanvas.sortingOrder = -layerOffset;
        Time.timeScale = 1;
    }

    // Called when pause or death menu appears.
    void DisplayMenu( Canvas canvas)
    {
        Time.timeScale = 0;
        ovenantDestroyedText.text = ovenantDestroyedCounter.ToString();
        cheatMealsText.text = cheatMealsCounter.ToString();
        mealsServedText.text = mealsServedCounter.ToString();
        canvas.sortingOrder = layerOffset;
        // Score Canvas goes on top of base canvas because base canvas changes
        // depending on if its the death or pause menu.
        scoreCanvas.sortingOrder = layerOffset * 2; 
    }
}
