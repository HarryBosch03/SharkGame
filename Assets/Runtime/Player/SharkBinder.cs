using UnityEngine;

public class SharkBinder : MonoBehaviour
{
    public SharkVisuals shark;
    public Vector3 position;
    public Vector3 rotation;

    private void Awake()
    {
        shark = GetComponentInParent<SharkVisuals>();
        GetValues();
    }

    protected virtual void FixedUpdate()
    {
        var sample = shark.Sample(-position.x / shark.length);
        transform.position = sample.position + sample.rotation * new Vector3(0f, position.y, position.z);
        transform.rotation = sample.rotation * Quaternion.Euler(rotation);
    }

    public void GetValues()
    {
        shark = GetComponentInParent<SharkVisuals>();
        if (shark)
        {
            var localPosition = shark.transform.InverseTransformPoint(transform.position);
            position = localPosition;
            rotation = (transform.rotation * Quaternion.Inverse(shark.transform.rotation)).eulerAngles;
        }
    }

    public void OnValidate()
    {
        if (Application.isPlaying) return;
        GetValues();
    }
}