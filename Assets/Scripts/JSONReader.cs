using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset textJSON;

    public MIDI.MidiFile midiFile = new MIDI.MidiFile();

    // Start is called before the first frame update
    void Start()
    {
        //midiFile = JsonUtility.FromJson<MIDI.MidiFile>(textJSON.text);
        midiFile = MIDI.CreateFromJSON(textJSON.text);
        Debug.Log($"BPM = {midiFile.tempo[0].bpm}, FIRST NOTE = {midiFile.tracks[0].notes[0].name}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
