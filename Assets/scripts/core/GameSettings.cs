namespace UnitySudoku.Core
{
    public static class GameSettings
    {
        public static DifficultyLevel SelectedDifficulty { get; set; } = DifficultyLevel.Easy;

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
    }
}
