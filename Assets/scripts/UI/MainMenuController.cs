using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnitySudoku.Core;
using UnitySudoku.SceneFlow;

namespace UnitySudoku.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class MainMenuController : MonoBehaviour
    {
        private Button _easyButton;
        private Button _mediumButton;
        private Button _hardButton;
        private Button _godlikeButton;

        private Button _playButton;
        private Button _aboutButton;
        private Button _quitButton;
        private Button _closeAboutButton;

        private Label _difficultyInfoLabel;
        private VisualElement _aboutPanel;

        private DifficultyLevel _selectedDifficulty = DifficultyLevel.Easy;

        private void OnEnable()
        {
            UIDocument document = GetComponent<UIDocument>();
            VisualElement root = document.rootVisualElement;

            _easyButton = root.Q<Button>("easyButton");
            _mediumButton = root.Q<Button>("mediumButton");
            _hardButton = root.Q<Button>("hardButton");
            _godlikeButton = root.Q<Button>("godlikeButton");

            _playButton = root.Q<Button>("playButton");
            _aboutButton = root.Q<Button>("aboutButton");
            _quitButton = root.Q<Button>("quitButton");
            _closeAboutButton = root.Q<Button>("closeAboutButton");

            _difficultyInfoLabel = root.Q<Label>("difficultyInfoLabel");
            _aboutPanel = root.Q<VisualElement>("aboutPanel");

            _easyButton.clicked += () => SelectDifficulty(DifficultyLevel.Easy);
            _mediumButton.clicked += () => SelectDifficulty(DifficultyLevel.Medium);
            _hardButton.clicked += () => SelectDifficulty(DifficultyLevel.Hard);
            _godlikeButton.clicked += () => SelectDifficulty(DifficultyLevel.Godlike);

            _playButton.clicked += PlayLevel;
            _aboutButton.clicked += ShowAboutPanel;
            _quitButton.clicked += QuitGame;
            _closeAboutButton.clicked += HideAboutPanel;

            SelectDifficulty(GameSettings.SelectedDifficulty);
        }

        private void OnDisable()
        {
            _playButton.clicked -= PlayLevel;
            _aboutButton.clicked -= ShowAboutPanel;
            _quitButton.clicked -= QuitGame;
            _closeAboutButton.clicked -= HideAboutPanel;
        }

        private void SelectDifficulty(DifficultyLevel difficulty)
        {
            _selectedDifficulty = difficulty;
            GameSettings.SelectedDifficulty = difficulty;

            ClearDifficultySelection();

            Button selectedButton = difficulty switch
            {
                DifficultyLevel.Easy => _easyButton,
                DifficultyLevel.Medium => _mediumButton,
                DifficultyLevel.Hard => _hardButton,
                DifficultyLevel.Godlike => _godlikeButton,
                _ => _easyButton
            };

            selectedButton.AddToClassList("selected");

            int clueCount = GameSettings.GetVisibleClueCount(difficulty);
            _difficultyInfoLabel.text = $"{difficulty}: {clueCount} starting clues";
        }

        private void ClearDifficultySelection()
        {
            _easyButton.RemoveFromClassList("selected");
            _mediumButton.RemoveFromClassList("selected");
            _hardButton.RemoveFromClassList("selected");
            _godlikeButton.RemoveFromClassList("selected");
        }

        private void PlayLevel()
        {
            GameSettings.SelectedDifficulty = _selectedDifficulty;
            SceneManager.LoadScene(SceneNames.GamePlay);
        }

        private void ShowAboutPanel()
        {
            _aboutPanel.RemoveFromClassList("hidden");
        }

        private void HideAboutPanel()
        {
            _aboutPanel.AddToClassList("hidden");
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            Debug.Log("Quit requested. Application.Quit is ignored in the Unity Editor.");
#elif UNITY_WEBGL
            Debug.Log("Quit requested. WebGL builds cannot close the browser tab.");
#else
            Application.Quit();
#endif
        }
    }
}
