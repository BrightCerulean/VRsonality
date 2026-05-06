using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using TMPro;

public class ImageUpload : MonoBehaviour
{
    [Header("Display")]
    public Renderer[] displaySurfaces; // drag 4 frames here in Inspector

    [Header("Portal")]
    public GameObject portal;
    public string firstVisitScene = "toddlerbedroom";
    public string secondVisitScene = "partyroom";

    [Header("Status")]
    public TextMeshProUGUI statusText;
    public float messageDuration = 3f;

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
            {
                StartCoroutine(LoadSavedImage(i));
                currentImageIndex = i + 1;
            }
        }

        if (portal != null && currentImageIndex >= 1)
            portal.SetActive(true);

        if (GameManager.Instance != null)
            GameManager.Instance.uploadRoomVisitCount++;

        Portal portalScript = portal != null ? portal.GetComponent<Portal>() : null;
        if (portalScript != null)
        {
            bool secondVisit = GameManager.Instance != null && GameManager.Instance.uploadRoomVisitCount >= 2;
            portalScript.sceneToLoad = secondVisit ? secondVisitScene : firstVisitScene;
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
        try
        {
            NativeGallery.GetImageFromGallery((path) =>
            {
                if (path == null)
                {
                    Debug.Log("[ImageUpload] No image selected");
                    StartCoroutine(ShowMessage("No image selected. Tap again to try."));
                    return;
                }
                Debug.Log("[ImageUpload] Image selected: " + path);
                StartCoroutine(LoadAndDisplayImage(path, currentImageIndex));
            }, "Select your photo", "image/*");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[ImageUpload] Gallery failed to open: " + e.Message);
            StartCoroutine(ShowMessage("Could not open gallery. Please try again."));
        }
    }

    IEnumerator ShowMessage(string msg)
    {
        if (statusText == null) yield break;
        statusText.text = msg;
        statusText.gameObject.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        statusText.gameObject.SetActive(false);
    }

    IEnumerator LoadAndDisplayImage(string path, int index)
    {
        string url = path.StartsWith("file://") ? path : "file://" + path;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[ImageUpload] Failed to load image: " + request.error);
            StartCoroutine(ShowMessage("Failed to load image. Please try a different photo."));
            yield break;
        }

        Texture2D raw = DownloadHandlerTexture.GetContent(request);
        Texture2D tex = ResizeTexture(raw, 1024);
        if (tex != raw) Destroy(raw);
        FlipTextureVertically(tex);

        if (index < displaySurfaces.Length && displaySurfaces[index] != null)
            displaySurfaces[index].material.mainTexture = tex;

        byte[] bytes = tex.EncodeToJPG();
        File.WriteAllBytes(savedImagePaths[index], bytes);
        Debug.Log("[ImageUpload] Image " + index + " saved");

        currentImageIndex++;

        StartCoroutine(ShowMessage("Photo " + currentImageIndex + " uploaded successfully!"));

        if (portal != null && currentImageIndex >= 1)
        {
            portal.SetActive(true);
            Debug.Log("[ImageUpload] Portal activated");
        }
    }

    Texture2D ResizeTexture(Texture2D source, int maxSize)
    {
        if (source.width <= maxSize && source.height <= maxSize)
            return source;

        float ratio = (float)source.width / source.height;
        int newWidth  = ratio >= 1 ? maxSize : Mathf.RoundToInt(maxSize * ratio);
        int newHeight = ratio >= 1 ? Mathf.RoundToInt(maxSize / ratio) : maxSize;

        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        Graphics.Blit(source, rt);
        RenderTexture.active = rt;
        Texture2D resized = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
        resized.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resized.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return resized;
    }

    void FlipTextureVertically(Texture2D tex)
    {
        Color[] pixels = tex.GetPixels();
        int width = tex.width;
        int height = tex.height;
        Color[] flipped = new Color[pixels.Length];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                flipped[y * width + x] = pixels[(height - 1 - y) * width + x];
        tex.SetPixels(flipped);
        tex.Apply();
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