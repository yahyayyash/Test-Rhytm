using System;
using UnityEngine;

public class MIDI
{
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

    public static MidiFile CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<MidiFile>(jsonString);
    }
}
