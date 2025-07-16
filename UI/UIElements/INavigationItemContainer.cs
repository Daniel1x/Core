using UnityEngine.UI;

public interface INavigationItemContainer<T> where T : Selectable, INavigationItem<T>
{
    public T NavigationItem { get; }
}
