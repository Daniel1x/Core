using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 10f, 0f);

    public Vector3 RotationSpeed
    {
        get => rotationSpeed;
        set => rotationSpeed = value;
    }

    private void Start()
    {
        if (rotationSpeed != Vector3.zero)
        {
            transform.Rotate(UnityEngine.Random.value * 360f * rotationSpeed);
        }
    }

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
