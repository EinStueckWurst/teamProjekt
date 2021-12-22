using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CameraController : MonoBehaviour
{
    [SerializeField] public RawImage rawCamDisplay;
    [SerializeField] public RawImage capturedPhotoRaw;

    public static WebCamTexture camera;
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
        if (devices.Length == 0) return;

        for (int i = 0; i < devices.Length; i++)
        {
            var deviceCam = devices[i];

            if (SystemInfo.deviceType == DeviceType.Desktop && deviceCam.isFrontFacing)
            {
                float aspect = (float)(Screen.width / Screen.height);
                camera = new WebCamTexture(deviceCam.name, Mathf.FloorToInt(aspect * 1000), 1000);
                break;
            }

            if (SystemInfo.deviceType == DeviceType.Handheld && !deviceCam.isFrontFacing)
            {
                float aspect = (float)(Screen.width / Screen.height);
                camera = new WebCamTexture(deviceCam.name, Mathf.FloorToInt(aspect * 1000), 1000);
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
        if (camAvailable && camera.isPlaying)
        {
            StartCoroutine(this.StartTakingPhotoCoroutine());
            this.DeactivateCamera();
        }
    }
    //public byte[] ImageToByteArray(System.Drawing.Image imageIn)
    //{
    //    using (var ms = new MemoryStream())
    //    {
    //        imageIn.Save(ms, imageIn.RawFormat);
    //        return ms.ToArray();
    //    }
    //}
    public static Texture2D LoadPNG(string filePath) {
 
    Texture2D tex = null;
    byte[] fileData;
 
    if (File.Exists(filePath))     {
        fileData = File.ReadAllBytes(filePath);
        tex = new Texture2D(2, 2);
        tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
    }
    return tex;
 }
    private IEnumerator StartTakingPhotoCoroutine()
    {

        RenderTexture renderTexture = new RenderTexture(camera.width, camera.height, 1);
        renderTexture.Create();
        string filePath = "C:/Users/Surlix/Documents/Figur3.png";
        //change filepath pls
        
        
        Texture2D tex2d = LoadPNG(filePath);
        
        Graphics.Blit(tex2d, renderTexture);
        //Graphics.Blit(camera, renderTexture);

        Texture2D tmp = Util.ToTexture2D(renderTexture, RenderTexture.active);

        this.capturedPhotoRaw.texture = Util.ToRenderTexture(Util.ResampleAndCrop(tmp, 848, 848), RenderTexture.active);
        yield return new WaitForEndOfFrame();
    }
}