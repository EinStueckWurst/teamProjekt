using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calibration : MonoBehaviour
{
    [SerializeField] RawImage capturedPhoto;
    [SerializeField] RawImage resultImg;

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

    private int imgWidth;
    private int imgHeight;



    public void Init()
   {
        this.imgWidth = this.capturedPhoto.texture.width;
        this.imgHeight = this.capturedPhoto.texture.width;

        this.InitializeFilters();
    }
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

    public void CalibrateWithCPU()
    {
        this.ApplyFilters();
        this.HoughComputeCPU();

        //Get Hough Information from Shader
        displayCircleMaterialFilter.SetFloat("_CenterA", this.hough.a);
        displayCircleMaterialFilter.SetFloat("_CenterB", this.hough.b);
        displayCircleMaterialFilter.SetFloat("_Radius", this.hough.radius);

        this.resultImg.texture = this.cropRenderTexture(RenderTexture.active);
    }

    public void CalibrateWithGPU()
    {
        Debug.Log("NEED To BE Implemented");
        //this.ApplyFilters();
        //this.HoughComputeCPU();

        ////Get Hough Information from Shader
        //displayCircleMaterialFilter.SetFloat("_CenterA", this.hough.a);
        //displayCircleMaterialFilter.SetFloat("_CenterB", this.hough.b);
        //displayCircleMaterialFilter.SetFloat("_Radius", this.hough.radius);

        //this.resultImg.texture = this.cropRenderTexture(RenderTexture.active);
    }

    void ApplyFilters()
    {
        Graphics.Blit(this.capturedPhoto.texture, unfiltered, identityMaterialFilter);
        Graphics.Blit(unfiltered, grayScale, grayscaleMaterialFilter);
        Graphics.Blit(grayScale, gaussianBlurr, gaussianBlurrMaterialFilter);
        Graphics.Blit(gaussianBlurr, sobel, sobelMaterialFilter);
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

        Debug.Log("-----------Hough Calculation with CPU-----------");
        Debug.Log("A " + maxA);
        Debug.Log("B " + maxB);
        Debug.Log("Val " + max);
        Debug.Log("R " + maxR);
        Debug.Log("----------------------------------------------");
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
}
