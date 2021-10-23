using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    double timeInstantiated;
    public double assignedTime;
    public float noteLength;

    // Start is called before the first frame update
    void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime();
        GetComponent<SpriteRenderer>().size = new Vector2(noteLength * 10, 0.6f);
    }

    //Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = CheckTime();
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(Vector3.right * SongManager.Instance.noteSpawnX, Vector3.right * SongManager.Instance.noteDespawnX, t);
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }

    private double CheckTime()
    {
        if (SongManager.Instance.noteTime >= assignedTime)
        {
            return SongManager.Instance.noteTime - assignedTime + SongManager.GetAudioSourceTime();
        }
        else
        {
            return SongManager.GetAudioSourceTime() - timeInstantiated;
        }
    }
}
