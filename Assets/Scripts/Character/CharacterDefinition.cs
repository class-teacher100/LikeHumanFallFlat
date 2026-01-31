using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Character/Character Definition")]
public class CharacterDefinition : ScriptableObject
{
    [Header("Body Part Masses")]
    public float pelvisMass = 15f;
    public float torsoMass = 10f;
    public float headMass = 5f;
    public float upperArmMass = 3f;
    public float lowerArmMass = 2f;
    public float handMass = 1f;
    public float upperLegMass = 5f;
    public float lowerLegMass = 4f;
    public float footMass = 2f;

    [Header("Body Part Dimensions (height, radius)")]
    public Vector2 pelvisSize = new Vector2(0.3f, 0.15f);
    public Vector2 torsoSize = new Vector2(0.4f, 0.18f);
    public float headRadius = 0.12f;
    public Vector2 upperArmSize = new Vector2(0.28f, 0.06f);
    public Vector2 lowerArmSize = new Vector2(0.25f, 0.05f);
    public float handRadius = 0.06f;
    public Vector2 upperLegSize = new Vector2(0.35f, 0.08f);
    public Vector2 lowerLegSize = new Vector2(0.35f, 0.07f);
    public Vector2 footSize = new Vector2(0.15f, 0.05f);

    [Header("Spine Joint")]
    public float spineSpring = 2500f;
    public float spineDamper = 150f;

    [Header("Neck Joint")]
    public float neckSpring = 500f;
    public float neckDamper = 50f;

    [Header("Shoulder Joint")]
    public float shoulderSpring = 800f;
    public float shoulderDamper = 80f;

    [Header("Elbow Joint")]
    public float elbowSpring = 600f;
    public float elbowDamper = 60f;

    [Header("Wrist Joint")]
    public float wristSpring = 300f;
    public float wristDamper = 30f;

    [Header("Hip Joint")]
    public float hipSpring = 1500f;
    public float hipDamper = 120f;

    [Header("Knee Joint")]
    public float kneeSpring = 1000f;
    public float kneeDamper = 100f;

    [Header("Ankle Joint")]
    public float ankleSpring = 400f;
    public float ankleDamper = 40f;

    [Header("Movement")]
    public float moveForce = 50f;
    public float jumpImpulse = 300f;
    public float maxSpeed = 5f;

    [Header("Balance")]
    public float balanceTorque = 3000f;
    public float balanceDamping = 200f;

    [Header("Grab")]
    public float grabBreakForce = 2000f;
    public float grabRange = 0.3f;
}
