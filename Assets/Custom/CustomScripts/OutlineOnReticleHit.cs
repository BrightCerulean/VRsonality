using UnityEngine;

public class OutlineOnReticleHit : MonoBehaviour{
    public Camera playerCamera;
    public float rayDistance = 120f;

    private Outline outline;

    void Start(){
        outline = GetComponent<Outline>();
        if (playerCamera == null){
            playerCamera = Camera.main;
        }
    }

    void Update(){
        bool enable_outline = false;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance)){
            if (hit.collider.gameObject == gameObject){
                enable_outline = true;
            }
        }
        if (outline != null){
            outline.enabled = enable_outline;
        }
    }
}