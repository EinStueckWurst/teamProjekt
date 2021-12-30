using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] public RawImage capturedPhotoRaw;
    [SerializeField] public Calibration capturedPhotoPanel;
    [SerializeField] public Camera arCamera;

    private int width = Screen.width;
    private int height = Screen.height;

    /** Starts PhotoCoroutine
     * 
     */
    public void TakePhoto()
    {
        StartCoroutine(this.StartTakingPhotoCoroutine());
    }

    private IEnumerator StartTakingPhotoCoroutine()
    {
        RenderTexture renderTexture = new RenderTexture(this.width, this.height,24);
        renderTexture.Create();

        this.arCamera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        this.arCamera.Render();
        Texture2D snap = new Texture2D(this.width, this.height, TextureFormat.RGB24, false);
        snap.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        snap.Apply();
        RenderTexture.active = null;
        this.arCamera.targetTexture = null;

        this.capturedPhotoRaw.texture = Util.ToRenderTexture(Util.ResampleAndCrop(snap,848, 848), RenderTexture.active);

        this.capturedPhotoPanel.OnCalibrationInit();
        yield return new WaitForEndOfFrame();
    }
}