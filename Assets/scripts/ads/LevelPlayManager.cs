using UnityEngine;
using Unity.Services.LevelPlay;

namespace UnitySudoku.Ads
{
    public sealed class LevelPlayManager : MonoBehaviour
    {
        private const string AndroidAppKey = "2873102c5";
        private const string AndroidBannerAdUnitId = "7dr3vqzweob1wzx2";

        private LevelPlayBannerAd _bannerAd;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            LevelPlay.OnInitSuccess += HandleInitSuccess;
            LevelPlay.OnInitFailed += HandleInitFailed;

            Debug.Log("[LevelPlay] Initializing...");
            LevelPlay.Init(AndroidAppKey);
        }

        private void OnDestroy()
        {
            LevelPlay.OnInitSuccess -= HandleInitSuccess;
            LevelPlay.OnInitFailed -= HandleInitFailed;

            UnregisterBannerEvents();

            _bannerAd?.DestroyAd();
            _bannerAd = null;
        }

        private void HandleInitSuccess(LevelPlayConfiguration configuration)
        {
            Debug.Log("[LevelPlay] Initialization successful.");

            CreateBanner();
        }

        private void HandleInitFailed(LevelPlayInitError error)
        {
            Debug.LogError($"[LevelPlay] Initialization failed: {error}");
        }

        private void CreateBanner()
        {
            _bannerAd = new LevelPlayBannerAd(AndroidBannerAdUnitId);

            _bannerAd.OnAdLoaded += HandleBannerLoaded;
            _bannerAd.OnAdLoadFailed += HandleBannerLoadFailed;
            _bannerAd.OnAdDisplayed += HandleBannerDisplayed;
            _bannerAd.OnAdDisplayFailed += HandleBannerDisplayFailed;
            _bannerAd.OnAdClicked += HandleBannerClicked;
            _bannerAd.OnAdCollapsed += HandleBannerCollapsed;
            _bannerAd.OnAdExpanded += HandleBannerExpanded;
            _bannerAd.OnAdLeftApplication += HandleBannerLeftApplication;

            Debug.Log("[LevelPlay] Loading banner...");
            _bannerAd.LoadAd();
        }

        private void HandleBannerLoaded(LevelPlayAdInfo adInfo)
        {
            Debug.Log("[LevelPlay] Banner loaded.");
        }

        private void HandleBannerLoadFailed(LevelPlayAdError error)
        {
            Debug.LogError(
                $"[LevelPlay] Banner failed to load: {error}"
            );
        }

        private void HandleBannerDisplayed(LevelPlayAdInfo adInfo)
        {
            Debug.Log("[LevelPlay] Banner displayed.");
        }

        private void HandleBannerDisplayFailed(
            LevelPlayAdInfo adInfo,
            LevelPlayAdError error)
        {
            Debug.LogError(
                $"[LevelPlay] Banner failed to display. " +
                $"AdInfo: {adInfo}, Error: {error}"
            );
        }

        private void HandleBannerClicked(LevelPlayAdInfo adInfo)
        {
            Debug.Log("[LevelPlay] Banner clicked.");
        }

        private void HandleBannerCollapsed(LevelPlayAdInfo adInfo)
        {
            Debug.Log("[LevelPlay] Banner collapsed.");
        }

        private void HandleBannerExpanded(LevelPlayAdInfo adInfo)
        {
            Debug.Log("[LevelPlay] Banner expanded.");
        }

        private void HandleBannerLeftApplication(LevelPlayAdInfo adInfo)
        {
            Debug.Log("[LevelPlay] Banner left application.");
        }

        private void UnregisterBannerEvents()
        {
            if (_bannerAd == null)
            {
                return;
            }

            _bannerAd.OnAdLoaded -= HandleBannerLoaded;
            _bannerAd.OnAdLoadFailed -= HandleBannerLoadFailed;
            _bannerAd.OnAdDisplayed -= HandleBannerDisplayed;
            _bannerAd.OnAdDisplayFailed -= HandleBannerDisplayFailed;
            _bannerAd.OnAdClicked -= HandleBannerClicked;
            _bannerAd.OnAdCollapsed -= HandleBannerCollapsed;
            _bannerAd.OnAdExpanded -= HandleBannerExpanded;
            _bannerAd.OnAdLeftApplication -= HandleBannerLeftApplication;
        }
    }
}
