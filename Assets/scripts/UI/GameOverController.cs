using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnitySudoku.Core;
using UnitySudoku.SceneFlow;

namespace UnitySudoku.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class GameOverController : MonoBehaviour
    {
        private Label _difficultyLabel;
        private Label _timeLabel;
        private Label _movesLabel;

        private Button _playAgainButton;
        private Button _mainMenuButton;

        private void OnEnable()
        {
            UIDocument document = GetComponent<UIDocument>();
            VisualElement root = document.rootVisualElement;

            _difficultyLabel = root.Q<Label>("difficultyLabel");
            _timeLabel = root.Q<Label>("timeLabel");
            _movesLabel = root.Q<Label>("movesLabel");

            _playAgainButton = root.Q<Button>("playAgainButton");
            _mainMenuButton = root.Q<Button>("mainMenuButton");

            _playAgainButton.clicked += PlayAgain;
            _mainMenuButton.clicked += ReturnToMainMenu;

            RenderResults();
        }

        private void OnDisable()
        {
            if (_playAgainButton != null)
            {
                _playAgainButton.clicked -= PlayAgain;
            }

            if (_mainMenuButton != null)
            {
                _mainMenuButton.clicked -= ReturnToMainMenu;
            }
        }

        private void RenderResults()
        {
            _difficultyLabel.text = $"Difficulty: {GameSettings.LastCompletedDifficulty}";
            _timeLabel.text = $"Time: {FormatTime(GameSettings.LastCompletedTimeSeconds)}";
            _movesLabel.text = $"Moves: {GameSettings.LastCompletedMoveCount}";
        }

        private static string FormatTime(int totalSeconds)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            return $"{minutes:00}:{seconds:00}";
        }

        private void PlayAgain()
        {
            SceneManager.LoadScene(SceneNames.GamePlay);
        }

        private void ReturnToMainMenu()
        {
            SceneManager.LoadScene(SceneNames.MainMenu);
        }
    }
}
