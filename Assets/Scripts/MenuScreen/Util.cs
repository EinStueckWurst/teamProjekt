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
    public static Texture2D fromTextureToTexture2D (Texture texture)
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
}
