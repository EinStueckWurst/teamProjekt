using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    /** Loads the Scene by given BuildIndex
     * 
     **/ 
    public void LoadSceneByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    /** Loads the Scene by BuildIndex
     * 
     **/ 
    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if(nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            this.LoadSceneByIndex(nextIndex);
        }
        else
        {
            Debug.Log("BuildIndex is out of Range" 
                + "Tried to Load SceneIndex = " + nextIndex 
                + " ... Max reachable index is " + (SceneManager.sceneCountInBuildSettings - 1));
        }
    }

    /** Quits the Game
     * 
     **/ 
    public void QuitApplication()
    {
        Application.Quit();
    }

    /** Enables MenuScreen
     *
     **/
    public void EnableMenu(GameObject menuToEnable)
    {
        menuToEnable.gameObject.SetActive(true);
    }

    /** Disables MenuScreen
     * 
     **/ 
    public void DisableMenu(GameObject menuToDisable)
    {
        menuToDisable.SetActive(false);
    }
}
