namespace UnitySudoku.Core
{
    public static class GameSettings
    {
        public static DifficultyLevel SelectedDifficulty { get; set; } = DifficultyLevel.Easy;

        public static DifficultyLevel LastCompletedDifficulty { get; private set; } = DifficultyLevel.Easy;
        public static int LastCompletedMoveCount { get; private set; }
        public static int LastCompletedTimeSeconds { get; private set; }

        public static int GetVisibleClueCount(DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                DifficultyLevel.Easy => 30,
                DifficultyLevel.Medium => 22,
                DifficultyLevel.Hard => 15,
                DifficultyLevel.Godlike => 5,
                _ => 30
            };
        }

        public static void SaveCompletedGame(
            DifficultyLevel difficulty,
            int moveCount,
            int timeSeconds
        )
        {
            LastCompletedDifficulty = difficulty;
            LastCompletedMoveCount = moveCount;
            LastCompletedTimeSeconds = timeSeconds;
        }
    }
}
