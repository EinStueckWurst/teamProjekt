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

    //TODO: @Rahil Filter, Kreiserkennung in separate Klasse verlagern ... Clean Code
    RenderTexture unfiltered;
    RenderTexture grayScale;
    RenderTexture gaussianBlurr;
    RenderTexture sobel;
    RenderTexture displayCircle;

    [SerializeField] Material identityMaterialFilter;
    [SerializeField] Material grayscaleMaterialFilter;
    [SerializeField] Material gaussianBlurrMaterialFilter;
    [SerializeField] Material sobelMaterialFilter;
    [SerializeField] Material displayCircleMaterialFilter;

    public struct HoughMaxVoteProp
    {
        public int maxVal;
        public int a;
        public int b;
        public int radius;
    }

    public HoughMaxVoteProp hough;


    /** Initializes settings for the Camera
     * 
     */
    public void Init()
    {
        InitializeCamera();
        //TODO : @Rahil separate Klasse --- Initialize RenderTexture
        this.InitializeFilters();
    }

    void InitializeFilters()
    {
        unfiltered = new RenderTexture(camera.width, camera.height, 1);
        unfiltered.enableRandomWrite = true;
        unfiltered.Create();
        
        grayScale = new RenderTexture(camera.width, camera.height, 1);
        grayScale.enableRandomWrite = true;
        grayScale.Create();
        
        gaussianBlurr = new RenderTexture(camera.width, camera.height, 1);
        gaussianBlurr.enableRandomWrite = true;
        gaussianBlurr.Create();
        
        sobel = new RenderTexture(camera.width, camera.height, 1);
        sobel.enableRandomWrite = true;
        sobel.Create();
        
        displayCircle = new RenderTexture(camera.width, camera.height, 1);
        displayCircle.enableRandomWrite = true;
        displayCircle.Create();
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
        ApplyFilters();
        HoughCompute();

        displayCircleMaterialFilter.SetFloat("_CenterA", this.hough.a);
        displayCircleMaterialFilter.SetFloat("_CenterB", this.hough.b);
        displayCircleMaterialFilter.SetFloat("_Radius", this.hough.radius);

        //Draw Circle around found Point -- for Debug
        //Graphics.Blit(sobel, displayCircle, displayCircleMaterialFilter);

        this.capturedPhoto.texture = this.cropRenderTexture(RenderTexture.active);

        this.DeactivateCamera();
        yield return new WaitForEndOfFrame();
    }

    void ApplyFilters()
    {
        Graphics.Blit(camera, unfiltered, identityMaterialFilter);
        Graphics.Blit(unfiltered, grayScale, grayscaleMaterialFilter);
        Graphics.Blit(grayScale, gaussianBlurr, gaussianBlurrMaterialFilter);
        Graphics.Blit(gaussianBlurr, sobel, sobelMaterialFilter);
    }

    void HoughCompute()
    {
        this.hough.maxVal = 0;

        int imgWidth = camera.width;
        int imgHeight = camera.height;

        int minRad = 57;
        int maxRad = camera.width/2;
        int[] voteBuffer = new int[camera.width * camera.height];

        int a, b;
        int max = 0; //vote vergleichen
        int temp = 0;
        int maxA = 0;
        int maxB = 0;
        int maxR = 0;
        float threshold = 0.2f;

        Texture2D sobelImage = Util.ToTexture2D(this.sobel, RenderTexture.active);

        for (int r = minRad; r < maxRad; r++)
        {
            for (int x = 0; x < imgWidth; x++)
            {
                for (int y = 0; y < imgHeight; y++)
                {
                    voteBuffer[y * imgWidth + x] = 0; // alles auf 0 gesetzt
                }
            }
            for (int x = 30; x < imgWidth; x++)
            {
                for (int y = 10; y < imgHeight; y++)
                {
                    Color color = sobelImage.GetPixel(x, y);

                    if (color.r > threshold)
                    {
                        for (int theta = 0; theta <= 360; theta++)
                        {
                            //Umwandlung in Bogenmaß theta/360 = x/2*Pi
                            a = (int)(x - r * Math.Cos(theta * Math.PI / 180));
                            b = (int)(y - r * Math.Sin(theta * Math.PI / 180));

                            if (a >= 0 && a < imgWidth && b >= 0 && b < imgHeight)
                            {
                                voteBuffer[b * imgWidth + a]++;
                                temp = voteBuffer[b * imgWidth + a];
                            }
                            if (temp > max)
                            {
                                max = temp; //max Value von Vote
                                maxA = a;
                                maxB = b;
                                maxR = r;
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("A "+maxA);
        Debug.Log("B "+maxB);
        Debug.Log("Val "+max);
        Debug.Log("R "+maxR);

        this.hough.a = maxA;
        this.hough.b = maxB;
        this.hough.maxVal = max;
        this.hough.radius = maxR;
    }



    /* crops Camera-Image based on calculated hough-transform-values
     * 
     */ 
    RenderTexture cropRenderTexture(RenderTexture activeRt)
    {
        int left = this.hough.a - this.hough.radius;
        int right = this.hough.a + this.hough.radius;

        int bot = this.hough.b - this.hough.radius;
        int top = this.hough.b + this.hough.radius;

        if(left < 0) left = 0;
        if(right > camera.width) right = camera.width;
        if(bot < 0) bot = 0;
        if (top > camera.height) top = camera.height;

        int cropWidth = right - left;
        int cropHeight = top - bot;
        Texture2D cropImgTex2D = new Texture2D(cropWidth, cropHeight);

        for (int x = 0; x < cropWidth; x++)
        {
            for (int y = 0; y < cropHeight; y++)
            {
                Color color = camera.GetPixel(x + left, y + bot);
                cropImgTex2D.SetPixel(x, y, color);
                cropImgTex2D.Apply();
            }
        }

        return Util.ToRenderTexture(cropImgTex2D, activeRt);
    }
}