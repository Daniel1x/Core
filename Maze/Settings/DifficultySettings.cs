namespace DL.Core.Maze
{
    using System;
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
}
