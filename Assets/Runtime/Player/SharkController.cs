using System;
using Runtime;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SharkController : MonoBehaviour
{
    public const int SharkLayer = 7;

    public float idleMoveSpeed = 6f;
    public float slowMoveSpeed = 14f;
    public float fastMoveSpeed = 25f;
    public float moveAcceleration = 8f;

    [Space]
    [Range(0f, 1f)]
    public float stamina = 1f;
    public float fastStaminaDrain = 2f;
    public float staminaRegenDelay = 1f;
    public float staminaRegenRate = 0.5f;

    [Space]
    public float turnSpring = 5f;
    public float turnDamping = 1f;

    [Space]
    public float foresightDistance = 2f;

    private float staminaRegenTimer;
    private bool exhausted;

    private float goalAngle;
    public Vector2 goalPosition { get; private set; }
    public Rigidbody2D body { get; private set; }
    public InputData input { get; set; }
    private InputData previousInput;

    public event Action ExhaustedEvent;
    public event Action UnExhaustedEvent;
    public Vector2 forward => body.rotation.FromAngle();

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        foreach (var child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = SharkLayer;
        }
    }

    private void OnEnable() { stamina = 1f; }

    private void FixedUpdate()
    {
        CalcGoal();
        Move();
        Turn();
        UpdateStamina();

        previousInput = input;
    }

    private void UpdateStamina()
    {
        if (staminaRegenTimer > staminaRegenDelay)
        {
            stamina += staminaRegenRate * Time.deltaTime;
        }

        if (stamina < 0f && !exhausted)
        {
            exhausted = true;
            ExhaustedEvent?.Invoke();
        }
        else if (stamina > 1f && exhausted)
        {
            exhausted = false;
            UnExhaustedEvent?.Invoke();
        }

        stamina = Mathf.Clamp01(stamina);
        staminaRegenTimer += Time.deltaTime;
    }

    private void CalcGoal()
    {
        if (input.fast)
        {
            var dir = goalPosition - body.position;
            goalPosition = dir.normalized * Mathf.Max(4f, dir.magnitude) + body.position;
        }
        else
        {
            if (input.moving)
            {
                goalPosition = (input.goalPosition - body.position).normalized * 4f + body.position;
            }

            ValidateGoalPosition();
        }

        var difference = goalPosition + body.rotation.FromAngle() - body.position;
        goalAngle = difference.normalized.ToAngle();
    }

    private void ValidateGoalPosition()
    {
        var hit = Physics2D.Linecast(body.position, goalPosition, 0b1);
        if (!hit) return;

        var tangent = new Vector2(-hit.normal.y, hit.normal.x);
        if (Vector2.Dot(tangent, body.rotation.FromAngle()) < 0f) tangent = -tangent;
        goalPosition = hit.point + (hit.normal * 0.5f + tangent) * foresightDistance;
    }

    private void Move()
    {
        var moveSpeed = idleMoveSpeed;
        if (input.fast && !exhausted)
        {
            moveSpeed = fastMoveSpeed;
            DrainStamina(fastStaminaDrain);
        }
        else if (input.moving) moveSpeed = slowMoveSpeed;

        var force = (forward * moveSpeed - body.velocity) * moveAcceleration;
        body.AddForce(force * body.mass);
    }

    private void Turn()
    {
        var torque = Mathf.DeltaAngle(body.rotation, goalAngle) * turnSpring - body.angularVelocity * turnDamping;
        body.AddTorque(torque * body.mass);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, goalPosition);
        Gizmos.DrawSphere(goalPosition, 0.5f);
    }

    public void DrainStamina(float rate)
    {
        this.stamina -= rate * Time.deltaTime;
        staminaRegenTimer = 0f;
    }

    public struct InputData
    {
        public Vector2 goalPosition;
        public bool moving;
        public bool fast;
    }
}