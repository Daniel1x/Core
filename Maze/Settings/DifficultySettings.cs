namespace DL.Core.Maze
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public interface IDifficulty : IDifficultyProvider, IDifficultySetter
    {

    }

    public interface IDifficultyProvider
    {
        public Difficulty GetDifficulty();
    }

    public interface IDifficultySetter
    {
        public event System.Action<Difficulty> OnDifficultyChanged;
        public void SetDifficulty(Difficulty _difficulty);
    }

    public sealed class DifficultySettings : IDifficulty, ISelectionGroup<Difficulty>
    {
        public event Action<Difficulty> OnDifficultyChanged;
        public event Action OnSelectionChanged;

        [SerializeField] private Difficulty difficulty = Difficulty.Easy;

        public DifficultySettings(Difficulty _difficulty)
        {
            difficulty = _difficulty;
        }

        public void SetDifficulty(Difficulty _difficulty)
        {
            if (difficulty != _difficulty)
            {
                difficulty = _difficulty;
                OnDifficultyChanged?.Invoke(difficulty);
                OnSelectionChanged?.Invoke();
            }
        }

        public Difficulty GetDifficulty() => difficulty;
        public Difficulty GetSelected() => difficulty;
    }

    public static class DifficultyUtils
    {
        public static readonly IReadOnlyList<Difficulty> Difficulties = (Difficulty[])Enum.GetValues(typeof(Difficulty));
        public static readonly int DifficultyCount = Difficulties.Count;

        public static string CreateDifficultyKey(this string _baseKey, Difficulty _difficulty) => createDifficultyKey(_baseKey, _difficulty);
        public static string CreateDifficultyKey(this Difficulty _difficulty, string _baseKey) => createDifficultyKey(_baseKey, _difficulty);

        private static string createDifficultyKey(string _baseKey, Difficulty _difficulty)
        {
            if (string.IsNullOrEmpty(_baseKey))
            {
                return _difficulty.ToString();
            }

            return $"{_baseKey}_{_difficulty}";
        }
    }
}
