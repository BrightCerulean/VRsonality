using UnityEngine;

public class PlayerColorSync : MonoBehaviour
{
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (rend != null && GameManager.Instance != null)
            rend.material.color = GameManager.Instance.playerColor;
    }
}