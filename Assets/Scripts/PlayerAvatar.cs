using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerAvatar : MonoBehaviourPun, IPunObservable
{
    [Header("References")]
    public Renderer headRenderer;

    private Vector3 _networkPosition;
    private Quaternion _networkRotation;

    void Start()
    {
        if (photonView.Owner == null) return;

        ApplyColorFromProperties(photonView.Owner.CustomProperties);

        if (photonView.IsMine)
        {
            if (headRenderer != null)
                headRenderer.enabled = false;

            GameObject rig = GameObject.Find("XRCardboardRig");
            if (rig != null)
            {
                rig.transform.SetParent(transform);
                rig.transform.localPosition = Vector3.zero;
                rig.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogWarning("[PlayerAvatar] XRCardboardRig not found in scene.");
            }

            Hashtable props = new Hashtable
            { { "scene", SceneManager.GetActiveScene().name } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        else
        {
            _networkPosition = transform.position;
            _networkRotation = transform.rotation;
            RefreshVisibility();
            StartCoroutine(RetryVisibility()); 
        }
    }
    void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(
                transform.position, _networkPosition, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(
                transform.rotation, _networkRotation, Time.deltaTime * 15f);
        }
    }

    public void RefreshVisibility()
    {
        if (photonView.IsMine) return;

        string myScene = SceneManager.GetActiveScene().name;
        string theirScene = photonView.Owner.CustomProperties.ContainsKey("scene")
                             ? (string)photonView.Owner.CustomProperties["scene"]
                             : "";

        bool sameScene = myScene == theirScene;
        if (headRenderer != null)
            headRenderer.enabled = sameScene;
    }

    public void OnOwnerPropertiesChanged(Hashtable changedProps)
    {
        if (photonView.Owner != null)
        {
            ApplyColorFromProperties(photonView.Owner.CustomProperties);
            RefreshVisibility();
        }
    }

    void ApplyColorFromProperties(Hashtable props)
    {
        if (props == null || headRenderer == null) return;

        float r = props.ContainsKey("colorR") ? (float)props["colorR"] : 1f;
        float g = props.ContainsKey("colorG") ? (float)props["colorG"] : 1f;
        float b = props.ContainsKey("colorB") ? (float)props["colorB"] : 1f;

        headRenderer.material.color = new Color(r, g, b);
    }
    IEnumerator RetryVisibility()
    {
        yield return new WaitForSeconds(1f); 
        RefreshVisibility();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            _networkPosition = (Vector3)stream.ReceiveNext();
            _networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}