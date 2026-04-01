using System.IO;
using UnityEngine;

public class ImageLoader
{
    public static Texture2D LoadImage(string filename)
    {
        if (!File.Exists(filename))
        {
            Debug.LogError("File not found: " + filename);
            return null;
        }

        byte[] bytes = File.ReadAllBytes(filename);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return texture;
    }
}