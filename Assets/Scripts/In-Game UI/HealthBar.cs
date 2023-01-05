using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour
{
    /// <summary>
    /// The health bar is essentially a controller and a specific version of a progressbar
    /// The healthbar class will take a progressbar prefab and control it with health bar specific
    /// functions and uses.
    /// </summary>

    //Whether to instantiate a new healthbar or to use one already existing in the scene
    public enum Modes { Instantiate, Existing}
    [Tooltip("the selected direction these feedbacks should play in")]
    public Modes mode = Modes.Instantiate;

    [Header("Select a Prefab")]
    /// the prefab to use as the health bar
    [Tooltip("the prefab to use as the health bar")]
    public ProgressBar HealthBarPrefab;
    [Header("Select a Healthbar")]
    /// the already existing health bar
    [Tooltip("the gameObject already in a scene to control")]
    public ProgressBar ExistingHealthBar;

    [Header("Death")]
    /// a gameobject (usually a particle system) to instantiate when the healthbar reaches zero
    [Tooltip("a gameobject (usually a particle system) to instantiate when the healthbar reaches zero")]
    public GameObject InstantiatedOnDeath;

    [Header("Display")]
/// whether or not the bar should be permanently displayed
    [Tooltip("whether or not the bar should be permanently displayed")]
    public bool AlwaysVisible = true;
    /// the duration (in seconds) during which to display the bar
    [Tooltip("the duration (in seconds) during which to display the bar")]
    public float DisplayDurationOnHit = 1f;
    /// if this is set to true the bar will hide itself when it reaches zero
    [Tooltip("if this is set to true the bar will hide itself when it reaches zero")]
    public bool HideBarAtZero = true;
    /// the delay (in seconds) after which to hide the bar
    [Tooltip("the delay (in seconds) after which to hide the bar")]
    public float HideBarAtZeroDelay = 1f;
    [Tooltip("if this is true, bumps the scale of the healthbar when its value changes")]
    public bool BumpScaleOnChange = true;

    [Header("Offset")]
     /// the offset to apply to the healthbar compared to the object's center
    [Tooltip("the offset to apply to the healthbar compared to the object's center")]
    public Vector3 HealthBarOffset = new Vector3(0f, 1f, 0f);

    protected ProgressBar progressBar;
    protected float lastShowTimestamp = 0f;
    protected bool showBar = false;
    protected bool finalHideStarted = false;

    #region Start

    /// <summary>
    /// On Start, sets the health bar up
    /// </summary>
    protected virtual void Awake()
    {
        Initialisation();
    }

    /// <summary>
    /// On enable, initializes the bar again
    /// </summary>
    protected void OnEnable()
    {
        finalHideStarted = false;

        if (!AlwaysVisible && (progressBar != null))
        {
            progressBar.gameObject.SetActive(false);
        }
    }

    public virtual void Initialisation()
    {
        finalHideStarted = false;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(AlwaysVisible);
            return;
        }

        if (mode == Modes.Instantiate)
        {
            if (HealthBarPrefab == null)
            {
                Debug.LogWarning(this.name + " : the HealthBar has no prefab associated to it, nothing will be displayed.");
                return;
            }
            progressBar = Instantiate(HealthBarPrefab, transform.position + HealthBarOffset, transform.rotation) as ProgressBar;
            SceneManager.MoveGameObjectToScene(progressBar.gameObject, this.gameObject.scene);
            progressBar.transform.SetParent(this.transform);
            progressBar.gameObject.name = "HealthBar";
        }
        else
        {
            progressBar = ExistingHealthBar;
        }
        
        if (!AlwaysVisible)
        {
            progressBar.gameObject.SetActive(false);
        }

        if (progressBar != null)
        {
            progressBar.SetBar(100f, 0f, 100f);
        }
    }

    #endregion

    #region Upate

    /// <summary>
    /// On Update, we hide or show our healthbar based on our current status
    /// </summary>
    protected virtual void Update()
    {
        if (progressBar == null)
        {
            return;
        }

        if (finalHideStarted)
        {
            return;
        }

        if (AlwaysVisible)
        {
            return;
        }

        if (showBar)
        {
            progressBar.gameObject.SetActive(true);
            float currentTime = Time.time;
            if (currentTime - lastShowTimestamp > DisplayDurationOnHit)
            {
                showBar = false;
            }
        }
        else
        {
            progressBar.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Hides the bar when it reaches zero
    /// </summary>
    /// <returns>The hide bar.</returns>
    protected virtual IEnumerator FinalHideBar()
    {
        finalHideStarted = true;
        if (InstantiatedOnDeath != null)
        {
            GameObject instantiatedOnDeath = Instantiate(InstantiatedOnDeath, this.transform.position + HealthBarOffset, this.transform.rotation);
            SceneManager.MoveGameObjectToScene(instantiatedOnDeath.gameObject, this.gameObject.scene);
        }
        if (HideBarAtZeroDelay == 0)
        {
            showBar = false;
            progressBar.gameObject.SetActive(false);
            yield return null;
        }
        else
        {
            progressBar.HideBar(HideBarAtZeroDelay);
        }
    }

    /// <summary>
    /// Updates the bar
    /// </summary>
    /// <param name="currentHealth">Current health.</param>
    /// <param name="minHealth">Minimum health.</param>
    /// <param name="maxHealth">Max health.</param>
    /// <param name="show">Whether or not we should show the bar.</param>
    public virtual void UpdateBar(float currentHealth, float minHealth, float maxHealth, bool show)
    {
        // if the healthbar isn't supposed to be always displayed, we turn it on for the specified duration
        if (!AlwaysVisible && show)
        {
            showBar = true;
            lastShowTimestamp = Time.time;
        }

        if (progressBar != null)
        {
            progressBar.UpdateBar(currentHealth, minHealth, maxHealth);

            if (HideBarAtZero && progressBar.BarTarget <= 0)
            {
                StartCoroutine(FinalHideBar());
            }

            if (BumpScaleOnChange)
            {
                progressBar.Bump();
            }
        }
    }

    #endregion
}
