namespace DL.Core.Maze
{
    using System;
    using UnityEngine;

    public interface IInteractableNode : InteractionHandler<IInteractableNode>, IInteractable
    {
        event Action<IInteractableNode> OnInteractableDestroyed;
        Vector2Int Position { get; set; }
    }

    public interface InteractionHandler<T>
    {
        event Action<T> Clicked;
        event Action<T> PressedOver;
        event Action<T> PressedEnter;
        event Action<T> PressedExit;
    }

    public interface IInteractable
    {
        void NotifyClicked();
        void NotifyPressedEnter();
        void NotifyPressedOver();
        void NotifyPressedExit();
    }
}
