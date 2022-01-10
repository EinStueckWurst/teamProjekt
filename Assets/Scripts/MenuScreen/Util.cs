using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    /* transform a RenderTexture to a Texture2D
     * 
     */
    public static Texture2D ToTexture2D(RenderTexture rt, RenderTexture currentActiveRt)
    {
        RenderTexture.active = rt;

        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D tex = new Texture2D(rt.width, rt.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

        // Restorie previously active render texture
        RenderTexture.active = currentActiveRt;
        return tex;
    }

    /* transform a Texture2D to a RenderTexture
     * 
     */
    public static RenderTexture ToRenderTexture(Texture2D texture2D, RenderTexture active)
    {
        RenderTexture rt = new RenderTexture(texture2D.width, texture2D.height, 24);
        rt.enableRandomWrite = true;
        rt.Create();
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        RenderTexture.active = active;

        return rt;
    }

    /** transforms a Texture to a RenderTexture
     * 
     */
    public static RenderTexture fromTextureToRenderTexture(Texture texture)
    {
        RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 24);
        Graphics.Blit(texture, renderTexture);

        return renderTexture;
    }

    /** transforms a Texture to a Texture2D
     * 
     */
    public static Texture2D fromTextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height);

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = Util.fromTextureToRenderTexture(texture);

        RenderTexture.active = renderTexture;

        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;

        return texture2D;
    }

    public static Texture2D ResampleAndCrop(this Texture2D source, int targetWidth, int targetHeight)
    {
        int sourceWidth = source.width;
        int sourceHeight = source.height;
        float sourceAspect = (float)sourceWidth / sourceHeight;
        float targetAspect = (float)targetWidth / targetHeight;
        int xOffset = 0;
        int yOffset = 0;
        float factor = 1;
        if (sourceAspect > targetAspect)
        { // crop width
            factor = (float)targetHeight / sourceHeight;
            xOffset = (int)((sourceWidth - sourceHeight * targetAspect) * 0.5f);
        }
        else
        { // crop height
            factor = (float)targetWidth / sourceWidth;
            yOffset = (int)((sourceHeight - sourceWidth / targetAspect) * 0.5f);
        }
        Color32[] data = source.GetPixels32();
        Color32[] data2 = new Color32[targetWidth * targetHeight];
        for (int y = 0; y < targetHeight; y++)
        {
            float yPos = y / factor + yOffset;
            int y1 = (int)yPos;
            if (y1 >= sourceHeight)
            {
                y1 = sourceHeight - 1;
                yPos = y1;
            }

            int y2 = y1 + 1;
            if (y2 >= sourceHeight)
                y2 = sourceHeight - 1;
            float fy = yPos - y1;
            y1 *= sourceWidth;
            y2 *= sourceWidth;
            for (int x = 0; x < targetWidth; x++)
            {
                float xPos = x / factor + xOffset;
                int x1 = (int)xPos;
                if (x1 >= sourceWidth)
                {
                    x1 = sourceWidth - 1;
                    xPos = x1;
                }
                int x2 = x1 + 1;
                if (x2 >= sourceWidth)
                    x2 = sourceWidth - 1;
                float fx = xPos - x1;
                var c11 = data[x1 + y1];
                var c12 = data[x1 + y2];
                var c21 = data[x2 + y1];
                var c22 = data[x2 + y2];
                float f11 = (1 - fx) * (1 - fy);
                float f12 = (1 - fx) * fy;
                float f21 = fx * (1 - fy);
                float f22 = fx * fy;
                float r = c11.r * f11 + c12.r * f12 + c21.r * f21 + c22.r * f22;
                float g = c11.g * f11 + c12.g * f12 + c21.g * f21 + c22.g * f22;
                float b = c11.b * f11 + c12.b * f12 + c21.b * f21 + c22.b * f22;
                float a = c11.a * f11 + c12.a * f12 + c21.a * f21 + c22.a * f22;
                int index = x + y * targetWidth;

                data2[index].r = (byte)r;
                data2[index].g = (byte)g;
                data2[index].b = (byte)b;
                data2[index].a = (byte)a;
            }
        }

        var tex = new Texture2D(targetWidth, targetHeight);
        tex.SetPixels32(data2);
        tex.Apply(true);
        return tex;
    }

    /** Generates a 4x4 GaussianKernel
    * 
    **/
    public static Matrix4x4 generateGaussKernel()
    {
        Matrix4x4 gaussianKernel = new Matrix4x4();

        float para = 0;
        float sigma = 500f; //standard deviation of the distribution (ca.1.0)
        float sum = 0; //sum is for normalization

        float sigmaParameter = 2 * sigma * sigma;

        for (int x = -2; x < 2; x++)
        {
            for (int y = -2; y < 2; y++)
            {
                para = x * x + y * y;
                gaussianKernel[(x + 2), (y + 2)] = (Mathf.Exp(-para / sigmaParameter)) / (Mathf.PI * sigmaParameter);
                sum += gaussianKernel[x + 2, y + 2];
            }
        }

        for (int i = 0; i < 4; ++i)
            for (int j = 0; j < 4; j++)
                gaussianKernel[i, j] /= sum;


        return gaussianKernel;
    }
}
