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

    [SerializeField] UserConfiguration myUserConfig;
    [SerializeField] CameraController makePhotoPanelCameraController;
    [SerializeField] Calibration capturedPhotoPanelCalibration;
    [SerializeField] Lighting viewLightOrientationPanelLighting;
    [SerializeField] Client client;
    [SerializeField] Server server;

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
        this.menuAnimator.SetTrigger(Triggers.MAKE_PHOTO_PANEL);
        this.myUserConfig.setActive();
        this.makePhotoPanelCameraController.Init();
    }
    
    /** Triggers MainMenue -> PassiveButton
     * 
     */ 
    public void OnPassivButton()
    {
        this.menuAnimator.SetTrigger(Triggers.LOBBY_PANEL);
        this.myUserConfig.setPassive();
        Debug.Log("HELLO");
    }
    #endregion

    #region MakePhotoPanel
    /** Triggers MakePhotoPanel -> BackButton
     * 
     */
    public void OnMakePhotoBackButton()
    {
        this.menuAnimator.SetTrigger(Triggers.MAIN_MENU);
        this.makePhotoPanelCameraController.DeactivateCamera();
    }
    
    /** Triggers MakePhotoPanel -> MakePhotoButton
     * 
     */ 
    public void OnMakePhotoButton()
    {
        this.menuAnimator.SetTrigger(Triggers.CAPTURED_PHOTO_PANEL);
        this.makePhotoPanelCameraController.TakePhoto();
    }
    #endregion

    #region CapturedPhotoPanel

    /** Triggers CapturedPhotoPanel -> BackButton
     * 
     */
    public void OnCapturedPhotoBackButton()
    {
        this.menuAnimator.SetTrigger(Triggers.MAKE_PHOTO_PANEL);
        this.makePhotoPanelCameraController.Init();
    }

    /** Triggers CapturedPhotoPanel -> CPU Compute Button
     * 
     */
    public void OnCpuComputeButton()
    {
        this.capturedPhotoPanelCalibration.CalibrateWithCPU();
        this.viewLightOrientationPanelLighting.orientLightDirection();
        this.viewLightOrientationPanelLighting.saveLightDirection();
        this.menuAnimator.SetTrigger(Triggers.VIEW_LIGHT_ORIENTATION_PANEL);
    }
    
    /** Triggers CapturedPhotoPanel -> GPU Compute Button
     * 
     */ 
    public void OnGpuComputeButton()
    {
        this.capturedPhotoPanelCalibration.CalibrateWithGPU();

        this.viewLightOrientationPanelLighting.orientLightDirection();
        this.viewLightOrientationPanelLighting.saveLightDirection();
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
    }

    /** Triggers ViewLightOrientationPanel -> SearchButton
     * 
     */
    public void OnSearchButton()
    {
        this.myUserConfig.setUserRoleToLobbyJoiner();
        this.client.StartClient();
        this.menuAnimator.SetTrigger(Triggers.LOBBY_PANEL);
    }

    /** Triggers ViewLightOrientationPanel -> HostButton
     * 
     */
    public void OnHostButton()
    {
        this.myUserConfig.setUserToLobbyCreator();
        this.server.startServer();
        this.menuAnimator.SetTrigger(Triggers.LOBBY_PANEL);
    }
    #endregion
    
    #region LobbyPanel

    /** Triggers LobbyPanel -> BackButton
     * 
     */
    public void OnLobbyBackButton()
    {
        if(this.myUserConfig.isActive)
        {
            this.menuAnimator.SetTrigger(Triggers.VIEW_LIGHT_ORIENTATION_PANEL);
            
        } 
        else
        {
            this.menuAnimator.SetTrigger(Triggers.MAIN_MENU);
        }

        //Stop Client or Server
        if (this.myUserConfig.role == UserRole.LOBBY_CREATOR)
        {
            this.server.StopServer();
        }
        else
        {
            this.client.StopClient();
        }

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
