using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CooldownController : MonoBehaviour
{
    public event UnityAction<CooldownController> OnCooldownStarted = null;
    public event UnityAction<CooldownController> OnCooldownFinished = null;

    [SerializeField] private float cooldownDuration = 1f; // Duration of the cooldown in seconds

    private Image imageToFill = null;
    private Coroutine cooldownCoroutine = null;
    private float currentCooldownTime = 0f;

    public float CooldownDuration
    {
        get => cooldownDuration;
        set
        {
            cooldownDuration = Mathf.Max(0, value); // Ensure duration is non-negative
        }
    }

    public bool CanUseAction => gameObject.activeSelf == false || cooldownDuration <= 0f;

    private void Awake()
    {
        imageToFill = GetComponent<Image>();

        if (imageToFill == null)
        {
            Debug.LogError("CooldownController requires an Image component to function properly!");
            enabled = false; // Disable this script if no Image component is found
        }
    }

    private void OnEnable()
    {
        if (cooldownDuration > 0f)
        {
            cooldownCoroutine = StartCoroutine(animateCooldown()); // Start the cooldown animation
        }
    }

    private void OnDisable()
    {
        stopCooldownAnimation(); // Stop the cooldown animation when the object is disabled
    }

    private void stopCooldownAnimation()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
    }

    private IEnumerator animateCooldown()
    {
        OnCooldownStarted?.Invoke(this); // Invoke the cooldown started event

        currentCooldownTime = 0f;
        imageToFill.fillAmount = 1f; // Start with the image filled

        while (currentCooldownTime < cooldownDuration)
        {
            float _fillAmount = Mathf.Clamp01(currentCooldownTime / cooldownDuration);

            if (imageToFill != null)
            {
                imageToFill.fillAmount = 1f - _fillAmount; // Update the fill amount of the image
            }

            yield return null; // Wait for the next frame

            currentCooldownTime += Time.deltaTime;
        }

        imageToFill.fillAmount = 0f; // Reset fill amount to 0 after cooldown
        cooldownCoroutine = null; // Reset the coroutine reference

        // Disable the GameObject after cooldown ends
        gameObject.SetActive(false);

        OnCooldownFinished?.Invoke(this); // Invoke the cooldown finished event
    }
}
