using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySound : MonoBehaviour
{
    [SerializeField]
    public AudioClip[] abilityClips;

    public AudioSource source;

    Dictionary<string, AudioClip> AbilitySounds = new Dictionary<string, AudioClip>() {};
    void Start()
    {
        AbilitySounds.Add("Dash", abilityClips[0]);
        AbilitySounds.Add("Hit", abilityClips[1]);
        AbilitySounds.Add("Stomp", abilityClips[2]);
    }

    public void Sound(string ability)
    {  
        source.clip = AbilitySounds[ability];
        source.Play();
    }
}
