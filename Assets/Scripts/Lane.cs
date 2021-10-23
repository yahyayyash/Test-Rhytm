using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Interaction;
using System;

public class Lane : MonoBehaviour
{
    public KeyCode input;
    public GameObject notePrefab;
    public GameObject barPrefab;
    public Sprite noteWrong;
    public Sprite noteRight;

    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();
    public List<float> noteDurations = new List<float>();
    
    int spawnIndex = 0;
    int inputIndex = 0;
    int barIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());
            timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);

            var noteTime = note.LengthAs<MusicalTimeSpan>(SongManager.midiFile.GetTempoMap());
            noteDurations.Add((float)noteTime.Numerator / (float)noteTime.Denominator);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (SongManager.GetCurrentBeat() >= barIndex)
        {
            //Debug.Log($"Current Beat = {SongManager.GetCurrentBeat()}, Current Index {barIndex}");
            SpawnMusicBar();
        }

        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
            {
                SpawnMusicNote();
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = SongManager.Instance.marginOfError;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

            if (Input.GetKeyDown(input))
            {
                if (Math.Abs(audioTime - timeStamp) < marginOfError)
                {
                    Hit();
                    notes[inputIndex].gameObject.GetComponent<SpriteRenderer>().sprite = noteRight;
                    //Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }
                else
                {
                    //print($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
                }
            }
            if (timeStamp + marginOfError <= audioTime)
            {
                Miss();
                notes[inputIndex].gameObject.GetComponent<SpriteRenderer>().sprite = noteWrong;
                inputIndex++;
            }

        }
    }

    private void SpawnMusicNote()
    {
        var note = Instantiate(notePrefab, transform);
        note.GetComponent<Note>().assignedTime = timeStamps[spawnIndex];
        note.GetComponent<Note>().noteLength = noteDurations[spawnIndex];
        notes.Add(note.GetComponent<Note>());
        Debug.Log($"Note Length for {spawnIndex} notes is {(float)noteDurations[spawnIndex]}");
        spawnIndex++;
    }

    private void SpawnMusicBar()
    {
        var bar = Instantiate(barPrefab, transform);
        bar.GetComponent<Bar>().assignedTime = SongManager.GetAudioSourceTime();
        barIndex ++;
    }

    private void Miss()
    {
        ScoreManager.Miss();
    }

    private void Hit()
    {
        ScoreManager.Hit();
    }
}
