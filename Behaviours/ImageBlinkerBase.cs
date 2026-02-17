using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image)), ExecuteInEditMode]
public abstract class ImageBlinkerBase : MonoBehaviour
{
    [SerializeField] protected Image image;

    protected virtual void Awake()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }

    private void Reset()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }

        updateBlinkState(image, Time.unscaledTime);
    }

    protected abstract void updateBlinkState(Image _image, float _time);
}