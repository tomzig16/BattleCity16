using UnityEngine;
using System.Collections;

/// <summary>
/// This class holds information about all sound effects in game.
/// </summary>
public class SoundManager : MonoBehaviour {

    #region Variables
    /// <summary>
    /// For easier acces I have created a bunch of AudioSources instead of one, fixed array. Same information applies for AudioClips.
    /// </summary>
    #region All possible sound clips.
    public AudioClip SFXpickupSpawned;
    public AudioClip SFXpickupTaken;
    public AudioClip SFXplayerDestroyed;
    public AudioClip SFXscoreTick;
    public AudioClip SFXenemyDestroyed;
    public AudioClip SFXobstacleDestroyed;
    public AudioClip SFXenteringIce;
    public AudioClip SFXadditionalHP;
    public AudioClip SFXhitsUnbreakable;
    public AudioClip SFXhitsArmor;
    public AudioClip SFXplayerMoving;
    public AudioClip SFXplayerShoting;
    public AudioClip SFXbackgroundNoise;
    #endregion
    #region Otuput sources
    public AudioSource SRCbackgroundNoise;
    public AudioSource SRCplayerShot;
    public AudioSource SRCpickup;
    public AudioSource SRChit1;
    public AudioSource SRChit2; // in case 2 bullets will hit at once.
    public AudioSource SRCexplosion1;
    public AudioSource SRCexplosion2; // in case second explosion will happen while first is still playing.
    public AudioSource SRCscoreTick;
    #endregion
    public static bool isBackgroundNoiseActive;
    public bool isGameBegan;
    #endregion
    // Use this for initialization
    void Start () {
        isBackgroundNoiseActive = false;
        isGameBegan = false;
        
	}
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.isGamePaused) {
            if (isBackgroundNoiseActive)
            {
                if (!isGameBegan)
                {
                    SRCbackgroundNoise = this.gameObject.AddComponent<AudioSource>();
                    SRCbackgroundNoise.playOnAwake = false;
                    SRCbackgroundNoise.clip = SFXbackgroundNoise;
                    SRCbackgroundNoise.loop = true;

                    SRCplayerShot = this.gameObject.AddComponent<AudioSource>();
                    SRCplayerShot.playOnAwake = false;
                    SRCplayerShot.clip = SFXplayerShoting;
                    SRCplayerShot.loop = false;

                    SRCpickup = this.gameObject.AddComponent<AudioSource>();
                    SRCpickup.playOnAwake = false;
                    SRCpickup.loop = false;

                    SRChit1 = this.gameObject.AddComponent<AudioSource>();
                    SRChit1.playOnAwake = false;
                    SRChit1.loop = false;

                    SRChit2 = this.gameObject.AddComponent<AudioSource>();
                    SRChit2.playOnAwake = false;
                    SRChit2.loop = false;

                    SRCexplosion1 = this.gameObject.AddComponent<AudioSource>();
                    SRCexplosion1.playOnAwake = false;
                    SRCexplosion1.loop = false;
                    SRCexplosion1.clip = SFXenemyDestroyed;

                    SRCexplosion2 = this.gameObject.AddComponent<AudioSource>();
                    SRCexplosion2.playOnAwake = false;
                    SRCexplosion2.loop = false;
                    SRCexplosion2.clip = SFXenemyDestroyed;

                    SRCscoreTick = this.gameObject.AddComponent<AudioSource>();
                    SRCscoreTick.playOnAwake = false;
                    SRCscoreTick.loop = false;
                    SRCscoreTick.clip = SFXscoreTick;

                    isGameBegan = true;
                    print("Sound Manager has been loaded successfully.");
                }
            }
        }
	}

    /// <summary>
    /// Gets right audio source and plays "hit" clip (which must be assigned before this method!)
    /// </summary>
    public void HitSound()
    {
        if(!SRChit1.isPlaying)
        {
            SRChit1.Play();
        }
        else
        {
            SRChit2.Play();
        }
    }

    /// <summary>
    /// Gets right audio source and plays "explosion" clip (which must be assigned before this method!)
    /// </summary>
    public void ExplosionSound()
    {
        if(!SRCexplosion1.isPlaying)
        {
            if(SRCexplosion1.clip != SFXenemyDestroyed)
            {
                SRCexplosion1.clip = SFXenemyDestroyed;
            }
            SRCexplosion1.Play();
        }
        else
        {
            // I did not check if clip is loaded, because this audi source cant be unloaded
            // In other words, all other explosions are played only on SRCexplosion1
            SRCexplosion2.Play();
        }
    }
}
