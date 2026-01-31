using System.Collections.Generic;
using UnityEngine;

public class RagdollCharacter : MonoBehaviour
{
    public CharacterDefinition definition;

    [Header("Body Parts")]
    public BodyPart pelvis;
    public BodyPart torso;
    public BodyPart head;
    public BodyPart leftUpperArm;
    public BodyPart leftLowerArm;
    public BodyPart leftHand;
    public BodyPart rightUpperArm;
    public BodyPart rightLowerArm;
    public BodyPart rightHand;
    public BodyPart leftUpperLeg;
    public BodyPart leftLowerLeg;
    public BodyPart leftFoot;
    public BodyPart rightUpperLeg;
    public BodyPart rightLowerLeg;
    public BodyPart rightFoot;

    Dictionary<BodyPartType, BodyPart> _parts;

    public Vector3 CenterOfMass => pelvis.Rb.worldCenterOfMass;

    void Awake()
    {
        _parts = new Dictionary<BodyPartType, BodyPart>();
        RegisterPart(pelvis);
        RegisterPart(torso);
        RegisterPart(head);
        RegisterPart(leftUpperArm);
        RegisterPart(leftLowerArm);
        RegisterPart(leftHand);
        RegisterPart(rightUpperArm);
        RegisterPart(rightLowerArm);
        RegisterPart(rightHand);
        RegisterPart(leftUpperLeg);
        RegisterPart(leftLowerLeg);
        RegisterPart(leftFoot);
        RegisterPart(rightUpperLeg);
        RegisterPart(rightLowerLeg);
        RegisterPart(rightFoot);
    }

    void RegisterPart(BodyPart part)
    {
        if (part != null)
            _parts[part.partType] = part;
    }

    public BodyPart GetPart(BodyPartType type)
    {
        _parts.TryGetValue(type, out var part);
        return part;
    }

    public IEnumerable<BodyPart> AllParts => _parts.Values;

    [ContextMenu("Build Ragdoll From Definition")]
    public void BuildRagdoll()
    {
        if (definition == null)
        {
            Debug.LogError("CharacterDefinition is not assigned!");
            return;
        }

        // Clear existing children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(transform.GetChild(i).gameObject);
            else
                DestroyImmediate(transform.GetChild(i).gameObject);
        }

        int characterLayer = LayerMask.NameToLayer("Character");

        // Create body parts hierarchy: Pelvis -> Torso -> Head, Arms; Pelvis -> Legs
        pelvis = CreateCapsulePart("Pelvis", BodyPartType.Pelvis, null,
            Vector3.zero, definition.pelvisSize, definition.pelvisMass, characterLayer);

        torso = CreateCapsulePart("Torso", BodyPartType.Torso, pelvis.transform,
            Vector3.up * (definition.pelvisSize.x / 2 + definition.torsoSize.x / 2),
            definition.torsoSize, definition.torsoMass, characterLayer);
        SetupJoint(torso, pelvis.Rb, definition.spineSpring, definition.spineDamper);

        float torsoTop = definition.pelvisSize.x / 2 + definition.torsoSize.x;

        head = CreateSpherePart("Head", BodyPartType.Head, torso.transform,
            Vector3.up * (torsoTop + definition.headRadius),
            definition.headRadius, definition.headMass, characterLayer);
        SetupJoint(head, torso.Rb, definition.neckSpring, definition.neckDamper);

        // Arms
        float shoulderHeight = torsoTop - 0.05f;
        float shoulderWidth = definition.torsoSize.y + definition.upperArmSize.y;

        leftUpperArm = CreateCapsulePart("LeftUpperArm", BodyPartType.LeftUpperArm, torso.transform,
            new Vector3(-shoulderWidth, shoulderHeight, 0),
            definition.upperArmSize, definition.upperArmMass, characterLayer, CapsuleDirection.X);
        SetupJoint(leftUpperArm, torso.Rb, definition.shoulderSpring, definition.shoulderDamper);

        leftLowerArm = CreateCapsulePart("LeftLowerArm", BodyPartType.LeftLowerArm, leftUpperArm.transform,
            new Vector3(-shoulderWidth - definition.upperArmSize.x, shoulderHeight, 0),
            definition.lowerArmSize, definition.lowerArmMass, characterLayer, CapsuleDirection.X);
        SetupJoint(leftLowerArm, leftUpperArm.Rb, definition.elbowSpring, definition.elbowDamper);

        leftHand = CreateSpherePart("LeftHand", BodyPartType.LeftHand, leftLowerArm.transform,
            new Vector3(-shoulderWidth - definition.upperArmSize.x - definition.lowerArmSize.x, shoulderHeight, 0),
            definition.handRadius, definition.handMass, characterLayer);
        SetupJoint(leftHand, leftLowerArm.Rb, definition.wristSpring, definition.wristDamper);

        rightUpperArm = CreateCapsulePart("RightUpperArm", BodyPartType.RightUpperArm, torso.transform,
            new Vector3(shoulderWidth, shoulderHeight, 0),
            definition.upperArmSize, definition.upperArmMass, characterLayer, CapsuleDirection.X);
        SetupJoint(rightUpperArm, torso.Rb, definition.shoulderSpring, definition.shoulderDamper);

        rightLowerArm = CreateCapsulePart("RightLowerArm", BodyPartType.RightLowerArm, rightUpperArm.transform,
            new Vector3(shoulderWidth + definition.upperArmSize.x, shoulderHeight, 0),
            definition.lowerArmSize, definition.lowerArmMass, characterLayer, CapsuleDirection.X);
        SetupJoint(rightLowerArm, rightUpperArm.Rb, definition.elbowSpring, definition.elbowDamper);

        rightHand = CreateSpherePart("RightHand", BodyPartType.RightHand, rightLowerArm.transform,
            new Vector3(shoulderWidth + definition.upperArmSize.x + definition.lowerArmSize.x, shoulderHeight, 0),
            definition.handRadius, definition.handMass, characterLayer);
        SetupJoint(rightHand, rightLowerArm.Rb, definition.wristSpring, definition.wristDamper);

        // Legs
        float hipOffset = definition.pelvisSize.y * 0.5f;
        float pelvisBottom = -definition.pelvisSize.x / 2;

        leftUpperLeg = CreateCapsulePart("LeftUpperLeg", BodyPartType.LeftUpperLeg, pelvis.transform,
            new Vector3(-hipOffset, pelvisBottom - definition.upperLegSize.x / 2, 0),
            definition.upperLegSize, definition.upperLegMass, characterLayer);
        SetupJoint(leftUpperLeg, pelvis.Rb, definition.hipSpring, definition.hipDamper);

        leftLowerLeg = CreateCapsulePart("LeftLowerLeg", BodyPartType.LeftLowerLeg, leftUpperLeg.transform,
            new Vector3(-hipOffset, pelvisBottom - definition.upperLegSize.x - definition.lowerLegSize.x / 2, 0),
            definition.lowerLegSize, definition.lowerLegMass, characterLayer);
        SetupJoint(leftLowerLeg, leftUpperLeg.Rb, definition.kneeSpring, definition.kneeDamper);

        leftFoot = CreateCapsulePart("LeftFoot", BodyPartType.LeftFoot, leftLowerLeg.transform,
            new Vector3(-hipOffset, pelvisBottom - definition.upperLegSize.x - definition.lowerLegSize.x - definition.footSize.x / 2, 0.05f),
            definition.footSize, definition.footMass, characterLayer, CapsuleDirection.Z);
        SetupJoint(leftFoot, leftLowerLeg.Rb, definition.ankleSpring, definition.ankleDamper);

        rightUpperLeg = CreateCapsulePart("RightUpperLeg", BodyPartType.RightUpperLeg, pelvis.transform,
            new Vector3(hipOffset, pelvisBottom - definition.upperLegSize.x / 2, 0),
            definition.upperLegSize, definition.upperLegMass, characterLayer);
        SetupJoint(rightUpperLeg, pelvis.Rb, definition.hipSpring, definition.hipDamper);

        rightLowerLeg = CreateCapsulePart("RightLowerLeg", BodyPartType.RightLowerLeg, rightUpperLeg.transform,
            new Vector3(hipOffset, pelvisBottom - definition.upperLegSize.x - definition.lowerLegSize.x / 2, 0),
            definition.lowerLegSize, definition.lowerLegMass, characterLayer);
        SetupJoint(rightLowerLeg, rightUpperLeg.Rb, definition.kneeSpring, definition.kneeDamper);

        rightFoot = CreateCapsulePart("RightFoot", BodyPartType.RightFoot, rightLowerLeg.transform,
            new Vector3(hipOffset, pelvisBottom - definition.upperLegSize.x - definition.lowerLegSize.x - definition.footSize.x / 2, 0.05f),
            definition.footSize, definition.footMass, characterLayer, CapsuleDirection.Z);
        SetupJoint(rightFoot, rightLowerLeg.Rb, definition.ankleSpring, definition.ankleDamper);

        // Disable collisions between character parts
        var allParts = GetComponentsInChildren<Collider>();
        for (int i = 0; i < allParts.Length; i++)
            for (int j = i + 1; j < allParts.Length; j++)
                UnityEngine.Physics.IgnoreCollision(allParts[i], allParts[j]);
    }

    enum CapsuleDirection { Y = 1, X = 0, Z = 2 }

    BodyPart CreateCapsulePart(string name, BodyPartType type, Transform parent,
        Vector3 worldOffset, Vector2 size, float mass, int layer, CapsuleDirection dir = CapsuleDirection.Y)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = name;
        go.layer = layer;
        go.transform.SetParent(transform);
        go.transform.localPosition = worldOffset;
        go.transform.localRotation = Quaternion.identity;

        // Adjust scale based on capsule direction
        float height = size.x;
        float radius = size.y;
        // Unity capsule primitive is 2 units tall, 1 unit diameter
        switch (dir)
        {
            case CapsuleDirection.Y:
                go.transform.localScale = new Vector3(radius * 2, height / 2, radius * 2);
                break;
            case CapsuleDirection.X:
                go.transform.localScale = new Vector3(height / 2, radius * 2, radius * 2);
                break;
            case CapsuleDirection.Z:
                go.transform.localScale = new Vector3(radius * 2, radius * 2, height / 2);
                break;
        }

        // Replace MeshCollider with CapsuleCollider
        Object.DestroyImmediate(go.GetComponent<Collider>());
        var col = go.AddComponent<CapsuleCollider>();
        col.direction = (int)dir;
        col.height = height / go.transform.localScale[(int)dir];
        col.radius = radius / Mathf.Min(
            go.transform.localScale[((int)dir + 1) % 3],
            go.transform.localScale[((int)dir + 2) % 3]);

        var rb = go.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        var bp = go.AddComponent<BodyPart>();
        bp.partType = type;

        return bp;
    }

    BodyPart CreateSpherePart(string name, BodyPartType type, Transform parent,
        Vector3 worldOffset, float radius, float mass, int layer)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.layer = layer;
        go.transform.SetParent(transform);
        go.transform.localPosition = worldOffset;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one * radius * 2;

        // Replace with SphereCollider
        Object.DestroyImmediate(go.GetComponent<Collider>());
        var col = go.AddComponent<SphereCollider>();
        col.radius = 0.5f;

        var rb = go.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        var bp = go.AddComponent<BodyPart>();
        bp.partType = type;

        return bp;
    }

    void SetupJoint(BodyPart child, Rigidbody connectedBody, float spring, float damper)
    {
        var joint = child.gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = connectedBody;

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Limited;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        var limit = new SoftJointLimit { limit = 45f };
        joint.lowAngularXLimit = new SoftJointLimit { limit = -45f };
        joint.highAngularXLimit = limit;
        joint.angularYLimit = limit;
        joint.angularZLimit = limit;

        var drive = new JointDrive
        {
            positionSpring = spring,
            positionDamper = damper,
            maximumForce = float.MaxValue
        };
        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;
        joint.slerpDrive = drive;
        joint.rotationDriveMode = RotationDriveMode.Slerp;

        joint.targetRotation = Quaternion.identity;
        joint.configuredInWorldSpace = false;
        joint.autoConfigureConnectedAnchor = true;
    }
}
