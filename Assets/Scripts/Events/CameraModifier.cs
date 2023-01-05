using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraModifier : MonoBehaviour
{
    [Header("Settings")]
    /// the channel to receive events on
    [Tooltip("the channel to receive events on")]
    public int Channel = 0;
    /// The default amplitude that will be applied to your shakes if you don't specify one
    [Tooltip("The default amplitude that will be applied to your shakes if you don't specify one")]
    public float DefaultShakeAmplitude = .5f;
    /// The default frequency that will be applied to your shakes if you don't specify one
    [Tooltip("The default frequency that will be applied to your shakes if you don't specify one")]
    public float DefaultShakeFrequency = 10f;
    /// the amplitude of the camera's noise when it's idle
    [Tooltip("the amplitude of the camera's noise when it's idle")]
    [MMFReadOnly]
    public float IdleAmplitude;
    /// the frequency of the camera's noise when it's idle
    [Tooltip("the frequency of the camera's noise when it's idle")]
    [MMFReadOnly]
    public float IdleFrequency = 1f;
    /// the speed at which to interpolate the shake
    [Tooltip("the speed at which to interpolate the shake")]
    public float LerpSpeed = 5f;

    public virtual float GetTime() { return (timescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime; }
    public virtual float GetDeltaTime() { return (timescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime; }

    protected TimescaleModes timescaleMode;
    protected Vector3 initialPosition;
    protected Quaternion initialRotation;
    protected CinemachineBasicMultiChannelPerlin cameraPerlin;
    protected CinemachineVirtualCamera virtualCamera;
    protected float targetAmplitude;
    protected float targetFrequency;
    private Coroutine shakeCoroutine;

    /// <summary>
    /// On awake we grab our components
    /// </summary>
    protected virtual void Awake()
    {
        virtualCamera = this.gameObject.GetComponent<CinemachineVirtualCamera>();
        cameraPerlin = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    /// <summary>
    /// On Start we reset our camera to apply our base amplitude and frequency
    /// </summary>
    protected virtual void Start()
    {
        if (cameraPerlin != null)
        {
            IdleAmplitude = cameraPerlin.m_AmplitudeGain;
            IdleFrequency = cameraPerlin.m_FrequencyGain;
        }

        targetAmplitude = IdleAmplitude;
        targetFrequency = IdleFrequency;
    }

    protected virtual void Update()
    {
        if (cameraPerlin != null)
        {
            cameraPerlin.m_AmplitudeGain = targetAmplitude;
            cameraPerlin.m_FrequencyGain = Mathf.Lerp(cameraPerlin.m_FrequencyGain, targetFrequency, GetDeltaTime() * LerpSpeed);
        }
    }

    /// <summary>
    /// Use this method to shake the camera for the specified duration (in seconds) with the default amplitude and frequency
    /// </summary>
    /// <param name="duration">Duration.</param>
    public virtual void ShakeCamera(float duration, bool infinite, bool useUnscaledTime = false)
    {
        StartCoroutine(ShakeCameraCo(duration, DefaultShakeAmplitude, DefaultShakeFrequency, infinite, useUnscaledTime));
    }

    /// <summary>
    /// Use this method to shake the camera for the specified duration (in seconds), amplitude and frequency
    /// </summary>
    /// <param name="duration">Duration.</param>
    /// <param name="amplitude">Amplitude.</param>
    /// <param name="frequency">Frequency.</param>
    public virtual void ShakeCamera(float duration, float amplitude, float frequency, bool infinite, bool useUnscaledTime = false)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeCameraCo(duration, amplitude, frequency, infinite, useUnscaledTime));
    }

    /// <summary>
    /// This coroutine will shake the 
    /// </summary>
    /// <returns>The camera co.</returns>
    /// <param name="duration">Duration.</param>
    /// <param name="amplitude">Amplitude.</param>
    /// <param name="frequency">Frequency.</param>
    protected virtual IEnumerator ShakeCameraCo(float duration, float amplitude, float frequency, bool infinite, bool useUnscaledTime)
    {
        targetAmplitude = amplitude;
        targetFrequency = frequency;
        timescaleMode = useUnscaledTime ? TimescaleModes.Unscaled : TimescaleModes.Scaled;
        if (!infinite)
        {
            yield return new WaitForSeconds(duration);
            CameraReset();
        }
    }

    /// <summary>
    /// Resets the camera's noise values to their idle values
    /// </summary>
    public virtual void CameraReset()
    {
        targetAmplitude = IdleAmplitude;
        targetFrequency = IdleFrequency;
    }

    public virtual void OnCameraShakeEvent(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite, int channel, bool useUnscaledTime)
    {
        if ((channel != Channel) && (channel != -1) && (Channel != -1))
        {
            return;
        }
        this.ShakeCamera(duration, amplitude, frequency, infinite, useUnscaledTime);
    }

    public virtual void OnCameraShakeStopEvent(int channel)
    {
        if ((channel != Channel) && (channel != -1) && (Channel != -1))
        {
            return;
        }
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        CameraReset();
    }

    protected virtual void OnEnable()
    {
        CameraShakeEvent.Register(OnCameraShakeEvent);
        CameraShakeStopEvent.Register(OnCameraShakeStopEvent);
    }

    protected virtual void OnDisable()
    {
        CameraShakeEvent.Unregister(OnCameraShakeEvent);
        CameraShakeStopEvent.Unregister(OnCameraShakeStopEvent);
    }

}

[Serializable]
/// <summary>
/// Camera shake properties
/// </summary>
public struct CameraShakeProperties
{
    public float Duration;
    public float Amplitude;
    public float Frequency;
    public float AmplitudeX;
    public float AmplitudeY;
    public float AmplitudeZ;

    public CameraShakeProperties(float duration, float amplitude, float frequency, float amplitudeX = 0f, float amplitudeY = 0f, float amplitudeZ = 0f)
    {
        Duration = duration;
        Amplitude = amplitude;
        Frequency = frequency;
        AmplitudeX = amplitudeX;
        AmplitudeY = amplitudeY;
        AmplitudeZ = amplitudeZ;
    }
}

public enum CameraZoomModes { For, Set, Reset }

public struct CameraZoomEvent
{
    public delegate void Delegate(CameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, int channel, bool useUnscaledTime = false, bool stop = false, bool relative = false);

    static private event Delegate OnEvent;

    static public void Register(Delegate callback)
    {
        OnEvent += callback;
    }

    static public void Unregister(Delegate callback)
    {
        OnEvent -= callback;
    }

    static public void Trigger(CameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, int channel, bool useUnscaledTime = false, bool stop = false, bool relative = false)
    {
        OnEvent?.Invoke(mode, newFieldOfView, transitionDuration, duration, channel, useUnscaledTime, stop, relative);
    }
}

public struct CameraShakeEvent
{
    public delegate void Delegate(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite = false, int channel = 0, bool useUnscaledTime = false);
    static private event Delegate OnEvent;

    static public void Register(Delegate callback)
    {
        OnEvent += callback;
    }

    static public void Unregister(Delegate callback)
    {
        OnEvent -= callback;
    }

    static public void Trigger(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite = false, int channel = 0, bool useUnscaledTime = false)
    {
        OnEvent?.Invoke(duration, amplitude, frequency, amplitudeX, amplitudeY, amplitudeZ, infinite, channel, useUnscaledTime);
    }
}

public struct CameraShakeStopEvent
{
    public delegate void Delegate(int channel);
    static private event Delegate OnEvent;

    static public void Register(Delegate callback)
    {
        OnEvent += callback;
    }

    static public void Unregister(Delegate callback)
    {
        OnEvent -= callback;
    }

    static public void Trigger(int channel)
    {
        OnEvent?.Invoke(channel);
    }
}
