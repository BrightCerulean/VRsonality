using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ImageUpload : MonoBehaviour
{
    [Header("Display")]
    public Renderer[] displaySurfaces; // drag 4 frames here in Inspector

    [Header("Portal")]
    public GameObject portal;

    private int currentImageIndex = 0;
    private string[] savedImagePaths = new string[4];

    void Start()
    {
        if (portal != null)
            portal.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            savedImagePaths[i] = Path.Combine(
                Application.persistentDataPath,
                "playerimage_" + i + ".jpg"
            );

            if (File.Exists(savedImagePaths[i]))
                StartCoroutine(LoadSavedImage(i));
        }
    }

    public void OnSelect()
    {
        if (currentImageIndex < 4)
            OpenGallery();
        else
            Debug.Log("[ImageUpload] All 4 images uploaded");
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
            StartCoroutine(LoadAndDisplayImage(path, currentImageIndex));
        }, "Select your photo", "image/*");
    }

    IEnumerator LoadAndDisplayImage(string path, int index)
    {
        Texture2D tex = NativeGallery.LoadImageAtPath(path, 512);

        if (tex == null)
        {
            Debug.LogError("[ImageUpload] Failed to load image");
            yield break;
        }

        // Make readable using RenderTexture
        RenderTexture rt = RenderTexture.GetTemporary(tex.width, tex.height);
        Graphics.Blit(tex, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D readableTex = new Texture2D(tex.width, tex.height);
        readableTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readableTex.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        // Display on correct frame
        if (index < displaySurfaces.Length && displaySurfaces[index] != null)
            displaySurfaces[index].material.mainTexture = readableTex;

        // Save image
        byte[] bytes = readableTex.EncodeToJPG();
        File.WriteAllBytes(savedImagePaths[index], bytes);
        Debug.Log("[ImageUpload] Image " + index + " saved");

        currentImageIndex++;

        if (portal != null && currentImageIndex >= 1)
        {
            portal.SetActive(true);
            Debug.Log("[ImageUpload] Portal activated");
        }

        yield return null;
    }

    IEnumerator LoadSavedImage(int index)
    {
        string path = "file://" + savedImagePaths[index];
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            if (index < displaySurfaces.Length && displaySurfaces[index] != null)
                displaySurfaces[index].material.mainTexture = tex;
            Debug.Log("[ImageUpload] Saved image " + index + " loaded");
        }
    }

    public static IEnumerator DisplaySavedImage(Renderer target, int index = 0)
    {
        string path = "file://" + Path.Combine(
            Application.persistentDataPath,
            "playerimage_" + index + ".jpg"
        );

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            target.material.mainTexture = DownloadHandlerTexture.GetContent(request);
            Debug.Log("[ImageUpload] Image " + index + " displayed");
        }
    }
}

/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ImageUpload : MonoBehaviour
{
    [Header("Display")]
    public Renderer displaySurface; // drag Quad here in Inspector

    [Header("Portal")]
    public GameObject portal;

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

        // Make texture readable before encoding
        Texture2D readableTex = new Texture2D(tex.width, tex.height, tex.format, false);
        Graphics.CopyTexture(tex, readableTex);
        byte[] bytes = readableTex.EncodeToJPG();
        File.WriteAllBytes(savedImagePath, bytes);
        Debug.Log("[ImageUpload] Image saved to: " + savedImagePath);

        imageUploaded = true;

        // Flip texture vertically
        Color[] pixels = tex.GetPixels();
        System.Array.Reverse(pixels);
        tex.SetPixels(pixels);
        tex.Apply();

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
}*/