using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Melanchall.DryWetMidi.Core;
//using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance;
    public AudioSource audioSource;
    public Lane lanes;
    public float songDelayInSeconds;
    public double marginOfError; // in seconds
    public int inputDelayInMilliseconds;

    public TextAsset midiJSON;
    public float noteTime; //Time needed for the note spawn location to the tap location
    public float noteSpawnX;
    public float noteTapX;

    public static MIDI.MidiFile midiFile;

    public float noteDespawnX
    {
        get
        {
            return noteTapX - (noteSpawnX - noteTapX);
        }
    }

    static float midiBPM = 0.6f;

    //static float midiBPM
    //{
    //    get
    //    {
    //        return (60 / (float)midiFile.header.tempo[0].bpm);
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        midiFile = MIDI.CreateFromJSON(midiJSON.text);
        GetDataFromMidi();
    }

    private void GetDataFromMidi()
    {
        var notes = midiFile.tracks[0].notes;
        var array = new MIDI.Notes[notes.Length];
        notes.CopyTo(array, 0);

        lanes.SetTimeStamps(array);

        Invoke(nameof(StartSong), songDelayInSeconds);
    }

    private void StartSong()
    {
        audioSource.Play();
    }

    // Get current playback position in metric times
    public static double GetAudioSourceTime()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    // Get current beat in float
    public static float GetCurrentBeat()
    {
        return (float)GetAudioSourceTime() / midiBPM;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
