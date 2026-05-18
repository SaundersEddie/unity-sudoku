using System.Collections.Generic;

namespace UnitySudoku.Core.Sudoku
{
    public static class SudokuValidator
    {
        private const int GridSize = 9;
        private const int BoxSize = 3;

        public static bool IsValidSolution(int[,] board)
        {
            if (board == null)
            {
                return false;
            }

            if (board.GetLength(0) != GridSize || board.GetLength(1) != GridSize)
            {
                return false;
            }

            for (int row = 0; row < GridSize; row++)
            {
                if (!IsValidGroup(GetRow(board, row)))
                {
                    return false;
                }
            }

            for (int col = 0; col < GridSize; col++)
            {
                if (!IsValidGroup(GetColumn(board, col)))
                {
                    return false;
                }
            }

            for (int startRow = 0; startRow < GridSize; startRow += BoxSize)
            {
                for (int startCol = 0; startCol < GridSize; startCol += BoxSize)
                {
                    if (!IsValidGroup(GetBox(board, startRow, startCol)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsValidGroup(IEnumerable<int> values)
        {
            HashSet<int> seenValues = new HashSet<int>();

            foreach (int value in values)
            {
                if (value < 1 || value > GridSize)
                {
                    return false;
                }

                if (!seenValues.Add(value))
                {
                    return false;
                }
            }

            return seenValues.Count == GridSize;
        }

        private static IEnumerable<int> GetRow(int[,] board, int row)
        {
            for (int col = 0; col < GridSize; col++)
            {
                yield return board[row, col];
            }
        }

        private static IEnumerable<int> GetColumn(int[,] board, int col)
        {
            for (int row = 0; row < GridSize; row++)
            {
                yield return board[row, col];
            }
        }

        private static IEnumerable<int> GetBox(int[,] board, int startRow, int startCol)
        {
            for (int row = startRow; row < startRow + BoxSize; row++)
            {
                for (int col = startCol; col < startCol + BoxSize; col++)
                {
                    yield return board[row, col];
                }
            }
        }
    }
}
