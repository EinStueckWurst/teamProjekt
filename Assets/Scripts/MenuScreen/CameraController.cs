using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] RawImage rawCamDisplay;
    [SerializeField] RawImage capturedPhoto;
    [SerializeField] RawImage capturedPhotoRaw;

    static WebCamTexture camera;
    bool camAvailable = false;

    /** Initializes settings for the Camera
     * 
     */
    public void Init()
    {
        InitializeCamera();
    }

    private void Update()
    {
        if (!camAvailable) return;
    }

    private void InitializeCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)    return;

        for (int i = 0; i < devices.Length; i++)
        {
            var deviceCam = devices[i];
            
            //Hier später einfach !device.isFrontFacing damit Frontkamera verwendet wird
            if (deviceCam.isFrontFacing)
            {
                float aspect = (float)(Screen.width / Screen.height);
                camera = new WebCamTexture(deviceCam.name, Mathf.FloorToInt(aspect * 400), 400);
                break;
            }
        }

        if (camera == null) return;

        camera.Play();
        this.rawCamDisplay.texture = camera;
        camAvailable = true;
    }

    /** Activates the Camera
     * 
     */ 
    public void ActivateCamera()
    {
        if (camera != null && !camera.isPlaying)
        {
            camera.Play();
            camAvailable = true;
        }
    }

    /** Deactivates the Camera
     * 
     */ 
    public void DeactivateCamera()
    {
        if (camera != null)
        {
            camera.Stop();
            camAvailable = false;
        }
    }

    /** Starts PhotoCoroutine
     * 
     */ 
    public void TakePhoto()
    {
        if(camAvailable && camera.isPlaying)
        {
            StartCoroutine(this.StartTakingPhotoCoroutine());
        }
    }

    private IEnumerator StartTakingPhotoCoroutine()
    {
        this.capturedPhotoRaw.texture = camera;

        //ApplyFilters();
        //HoughCompute();

        ////Draw Circle around found Point -- for Debug
        ////Graphics.Blit(sobel, displayCircle, displayCircleMaterialFilter);

        this.DeactivateCamera();
        yield return new WaitForEndOfFrame();
    }
}