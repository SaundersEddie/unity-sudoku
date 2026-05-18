using System;
using NUnit.Framework;
using UnitySudoku.Core.Sudoku;

namespace UnitySudoku.Tests.EditMode
{
    public sealed class SudokuGameStateTests
    {
        [Test]
        public void Constructor_RejectsNullSolution()
        {
            bool[,] givenCells = new bool[9, 9];

            Assert.Throws<ArgumentNullException>(() =>
                new SudokuGameState(null, givenCells)
            );
        }

        [Test]
        public void Constructor_RejectsNullGivenCells()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);

            Assert.Throws<ArgumentNullException>(() =>
                new SudokuGameState(solution, null)
            );
        }

        [Test]
        public void IsGivenCell_ReturnsTrueForGivenCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];
            givenCells[0, 0] = true;

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            Assert.IsTrue(gameState.IsGivenCell(0, 0));
        }

        [Test]
        public void IsGivenCell_ReturnsFalseForEditableCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            Assert.IsFalse(gameState.IsGivenCell(0, 0));
        }

        [Test]
        public void TrySetPlayerValue_RejectsGivenCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];
            givenCells[0, 0] = true;

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            bool changed = gameState.TrySetPlayerValue(0, 0, 5);

            Assert.IsFalse(changed);
            Assert.AreEqual(0, gameState.GetPlayerValue(0, 0));
            Assert.AreEqual(0, gameState.MoveCount);
        }

        [Test]
        public void TrySetPlayerValue_AcceptsEditableCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            bool changed = gameState.TrySetPlayerValue(0, 0, 5);

            Assert.IsTrue(changed);
            Assert.AreEqual(5, gameState.GetPlayerValue(0, 0));
            Assert.AreEqual(1, gameState.MoveCount);
        }

        [Test]
        public void TrySetPlayerValue_DoesNotCountSameValueTwice()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            gameState.TrySetPlayerValue(0, 0, 5);
            bool changed = gameState.TrySetPlayerValue(0, 0, 5);

            Assert.IsFalse(changed);
            Assert.AreEqual(5, gameState.GetPlayerValue(0, 0));
            Assert.AreEqual(1, gameState.MoveCount);
        }

        [Test]
        public void TrySetPlayerValue_CountsChangedValue()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            gameState.TrySetPlayerValue(0, 0, 5);
            bool changed = gameState.TrySetPlayerValue(0, 0, 6);

            Assert.IsTrue(changed);
            Assert.AreEqual(6, gameState.GetPlayerValue(0, 0));
            Assert.AreEqual(2, gameState.MoveCount);
        }

        [Test]
        public void TryClearPlayerValue_ClearsEditableCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            gameState.TrySetPlayerValue(0, 0, 5);
            bool changed = gameState.TryClearPlayerValue(0, 0);

            Assert.IsTrue(changed);
            Assert.AreEqual(0, gameState.GetPlayerValue(0, 0));
            Assert.AreEqual(2, gameState.MoveCount);
        }

        [Test]
        public void TryClearPlayerValue_DoesNotCountEmptyCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            bool changed = gameState.TryClearPlayerValue(0, 0);

            Assert.IsFalse(changed);
            Assert.AreEqual(0, gameState.MoveCount);
        }

        [Test]
        public void TryClearPlayerValue_RejectsGivenCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];
            givenCells[0, 0] = true;

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            bool changed = gameState.TryClearPlayerValue(0, 0);

            Assert.IsFalse(changed);
            Assert.AreEqual(0, gameState.MoveCount);
        }

        [Test]
        public void IsCorrectValue_ReturnsTrueWhenPlayerValueMatchesSolution()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            int correctValue = solution[0, 0];
            gameState.TrySetPlayerValue(0, 0, correctValue);

            Assert.IsTrue(gameState.IsCorrectValue(0, 0));
            Assert.IsFalse(gameState.IsWrongValue(0, 0));
        }

        [Test]
        public void IsWrongValue_ReturnsTrueWhenPlayerValueDoesNotMatchSolution()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            int wrongValue = solution[0, 0] == 1 ? 2 : 1;
            gameState.TrySetPlayerValue(0, 0, wrongValue);

            Assert.IsTrue(gameState.IsWrongValue(0, 0));
            Assert.IsFalse(gameState.IsCorrectValue(0, 0));
        }
    }
}
