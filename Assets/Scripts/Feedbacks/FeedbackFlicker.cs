using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This feedback will make the bound renderer flicker for the set duration when played (and restore its initial color when stopped)
/// </summary>
public class FeedbackFlicker : Feedback
{
    /// the possible modes
    /// Color : will control material.color
    /// PropertyName : will target a specific shader property by name
    public enum Modes { Colour, PropertyName }

    [Header("Flicker")]
    /// the renderer to flicker when played
    [Tooltip("the renderer to flicker when played")]
    public SpriteRenderer BoundRenderer;
    /// the selected mode to flicker the renderer 
    [Tooltip("the selected mode to flicker the renderer")]
    public Modes Mode = Modes.Colour;
    /// the name of the property to target
    [MMFEnumCondition("Mode", (int)Modes.PropertyName)]
    [Tooltip("the name of the property to target")]
    public string PropertyName = "_Tint";
    /// the duration of the flicker when getting damage
    [Tooltip("the duration of the flicker when getting damage")]
    public float FlickerDuration = 0.2f;
    /// the frequency at which to flicker
    [Tooltip("the frequency at which to flicker")]
    public float FlickerOctave = 0.04f;
    /// the color we should flicker the sprite to 
    [Tooltip("the color we should flicker the sprite to")]
    [ColorUsage(true, false)]
    public Color FlickerColour = new Color(255, 20, 255);
    /// the list of material indexes we want to flicker on the target renderer. If left empty, will only target the material at index 0 
    [Tooltip("the list of material indexes we want to flicker on the target renderer. If left empty, will only target the material at index 0")]
    public int[] MaterialIndexes;
    /// if this is true, this component will use material property blocks instead of working on an instance of the material.
    [Tooltip("if this is true, this component will use material property blocks instead of working on an instance of the material.")]
    public bool UseMaterialPropertyBlocks = false;

    /// the duration of this feedback is the duration of the flicker
    public override float FeedbackDuration { get { return (FlickerDuration); } set { FlickerDuration = value; } }

    protected const string _colorPropertyName = "_Color";

    protected Color[] _initialFlickerColors;
    protected int[] _propertyIDs;
    protected bool[] _propertiesFound;
    protected Coroutine[] _coroutines;
    protected MaterialPropertyBlock _propertyBlock;

    /// <summary>
    /// On init we grab our initial color and components
    /// </summary>
    /// <param name="owner"></param>
    protected override void CustomInitialisation(GameObject owner)
    {
        if (MaterialIndexes.Length == 0)
        {
            MaterialIndexes = new int[1];
            MaterialIndexes[0] = 0;
        }

        _coroutines = new Coroutine[MaterialIndexes.Length];

        _initialFlickerColors = new Color[MaterialIndexes.Length];
        _propertyIDs = new int[MaterialIndexes.Length];
        _propertiesFound = new bool[MaterialIndexes.Length];
        _propertyBlock = new MaterialPropertyBlock();

        if (Active && (BoundRenderer == null) && (owner != null))
        {
            if (owner.MMFGetComponentNoAlloc<SpriteRenderer>() != null)
            {
                BoundRenderer = owner.GetComponent<SpriteRenderer>();
            }
            if (BoundRenderer == null)
            {
                BoundRenderer = owner.GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (BoundRenderer == null)
        {
            Debug.LogWarning("[MMFeedbackFlicker] The flicker feedback on " + this.name + " doesn't have a bound renderer, it won't work. You need to specify a renderer to flicker in its inspector.");
        }

        if (Active)
        {
            if (BoundRenderer != null)
            {
                BoundRenderer.GetPropertyBlock(_propertyBlock);
            }
        }

        for (int i = 0; i < MaterialIndexes.Length; i++)
        {
            _propertiesFound[i] = false;

            if (Active && (BoundRenderer != null))
            {
                if (Mode == Modes.Colour)
                {
                    _propertiesFound[i] = UseMaterialPropertyBlocks ? BoundRenderer.sharedMaterials[i].HasProperty(_colorPropertyName) : BoundRenderer.materials[i].HasProperty(_colorPropertyName);
                    if (_propertiesFound[i])
                    {
                        _initialFlickerColors[i] = UseMaterialPropertyBlocks ? BoundRenderer.sharedMaterials[i].color : BoundRenderer.materials[i].color;
                    }
                }
                else
                {
                    _propertiesFound[i] = UseMaterialPropertyBlocks ? BoundRenderer.sharedMaterials[i].HasProperty(PropertyName) : BoundRenderer.materials[i].HasProperty(PropertyName);
                    if (_propertiesFound[i])
                    {
                        _propertyIDs[i] = Shader.PropertyToID(PropertyName);
                        _initialFlickerColors[i] = UseMaterialPropertyBlocks ? BoundRenderer.sharedMaterials[i].GetColor(_propertyIDs[i]) : BoundRenderer.materials[i].GetColor(_propertyIDs[i]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// On play we make our renderer flicker
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
    {
        if (!Active || (BoundRenderer == null))
        {
            return;
        }
        for (int i = 0; i < MaterialIndexes.Length; i++)
        {
            _coroutines[i] = StartCoroutine(Flicker(BoundRenderer, i, _initialFlickerColors[i], FlickerColour, FlickerOctave, FeedbackDuration));
        }
    }

    /// <summary>
    /// On reset we make our renderer stop flickering
    /// </summary>
    protected override void CustomReset()
    {
        base.CustomReset();

        if (InCooldown)
        {
            return;
        }

        if (Active  && (BoundRenderer != null))
        {
            for (int i = 0; i < MaterialIndexes.Length; i++)
            {
                SetColor(i, _initialFlickerColors[i]);
            }
        }
    }

    public virtual IEnumerator Flicker(Renderer renderer, int materialIndex, Color initialColor, Color flickerColor, float flickerSpeed, float flickerDuration)
    {
        if (renderer == null)
        {
            yield break;
        }

        if (!_propertiesFound[materialIndex])
        {
            yield break;
        }

        if (initialColor == flickerColor)
        {
            yield break;
        }

        float flickerStop = FeedbackTime + flickerDuration;
        IsPlaying = true;

        while (FeedbackTime < flickerStop)
        {
            SetColor(materialIndex, flickerColor);
            if (Timing.TimescaleMode == TimescaleModes.Scaled)
            {
                yield return FeedbacksCoroutine.WaitFor(flickerSpeed);
            }
            else
            {
                yield return FeedbacksCoroutine.WaitForUnscaled(flickerSpeed);
            }
            SetColor(materialIndex, initialColor);
            if (Timing.TimescaleMode == TimescaleModes.Scaled)
            {
                yield return FeedbacksCoroutine.WaitFor(flickerSpeed);
            }
            else
            {
                yield return FeedbacksCoroutine.WaitForUnscaled(flickerSpeed);
            }
        }

        SetColor(materialIndex, initialColor);
        IsPlaying = false;
    }

    protected virtual void SetColor(int materialIndex, Color colour)
    {
        if (!_propertiesFound[materialIndex])
        {
            return;
        }

        if (Mode == Modes.Colour)
        {
            if (UseMaterialPropertyBlocks)
            {
                BoundRenderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(_colorPropertyName, colour);
                BoundRenderer.SetPropertyBlock(_propertyBlock, materialIndex);
            }
            else
            {
                BoundRenderer.color = colour;
            }
        }
        else
        {
            if (UseMaterialPropertyBlocks)
            {
                BoundRenderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(_propertyIDs[materialIndex], colour);
                BoundRenderer.SetPropertyBlock(_propertyBlock, materialIndex);
            }
            else
            {
                BoundRenderer.materials[materialIndex].SetColor(_propertyIDs[materialIndex], colour);
            }
        }
    }

    /// <summary>
    /// Stops this feedback
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
    {
        if (!Active)
        {
            return;
        }
        base.CustomStopFeedback(position, feedbacksIntensity);

        IsPlaying = false;
        for (int i = 0; i < _coroutines.Length; i++)
        {
            if (_coroutines[i] != null)
            {
                StopCoroutine(_coroutines[i]);
            }
            _coroutines[i] = null;
        }
    }
}
