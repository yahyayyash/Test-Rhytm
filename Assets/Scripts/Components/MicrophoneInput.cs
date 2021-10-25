using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public AudioSource source;
    public string deviceName;
    public bool isRecording = true;
    public int sampleRate = 44100;
}
