using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastYinSystem : MonoBehaviour
{

    public TMPro.TextMeshProUGUI frequencyDisplay;

    PitchDetector pitchDetector;

    FastYin fastYin;

    int tempMidi = 0;

    private void Awake()
    {
        pitchDetector = GetComponent<PitchDetector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        fastYin = new FastYin(44100, 1024);
        //MicrophoneSetup();
    }

    // Update is called once per frame
    void Update()
    {
        FastYin();
    }

    private void FastYin()
    {
        var buffer = new float[1024];
        pitchDetector.source.GetOutputData(buffer, 0);

        var result = fastYin.getPitch(buffer);

        var pitch = result.getPitch();
        var midiNote = 0;
        var midiCents = 0;

        Pitch.PitchDsp.PitchToMidiNote(pitch, out midiNote, out midiCents);

        pitchDetector.pitch = pitch;
        pitchDetector.midiNote = midiNote;

        frequencyDisplay.text = $"{pitch} Hz";

        if (midiNote != 0 && midiNote != tempMidi)
        {
            tempMidi = midiNote;
            Debug.Log($"Transcribed : {midiNote}, time : {SongManager.GetAudioSourceTime()}");
        }
    }

    void StartPlaying()
    {
        pitchDetector.source.Play();
    }

    void MicrophoneSetup()
    {
        if (Microphone.IsRecording(null))
        {
            pitchDetector.source.Stop();
            pitchDetector.source.clip = Microphone.Start(null, true, 1, 44100);
            pitchDetector.source.loop = true;
            pitchDetector.source.mute = false;
            pitchDetector.source.Play();
        }
        else
        {
            pitchDetector.source.Stop();
            Microphone.End(null);
        }
    }
}
