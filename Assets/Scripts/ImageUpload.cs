using UnityEngine;
using System.IO;

public class ImageUpload : MonoBehaviour
{
    // reference for Quad - ImageUpload
    public Renderer displaySurface; 

    public void LoadAndDisplay(string path)
    {
        Texture2D tex = ImageLoader.LoadImage(path);
        if (tex != null)
            displaySurface.material.mainTexture = tex;
    }

    // Call this to load from Android's Pictures folder
    public void LoadFromAndroid()
    {
        string path = Path.Combine(
            Application.persistentDataPath, 
            "upload.jpg"
        );
        LoadAndDisplay(path);
    }
}