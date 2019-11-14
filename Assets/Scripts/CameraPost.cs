#region usings
using UnityEngine;
#if UNITY_WINRT
using UnityEngine.Windows;
using File = UnityEngine.Windows.File;
using Directory = UnityEngine.Windows.Directory;
 
#else
using System.IO;
using File = System.IO.File;
using Directory = System.IO.Directory;
#endif
#endregion

public class CameraPost : MonoBehaviour
{
    public int width;
    public int height;
    public string fileName = "texture";
    [Space]
    public Material renderMat;
    public Material nothing;

    private int i = 0;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, nothing);

        Texture2D outputTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture buffer = new RenderTexture(
                               width,
                               height,
                               0,                            // No depth/stencil buffer
                               RenderTextureFormat.ARGB32,   // Standard colour format
                               RenderTextureReadWrite.Linear // No sRGB conversions
                           );

        Graphics.Blit(dest, buffer, nothing, -1);
        RenderTexture.active = new RenderTexture(src);           // If not using a scene camera
        outputTex.ReadPixels(
                  new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), // Capture the whole texture
                  0, 0,                          // Write starting at the top-left texel
                  false                          // No mipmaps
        );

        File.WriteAllBytes(Application.persistentDataPath + "/renderedPNGs/" + fileName + i++ + ".png", outputTex.EncodeToPNG());
        Debug.Log(Application.persistentDataPath + "/" + fileName + ".png");
    }
}
