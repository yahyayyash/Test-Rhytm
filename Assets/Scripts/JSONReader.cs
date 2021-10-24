using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset textJSON;

    [System.Serializable]
    public class Tempo
    {
        public int absoluteTime;
        public int seconds;
        public int bpm;
    }

    [System.Serializable]
    public class TimeSignature
    {
        public int absoluteTime;
        public int seconds;
        public int numerator;
        public int denominator;
        public int click;
        public int notesQ;
    }

    [System.Serializable]
    public class Tracks
    {
        public float startTime;
        public double duration;
        public int length;
        public Notes[] notes;
    }

    [System.Serializable]
    public class Notes
    {
        public string name;
        public int midi;
        public float time;
        public double velocity;
        public double duration;
    }

    [System.Serializable]
    public class MidiFile
    {
        public Tempo[] tempo;
        public TimeSignature[] timeSignature;
        public float startTime;
        public double duration;
        public Tracks[] tracks;
    }

    public MidiFile midiFile = new MidiFile();

    // Start is called before the first frame update
    void Start()
    {
        midiFile = JsonUtility.FromJson<MidiFile>(textJSON.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
