using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField]
    public AudioClip[] stepClips;

    public AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        if (!source)
        {
            source = GetComponent<AudioSource>();
        }
    }

    public void Step()
    {
        source.clip = stepClips[Random.Range(0, stepClips.Length)];
        source.Play();
    }
}
