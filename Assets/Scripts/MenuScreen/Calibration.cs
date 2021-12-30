using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Calibration : MonoBehaviour
{
    [SerializeField] public CameraController cameraController;
    [SerializeField] public RawImage resultImg;
    [SerializeField] public RawImage capturedPhoto;

    RenderTexture unfiltered;
    RenderTexture grayScale;
    RenderTexture gaussianBlurr;
    RenderTexture sobel;
    RenderTexture displayCircle;

    [SerializeField] Material identityMaterialFilter;
    [SerializeField] Material grayscaleMaterialFilter;
    [SerializeField] Material gaussianBlurrMaterialFilter;
    [SerializeField] Material sobelMaterialFilter;

    [SerializeField] ComputeShader houghComputeShader;

    public struct HoughMaxVoteProp
    {
        public int maxVal;
        public int a;
        public int b;
        public int radius;
    }

    public HoughMaxVoteProp hough;

    private int imgWidth;
    private int imgHeight;



    public void OnCalibrationInit()
    {    
        this.capturedPhoto.texture = cameraController.capturedPhotoRaw.texture;

        this.imgWidth = this.cameraController.capturedPhotoRaw.texture.width;
        this.imgHeight = this.cameraController.capturedPhotoRaw.texture.height;

        this.InitializeFilters();
    }

    /** Initializes All RenderTextures
     * 
     */ 
    void InitializeFilters()
    {
        unfiltered = new RenderTexture(imgWidth, this.imgHeight, 1);
        unfiltered.enableRandomWrite = true;
        unfiltered.Create();

        grayScale = new RenderTexture(imgWidth, this.imgHeight, 1);
        grayScale.enableRandomWrite = true;
        grayScale.Create();

        gaussianBlurr = new RenderTexture(imgWidth, imgHeight, 1);
        gaussianBlurr.enableRandomWrite = true;
        gaussianBlurr.Create();

        sobel = new RenderTexture(imgWidth, imgHeight, 1);
        sobel.enableRandomWrite = true;
        sobel.Create();

        displayCircle = new RenderTexture(imgWidth, imgHeight, 1);
        displayCircle.enableRandomWrite = true;
        displayCircle.Create();
    }

    /** Executes the Calibration over the CPU
     * 
     */
    public void CalibrateWithCPU()
    {
        this.ApplyFilters();
        this.HoughComputeCPU();
        Debug.Log("-----------Hough Calculation with CPU-----------");
        Debug.Log("A " + this.hough.a);
        Debug.Log("B " + this.hough.b);
        Debug.Log("Val " + this.hough.maxVal);
        Debug.Log("R " + this.hough.radius);
        Debug.Log("----------------------------------------------");
        this.resultImg.texture = this.cropRenderTextureWithCPU(RenderTexture.active);
    }

    /** Executes the Calibration over the GPU
     * 
     */ 
    public void CalibrateWithGPU()
    {
        this.ApplyFilters();
        this.HoughComputeGPU();
        Debug.Log("-----------Hough Calculation with GPU-----------");
        Debug.Log("A " + this.hough.a);
        Debug.Log("B " + this.hough.b);
        Debug.Log("Val " + this.hough.maxVal);
        Debug.Log("R " + this.hough.radius);
        Debug.Log("----------------------------------------------");
        this.resultImg.texture = this.cropRenderTextureWithGPU();
    }

    /** Applying Filters before HoughComputation grayscale -> gaussian Blurr -> sobel
     * 
     */
    void ApplyFilters()
    {
        //Just a Copy
        Graphics.Blit(this.capturedPhoto.texture, unfiltered, identityMaterialFilter);

        //Grayscale
        Graphics.Blit(unfiltered, grayScale, grayscaleMaterialFilter);

        //GaussianBlurr
        Matrix4x4 gaussKernel = Util.generateGaussKernel();
        this.gaussianBlurrMaterialFilter.SetMatrix("_KernelGaussianFilter", gaussKernel);
        Graphics.Blit(grayScale, gaussianBlurr, gaussianBlurrMaterialFilter);

        //Sobel
        Graphics.Blit(gaussianBlurr, sobel, sobelMaterialFilter);
    }

    void HoughComputeGPU()
    {
        int minRad = 57;
        int maxRad = imgWidth / 2;
        this.hough.maxVal = 0;
        float threshHold = 0.2f;

        //Select Kernel
        int houghKernel = this.houghComputeShader.FindKernel("CSMain");

        //Initialize Buffer
        ComputeBuffer voteBuffer = new ComputeBuffer(imgWidth * imgHeight, sizeof(int));
        int[] resultVotes = new int[imgWidth * imgHeight];

        for (int rad = minRad; rad < maxRad; rad++)
        {
            //Clear VoteBuffer
            voteBuffer.SetData(new int[imgWidth * imgHeight]);

            //Assign Properties 

            this.houghComputeShader.SetTexture(houghKernel, "_InputTexture", this.sobel);
            this.houghComputeShader.SetBuffer(houghKernel, "_VoteBuffer", voteBuffer);

            this.houghComputeShader.SetInt("_ResWidth", imgWidth);
            this.houghComputeShader.SetInt("_ResHeight", imgHeight);
            this.houghComputeShader.SetInt("_Radius", rad);
            this.houghComputeShader.SetFloat("_Threshold", threshHold);

            this.houghComputeShader.Dispatch(houghKernel, imgWidth/8, imgHeight/8, 1);

            //Extracts the Calculated Votebuffer and takes the highest Value
            resultVotes = new int[imgWidth * imgHeight];
            voteBuffer.GetData(resultVotes);
            int currentMaxVal = resultVotes.Max();

            if (currentMaxVal > this.hough.maxVal)
            {
                this.hough.maxVal = currentMaxVal;
                int indx = Array.IndexOf(resultVotes, currentMaxVal);

                this.hough.a = (indx % imgWidth);
                this.hough.b = (indx - this.hough.a) / this.imgWidth;
                this.hough.radius = rad;
            }
        }
        voteBuffer.Dispose();
    }

    void HoughComputeCPU()
    {
        this.hough.maxVal = 0;

        int imgWidth = this.imgWidth;
        int imgHeight = this.imgHeight;

        int minRad = 57;
        int maxRad = imgWidth / 2;
        int[] voteBuffer = new int[imgWidth * imgWidth];

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
        this.hough.a = maxA;
        this.hough.b = maxB;
        this.hough.maxVal = max;
        this.hough.radius = maxR;
    }

    /* crops Camera-Image based on calculated hough-transform-values
     * 
     */
    RenderTexture cropRenderTextureWithCPU(RenderTexture activeRt)
    {
        int left = this.hough.a - this.hough.radius;
        int right = this.hough.a + this.hough.radius;

        int bot = this.hough.b - this.hough.radius;
        int top = this.hough.b + this.hough.radius;

        if (left < 0) left = 0;
        if (right > this.imgWidth) right = this.imgWidth;
        if (bot < 0) bot = 0;
        if (top > this.imgHeight) top = this.imgHeight;

        int cropWidth = right - left;
        int cropHeight = top - bot;

        Texture2D cropImgTex2D = new Texture2D(cropWidth, cropHeight);
        Texture2D capturedPhotoAsTex2d = Util.fromTextureToTexture2D(this.capturedPhoto.texture);

        for (int x = 0; x < cropWidth; x++)
        {
            for (int y = 0; y < cropHeight; y++)
            {
                Color color = capturedPhotoAsTex2d.GetPixel(x + left, y + bot);
                cropImgTex2D.SetPixel(x, y, color);
                cropImgTex2D.Apply();
            }
        }

        return Util.ToRenderTexture(cropImgTex2D, activeRt);
    }

    /** Crops the Image "capturedPhoto" and assigns it to "ResultImage"
     * 
     */
    public RenderTexture cropRenderTextureWithGPU()
    {
        RenderTexture rtx = RenderTexture.GetTemporary(this.imgWidth, this.imgHeight, 1);
        Graphics.Blit(this.capturedPhoto.texture, rtx);

        int size = this.hough.radius *2;

        //Coordinates in the original image to start to copy from
        int startCoordX = this.hough.a - this.hough.radius;
        if (startCoordX < 0) {
            startCoordX = 0; 
        } else if (startCoordX > this.imgWidth){
            startCoordX = this.imgWidth;
        }

        int startCoordY = this.hough.b - this.hough.radius;
        if (startCoordY < 0)
        {
            startCoordY = 0;
        }
        else if (startCoordY > this.imgHeight)
        {
            startCoordY = this.imgHeight;
        }

        RenderTexture tmpTexture = RenderTexture.GetTemporary(size, size, 1);

        Graphics.CopyTexture(rtx, 0, 0, startCoordX, startCoordY, size, size, tmpTexture, 0, 0, 0, 0);
        RenderTexture.ReleaseTemporary(rtx);

        rtx = RenderTexture.GetTemporary(size, size, 1);
        Graphics.Blit(tmpTexture, rtx);

        RenderTexture.ReleaseTemporary(tmpTexture);
        return rtx;

    }
}
