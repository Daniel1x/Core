using UnityEngine;

public abstract class ShaderGlobalPropertySetter<PropertyType> : MonoBehaviour
{
    [SerializeField] private string propertyName = "_Color";
    [SerializeField] private PropertyType value = default;

    public PropertyType Value
    {
        get => value;
        set
        {
            this.value = value;
            applyPropertyChanges();
        }
    }

    private void OnValidate() => applyPropertyChanges();
    private void OnEnable() => applyPropertyChanges();

    protected void applyPropertyChanges()
    {
        if (propertyName != null && propertyName.Length > 0)
        {
            setShaderGlobalProperty(propertyName, value);
        }
    }

    protected abstract void setShaderGlobalProperty(string _name, PropertyType _value);
}
