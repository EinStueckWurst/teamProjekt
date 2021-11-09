using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] RawImage rawCamDisplay;
    [SerializeField] RawImage capturedPhoto;

    static WebCamTexture camera;
    bool camAvailable = false;

    //TODO: @Rahil Filter, Kreiserkennung in separate Klasse verlagern ... Clean Code ;)
    RenderTexture unfiltered;
    [SerializeField] Material identityMaterialFilter;

    /** Initializes settings for the Camera
     * 
     */ 
    public void Init()
    {
        InitializeCamera();
        //TODO : @Rahil separate Klasse --- Initialize RenderTexture
        unfiltered = new RenderTexture(camera.width, camera.height, 1);
        unfiltered.enableRandomWrite = true;
        unfiltered.Create();
    }

    private void Update()
    {
        //if (!camAvailable) return;
        //Graphics.Blit(camera, unfiltered, identityMaterialFilter);
        //this.rawCamDisplay.texture = unfiltered;
    }

    private void InitializeCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)    return;

        for (int i = 0; i < devices.Length; i++)
        {
            var deviceCam = devices[i];

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
        capturedPhoto.texture = rawCamDisplay.texture;
        this.DeactivateCamera();
        yield return new WaitForEndOfFrame();
    }
}
