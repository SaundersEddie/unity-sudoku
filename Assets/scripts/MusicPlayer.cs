using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.25f;
    [SerializeField, Min(0f)] private float fadeOutSeconds = 10f;
    [SerializeField, Min(0f)] private float restartDelay = 0.25f;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        // DontDestroyOnLoad(gameObject);
    }

    private void Start() => StartCoroutine(PlayMusicCycle());

    private IEnumerator PlayMusicCycle()
    {
        while (music != null)
        {
            source.clip = music;
            source.volume = musicVolume;
            source.time = 0f;
            source.Play();

            float fadeStart = Mathf.Max(0f, music.length - fadeOutSeconds);
            yield return new WaitForSecondsRealtime(fadeStart);

            float actualFade = Mathf.Min(fadeOutSeconds, music.length);
            for (float elapsed = 0f; elapsed < actualFade; elapsed += Time.unscaledDeltaTime)
            {
                source.volume = Mathf.Lerp(musicVolume, 0f, elapsed / actualFade);
                yield return null;
            }

            source.Stop();
            yield return new WaitForSecondsRealtime(restartDelay);
        }
    }
}
