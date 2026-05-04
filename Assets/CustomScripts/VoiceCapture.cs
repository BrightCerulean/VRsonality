using Unity.Netcode;
using UnityEngine;

public class VoiceCapture : NetworkBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Settings")]
    public int sampleRate = 16000;
    public int captureFrequency = 20; 

    private AudioClip _micClip;
    private float _captureTimer;
    private int _lastSamplePos;
    private float _captureInterval;
    private string _micDevice;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _captureInterval = 1f / captureFrequency;
        _micDevice = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;

        if (_micDevice == null)
        {
            Debug.LogError("No microphone found!");
            return;
        }

        _micClip = Microphone.Start(_micDevice, true, 1, sampleRate);
        Debug.Log($"Microphone started: {_micDevice}");
    }

    void Update()
    {
        if (!IsOwner || _micClip == null) return;

        _captureTimer += Time.deltaTime;
        if (_captureTimer < _captureInterval) return;
        _captureTimer = 0f;

        int currentPos = Microphone.GetPosition(_micDevice);
        if (currentPos < 0 || currentPos == _lastSamplePos) return;

        int sampleCount;
        if (currentPos > _lastSamplePos)
            sampleCount = currentPos - _lastSamplePos;
        else
            sampleCount = (_micClip.samples - _lastSamplePos) + currentPos;

        if (sampleCount <= 0) return;

        float[] samples = new float[sampleCount];
        _micClip.GetData(samples, _lastSamplePos);
        _lastSamplePos = currentPos;

        SendVoiceServerRpc(samples);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    void SendVoiceServerRpc(float[] samples)
    {
        ReceiveVoiceClientRpc(samples);
    }

    [Rpc(SendTo.NotOwner)]
    void ReceiveVoiceClientRpc(float[] samples)
    {
        if (audioSource == null) return;

        AudioClip clip = AudioClip.Create("voice", samples.Length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        audioSource.PlayOneShot(clip);
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && _micDevice != null)
            Microphone.End(_micDevice);
    }
}