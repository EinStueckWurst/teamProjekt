using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class Triggers
{
    public static string MAIN_MENU = "MainMenu";
    public static string MAKE_PHOTO_PANEL = "MakePhotoPanel";
    public static string CAPTURED_PHOTO_PANEL = "CapturedPhotoPanel";
    public static string VIEW_LIGHT_ORIENTATION_PANEL = "ViewLightOrientationPanel";
    public static string LOBBY_PANEL = "LobbyPanel";
    public static string WINNING_PANEL = "WinningPanel";
    public static string INGAME = "Ingame";
}


public class Navigation : MonoBehaviour
{
    [SerializeField] Animator menuAnimator;

    #region Stuff
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

    #endregion


    #region MainMenuePanel
    /** Triggers MainMenue -> AciveButton
     * 
     */
    public void OnActiveButton()
    {
        Debug.Log("Active Button");
        this.menuAnimator.SetTrigger(Triggers.MAKE_PHOTO_PANEL);
    }
    
    /** Triggers MainMenue -> PassiveButton
     * 
     */ 
    public void OnPassivButton()
    {
        Debug.Log("Passive Button");
        this.menuAnimator.SetTrigger(Triggers.LOBBY_PANEL);
    }
    #endregion

    #region MakePhotoPanel
    /** Triggers MakePhotoPanel -> BackButton
     * 
     */
    public void OnMakePhotoBackButton()
    {
        this.menuAnimator.SetTrigger(Triggers.MAIN_MENU);
        Debug.Log("Back To MainMenue");
    }
    
    /** Triggers MakePhotoPanel -> MakePhotoButton
     * 
     */ 
    public void OnMakePhotoButton()
    {
        this.menuAnimator.SetTrigger(Triggers.CAPTURED_PHOTO_PANEL);
        Debug.Log("Make Photo Button");
    }
    #endregion

    #region CapturedPhotoPanel

    /** Triggers CapturedPhotoPanel -> BackButton
     * 
     */
    public void OnCapturedPhotoBackButton()
    {
        this.menuAnimator.SetTrigger(Triggers.MAKE_PHOTO_PANEL);
        Debug.Log("Back To Make Photo");
    }

    /** Triggers CapturedPhotoPanel -> CPU Compute Button
     * 
     */
    public void OnCpuComputeButton()
    {
        Debug.Log("HoughCompute with CPU");
        this.menuAnimator.SetTrigger(Triggers.VIEW_LIGHT_ORIENTATION_PANEL);
    }
    
    /** Triggers CapturedPhotoPanel -> GPU Compute Button
     * 
     */ 
    public void OnGpuComputeButton()
    {
        Debug.Log("HoughCompute with GPU");

        this.menuAnimator.SetTrigger(Triggers.VIEW_LIGHT_ORIENTATION_PANEL);
    }
    #endregion

    #region ViewLightOrientationPanel
    
    /** Triggers ViewLightOrientationPanel -> BackButton
     * 
     */
    public void OnViewLightOrientationBackButton()
    {
        this.menuAnimator.SetTrigger(Triggers.CAPTURED_PHOTO_PANEL);
        Debug.Log("Back To Captured Photo Panel");
    }

    /** Triggers ViewLightOrientationPanel -> SearchButton
     * 
     */
    public void OnSearchButton()
    {
        Debug.Log("Start Searching for Host");
        this.menuAnimator.SetTrigger(Triggers.LOBBY_PANEL);
    }

    /** Triggers ViewLightOrientationPanel -> HostButton
     * 
     */
    public void OnHostButton()
    {
        Debug.Log("Launch Host");
        this.menuAnimator.SetTrigger(Triggers.LOBBY_PANEL);
    }
    #endregion
    
    #region LobbyPanel

    /** Triggers ViewLightOrientationPanel -> BackButton
     * 
     */
    public void OnLobbyPanelActiveBackButton()
    {
        Debug.Log("Back To ViewLightOrientationPanel");
        this.menuAnimator.SetTrigger(Triggers.VIEW_LIGHT_ORIENTATION_PANEL);

    }

    /** Triggers ViewLightOrientationPanel -> BackButton
     * 
     */
    public void OnLobbyPanelPassiveBackButton()
    {
        Debug.Log("Back To Main Menue");
        this.menuAnimator.SetTrigger(Triggers.MAIN_MENU);

    }

    /** Triggers LobbyPanel -> SearchButton
     * 
     */
    public void OnStartGameButton()
    {
        this.menuAnimator.SetTrigger(Triggers.INGAME);
        Debug.Log("START GAme");
    }
    #endregion



















}
