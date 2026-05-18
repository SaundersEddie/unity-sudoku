using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnitySudoku.Core;
using UnitySudoku.Core.Sudoku;
using UnitySudoku.SceneFlow;

namespace UnitySudoku.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class GamePlayController : MonoBehaviour
    {
        private const int GridSize = 9;

        private Label _difficultyLabel;
        private Label _timerLabel;
        private Label _movesLabel;

        private VisualElement _sudokuBoard;

        private Button _notesButton;
        private Button _clearButton;
        private Button _pauseButton;
        private Button _menuButton;

        private readonly List<Label> _cellLabels = new();

        private int[,] _solutionBoard;

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
            GenerateAndRenderPuzzle();
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
            _cellLabels.Clear();

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
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
                    _cellLabels.Add(cell);
                }
            }
        }

        private void GenerateAndRenderPuzzle()
        {
            DifficultyLevel difficulty = GameSettings.SelectedDifficulty;
            int visibleClueCount = GameSettings.GetVisibleClueCount(difficulty);

            _solutionBoard = SudokuGenerator.GenerateSolution();

            SudokuPuzzle puzzle = SudokuPuzzleBuilder.CreatePuzzle(
                _solutionBoard,
                visibleClueCount
            );

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    int cellIndex = row * GridSize + col;
                    Label cell = _cellLabels[cellIndex];

                    if (puzzle.VisibleCells[row, col])
                    {
                        cell.text = _solutionBoard[row, col].ToString();
                        cell.AddToClassList("given-cell");
                    }
                    else
                    {
                        cell.text = string.Empty;
                        cell.RemoveFromClassList("given-cell");
                    }
                }
            }
        }

        private void ReturnToMainMenu()
        {
            SceneManager.LoadScene(SceneNames.MainMenu);
        }
    }
}
