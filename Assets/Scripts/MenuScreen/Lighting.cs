using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lighting : MonoBehaviour
{
    [SerializeField] public Light lightObj;
    [SerializeField] public RawImage capturedPhoto;
    [SerializeField] public UserConfiguration userConfiguration;

    /* Searches the brightest part of the sphere
     * 
     */
    Vector3 getBrightPart()
    {
        int imgWidth = capturedPhoto.texture.width;
        int imgHeight = capturedPhoto.texture.height;
        float rad = imgWidth * 0.5f;
        int centerX = imgWidth / 2;
        int centerY = imgHeight / 2;
        float brightnessMax = 0;
        int maxX = 0;
        int maxY = 0;

        Texture2D texture2D = Util.fromTextureToTexture2D(capturedPhoto.texture);

        for (int i = 0; i < imgWidth; i++)
        {
            for (int j = 0; j < imgHeight; j++)
            {
                float pyth = Mathf.Sqrt((i - centerX) * (i - centerX) + (j - centerY) * (j - centerY));
                if (pyth <= rad)
                {
                    Color col = texture2D.GetPixel(i, j);

                    float redPart = 0.299f * col.r * col.r;
                    float greenPart = 0.587f * col.g * col.g;
                    float bluePart = 0.114f * col.b * col.b;

                    float brightness = Mathf.Sqrt(redPart + greenPart + bluePart);

                    if(brightness > brightnessMax)
                    {
                        brightnessMax = brightness;
                        maxX = i;
                        maxY = j;
                    }
                }
            }
        }
        float u = (float)maxX / (float)imgWidth;
        float v = (float)maxY / (float)imgHeight;
        float x = 2 * u - 1;
        float y = 2 * v - 1;
        float z = 1 - x * x - y * y;

        Debug.Log("X " + x);
        Debug.Log("Y " + y);
        Debug.Log("Z " + z);
        return new Vector3(x,y,z);
    }

    /** reorients the directional Light
     * 
     */ 
    public void orientLightDirection()
    {
        //falls GetBrightPart direkt die direction ist
        Vector3 direction = getBrightPart().normalized; // in Eyspace

        //falls GetBrightPart die LookAt Position des Lichtes sein soll
        //Vector3 direction = (getBrightPart() - light.transform.position).normalized;

        Quaternion lightDirection = Quaternion.LookRotation(direction);

        lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, lightDirection, 1);
    }

    /** Saves Lightdirection in Userconfig
     * 
     */ 
    public void saveLightDirection()
    {
        Vector3 direction = getBrightPart().normalized; // in Eyspace
        userConfiguration.setLightDir(direction);
    }

    
}
