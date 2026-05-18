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
        private const int BoxSize = 3;
        private float _elapsedSeconds;
        private bool _timerIsRunning;

        private Label _difficultyLabel;
        private Label _timerLabel;
        private Label _movesLabel;

        private VisualElement _sudokuBoard;

        private Button _notesButton;
        private Button _clearButton;
        private Button _pauseButton;
        private Button _menuButton;

        private readonly List<Label> _cellLabels = new();
        private readonly List<Button> _numberButtons = new();
        private readonly Dictionary<Button, EventCallback<ClickEvent>> _numberButtonCallbacks = new();

        private int[,] _solutionBoard;
        private SudokuPuzzle _puzzle;
        private SudokuGameState _gameState;

        private int _selectedRow = -1;
        private int _selectedCol = -1;

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

            CacheNumberButtons(root);

            _clearButton.clicked += ClearSelectedCell;
            _menuButton.clicked += ReturnToMainMenu;

            SetInitialLabels();
            BuildEmptyBoard();
            GenerateAndRenderPuzzle();
        }

        private void OnDisable()
        {
            if (_clearButton != null)
            {
                _clearButton.clicked -= ClearSelectedCell;
            }

            if (_menuButton != null)
            {
                _menuButton.clicked -= ReturnToMainMenu;
            }

            foreach (KeyValuePair<Button, EventCallback<ClickEvent>> pair in _numberButtonCallbacks)
            {
                pair.Key.UnregisterCallback(pair.Value);
            }

            _numberButtonCallbacks.Clear();
        }

        private void Update()
        {
            if (!_timerIsRunning)
            {
                return;
            }

            _elapsedSeconds += Time.deltaTime;
            UpdateTimerLabel();
        }

        private void CacheNumberButtons(VisualElement root)
        {
            _numberButtons.Clear();
            _numberButtonCallbacks.Clear();

            for (int number = 1; number <= 9; number++)
            {
                int capturedNumber = number;
                Button button = root.Q<Button>($"numberButton{number}");

                EventCallback<ClickEvent> callback = _ => SetSelectedCellValue(capturedNumber);

                button.RegisterCallback(callback);
                _numberButtons.Add(button);
                _numberButtonCallbacks.Add(button, callback);
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
                    int capturedRow = row;
                    int capturedCol = col;

                    Label cell = new Label(string.Empty)
                    {
                        name = $"cell-{row}-{col}"
                    };

                    cell.AddToClassList("sudoku-cell");
                    cell.RegisterCallback<ClickEvent>(_ => SelectCell(capturedRow, capturedCol));

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

            _puzzle = SudokuPuzzleBuilder.CreatePuzzle(
                _solutionBoard,
                visibleClueCount
            );

            _gameState = new SudokuGameState(
                _solutionBoard,
                _puzzle.VisibleCells
            );

            _selectedRow = -1;
            _selectedCol = -1;

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Label cell = GetCellLabel(row, col);

                    cell.RemoveFromClassList("selected-cell");
                    cell.RemoveFromClassList("related-cell");
                    cell.RemoveFromClassList("matching-cell");
                    cell.RemoveFromClassList("given-cell");
                    cell.RemoveFromClassList("correct-cell");
                    cell.RemoveFromClassList("wrong-cell");

                    if (_puzzle.VisibleCells[row, col])
                    {
                        cell.text = _solutionBoard[row, col].ToString();
                        cell.AddToClassList("given-cell");
                    }
                    else
                    {
                        cell.text = string.Empty;
                    }
                }
            }

            UpdateMovesLabel();
            ResetTimer();
            UpdateCompletedNumberButtons();
        }

        private void SelectCell(int row, int col)
        {
            _selectedRow = row;
            _selectedCol = col;

            RefreshCellHighlights();
        }

        private void SetSelectedCellValue(int value)
        {
            if (!HasSelectedCell())
            {
                return;
            }

            bool changed = _gameState.TrySetPlayerValue(_selectedRow, _selectedCol, value);

            if (!changed)
            {
                return;
            }

            StartTimerIfNeeded();

            RenderCell(_selectedRow, _selectedCol);
            UpdateMovesLabel();
            RefreshCellHighlights();
            UpdateCompletedNumberButtons();
            CheckForCompletion();
        }

        private void CheckForCompletion()
        {
            if (!_gameState.IsCompleteAndCorrect())
            {
                return;
            }

            StopTimer();

            GameSettings.SaveCompletedGame(
                GameSettings.SelectedDifficulty,
                _gameState.MoveCount,
                GetElapsedTimeSeconds()
            );

            SceneManager.LoadScene(SceneNames.GameOver);
        }

        private void ClearSelectedCell()
        {
            if (!HasSelectedCell())
            {
                return;
            }

            bool changed = _gameState.TryClearPlayerValue(_selectedRow, _selectedCol);

            if (!changed)
            {
                return;
            }

            StartTimerIfNeeded();

            RenderCell(_selectedRow, _selectedCol);
            UpdateMovesLabel();
            RefreshCellHighlights();
            UpdateCompletedNumberButtons();
        }

        private void RenderCell(int row, int col)
        {
            Label cell = GetCellLabel(row, col);

            cell.RemoveFromClassList("correct-cell");
            cell.RemoveFromClassList("wrong-cell");

            if (_gameState.IsGivenCell(row, col))
            {
                cell.text = _solutionBoard[row, col].ToString();
                cell.AddToClassList("given-cell");
                return;
            }

            int playerValue = _gameState.GetPlayerValue(row, col);

            cell.text = playerValue == 0 ? string.Empty : playerValue.ToString();

            if (_gameState.IsCorrectValue(row, col))
            {
                cell.AddToClassList("correct-cell");
            }
            else if (_gameState.IsWrongValue(row, col))
            {
                cell.AddToClassList("wrong-cell");
            }
        }

        private void RefreshCellHighlights()
        {
            ClearHighlightStyles();

            if (!HasSelectedCell())
            {
                return;
            }

            int selectedValue = GetDisplayedCellValue(_selectedRow, _selectedCol);

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Label cell = GetCellLabel(row, col);

                    bool sameRow = row == _selectedRow;
                    bool sameCol = col == _selectedCol;
                    bool sameBox = IsSameBox(row, col, _selectedRow, _selectedCol);

                    if (sameRow || sameCol || sameBox)
                    {
                        cell.AddToClassList("related-cell");
                    }

                    int cellValue = GetDisplayedCellValue(row, col);

                    if (selectedValue != 0 && cellValue == selectedValue)
                    {
                        cell.AddToClassList("matching-cell");
                    }
                }
            }

            GetCellLabel(_selectedRow, _selectedCol).AddToClassList("selected-cell");
        }

        private void ClearHighlightStyles()
        {
            foreach (Label cell in _cellLabels)
            {
                cell.RemoveFromClassList("selected-cell");
                cell.RemoveFromClassList("related-cell");
                cell.RemoveFromClassList("matching-cell");
            }
        }

        private int GetDisplayedCellValue(int row, int col)
        {
            if (_gameState.IsGivenCell(row, col))
            {
                return _solutionBoard[row, col];
            }

            return _gameState.GetPlayerValue(row, col);
        }

        private static bool IsSameBox(int firstRow, int firstCol, int secondRow, int secondCol)
        {
            return firstRow / BoxSize == secondRow / BoxSize &&
                   firstCol / BoxSize == secondCol / BoxSize;
        }

        private bool HasSelectedCell()
        {
            return _selectedRow >= 0 && _selectedCol >= 0;
        }

        private Label GetCellLabel(int row, int col)
        {
            int cellIndex = row * GridSize + col;
            return _cellLabels[cellIndex];
        }

        private void StartTimerIfNeeded()
        {
            if (_timerIsRunning)
            {
                return;
            }

            _timerIsRunning = true;
        }

        private void ResetTimer()
        {
            _elapsedSeconds = 0f;
            _timerIsRunning = false;
            UpdateTimerLabel();
        }

        private void StopTimer()
        {
            _timerIsRunning = false;
        }       

        private void UpdateTimerLabel()
        {
            int totalSeconds = Mathf.FloorToInt(_elapsedSeconds);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            _timerLabel.text = $"Time: {minutes:00}:{seconds:00}";
        }

        private int GetElapsedTimeSeconds()
        {
            return Mathf.FloorToInt(_elapsedSeconds);
        }

        private void UpdateMovesLabel()
        {
            _movesLabel.text = $"Moves: {_gameState.MoveCount}";
        }

        private void ReturnToMainMenu()
        {
            SceneManager.LoadScene(SceneNames.MainMenu);
        }

        private void UpdateCompletedNumberButtons()
        {
            int[] numberCounts = new int[GridSize + 1];

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    int value = GetDisplayedCellValue(row, col);

                    if (value >= 1 && value <= GridSize)
                    {
                        numberCounts[value]++;
                    }
                }
            }

            for (int index = 0; index < _numberButtons.Count; index++)
            {
                int number = index + 1;
                Button button = _numberButtons[index];

                if (numberCounts[number] >= GridSize)
                {
                    button.AddToClassList("number-complete");
                }
                else
                {
                    button.RemoveFromClassList("number-complete");
                }
            }
        }
    }
}
