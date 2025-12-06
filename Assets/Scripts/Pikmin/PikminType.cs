using UnityEngine;

/// <summary>
/// Enum defining all Pikmin types
/// </summary>
public enum PikminColor
{
    Red,
    Blue,
    Yellow,
    White,
    Purple,
    Bulbmin,
    Pink,
    Rock,
    Ice,
    Luminous,
    Dark
}

/// <summary>
/// Base class for Pikmin type-specific behaviors
/// This should be attached alongside the Pikmin.cs script
/// </summary>
public abstract class PikminType : MonoBehaviour
{
    [Header("Type Info")]
    [SerializeField] protected PikminColor pikminColor;
    [SerializeField] protected Color visualColor = Color.white;

    [Header("Resistances")]
    [SerializeField] protected bool resistsFire = false;
    [SerializeField] protected bool resistsWater = false;
    [SerializeField] protected bool resistsElectricity = false;
    [SerializeField] protected bool resistsPoison = false;
    [SerializeField] protected bool resistsCold = false;
    [SerializeField] protected bool resistsDark = false;

    [Header("Special Abilities")]
    [SerializeField] protected bool canSwim = false;
    [SerializeField] protected bool canFly = false;
    [SerializeField] protected bool canDigUnderground = false;
    [SerializeField] protected bool canBreakRocks = false;

    [Header("Stats Modifiers")]
    [SerializeField] protected float strengthMultiplier = 1f;
    [SerializeField] protected float speedMultiplier = 1f;
    [SerializeField] protected float jumpHeightMultiplier = 1f;

    protected Pikmin basePikmin;
    protected Renderer pikminRenderer;

    protected virtual void Awake()
    {
        basePikmin = GetComponent<Pikmin>();
        pikminRenderer = GetComponent<Renderer>();

        if (basePikmin == null)
        {
            Debug.LogError($"[PikminType] {GetType().Name} requires a Pikmin component!");
        }
    }

    protected virtual void Start()
    {
        // Apply visual color
        ApplyColor();
    }

    /// <summary>
    /// Apply the Pikmin's color to its renderer
    /// </summary>
    protected virtual void ApplyColor()
    {
        if (pikminRenderer != null && pikminRenderer.material != null)
        {
            pikminRenderer.material.color = visualColor;
        }
    }

    /// <summary>
    /// Called when entering a hazard zone
    /// </summary>
    public virtual bool CanSurviveHazard(string hazardType)
    {
        switch (hazardType.ToLower())
        {
            case "fire":
                return resistsFire;
            case "water":
                return resistsWater || canSwim;
            case "electricity":
            case "electric":
                return resistsElectricity;
            case "poison":
                return resistsPoison;
            case "cold":
            case "ice":
                return resistsCold;
            case "dark":
                return resistsDark;
            default:
                return false;
        }
    }

    /// <summary>
    /// Special ability activation
    /// Override in derived classes for specific abilities
    /// </summary>
    public virtual void ActivateSpecialAbility()
    {
        // Override in derived classes
    }

    /// <summary>
    /// Check if this Pikmin can perform a specific task
    /// </summary>
    public virtual bool CanPerformTask(string taskType)
    {
        switch (taskType.ToLower())
        {
            case "swim":
                return canSwim;
            case "fly":
                return canFly;
            case "dig":
                return canDigUnderground;
            case "break_rocks":
                return canBreakRocks;
            default:
                return true; // Default: can perform basic tasks
        }
    }

    // Public getters
    public PikminColor GetPikminColor() => pikminColor;
    public float GetStrengthMultiplier() => strengthMultiplier;
    public float GetSpeedMultiplier() => speedMultiplier;
    public float GetJumpHeightMultiplier() => jumpHeightMultiplier;
    public bool CanSwim() => canSwim;
    public bool CanFly() => canFly;
}
