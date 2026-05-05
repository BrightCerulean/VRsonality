using UnityEngine;

public class RoomPhotoDisplay : MonoBehaviour
{
    [Header("Frames")]
    public Renderer[] frames;

    [Header("Image Indices")]
    public int[] imageIndices;

    void Start()
    {
        for (int i = 0; i < frames.Length; i++)
        {
            if (frames[i] != null && i < imageIndices.Length)
                StartCoroutine(ImageUpload.DisplaySavedImage(frames[i], imageIndices[i]));
        }
    }
}
