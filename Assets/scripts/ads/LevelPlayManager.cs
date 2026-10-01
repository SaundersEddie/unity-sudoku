using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Services.LevelPlay;

namespace UnitySudoku.Ads
{
    public sealed class LevelPlayManager : MonoBehaviour
    {
        private const string AndroidAppKey = "2873102c5";
        private const string AndroidInterstitialAdUnitId = "kw75g3dypx6bot6v";

        private LevelPlayInterstitialAd _interstitialAd;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            LevelPlay.OnInitSuccess += HandleInitSuccess;
            LevelPlay.OnInitFailed += HandleInitFailed;

            Debug.Log("[LevelPlay] Initializing...");
            LevelPlay.Init(AndroidAppKey);
        }

        private void Update()
        {
            if (Keyboard.current != null &&
                Keyboard.current.tKey.wasPressedThisFrame)
            {
                Debug.Log("[LevelPlay] Test interstitial requested.");
                ShowInterstitial();
            }
        }

        private void OnDestroy()
        {
            LevelPlay.OnInitSuccess -= HandleInitSuccess;
            LevelPlay.OnInitFailed -= HandleInitFailed;

            UnregisterInterstitialEvents();
        }

        private void HandleInitSuccess(LevelPlayConfiguration configuration)
        {
            Debug.Log("[LevelPlay] Initialization successful.");

            CreateInterstitial();
        }

        private void HandleInitFailed(LevelPlayInitError error)
        {
            Debug.LogError($"[LevelPlay] Initialization failed: {error}");
        }

        private void CreateInterstitial()
        {
            _interstitialAd =
                new LevelPlayInterstitialAd(AndroidInterstitialAdUnitId);

            _interstitialAd.OnAdLoaded += HandleInterstitialLoaded;
            _interstitialAd.OnAdLoadFailed += HandleInterstitialLoadFailed;
            _interstitialAd.OnAdDisplayed += HandleInterstitialDisplayed;
            _interstitialAd.OnAdDisplayFailed += HandleInterstitialDisplayFailed;
            _interstitialAd.OnAdClosed += HandleInterstitialClosed;
            _interstitialAd.OnAdClicked += HandleInterstitialClicked;

            Debug.Log("[LevelPlay] Loading interstitial...");
            _interstitialAd.LoadAd();
        }

        public void ShowInterstitial()
        {
            if (_interstitialAd == null)
            {
                Debug.LogWarning("[LevelPlay] Interstitial has not been created.");
                return;
            }

            if (!_interstitialAd.IsAdReady())
            {
                Debug.LogWarning("[LevelPlay] Interstitial is not ready.");
                return;
            }

            Debug.Log("[LevelPlay] Showing interstitial.");
            _interstitialAd.ShowAd();
        }

        private void HandleInterstitialLoaded(LevelPlayAdInfo adInfo)
        {
            Debug.Log("[LevelPlay] Interstitial loaded.");

        #if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log("[LevelPlay] Android test: showing interstitial in 3 seconds.");
            Invoke(nameof(ShowInterstitial), 3f);
        #endif
        }

        private void HandleInterstitialLoadFailed(LevelPlayAdError error)
        {
            Debug.LogError(
                $"[LevelPlay] Interstitial failed to load: {error}"
            );
        }

        private void HandleInterstitialDisplayed(LevelPlayAdInfo adInfo)
        {
            Debug.Log("[LevelPlay] Interstitial displayed.");
        }

        private void HandleInterstitialDisplayFailed(
            LevelPlayAdInfo adInfo,
            LevelPlayAdError error)
        {
            Debug.LogError(
                $"[LevelPlay] Interstitial failed to display. " +
                $"AdInfo: {adInfo}, Error: {error}"
            );

            _interstitialAd?.LoadAd();
        }

        private void HandleInterstitialClosed(LevelPlayAdInfo adInfo)
        {
            Debug.Log("[LevelPlay] Interstitial closed.");

            // Interstitial objects are reusable.
            // Load the next ad after this one closes.
            _interstitialAd?.LoadAd();
        }

        private void HandleInterstitialClicked(LevelPlayAdInfo adInfo)
        {
            Debug.Log("[LevelPlay] Interstitial clicked.");
        }

        private void UnregisterInterstitialEvents()
        {
            if (_interstitialAd == null)
            {
                return;
            }

            _interstitialAd.OnAdLoaded -= HandleInterstitialLoaded;
            _interstitialAd.OnAdLoadFailed -= HandleInterstitialLoadFailed;
            _interstitialAd.OnAdDisplayed -= HandleInterstitialDisplayed;
            _interstitialAd.OnAdDisplayFailed -= HandleInterstitialDisplayFailed;
            _interstitialAd.OnAdClosed -= HandleInterstitialClosed;
            _interstitialAd.OnAdClicked -= HandleInterstitialClicked;
        }
    }
}
