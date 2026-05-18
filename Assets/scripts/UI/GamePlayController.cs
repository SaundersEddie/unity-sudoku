using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnitySudoku.Core;
using UnitySudoku.SceneFlow;

namespace UnitySudoku.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class GamePlayController : MonoBehaviour
    {
        private Label _difficultyLabel;
        private Label _timerLabel;
        private Label _movesLabel;

        private VisualElement _sudokuBoard;

        private Button _notesButton;
        private Button _clearButton;
        private Button _pauseButton;
        private Button _menuButton;

        private void OnEnable()
        {
            UIDocument document = GetComponent<UIDocument>();
            VisualElement root = document.rootVisualElement;

            _difficultyLabel = root.Q<Label>("difficultyLabel");
            _timerLabel = root.Q<Label>("timerLabel");
            _movesLabel = root.Q<Label>("movesLabel");

            _sudokuBoard = root.Q<VisualElement>("sudokuBoard");

            _notesButton = root.Q<Button>("notesButton");
            _clearButton = root.Q<Button>("clearButton");
            _pauseButton = root.Q<Button>("pauseButton");
            _menuButton = root.Q<Button>("menuButton");

            _menuButton.clicked += ReturnToMainMenu;

            SetInitialLabels();
            BuildEmptyBoard();
        }

        private void OnDisable()
        {
            if (_menuButton != null)
            {
                _menuButton.clicked -= ReturnToMainMenu;
            }
        }

        private void SetInitialLabels()
        {
            DifficultyLevel difficulty = GameSettings.SelectedDifficulty;
            int clueCount = GameSettings.GetVisibleClueCount(difficulty);

            _difficultyLabel.text = $"Difficulty: {difficulty} ({clueCount} clues)";
            _timerLabel.text = "Time: 00:00";
            _movesLabel.text = "Moves: 0";
        }

        private void BuildEmptyBoard()
        {
            _sudokuBoard.Clear();

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Label cell = new Label(string.Empty)
                    {
                        name = $"cell-{row}-{col}"
                    };

                    cell.AddToClassList("sudoku-cell");

                    if (col == 2 || col == 5)
                    {
                        cell.AddToClassList("box-right");
                    }

                    if (row == 2 || row == 5)
                    {
                        cell.AddToClassList("box-bottom");
                    }

                    _sudokuBoard.Add(cell);
                }
            }
        }

        private void ReturnToMainMenu()
        {
            SceneManager.LoadScene(SceneNames.MainMenu);
        }
    }
}
