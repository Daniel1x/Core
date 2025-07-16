using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OptionData = TMPro.TMP_Dropdown.OptionData;

[RequireComponent(typeof(CustomDropdown))]
public class DropdownOptionsHandler : MonoBehaviour, INavigationItemContainer<CustomDropdown>, IDropdownContainer
{
    public event UnityAction<int> OnDropdownValueChanged = null;

    [SerializeField] protected List<string> options = new();

    protected List<OptionData> optionDatas = new();
    protected CustomDropdown dropdown = null;

    public CustomDropdown Dropdown => dropdown;
    public CustomDropdown NavigationItem => dropdown;

    protected void Awake()
    {
        if (dropdown == null)
        {
            dropdown = GetComponent<CustomDropdown>();
        }

        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(dropdownValueChanged);
        }

        resetOptionDatasOnDropdown();
    }

    protected void OnDestroy()
    {
        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveListener(dropdownValueChanged);
        }
    }

    public void SetDropdownValueWithoutNotify(int _value)
    {
        if (dropdown == null)
        {
            return;
        }

        if (optionDatas.IsIndexOutOfRange(_value) || dropdown.value == _value)
        {
            return;
        }

        dropdown.SetValueWithoutNotify(_value);
    }

    public void SetNewOptions(List<string> _options, int _currentSelected)
    {
        if (_options.IsNullOrEmpty() || _options.IsIndexOutOfRange(_currentSelected))
        {
            Debug.LogError($"Dropdown Options Handler: Set New Options: Invalid options or current selected index. Options: {_options}, Current Selected: {_currentSelected}");
            return;
        }

        options = _options;
        resetOptionDatasOnDropdown();
        SetDropdownValueWithoutNotify(_currentSelected);
    }

    protected void resetOptionDatasOnDropdown()
    {
        if (dropdown == null)
        {
            return;
        }

        dropdown.ClearOptions();
        optionDatas.Clear();

        for (int i = 0; i < options.Count; i++)
        {
            optionDatas.Add(new OptionData(options[i]));
        }

        dropdown.options = optionDatas;
    }

    protected void dropdownValueChanged(int _index)
    {
        OnDropdownValueChanged?.Invoke(_index);
    }
}
