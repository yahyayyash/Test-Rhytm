using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset textJSON1;
    public TextAsset textJSON2;

    public MIDI.MidiFile midiFile = new MIDI.MidiFile();
    public MIDI2.MidiFile midiFile2 = new MIDI2.MidiFile();

    // Start is called before the first frame update
    void Start()
    {
        //midiFile = JsonUtility.FromJson<MIDI.MidiFile>(textJSON.text);
        midiFile = MIDI.CreateFromJSON(textJSON1.text);
        //Debug.Log($"BPM = {midiFile.tempo[0].bpm}, FIRST NOTE = {midiFile.tracks[0].notes[0].name}");

        midiFile2 = MIDI2.CreateFromJSON(textJSON2.text);
        //Debug.Log($"BPM = {midiFile2.header.name}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
