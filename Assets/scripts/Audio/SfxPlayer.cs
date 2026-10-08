using UnityEngine;

namespace UnitySudoku.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SfxPlayer : MonoBehaviour
    {
        public static SfxPlayer Instance { get; private set; }

        [SerializeField] private AudioClip menuClick;
        [SerializeField] private AudioClip goodEntry;
        [SerializeField] private AudioClip badEntry;

        [SerializeField, Range(0f, 1f)]
        private float sfxVolume = 1f;

        private AudioSource _audioSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.spatialBlend = 0f;

            DontDestroyOnLoad(gameObject);
        }

        public void PlayMenuClick()
        {
            Play(menuClick);
        }

        public void PlayGoodEntry()
        {
            Play(goodEntry);
        }

        public void PlayBadEntry()
        {
            Play(badEntry);
        }

        private void Play(AudioClip clip)
        {
            if (clip == null || _audioSource == null)
            {
                return;
            }

            _audioSource.PlayOneShot(clip, sfxVolume);
        }
    }
}
