using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ProgressBar : MonoBehaviour
{
    public enum ProgressBarStates { Idle, Decreasing, Increasing, InDecreasingDelay, InIncreasingDelay }
    /// the possible fill modes 
    public enum FillModes { LocalScale, FillAmount, Width, Height, Anchor }
    /// the possible directions for the fill (for local scale and fill amount only)
    public enum BarDirections { LeftToRight, RightToLeft, UpToDown, DownToUp }
    /// the possible timescales the bar can work on
    public enum TimeScales { UnscaledTime, Time }
    /// the possible ways to animate the bar fill
    public enum BarFillModes { SpeedBased, FixedDuration }

    [Header("Main")]
    /// the main, foreground bar
    public Transform ForegroundBar;
    /// the delayed bar that will show when moving from a value to a new, lower value
    [FormerlySerializedAs("DelayedBar")]
    public Transform DelayedBarDecreasing;
    /// the delayed bar that will show when moving from a value to a new, higher value
    public Transform DelayedBarIncreasing;

    [Header("Fill Settings")]
    /// the local scale or fillamount value to reach when the value associated to the bar is at 0%
    [FormerlySerializedAs("StartValue")]
    [Range(0f, 1f)]
    public float MinimumBarFillValue = 0f;
    /// the local scale or fillamount value to reach when the bar is full
    [FormerlySerializedAs("EndValue")]
    [Range(0f, 1f)]
    public float MaximumBarFillValue = 1f;
    /// whether or not to initialize the value of the bar on start
    public bool SetInitialFillValueOnStart = false;
    /// the direction this bar moves to
    public BarDirections BarDirection = BarDirections.LeftToRight;
    /// the foreground bar's fill mode
    public FillModes FillMode = FillModes.LocalScale;
    /// defines whether the bar will work on scaled or unscaled time (whether or not it'll keep moving if time is slowed down for example)
    public TimeScales TimeScale = TimeScales.UnscaledTime;
    /// the selected fill animation mode
    public BarFillModes BarFillMode = BarFillModes.SpeedBased;

    [Header("Foreground Bar Settings")]
    /// whether or not the foreground bar should lerp
    public bool LerpForegroundBar = true;
    /// the speed at which to lerp the foreground bar
    public float LerpForegroundBarSpeedDecreasing = 15f;
    public float LerpForegroundBarSpeedIncreasing = 15f;
    public float LerpForegroundBarDurationDecreasing = 0.2f;
    /// the duration each update of the foreground bar should take (only if in fixed duration bar fill mode)
    public float LerpForegroundBarDurationIncreasing = 0.2f;
    /// the curve to use when animating the foreground bar fill
    public AnimationCurve LerpForegroundBarCurveDecreasing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public AnimationCurve LerpForegroundBarCurveIncreasing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Delayed Bar Decreasing")]

    /// the delay before the delayed bar moves (in seconds)
    [FormerlySerializedAs("Delay")]
    public float DecreasingDelay = 1f;
    /// whether or not the delayed bar's animation should lerp
    [FormerlySerializedAs("LerpDelayedBar")]
    public bool LerpDecreasingDelayedBar = true;
    /// the speed at which to lerp the delayed bar
    [FormerlySerializedAs("LerpDelayedBarSpeed")]
    public float LerpDecreasingDelayedBarSpeed = 15f;
    /// the duration each update of the foreground bar should take (only if in fixed duration bar fill mode)
    [FormerlySerializedAs("LerpDelayedBarDuration")]
    public float LerpDecreasingDelayedBarDuration = 0.2f;
    /// the curve to use when animating the delayed bar fill
    [FormerlySerializedAs("LerpDelayedBarCurve")]
    public AnimationCurve LerpDecreasingDelayedBarCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Delayed Bar Increasing")]

    /// the delay before the delayed bar moves (in seconds)
    public float IncreasingDelay = 1f;
    /// whether or not the delayed bar's animation should lerp
    public bool LerpIncreasingDelayedBar = true;
    /// the speed at which to lerp the delayed bar
    public float LerpIncreasingDelayedBarSpeed = 15f;
    /// the duration each update of the foreground bar should take (only if in fixed duration bar fill mode)
    public float LerpIncreasingDelayedBarDuration = 0.2f;
    /// the curve to use when animating the delayed bar fill
    public AnimationCurve LerpIncreasingDelayedBarCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Bump")]

    /// whether or not the bar should "bump" when changing value
    public bool BumpScaleOnChange = true;
    /// whether or not the bar should bump when its value increases
    public bool BumpOnIncrease = false;
    /// whether or not the bar should bump when its value decreases
    public bool BumpOnDecrease = false;
    /// the duration of the bump animation
    public float BumpDuration = 0.2f;
    /// whether or not the bar should flash when bumping
    public bool ChangeColorWhenBumping = true;
    /// the color to apply to the bar when bumping
    public Color BumpColor = Color.white;
    /// the curve to map the bump animation on
    [FormerlySerializedAs("BumpAnimationCurve")]
    public AnimationCurve BumpScaleAnimationCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));
    /// the curve to map the bump animation color animation on
    public AnimationCurve BumpColorAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
    /// whether or not the bar is bumping right now
    public bool Bumping { get; protected set; }

    [Header("Text")]
    public Text PercentageText;

    public string TextPrefix;
    public string TextSuffix;
    public float TextValueMultiplier = 1f;
    public string TextFormat = "{000}";

    [Header("Events")]
    /// an event to trigger every time the bar bumps
    public UnityEvent OnBump;
    // an event to trigger every time the bar starts decreasing
    public UnityEvent OnBarMovementDecreasingStart;
    // an event to trigger every time the bar stops decreasing
    public UnityEvent OnBarMovementDecreasingStop;
    // an event to trigger every time the bar starts increasing
    public UnityEvent OnBarMovementIncreasingStart;
    // an event to trigger every time the bar stops increasing
    public UnityEvent OnBarMovementIncreasingStop;

    [Header("For Debugging. Read Only!")]
    /// the current progress of the bar, ideally read only
    [Range(0f, 1f)]
    public float BarProgress;/// the current progress of the bar, ideally read only
    [Range(0f, 1f)]
    public float BarTarget;
    [Range(0f, 1f)]
    public float DelayedBarIncreasingProgress;
    [Range(0f, 1f)]
    public float DelayedBarDecreasingProgress;

    protected bool _initialized;
    protected Vector2 _initialBarSize;
    protected Color _initialColor;
    protected Vector3 _initialScale;

    protected Image _foregroundImage;
    protected Image _delayedDecreasingImage;
    protected Image _delayedIncreasingImage;

    protected Vector3 _targetLocalScale = Vector3.one;
    protected float _newPercent;
    protected float _percentLastTimeBarWasUpdated;
    protected float _lastUpdateTimestamp;

    protected float _time;
    protected float _deltaTime;
    protected int _direction;
    protected Coroutine _coroutine;
    protected bool _coroutineShouldRun = false;
    protected bool _isDelayedBarIncreasingNotNull;
    protected bool _isDelayedBarDecreasingNotNull;
    protected bool _actualUpdate;
    protected Vector2 _anchorVector;

    protected float _delayedBarDecreasingProgress;
    protected float _delayedBarIncreasingProgress;
    protected ProgressBarStates CurrentState = ProgressBarStates.Idle;
    protected string _updatedText;


    #region PUBLIC_API

    /// <summary>
    /// Updates the bar's values, using a normalized value
    /// </summary>
    /// <param name="normalizedValue"></param>
    public virtual void UpdateBar01(float normalizedValue)
    {
        UpdateBar(Mathf.Clamp01(normalizedValue), 0f, 1f);
    }

    /// <summary>
    /// Updates the bar's values based on the specified parameters
    /// </summary>
    /// <param name="currentValue">Current value.</param>
    /// <param name="minValue">Minimum value.</param>
    /// <param name="maxValue">Max value.</param>
    public virtual void UpdateBar(float currentValue, float minValue, float maxValue)
    {
        if (!_initialized)
        {
            Initialization();
        }

        if (!this.gameObject.activeInHierarchy)
        {
            this.gameObject.SetActive(true);
        }

        _newPercent = ExtraMaths.Remap(currentValue, minValue, maxValue, MinimumBarFillValue, MaximumBarFillValue);

        _actualUpdate = (BarTarget != _newPercent);

        if (!_actualUpdate)
        {
            return;
        }

        if (CurrentState != ProgressBarStates.Idle)
        {
            if ((CurrentState == ProgressBarStates.Decreasing) ||
                (CurrentState == ProgressBarStates.InDecreasingDelay))
            {
                if (_newPercent >= BarTarget)
                {
                    StopCoroutine(_coroutine);
                    SetBar01(BarTarget);
                }
            }
            if ((CurrentState == ProgressBarStates.Increasing) ||
                (CurrentState == ProgressBarStates.InIncreasingDelay))
            {
                if (_newPercent <= BarTarget)
                {
                    StopCoroutine(_coroutine);
                    SetBar01(BarTarget);
                }
            }
        }

        _percentLastTimeBarWasUpdated = BarProgress;
        _delayedBarDecreasingProgress = DelayedBarDecreasingProgress;
        _delayedBarIncreasingProgress = DelayedBarIncreasingProgress;

        BarTarget = _newPercent;

        if ((_newPercent != _percentLastTimeBarWasUpdated) && !Bumping)
        {
            Bump();
        }

        DetermineDeltaTime();
        _lastUpdateTimestamp = _time;

        DetermineDirection();
        if (_direction < 0)
        {
            OnBarMovementDecreasingStart?.Invoke();
        }
        else
        {
            OnBarMovementIncreasingStart?.Invoke();
        }

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutineShouldRun = true;


        if (this.gameObject.activeInHierarchy)
        {
            _coroutine = StartCoroutine(UpdateBarsCo());
        }
        else
        {
            SetBar(currentValue, minValue, maxValue);
        }

        UpdateText();
    }

    /// <summary>
    /// Sets the bar value to the one specified 
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    public virtual void SetBar(float currentValue, float minValue, float maxValue)
    {
        float newPercent = ExtraMaths.Remap(currentValue, minValue, maxValue, 0f, 1f);
        SetBar01(newPercent);
    }

    /// <summary>
    /// Sets the bar value to the normalized value set in parameter
    /// </summary>
    /// <param name="newPercent"></param>
    public virtual void SetBar01(float newPercent)
    {
        if (!_initialized)
        {
            Initialization();
        }

        newPercent = ExtraMaths.Remap(newPercent, 0f, 1f, MinimumBarFillValue, MaximumBarFillValue);
        BarProgress = newPercent;
        DelayedBarDecreasingProgress = newPercent;
        DelayedBarIncreasingProgress = newPercent;
        //_newPercent = newPercent;
        BarTarget = newPercent;
        _percentLastTimeBarWasUpdated = newPercent;
        _delayedBarDecreasingProgress = DelayedBarDecreasingProgress;
        _delayedBarIncreasingProgress = DelayedBarIncreasingProgress;
        SetBarInternal(newPercent, ForegroundBar, _foregroundImage, _initialBarSize);
        SetBarInternal(newPercent, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);
        SetBarInternal(newPercent, DelayedBarIncreasing, _delayedIncreasingImage, _initialBarSize);
        UpdateText();
        _coroutineShouldRun = false;
        CurrentState = ProgressBarStates.Idle;
    }

    #endregion PUBLIC_API

    #region START

    /// <summary>
    /// On start we store our image component
    /// </summary>
    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void OnEnable()
    {
        if (!_initialized)
        {
            return;
        }

        if (_foregroundImage != null)
        {
            _foregroundImage.color = _initialColor;
        }
    }

    public virtual void Initialization()
    {
        _isDelayedBarDecreasingNotNull = DelayedBarDecreasing != null;
        _isDelayedBarIncreasingNotNull = DelayedBarIncreasing != null;
        _initialScale = this.transform.localScale;

        if (ForegroundBar != null)
        {
            _foregroundImage = ForegroundBar.GetComponent<Image>();
            _initialBarSize = _foregroundImage.rectTransform.sizeDelta;
        }
        if (DelayedBarDecreasing != null)
        {
            _delayedDecreasingImage = DelayedBarDecreasing.GetComponent<Image>();
        }
        if (DelayedBarIncreasing != null)
        {
            _delayedIncreasingImage = DelayedBarIncreasing.GetComponent<Image>();
        }
        _initialized = true;

        if (_foregroundImage != null)
        {
            _initialColor = _foregroundImage.color;
        }

        _percentLastTimeBarWasUpdated = BarProgress;
    }

    #endregion START

    #region UpdatingBar

    protected virtual void UpdateText()
    {
        _updatedText = TextPrefix + (BarTarget * TextValueMultiplier).ToString(TextFormat) + TextSuffix;
        if (PercentageText != null)
        {
            PercentageText.text = _updatedText;
        }
    }

    /// <summary>
    /// On Update we update our bars
    /// </summary>
    protected virtual IEnumerator UpdateBarsCo()
    {
        while (_coroutineShouldRun)
        {
            DetermineDeltaTime();
            DetermineDirection();
            UpdateBars();
            yield return null;
        }

        CurrentState = ProgressBarStates.Idle;
        yield break;
    }

    protected virtual void DetermineDeltaTime()
    {
        _deltaTime = (TimeScale == TimeScales.Time) ? Time.deltaTime : Time.unscaledDeltaTime;
        _time = (TimeScale == TimeScales.Time) ? Time.time : Time.unscaledTime;
    }

    protected virtual void DetermineDirection()
    {
        _direction = (_newPercent > _percentLastTimeBarWasUpdated) ? 1 : -1;
    }

    /// <summary>
    /// Updates the foreground bar's scale
    /// </summary>
    protected virtual void UpdateBars()
    {
        float newFill;
        float newFillDelayed;
        float t1, t2 = 0f;

        // if the value is decreasing
        if (_direction < 0)
        {
            newFill = ComputeNewFill(LerpForegroundBar, LerpForegroundBarSpeedDecreasing, LerpForegroundBarDurationDecreasing, LerpForegroundBarCurveDecreasing, 0f, _percentLastTimeBarWasUpdated, out t1);
            SetBarInternal(newFill, ForegroundBar, _foregroundImage, _initialBarSize);
            SetBarInternal(newFill, DelayedBarIncreasing, _delayedIncreasingImage, _initialBarSize);

            BarProgress = newFill;
            DelayedBarIncreasingProgress = newFill;

            CurrentState = ProgressBarStates.Decreasing;

            if (_time - _lastUpdateTimestamp > DecreasingDelay)
            {
                newFillDelayed = ComputeNewFill(LerpDecreasingDelayedBar, LerpDecreasingDelayedBarSpeed, LerpDecreasingDelayedBarDuration, LerpDecreasingDelayedBarCurve, DecreasingDelay, _delayedBarDecreasingProgress, out t2);
                SetBarInternal(newFillDelayed, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);

                DelayedBarDecreasingProgress = newFillDelayed;
                CurrentState = ProgressBarStates.InDecreasingDelay;
            }
        }
        else // if the value is increasing
        {
            newFill = ComputeNewFill(LerpForegroundBar, LerpForegroundBarSpeedIncreasing, LerpForegroundBarDurationIncreasing, LerpForegroundBarCurveIncreasing, 0f, _delayedBarIncreasingProgress, out t1);
            SetBarInternal(newFill, DelayedBarIncreasing, _delayedIncreasingImage, _initialBarSize);

            DelayedBarIncreasingProgress = newFill;
            CurrentState = ProgressBarStates.Increasing;

            if (DelayedBarIncreasing == null)
            {
                newFill = ComputeNewFill(LerpForegroundBar, LerpForegroundBarSpeedIncreasing, LerpForegroundBarDurationIncreasing, LerpForegroundBarCurveIncreasing, 0f, _percentLastTimeBarWasUpdated, out t2);
                SetBarInternal(newFill, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);
                SetBarInternal(newFill, ForegroundBar, _foregroundImage, _initialBarSize);

                BarProgress = newFill;
                DelayedBarDecreasingProgress = newFill;
                CurrentState = ProgressBarStates.InDecreasingDelay;
            }
            else
            {
                if (_time - _lastUpdateTimestamp > IncreasingDelay)
                {
                    newFillDelayed = ComputeNewFill(LerpIncreasingDelayedBar, LerpForegroundBarSpeedIncreasing, LerpForegroundBarDurationIncreasing, LerpForegroundBarCurveIncreasing, IncreasingDelay, _delayedBarDecreasingProgress, out t2);

                    SetBarInternal(newFillDelayed, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);
                    SetBarInternal(newFillDelayed, ForegroundBar, _foregroundImage, _initialBarSize);

                    BarProgress = newFillDelayed;
                    DelayedBarDecreasingProgress = newFillDelayed;
                    CurrentState = ProgressBarStates.InDecreasingDelay;
                }
            }
        }

        if ((t1 >= 1f) && (t2 >= 1f))
        {
            _coroutineShouldRun = false;
            if (_direction > 0)
            {
                OnBarMovementIncreasingStop?.Invoke();
            }
            else
            {
                OnBarMovementDecreasingStop?.Invoke();
            }
        }
    }

    protected virtual float ComputeNewFill(bool lerpBar, float barSpeed, float barDuration, AnimationCurve barCurve, float delay, float lastPercent, out float t)
    {
        float newFill = 0f;
        t = 0f;
        if (lerpBar)
        {
            float delta = 0f;
            float timeSpent = _time - _lastUpdateTimestamp - delay;
            float speed = barSpeed;
            if (speed == 0f) { speed = 1f; }

            float duration = (BarFillMode == BarFillModes.FixedDuration) ? barDuration : (Mathf.Abs(_newPercent - lastPercent)) / speed;

            delta = ExtraMaths.Remap(timeSpent, 0f, duration, 0f, 1f);
            delta = Mathf.Clamp(delta, 0f, 1f);
            t = delta;
            if (t < 1f)
            {
                delta = barCurve.Evaluate(delta);
                newFill = Mathf.LerpUnclamped(lastPercent, _newPercent, delta);
            }
            else
            {
                newFill = _newPercent;
            }
        }
        else
        {
            newFill = _newPercent;
        }

        newFill = Mathf.Clamp(newFill, 0f, 1f);

        return newFill;
    }

    protected virtual void SetBarInternal(float newAmount, Transform bar, Image image, Vector2 initialSize)
    {
        if (bar == null)
        {
            return;
        }

        switch (FillMode)
        {
            case FillModes.LocalScale:
                _targetLocalScale = Vector3.one;
                switch (BarDirection)
                {
                    case BarDirections.LeftToRight:
                        _targetLocalScale.x = newAmount;
                        break;
                    case BarDirections.RightToLeft:
                        _targetLocalScale.x = 1f - newAmount;
                        break;
                    case BarDirections.DownToUp:
                        _targetLocalScale.y = newAmount;
                        break;
                    case BarDirections.UpToDown:
                        _targetLocalScale.y = 1f - newAmount;
                        break;
                }

                bar.localScale = _targetLocalScale;
                break;

            case FillModes.Width:
                if (image == null)
                {
                    return;
                }
                float newSizeX = ExtraMaths.Remap(newAmount, 0f, 1f, 0, initialSize.x);
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSizeX);
                break;

            case FillModes.Height:
                if (image == null)
                {
                    return;
                }
                float newSizeY = ExtraMaths.Remap(newAmount, 0f, 1f, 0, initialSize.y);
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSizeY);
                break;

            case FillModes.FillAmount:
                if (image == null)
                {
                    return;
                }
                image.fillAmount = newAmount;
                break;
            case FillModes.Anchor:
                if (image == null)
                {
                    return;
                }
                switch (BarDirection)
                {
                    case BarDirections.LeftToRight:
                        _anchorVector.x = 0f;
                        _anchorVector.y = 0f;
                        image.rectTransform.anchorMin = _anchorVector;
                        _anchorVector.x = newAmount;
                        _anchorVector.y = 1f;
                        image.rectTransform.anchorMax = _anchorVector;
                        break;
                    case BarDirections.RightToLeft:
                        _anchorVector.x = newAmount;
                        _anchorVector.y = 0f;
                        image.rectTransform.anchorMin = _anchorVector;
                        _anchorVector.x = 1f;
                        _anchorVector.y = 1f;
                        image.rectTransform.anchorMax = _anchorVector;
                        break;
                    case BarDirections.DownToUp:
                        _anchorVector.x = 0f;
                        _anchorVector.y = 0f;
                        image.rectTransform.anchorMin = _anchorVector;
                        _anchorVector.x = 1f;
                        _anchorVector.y = newAmount;
                        image.rectTransform.anchorMax = _anchorVector;
                        break;
                    case BarDirections.UpToDown:
                        _anchorVector.x = 0f;
                        _anchorVector.y = newAmount;
                        image.rectTransform.anchorMin = _anchorVector;
                        _anchorVector.x = 1f;
                        _anchorVector.y = 1f;
                        image.rectTransform.anchorMax = _anchorVector;
                        break;
                }
                break;
        }
    }
    #endregion

    #region  Bump

    /// <summary>
    /// Triggers a camera bump
    /// </summary>
    public virtual void Bump()
    {
        bool shouldBump = false;

        if (!_initialized)
        {
            return;
        }

        DetermineDirection();

        if (BumpOnIncrease && (_direction > 0))
        {
            shouldBump = true;
        }

        if (BumpOnDecrease && (_direction < 0))
        {
            shouldBump = true;
        }

        if (BumpScaleOnChange)
        {
            shouldBump = true;
        }

        if (!shouldBump)
        {
            return;
        }

        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(BumpCoroutine());
        }

        OnBump?.Invoke();
    }

    /// <summary>
    /// A coroutine that (usually quickly) changes the scale of the bar 
    /// </summary>
    /// <returns>The coroutine.</returns>
    protected virtual IEnumerator BumpCoroutine()
    {
        float journey = 0f;

        Bumping = true;

        while (journey <= BumpDuration)
        {
            journey = journey + _deltaTime;
            float percent = Mathf.Clamp01(journey / BumpDuration);
            float curvePercent = BumpScaleAnimationCurve.Evaluate(percent);
            float colorCurvePercent = BumpColorAnimationCurve.Evaluate(percent);
            this.transform.localScale = curvePercent * _initialScale;

            if (ChangeColorWhenBumping && (_foregroundImage != null))
            {
                _foregroundImage.color = Color.Lerp(_initialColor, BumpColor, colorCurvePercent);
            }

            yield return null;
        }
        if (ChangeColorWhenBumping && (_foregroundImage != null))
        {
            _foregroundImage.color = _initialColor;
        }
        Bumping = false;
        yield return null;
    }

    #endregion Bump

    #region ShowHide

    /// <summary>
    /// A simple method you can call to show the bar (set active true)
    /// </summary>
    public virtual void ShowBar()
    {
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides (SetActive false) the progress bar object, after an optional delay
    /// </summary>
    /// <param name="delay"></param>
    public virtual void HideBar(float delay)
    {
        if (delay <= 0)
        {
            this.gameObject.SetActive(false);
        }
        else if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(HideBarCo(delay));
        }
    }

    /// <summary>
    /// An internal coroutine used to handle the disabling of the progress bar after a delay
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    protected virtual IEnumerator HideBarCo(float delay)
    {
        yield return ExtraCoroutines.WaitFor(delay);
        this.gameObject.SetActive(false);
    }

    #endregion ShowHide
}
