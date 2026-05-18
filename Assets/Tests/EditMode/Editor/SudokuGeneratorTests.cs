using NUnit.Framework;
using UnitySudoku.Core.Sudoku;

namespace UnitySudoku.Tests.EditMode
{
    public sealed class SudokuGeneratorTests
    {
        [Test]
        public void GenerateSolution_ReturnsNineByNineBoard()
        {
            int[,] board = SudokuGenerator.GenerateSolution(seed: 12345);

            Assert.AreEqual(9, board.GetLength(0));
            Assert.AreEqual(9, board.GetLength(1));
        }

        [Test]
        public void GenerateSolution_ReturnsValidSudokuSolution()
        {
            int[,] board = SudokuGenerator.GenerateSolution(seed: 12345);

            Assert.IsTrue(SudokuValidator.IsValidSolution(board));
        }

        [Test]
        public void GenerateSolution_WithSameSeed_ReturnsSameBoard()
        {
            int[,] firstBoard = SudokuGenerator.GenerateSolution(seed: 12345);
            int[,] secondBoard = SudokuGenerator.GenerateSolution(seed: 12345);

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Assert.AreEqual(firstBoard[row, col], secondBoard[row, col]);
                }
            }
        }

        [Test]
        public void GenerateSolution_WithDifferentSeeds_ReturnsValidBoards()
        {
            for (int seed = 0; seed < 100; seed++)
            {
                int[,] board = SudokuGenerator.GenerateSolution(seed);

                Assert.IsTrue(
                    SudokuValidator.IsValidSolution(board),
                    $"Generated board failed validation for seed {seed}."
                );
            }
        }

        [Test]
        public void Validator_RejectsRowDuplicate()
        {
            int[,] board = SudokuGenerator.GenerateSolution(seed: 12345);

            board[0, 1] = board[0, 0];

            Assert.IsFalse(SudokuValidator.IsValidSolution(board));
        }

        [Test]
        public void Validator_RejectsColumnDuplicate()
        {
            int[,] board = SudokuGenerator.GenerateSolution(seed: 12345);

            board[1, 0] = board[0, 0];

            Assert.IsFalse(SudokuValidator.IsValidSolution(board));
        }

        [Test]
        public void Validator_RejectsBoxDuplicate()
        {
            int[,] board = SudokuGenerator.GenerateSolution(seed: 12345);

            board[1, 1] = board[0, 0];

            Assert.IsFalse(SudokuValidator.IsValidSolution(board));
        }

        [Test]
        public void Validator_RejectsZeroValue()
        {
            int[,] board = SudokuGenerator.GenerateSolution(seed: 12345);

            board[0, 0] = 0;

            Assert.IsFalse(SudokuValidator.IsValidSolution(board));
        }

        [Test]
        public void Validator_RejectsTenValue()
        {
            int[,] board = SudokuGenerator.GenerateSolution(seed: 12345);

            board[0, 0] = 10;

            Assert.IsFalse(SudokuValidator.IsValidSolution(board));
        }

        [Test]
        public void Validator_RejectsNullBoard()
        {
            Assert.IsFalse(SudokuValidator.IsValidSolution(null));
        }
    }
}