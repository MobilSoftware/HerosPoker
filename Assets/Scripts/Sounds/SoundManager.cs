using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    #region Singleton
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static SoundManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static SoundManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first SoundManager object in the scene.
                s_Instance = FindObjectOfType(typeof(SoundManager)) as SoundManager;
                //if (s_Instance == null)
                    //Debug.Log("Could not locate an SoundManager object. \n You have to have exactly one SoundManager in the scene.");
            }
            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
        s_Instance = null;
    }
    #endregion

    public bool bMenu = false;

    [SerializeField] AudioSource curBackgroundMusic;
    public AudioSource sfxSource, sfxSource2;

    [SerializeField] AudioClip[] BGMs;

    public AudioClip[] SFXs;

    // Use this for initialization
    void Awake()
    {
        //LoadAudioClips();

        //Sound Mute or not
        //MuteSound(PlayerProfile.MuteMusic);
        SetVolume(PlayerPrefs.GetFloat (PrefKey.VolBGM.ToString(), 0.8f));
    }

    void LoadAudioClips()
    {
        string Path = "MusicAndSFXs/SFXs/";

        SFXs = new AudioClip[(int)SFXType.Max];
        for (int i = 0; i < SFXs.Length; i++)
        {
            string iconName = ((SFXType)i).ToString();
            SFXs[i] = (AudioClip)Resources.Load(Path + iconName);
            CheckAudioClip(SFXs[i], iconName);

        }
    }

    public void CheckAudioClip(AudioClip obj, string Name)
    {
        if (!obj)
            Debug.LogError(Name + " : Not Loaded");
    }

    public void PlaySFX(SFXType type, Vector3 position, bool isFirst = false, int channel = 1)
    {
        //float volume = PlayerPrefs.GetFloat (PrefKey.VolSFX.ToString(), 0.8f);
        //if (isFirst)
        //    return;
        //if (SFXs == null)
        //    return;
        //if (SFXs[(int)type] == null)
        //    return;

        //AudioClip clip = SFXs[(int)type];
        //if (channel == 1)
        //{
        //    sfxSource.Stop();
        //    sfxSource.clip = clip;
        //    sfxSource.volume = volume;
        //    sfxSource.PlayOneShot(clip, volume);
        //}
        //else
        //{
        //    sfxSource2.clip = clip;
        //    sfxSource2.volume = volume;
        //    sfxSource2.PlayOneShot(clip, volume);
        //}

    }

    public void PlaySFX2Loop (SFXType type )
    {
        //float volume = PlayerPrefs.GetFloat (PrefKey.VolSFX.ToString (), 0.8f);

        //if (SFXs == null)
        //    return;
        //if (SFXs[(int) type] == null)
        //    return;

        //AudioClip clip = SFXs[(int) type];
        //sfxSource2.clip = clip;
        //sfxSource2.volume = volume;
        //sfxSource2.loop = true;
        ////sfxSource2.PlayOneShot (clip, volume);
        //sfxSource2.Play ();
    }


    public void StopSFX2()
    {
        sfxSource2.Stop();
    }

    public void ResumeMusic()
    {
        curBackgroundMusic.Play();
    }

    public void PauseMusic()
    {
        curBackgroundMusic.Pause();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"> 0 = home, 1 = capsa, 2 = poker </param>
    /// <param name="bIncludeIntro"></param>
    public void ChangeBackgroundMusic(int index)
    {
        //curBackgroundMusic.clip = BGMs[index];
        //curBackgroundMusic.Play();
    }

    public void SetPitch(float value)
    {
        if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().pitch = value;
    }

    public void SetVolume(float value)
    {
        if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().volume = value;
    }

    public void MuteSound(bool bMute)
    {
        //Get the audio source 
        if (bMute)
        {
            if (GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().mute = true;
                GetComponent<AudioSource>().Pause();
            }
        }
        else
        {
            if (GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().mute = false;
                GetComponent<AudioSource>().Play();
            }
        }
    }

    private void OnMusicVolumeChanged(float val)
    {
        SetVolume(val);
    }
}
