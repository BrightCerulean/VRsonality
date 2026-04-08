using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ImageUpload : MonoBehaviour
{
    [Header("Display")]
    public Renderer displaySurface; // drag Quad here in Inspector

    [Header("Portal")]
    public GameObject portal; // drag Portal_To_Pastroom here

    private bool imageUploaded = false;
    private string savedImagePath;

    void Start()
    {
        // Keep portal inactive until image is uploaded
        if (portal != null)
            portal.SetActive(false);

        // Load previously saved image if exists
        savedImagePath = Path.Combine(
            Application.persistentDataPath, 
            "playerimage.jpg"
        );

        if (File.Exists(savedImagePath))
        {
            StartCoroutine(LoadSavedImage());
        }
    }

    // Call this from your raycast interaction
    public void OnSelect()
    {
        if (!imageUploaded)
        {
            OpenGallery();
        }
        else
        {
            Debug.Log("[ImageUpload] Image already uploaded");
        }
    }

    void OpenGallery()
    {

        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path == null)
            {
                Debug.Log("[ImageUpload] No image selected");
                return;
            }

            Debug.Log("[ImageUpload] Image selected: " + path);
            StartCoroutine(LoadAndDisplayImage(path));

        }, "Select your photo", "image/*");

        Debug.Log("[ImageUpload] Gallery permission: ");
    }

    IEnumerator LoadAndDisplayImage(string path)
    {
        // Load image using NativeGallery
        Texture2D tex = NativeGallery.LoadImageAtPath(path, 512);

        if (tex == null)
        {
            Debug.LogError("[ImageUpload] Failed to load image");
            yield break;
        }

        // Display on Quad
        if (displaySurface != null)
            displaySurface.material.mainTexture = tex;

        // Save image to persistentDataPath so other scenes can use it
        byte[] bytes = tex.EncodeToJPG();
        File.WriteAllBytes(savedImagePath, bytes);
        Debug.Log("[ImageUpload] Image saved to: " + savedImagePath);

        imageUploaded = true;

        // Activate portal to pastroom
        if (portal != null)
        {
            portal.SetActive(true);
            Debug.Log("[ImageUpload] Portal activated");
        }

        yield return null;
    }

    IEnumerator LoadSavedImage()
    {
        string path = "file://" + savedImagePath;

        UnityWebRequest request = 
            UnityWebRequestTexture.GetTexture(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D tex = 
                DownloadHandlerTexture.GetContent(request);
            if (displaySurface != null)
                displaySurface.material.mainTexture = tex;

            imageUploaded = true;

            if (portal != null)
                portal.SetActive(true);

            Debug.Log("[ImageUpload] Saved image loaded");
        }
        else
        {
            Debug.LogError("[ImageUpload] Failed to load saved image: " 
                + request.error);
        }
    }

    // Call this from other scenes to display the uploaded image
    public static IEnumerator DisplaySavedImage(Renderer target)
    {
        string path = "file://" + Path.Combine(
            Application.persistentDataPath, 
            "playerimage.jpg"
        );

        UnityWebRequest request = 
            UnityWebRequestTexture.GetTexture(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            target.material.mainTexture = 
                DownloadHandlerTexture.GetContent(request);
            Debug.Log("[ImageUpload] Image displayed in scene");
        }
        else
        {
            Debug.LogError("[ImageUpload] Error: " + request.error);
        }
    }
}