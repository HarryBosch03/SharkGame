using System;
using System.Collections.Generic;
using Runtime;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SharkController))]
public class SharkVisuals : MonoBehaviour
{
    public float length;
    [Range(-1f, 1f)]
    public float offset;
    public int segmentCount;
    public GameObject segmentInstance;
    public float maxRadius;

    [Space]
    public float modulationFrequency;
    public float modulationAmplitude;
    
    [Space]
    public LineRenderer goalLine;

    [Space]
    public Canvas worldOverlay;
    public float worldOverlaySmoothing = 0.06f;
    public CanvasGroup staminaGroup;
    public Image staminaBackground;
    public Image staminaDelta;
    public Image staminaFill;
    public float staminaGroupFadeSpeed;
    public float staminaDeltaSpeed;
    
    [Space]
    public List<SpriteRenderer> sprites;

    private SharkController shark;
    private float distance;
    private float staminaLastFrame;

    private Transform[] segments;
    private Vector2[] points;

    private void Awake()
    {
        shark = GetComponent<SharkController>();
        
        segments = new Transform[segmentCount];
        points = new Vector2[segmentCount];
        
        segments[0] = segmentInstance.transform;
        for (var i = 1; i < segmentCount; i++)
        {
            var segment = Instantiate(segmentInstance, segmentInstance.transform.parent).transform;
            segments[i] = segment;
            sprites.AddRange(segment.GetComponentsInChildren<SpriteRenderer>());
        }
        
        for (var i = 0; i < segmentCount; i++)
        {
            var p = i / (segmentCount - 1f);
            var segment = segments[i];
            
            segment.name = $"Segment.{i + 1}";
            segment.localScale = Vector2.one * Profile(p) * 2f;
            points[i] = transform.position - transform.right * (p + offset) * length;
        }

        worldOverlay.transform.SetParent(null);
    }

    private void OnEnable()
    {
        shark.ExhaustedEvent += OnSharkExhausted;
        shark.UnExhaustedEvent += OnSharkUnExhausted;
    }

    private void OnDisable()
    {
        shark.ExhaustedEvent -= OnSharkExhausted;
        shark.UnExhaustedEvent -= OnSharkUnExhausted;
    }

    private void OnSharkExhausted()
    {
        staminaBackground.color = Utility.HueShift(staminaBackground.color, 0f);
        staminaFill.color = Utility.HueShift(staminaFill.color, 0f);
    }

    private void OnSharkUnExhausted()
    {
        staminaBackground.color = Utility.HueShift(staminaBackground.color, 120f / 360f);
        staminaFill.color = Utility.HueShift(staminaFill.color, 120f / 360f);
    }

    public void SetColor(Color color)
    {
        Color.RGBToHSV(color, out var h, out _, out _);
        foreach (var sprite in sprites)
        {
            Color.RGBToHSV(sprite.color, out var _, out var s, out var v);
            sprite.color = Color.HSVToRGB(h, s, v);
        }
    }

    private void Update()
    {
        var target = (Vector2)transform.position + new Vector2(0f, -2.5f);
        var current = (Vector2)worldOverlay.transform.position;
        worldOverlay.transform.position = Vector2.Lerp(current, target, Time.deltaTime / Mathf.Max(Time.deltaTime, worldOverlaySmoothing));
    }

    private void FixedUpdate()
    {
        var segmentLength = length / segments.Length;

        points[0] = transform.position + transform.right * length * offset + transform.up * Mathf.Sin(distance * modulationFrequency) * modulationAmplitude;
        for (var i = 1; i < segments.Length; i++)
        {
            ref var a = ref points[i - 1];
            ref var b = ref points[i];
            
            b = (b - a).normalized * segmentLength + a;
        }

        for (var i = 0; i < segments.Length; i++)
        {
            segments[i].position = points[i];
        }

        if (goalLine)
        {
            goalLine.useWorldSpace = true;
            goalLine.positionCount = 2;
            goalLine.SetPosition(0, shark.body.position);
            goalLine.SetPosition(1, shark.goalPosition);
        }
        
        distance += shark.body.velocity.magnitude * Time.deltaTime;

        UpdateStaminaBar();
    }

    private void UpdateStaminaBar()
    {
        var staminaDelta = Mathf.Abs(shark.stamina - staminaLastFrame) / Time.deltaTime;
        
        staminaFill.fillAmount = shark.stamina;
        this.staminaDelta.fillAmount = staminaFill.fillAmount + staminaDelta * staminaDeltaSpeed;
        staminaGroup.alpha += ((shark.stamina < 1f ? 1f : 0f) - staminaGroup.alpha) * staminaGroupFadeSpeed * Time.deltaTime;

        staminaLastFrame = shark.stamina;
    }

    public void BindPart(Transform target, float percent, float verticalOffset, float angleOffset, float splayAngle)
    {
        var di = percent * points.Length;
        var i1 = Mathf.FloorToInt(di);
        var i2 = i1 + 1;
        if (i2 >= points.Length)
        {
            i2 = points.Length - 1;
            i1 = points.Length - 2;
        }

        var a = points[i1];
        var b = points[i2];
            
        target.position = Vector2.Lerp(a, b, Mathf.InverseLerp(i1, i2, di));
        target.rotation = Quaternion.Euler(0f, 0f, (a - b).ToAngle());

        var right = (points[0] - points[1]).normalized;
        
        var angle = (right.ToAngle() + angleOffset) * Mathf.Deg2Rad;
        target.position += target.rotation * Vector3.up * Mathf.Sin(angle) * verticalOffset;
        target.position += Vector3.forward * Mathf.Cos(angle) * verticalOffset;

        target.rotation *= Quaternion.Euler(0f, 0f, splayAngle * Mathf.Sin(angle));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (var i = 0; i < segmentCount; i++)
        {
            var p = i / (segmentCount - 1f);
            Gizmos.DrawWireSphere(transform.position - transform.right * (p - offset) * length, Profile(p));
        }
    }

    private void OnValidate()
    {
        segmentCount = Mathf.Max(1, segmentCount);
    }

    private float Profile(float p)
    {
        var width = Mathf.Pow(p, 0.8f);
        return (1f - Mathf.Pow(2f * width - 1f, 2f)) * maxRadius;
    }

    [Serializable]
    public struct Attachment
    {
        public Transform target;
        [Range(0f, 1f)]
        public float percent;
    }
}
