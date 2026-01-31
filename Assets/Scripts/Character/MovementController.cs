using UnityEngine;

[RequireComponent(typeof(RagdollCharacter))]
[RequireComponent(typeof(GroundDetector))]
public class MovementController : MonoBehaviour
{
    [SerializeField] PlayerInputHandler _input;
    [SerializeField] float _turnTorque = 5000f;

    RagdollCharacter _character;
    GroundDetector _groundDetector;

    float _moveForce;
    float _maxSpeed;

    void Awake()
    {
        _character = GetComponent<RagdollCharacter>();
        _groundDetector = GetComponent<GroundDetector>();
    }

    void Start()
    {
        var def = _character.definition;
        _moveForce = def.moveForce;
        _maxSpeed = def.maxSpeed;
    }

    void FixedUpdate()
    {
        if (!_groundDetector.IsGrounded) return;

        var input = _input.MoveInput;
        if (input.sqrMagnitude < 0.01f) return;

        var pelvisRb = _character.pelvis.Rb;

        // Rotation: input.x applies Y-axis torque
        pelvisRb.AddTorque(Vector3.up * input.x * _turnTorque, ForceMode.Force);

        // Movement: character-forward direction × input.y
        Vector3 forward = pelvisRb.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 moveDir = forward * input.y;

        Vector3 horizontalVel = pelvisRb.linearVelocity;
        horizontalVel.y = 0f;

        if (horizontalVel.magnitude < _maxSpeed)
        {
            pelvisRb.AddForce(moveDir * _moveForce, ForceMode.Force);
        }
    }
}
