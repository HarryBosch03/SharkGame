using System;
using UnityEngine;

public class SharkBinder : MonoBehaviour
{
    public SharkVisuals shark;
    [Range(0f, 1f)]
    public float percent;
    public float verticalOffset;
    [Range(-180f, 180f)]
    public float angleOffset;
    [Range(-180f, 180f)]
    public float splayAngle;

    private void Awake()
    {
        shark = GetComponentInParent<SharkVisuals>();
    }

    private void FixedUpdate()
    {
        shark.BindPart(transform, percent, verticalOffset, angleOffset, splayAngle);
    }

    public void OnValidate()
    {
        if (Application.isPlaying) return;

        shark = GetComponentInParent<SharkVisuals>();
        if (shark)
        {
            var localPosition = (Vector2)shark.transform.InverseTransformPoint(transform.position);
            percent = -localPosition.x / shark.length;
            verticalOffset = localPosition.y;
            splayAngle = Mathf.DeltaAngle(shark.transform.eulerAngles.z, transform.eulerAngles.z);
        }

        percent = Mathf.Clamp01(percent);
    }
}