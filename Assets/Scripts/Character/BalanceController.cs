using UnityEngine;

[RequireComponent(typeof(RagdollCharacter))]
public class BalanceController : MonoBehaviour
{
    RagdollCharacter _character;

    float _balanceTorque;
    float _balanceDamping;

    void Awake()
    {
        _character = GetComponent<RagdollCharacter>();
    }

    void Start()
    {
        var def = _character.definition;
        _balanceTorque = def.balanceTorque;
        _balanceDamping = def.balanceDamping;
    }

    void FixedUpdate()
    {
        ApplyBalanceTorque(_character.pelvis);
        ApplyBalanceTorque(_character.torso);
    }

    void ApplyBalanceTorque(BodyPart part)
    {
        if (part == null) return;

        var rb = part.Rb;
        Vector3 up = Vector3.up;
        Vector3 currentUp = rb.transform.up;

        // Calculate the rotation needed to align currentUp with world up
        Vector3 torqueAxis = Vector3.Cross(currentUp, up);
        float angle = Vector3.Angle(currentUp, up);

        // Proportional torque
        Vector3 correctionTorque = torqueAxis.normalized * (angle * Mathf.Deg2Rad * _balanceTorque);

        // Damping (resist angular velocity)
        Vector3 dampingTorque = -rb.angularVelocity * _balanceDamping;

        rb.AddTorque(correctionTorque + dampingTorque);
    }
}
