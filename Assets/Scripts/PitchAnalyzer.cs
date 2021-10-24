using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PitchAnalyzer : MonoBehaviour
{

    public int qSamples = 1024;  // array size
    public float refValue = 0.1f; // RMS value for 0 dB
    public float threshold = 0.02f;      // minimum amplitude to extract pitch
    public float rmsValue;   // sound level - RMS
    public float dbValue;    // sound level - dB
    public float pitchValue; // sound pitch - Hz
    public float sensitivity = 100;
    public float loudness = 0;

    public TMPro.TextMeshPro display; // drag a GUIText here to show results
    public string microphone = null;

    private List<string> options = new List<string>();
    private float[] samples; // audio samples
    private float[] spectrum; // audio spectrum
    private float fSample;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        //audioSource.Stop();

        GetMicrophoneDevice();

        samples = new float[qSamples];
        spectrum = new float[qSamples];
        fSample = AudioSettings.outputSampleRate;
    }

    void AnalyzeSound()
    {
        audioSource.GetOutputData(samples, 0);

        int i;
        float sum = 0;
        for (i = 0; i < qSamples; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / qSamples);
        dbValue = 20 * Mathf.Log10(rmsValue / refValue);
        if (dbValue < -160) dbValue = -160;

        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < qSamples; i++)
        {
            if (spectrum[i] > maxV && spectrum[i] > threshold)
            {
                maxV = spectrum[i];
                maxN = i;
            }
        }
        float freqN = maxN;
        if (maxN > 0 && maxN < qSamples - 1)
        {
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }

        pitchValue = freqN * (fSample / 2) / qSamples;
    }



    void Update()
    {
        //loudness = GetAveragedVolume() * sensitivity;
        //if (loudness > 1)
        //{
        //    AnalyzeSound();
        //}

        //AnalyzeSound();

        //display.text = $"{pitchValue} Hz";

    }

    void GetMicrophoneDevice()
    {
        audioSource.clip = Microphone.Start(null, true, 100, 44100);
        audioSource.loop = true;
        audioSource.mute = false;
        while (!(Microphone.GetPosition(null) > 0)) { }
        audioSource.Play();
    }

    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        audioSource.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }

}
