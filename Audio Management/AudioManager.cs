using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Revision 1.011 //
/// Author: <see href="https://github.com/tari-cat/UnityStuff"/>
/// 
/// <para>A relatively powerful audio manager for Unity.</para>
/// <para>Use it as you would use a static class.</para>
/// <para>To set it up, use <seealso cref="Resources"/>. It pulls <seealso cref="AudioClip"/> objects from the "Resources/Audio" folder.</para>
/// <para>You can manually create an <seealso cref="AudioManager"/> by giving the <seealso cref="AudioManager"/> component to an empty GameObject, then adding 32 (or however many sources you want) child objects and giving each the <seealso cref="AudioSource"/> component.</para>
/// <code>
/// AudioManager.Play("mySound");
/// </code>
/// </summary>
public class AudioManager : MonoBehaviour
{
    // The audio mixer to use.
    public AudioMixer mixer;

    // A list of audio sources for caching.
    private readonly List<AudioSRC> sources = new List<AudioSRC>();

    // A readonly list of audio clips. Added via the "Resources/Audio" folder.
    private AudioClip[] clips = new AudioClip[0];

    public static AudioClip[] Clips { get => Instance.clips; }

    private static GameObject _instance;

    // Instance property. Creates one if it doesn't exist.
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject
                {
                    name = "(Generated) Audio Manager"
                };
                _instance.AddComponent<AudioManager>();
            }

            return _instance.GetComponent<AudioManager>();
        }
    }

    private void GenerateAudioSources()
    {
        // Generate Audio Sources
        if (transform.childCount == 0)
        {
            for (int i = 0; i < 32; i++)
            {
                GameObject go = new GameObject
                {
                    name = $"(Generated) Audio Source #{i + 1}"
                };
                go.AddComponent<AudioSource>();
                go.transform.parent = gameObject.transform;
            }
        }
    }

    private void GetAudioSources()
    {
        int count = Mathf.Min(32, transform.childCount);
        for (int i = 0; i < count; i++)
        {
            Transform c = transform.GetChild(i);
            if (c.GetComponent<AudioSource>() != null)
            {
                sources.Add(new AudioSRC(c.gameObject));
            }
        }
    }

    // Build a cache of all the existing audio clips. Really cheap and easy to do, no longer have to manually add sounds.
    private static void BuildCache()
    {
        try
        {
            Instance.clips = Resources.LoadAll<AudioClip>("Audio");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private static void OnSceneChange(Scene scene1, Scene scene2)
    {
        StopAllSounds();
    }

    private void Awake()
    {
        if (_instance != null && _instance != gameObject)
            return;
        _instance = gameObject;

        SceneManager.activeSceneChanged -= OnSceneChange; // unsub if subbed or not
        SceneManager.activeSceneChanged += OnSceneChange; // prevents double sub

        GenerateAudioSources();

        // Get Audio Sources and add them
        GetAudioSources();

        // Generate Clips cache from resources
        BuildCache();
    }

    // A lot of methods that go without explanation. These ones simply play a sound.

    public static AudioSource Play(int index, bool oneshot, float volume, Vector3 location)
    {
        return Instance.PlayClip(index, oneshot, volume, location);
    }

    public static AudioSource Play(AudioClip clip, float volume)
    {
        return Instance.PlayClip(clip, volume, Vector3.zero);
    }

    public static AudioSource Play(AudioClip clip, float volume, Vector3 location)
    {
        return Instance.PlayClip(clip, volume, location);
    }

    public static AudioSource Play(AudioClip clip, bool oneshot, float volume, Vector3 location)
    {
        return Instance.PlayClip(clip, oneshot, volume, location);
    }

    public static AudioSource Play(string name, bool oneshot, float volume, Vector3 location)
    {
        foreach (AudioClip clip in Instance.clips)
        {
            if (clip.name.ToLower() == name.ToLower())
            {
                return Instance.PlayClip(clip, oneshot, volume, location);
            }
        }

        Debug.LogWarning("No sound named " + name + ", skipping");
        return null;
    }

    public static AudioSource Play(int index, float volume, Vector3 location)
    {
        return Play(index, false, volume, location);
    }

    public static AudioSource Play(string name, float volume, Vector3 location)
    {
        return Play(name, false, volume, location);
    }

    public static AudioSource Play(int index, float volume)
    {
        return Play(index, false, volume, new Vector3());
    }

    public static AudioSource Play(string name, float volume)
    {
        return Play(name, false, volume, new Vector3());
    }

    // Play a sound, but with a lerping volume.

    public static AudioSource PlayLerp(string name, float startVolume, float endVolume, Vector3 location)
    {
        foreach (AudioClip clip in Instance.clips)
        {
            if (clip.name.ToLower() == name.ToLower())
            {
                return PlayLerp(clip, startVolume, endVolume, location);
            }
        }

        Debug.LogWarning("No sound named " + name + ", skipping");
        return null;
    }

    public static AudioSource PlayLerp(string name, float startVolume, float endVolume)
    {
        return PlayLerp(name, startVolume, endVolume, new Vector3());
    }

    public static AudioSource PlayLerp(int index, float startVolume, float endVolume, Vector3 location)
    {
        return PlayLerp(Instance.clips[index], startVolume, endVolume, location);
    }

    public static AudioSource PlayLerp(int index, float startVolume, float endVolume)
    {
        return PlayLerp(index, startVolume, endVolume, new Vector3());
    }

    public static AudioSource PlayLerp(AudioClip clip, float startVolume, float endVolume, Vector3 location)
    {
        AudioSRC asrc = Instance.GetAudioSource();
        if (asrc != null)
        {
            asrc.PlayLerp(clip, location, clip.length, startVolume, endVolume);
            return asrc.audioSource;
        }
        return null;
    }

    public static AudioSource PlayLerp(AudioClip clip, float startVolume, float endVolume)
    {
        return PlayLerp(clip, startVolume, endVolume, new Vector3());
    }

    // Play a sound, but it loops indefinitely until told to stop.
    public static AudioSource PlayLoop(string name, float volume, Vector3 location)
    {
        foreach (AudioClip clip in Instance.clips)
        {
            if (clip.name.ToLower() == name.ToLower())
            {
                return PlayLoop(clip, volume, location);
            }
        }
        return null;
    }

    public static AudioSource PlayLoop(string name, float volume)
    {
        return PlayLoop(name, volume, new Vector3());
    }

    public static AudioSource PlayLoop(AudioClip clip, float volume, Vector3 location)
    {
        return Instance.PlayLoopClip(clip, volume, location);
    }

    public static AudioSource PlayLoop(AudioClip clip, float volume)
    {
        return PlayLoop(clip, volume, new Vector3());
    }

    public static AudioSource PlayLoop(int index, float volume, Vector3 location)
    {
        return Instance.PlayLoopClip(index, volume, location);
    }

    public static AudioSource PlayLoop(int index, float volume)
    {
        return PlayLoop(index, volume, new Vector3());
    }

    // Local / Instance methods. Used internally.

    private AudioSource PlayClip(int index, bool oneshot, float volume, Vector3 location)
    {
        return PlayClip(clips[index], oneshot, volume, location);
    }

    private AudioSource PlayClip(AudioClip clip, bool oneshot, float volume, Vector3 location)
    {
        AudioSRC asrc = GetAudioSource(); // Get a free audio source.
        if (asrc != null) // If we have one
        {
            asrc.ResetSource();
            if (oneshot) // Play the sound
            {
                asrc.PlayOneShot(clip, volume, location);
            }
            else
            {
                asrc.Play(clip, volume, location);
            }
            return asrc.audioSource;
        }
        return null;
    }

    private AudioSource PlayClip(int index, float volume, Vector3 location)
    {
        return PlayClip(clips[index], false, volume, location);
    }

    private AudioSource PlayClip(AudioClip clip, float volume, Vector3 location)
    {
        AudioSRC asrc = GetAudioSource(); // Get a free audio source
        if (asrc != null) // Play the sound
        {
            asrc.ResetSource();
            asrc.Play(clip, volume, location);
            return asrc.audioSource;
        }
        return null;
    }

    private AudioSource PlayLoopClip(int index, float volume, Vector3 location)
    {
        AudioSRC asrc = GetAudioSource();
        if (asrc != null)
        {
            asrc.ResetSource();
            asrc.PlayLoop(clips[index], volume, location);
            return asrc.audioSource;
        }
        return null;
    }

    private AudioSource PlayLoopClip(AudioClip clip, float volume, Vector3 location)
    {
        AudioSRC asrc = GetAudioSource();
        if (asrc != null)
        {
            asrc.ResetSource();
            asrc.PlayLoop(clip, volume, location);
            return asrc.audioSource;
        }
        return null;
    }

    private AudioSource PlayLerpCoroutine(AudioSource src, float length, float startVolume, float endVolume)
    {
        StartCoroutine(LerpCoroutine(src, length, startVolume, endVolume));
        return src;
    }

    private IEnumerator LerpCoroutine(AudioSource src, float length, float startVolume, float endVolume)
    {
        float startTime = Time.time;
        float endTime = startTime + length;
        while (src.isPlaying && Time.time < endTime) // While the clip is playing
        {
            float currentTime = Time.time;
            float progress = (currentTime - startTime) / length;
            src.volume = Mathf.Lerp(startVolume, endVolume, progress); // Linearly lerp the volume
            yield return null;
        }
    }

    public static void StopAllSounds() // Stops all sounds.
    {
        List<AudioSRC> sources = Instance.sources;
        foreach (AudioSRC src in sources)
        {
            src.Stop();
        }
    }

    public static void Stop(string name) // Stop all sounds featuring a specific clip name.
    {
        foreach (AudioSRC src in Instance.sources)
        {
            if (src.audioSource.clip != null && src.audioSource.clip.name.ToLower().Trim() == name.ToLower().Trim())
            {
                src.Stop();
            }
        }
    }

    public static void Stop(int index) // Stop all sounds featuring a specific clip index.
    {
        if (index > Instance.clips.Length - 1 || index < 0)
            return;
        AudioClip clip = Instance.clips[index];

        foreach (AudioSRC src in Instance.sources)
        {
            AudioSource asrc = src.go.GetComponent<AudioSource>();
            if (asrc.clip == clip)
            {
                asrc.Stop();
            }
        }
    }

    private void StopAll() // Stops all sounds internally.
    {
        foreach (AudioSRC src in sources)
        {
            src.Stop();
        }
    }

    private AudioSRC GetAudioSource() // Get a free audio source.
    {
        foreach (AudioSRC src in sources)
        {
            if (src.IsFree)
            {
                return src;
            }
        }
        return null;
    }

    class AudioSRC
    {
        public GameObject go;
        public AudioSource audioSource;

        public bool IsFree // If the audio source can be used.
        {
            get
            {
                return Time.time > freeAt;
            }
        }

        private float freeAt = 0f;

        public AudioSRC(GameObject go)
        {
            this.go = go;
            audioSource = go.GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip, float volume, Vector3 location) // Play a clip internally.
        {
            freeAt = Time.time + clip.length;
            audioSource.clip = clip;
            PlayClip(volume, false, location);
        }

        public void PlayOneShot(AudioClip clip, float volume, Vector3 location) // Play a clip oneshot internally.
        {
            audioSource.clip = clip;
            PlayClip(volume, true, location);
        }

        private void PlayClip(float volume, bool oneshot, Vector3 location) // Play a clip internally
        {
            audioSource.gameObject.transform.position = location;
            audioSource.volume = volume;
            audioSource.clip.LoadAudioData();
            if (oneshot)
            {
                audioSource.PlayOneShot(audioSource.clip);
                return;
            }
            audioSource.loop = false;
            audioSource.Play();
        }

        public void PlayLoop(AudioClip clip, float volume, Vector3 location) // Loop a clip internally
        {
            audioSource.clip = clip;
            audioSource.transform.position = location;
            audioSource.volume = volume;
            audioSource.clip.LoadAudioData();
            audioSource.loop = true;
            freeAt = float.MaxValue;
            audioSource.Play();
        }

        public void Stop() // Stops the source
        {
            audioSource.Stop();
            freeAt = Time.time;
        }

        public void PlayLerp(AudioClip clip, Vector3 location, float length, float startVolume, float endVolume) // Plays a lerp
        {
            Play(clip, startVolume, location);
            Instance.PlayLerpCoroutine(audioSource, length, startVolume, endVolume);
        }

        public void ResetSource()
        {
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.spatialBlend = 1f;
            audioSource.volume = 1f;
            audioSource.loop = false;
            audioSource.pitch = 1f;
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 500f;
            audioSource.mute = false;
            audioSource.bypassEffects = false;
            audioSource.bypassListenerEffects = false;
            audioSource.bypassReverbZones = false;
            audioSource.priority = 128;
            audioSource.panStereo = 0f;
            audioSource.reverbZoneMix = 1f;
            audioSource.dopplerLevel = 1f;
            audioSource.spread = 0f;
        }
    }
}
