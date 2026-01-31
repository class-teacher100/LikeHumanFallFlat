using UnityEngine;

public enum BodyPartType
{
    Pelvis, Torso, Head,
    LeftUpperArm, LeftLowerArm, LeftHand,
    RightUpperArm, RightLowerArm, RightHand,
    LeftUpperLeg, LeftLowerLeg, LeftFoot,
    RightUpperLeg, RightLowerLeg, RightFoot
}

public class BodyPart : MonoBehaviour
{
    public BodyPartType partType;
    public Rigidbody Rb { get; private set; }
    public ConfigurableJoint Joint { get; private set; }
    public Collider Col { get; private set; }

    void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Joint = GetComponent<ConfigurableJoint>();
        Col = GetComponent<Collider>();
    }

    public void ApplyForce(Vector3 force, ForceMode mode = ForceMode.Force)
    {
        Rb.AddForce(force, mode);
    }

    public void ApplyTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
    {
        Rb.AddTorque(torque, mode);
    }

    public void SetJointTargetRotation(Quaternion targetLocalRotation)
    {
        if (Joint == null) return;
        Joint.targetRotation = targetLocalRotation;
    }

    public void SetJointDrive(float spring, float damper)
    {
        if (Joint == null) return;
        var drive = new JointDrive
        {
            positionSpring = spring,
            positionDamper = damper,
            maximumForce = float.MaxValue
        };
        Joint.angularXDrive = drive;
        Joint.angularYZDrive = drive;
        Joint.slerpDrive = drive;
    }
}
