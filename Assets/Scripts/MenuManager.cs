/* Name: Ross A. Metcalf
 * Email: pg09ross@vfs.com
 * Date: 20170219
 * Summary: Menu Manger is used by the MenuScene.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    void Start()
    {
        // Make the mouse visible and moveable so they can click.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Allow player to start game with Space or Return. Alternatively can
        // click the button.
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Return))
            StartGame();
    }

    // Called by button or Update() function.
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}