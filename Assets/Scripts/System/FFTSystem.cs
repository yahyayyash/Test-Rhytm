using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FFTSystem : MonoBehaviour
{
    public int qSamples = 1024;  // array size
    public float refValue = 0.1f; // RMS value for 0 dB
    public float threshold = 0.02f;      // minimum amplitude to extract pitch
    public float rmsValue;   // sound level - RMS
    public float dbValue;    // sound level - dB
    public float sensitivity = 100;
    public float loudness = 0;

    public string microphone = null;

    private float[] samples; // audio samples
    private float[] spectrum; // audio spectrum
    private float fSample;
    private AudioSource audioSource;

    PitchDetector pitchDetector;

    int tempMidi = 0;
    double dspTime;

    private void Awake()
    {
        pitchDetector = GetComponent<PitchDetector>();
        audioSource = pitchDetector.source;
    }

    void Start()
    {
        samples = new float[qSamples];
        spectrum = new float[qSamples];
        fSample = AudioSettings.outputSampleRate;
        StartPlaying();
    }

    void Update()
    {
        AnalyzeSound();
    }

    void StartPlaying()
    {
        dspTime = AudioSettings.dspTime;
        pitchDetector.source.PlayScheduled(0);
    }

    void AnalyzeSound()
    {
        audioSource.GetOutputData(samples, 0); // fill array with samples

        int i;
        float sum = 0;
        for (i = 0; i < qSamples; i++)
        {
            sum += samples[i] * samples[i]; // sum squared samples
        }
        rmsValue = Mathf.Sqrt(sum / qSamples); // rms = square root of average
        dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
        if (dbValue < -160) dbValue = -160; // clamp it to -160dB min
                                            // get sound spectrum
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < qSamples; i++)
        { // find max 
            if (spectrum[i] > maxV && spectrum[i] > threshold)
            {
                maxV = spectrum[i];
                maxN = i; // maxN is the index of max
            }
        }

        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < qSamples - 1)
        { // interpolate index using neighbours
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }

        var pitch = freqN * (fSample / 2) / qSamples; // convert index to frequency
        var midiNote = 0;
        var midiCents = 0;
        
        Pitch.PitchDsp.PitchToMidiNote(pitch, out midiNote, out midiCents);

        pitchDetector.pitch = pitch;
        pitchDetector.midiNote = midiNote;

        //frequencyDisplay.text = $"{pitch} Hz";

        if (midiNote != 0 && midiNote != tempMidi)
        {
            tempMidi = midiNote;
            Debug.Log($"FFT Transcribed : {midiNote}, time : {AudioSettings.dspTime - dspTime}");
        }
    }

    void MicrophoneSetup()
    {
        foreach (string device in Microphone.devices)
        {
            if (microphone == null)
            {
                microphone = device;
            }
        }

        audioSource.clip = Microphone.Start(null, true, 10, 44100);
        audioSource.loop = false;
        audioSource.mute = false;
        while (!(Microphone.GetPosition(null) > 0)) { }

        audioSource.Play();

        Debug.Log(Microphone.IsRecording(microphone).ToString());

        if (Microphone.IsRecording(microphone))
        { //check that the mic is recording, otherwise you'll get stuck in an infinite loop waiting for it to start
            while (!(Microphone.GetPosition(microphone) > 0))
            {
            } // Wait until the recording has started. 

            Debug.Log("recording started with " + microphone);
            audioSource.Play();
        }
        else
        {
            Debug.Log(microphone + " doesn't work!");
        }
    }
}
