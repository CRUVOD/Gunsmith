using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to store information about sound that can be played through the audiomanager
public struct AudioManagerSound
{
    /// the ID of the sound 
    public int ID;
    /// the track the sound is being played on
    public AudioManager.AudioManagerTracks Track;
    /// the associated audiosource
    public AudioSource Source;
    /// whether or not this sound will play over multiple scenes
    public bool Persistent;
}
