using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

public class VoiceChatManager : MonoBehaviour
{
    [Header("Components (assign in Inspector)")]
    public Recorder voiceRecorder;  
    public Button muteButton;
    public Text muteButtonLabel;

    private bool isMuted = false;

    void Start()
    {
        voiceRecorder.TransmitEnabled = true;
        muteButton.onClick.AddListener(ToggleMute);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        voiceRecorder.TransmitEnabled = !isMuted;
        muteButtonLabel.text = isMuted ? "Unmute" : "Mute";
    }
}