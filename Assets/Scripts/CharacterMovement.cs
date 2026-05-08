using UnityEngine;
using Photon.Pun;

public class CharacterMovement : MonoBehaviourPun
{
    CharacterController charCntrl;

    [Tooltip("The speed at which the character will move.")]
    public float speed = 5f;

    [Tooltip("The camera representing where the character is looking.")]
    public GameObject cameraObj;

    [Tooltip("Should be checked if using the Bluetooth Controller to move.")]
    public bool joyStickMode;

    void Start()
    {
        charCntrl = GetComponent<CharacterController>();

        if (!photonView.IsMine)
        {
            charCntrl.enabled = false;
            enabled = false;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (cameraObj == null)
        {
            if (Camera.main != null)
                cameraObj = Camera.main.gameObject;
            else
                return;
        }

        float horComp = Input.GetAxis("Horizontal");
        float vertComp = Input.GetAxis("Vertical");

        if (joyStickMode)
        {
            horComp = Input.GetAxis("Vertical");
            vertComp = Input.GetAxis("Horizontal") * -1;
        }

        Vector3 cameraLook = cameraObj.transform.forward;
        cameraLook.y = 0f;
        cameraLook = cameraLook.normalized;

        Vector3 forwardVect = cameraLook;
        Vector3 rightVect = Vector3.Cross(forwardVect, Vector3.up).normalized * -1;

        Vector3 moveVect = Vector3.zero;
        moveVect += rightVect * horComp;
        moveVect += forwardVect * vertComp;
        moveVect *= speed;

        charCntrl.SimpleMove(moveVect);
    }
}