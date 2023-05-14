using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A list of the possible meta-game events base events
/// LevelStart : triggered by the LevelManager when a level starts
///	LevelComplete : can be triggered when the end of a level is reached
/// LevelEnd : same thing
///	Pause : triggered when a pause is starting
///	UnPause : triggered when a pause is ending and going back to normal
///	PlayerDeath : triggered when the player character dies
///	RespawnStarted : triggered when the player character respawn sequence starts
///	RespawnComplete : triggered when the player character respawn sequence ends
///	StarPicked : triggered when a star bonus gets picked
///	GameOver : triggered by the LevelManager when all lives are lost
/// CharacterSwap : triggered when the character gets swapped
/// CharacterSwitch : triggered when the character gets switched
/// Repaint : triggered to ask for a UI refresh
/// TogglePause : triggered to request a pause (or unpause)
/// </summary>
public enum GameEvents
{
    SpawnCharacterStarts,
    LevelStart,
    LevelEnd,
    Pause,
    UnPause,
    PlayerDeath,
    SpawnComplete,
    RespawnStarted,
    RespawnComplete,
    StarPicked,
    GameOver,
    CharacterSwap,
    CharacterSwitch,
    Repaint,
    TogglePause,
    LoadNextScene,
    LevelReset
}

/// <summary>
/// A type of events used to signal game events
/// </summary>
public struct GameEvent
{
    public GameEvents EventType;
    public Character OriginCharacter;

    /// <summary>
    /// Initializes a new instance of gamevent
    /// </summary>
    /// <param name="eventType">Event type.</param>
    public GameEvent(GameEvents eventType, Character originCharacter)
    {
        EventType = eventType;
        OriginCharacter = originCharacter;
    }

    static GameEvent e;
    public static void Trigger(GameEvents eventType, Character originCharacter)
    {
        e.EventType = eventType;
        e.OriginCharacter = originCharacter;
        ExtendedEventManager.TriggerEvent(e);
    }
    public static void Trigger(GameEvents eventType)
    {
        e.EventType = eventType;
        ExtendedEventManager.TriggerEvent(e);
    }
}

/// <summary>
/// A class to store points of entry into levels, one per level.
/// </summary>
public class PointsOfEntryStorage
{
    public string LevelName;
    public int PointOfEntryIndex;

    public PointsOfEntryStorage(string levelName, int pointOfEntryIndex)
    {
        LevelName = levelName;
        PointOfEntryIndex = pointOfEntryIndex;
    }
}

/// <summary>
/// The game manager is a persistent singleton that handles points and time
/// </summary>
public class GameManager : MonoBehaviour, ExtendedEventListener<GeneralEvent>, ExtendedEventListener<GameEvent>
{
    // Manages overall game events, a singleton
    public static GameManager instance;

    /// the target frame rate for the game
    [Tooltip("the target frame rate for the game")]
    public int TargetFrameRate = 300;
    [Header("Lives")]
    /// the maximum amount of lives the character can currently have
    [Tooltip("the maximum amount of lives the character can currently have")]
    public int MaximumLives = 0;
    /// the current number of lives 
    [Tooltip("the current number of lives ")]
    public int CurrentLives = 0;

    [Header("Bindings")]
    /// the name of the scene to redirect to when all lives are lost
    [Tooltip("the name of the scene to redirect to when all lives are lost")]
    public string GameOverScene;

    [Header("Pause")]
    /// true if the game is currently paused
    public bool Paused;
    // true if we've stored a map position at least once
    public bool StoredLevelMapPosition { get; set; }
    /// the current player
    public Vector2 LevelMapPosition { get; set; }
    /// the stored selected character
    public Character PersistentCharacter { get; set; }
    /// the list of points of entry and exit
    [Tooltip("the list of points of entry and exit")]
    public List<PointsOfEntryStorage> PointsOfEntry;
    /// the stored selected character
    public Character StoredCharacter { get; set; }

    [Header("Player Death")]
    /// the delay, in seconds, before displaying the death screen once the player is dead
    [Tooltip("the delay, in seconds, before displaying the death screen once the player is dead")]
    public float DelayBeforeDeathScreen = 1f;

    // storage
    protected bool pauseMenuOpen = false;
    protected int _initialMaximumLives;
    protected int _initialCurrentLives;

    /// <summary>
    /// On Awake we initialize our list of points of entry
    /// </summary>
    protected void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        PointsOfEntry = new List<PointsOfEntryStorage>();
    }

    /// <summary>
    /// On Start(), sets the target framerate to whatever's been specified
    /// </summary>
    protected virtual void Start()
    {
        Application.targetFrameRate = TargetFrameRate;
        _initialCurrentLives = CurrentLives;
        _initialMaximumLives = MaximumLives;
    }

    /// <summary>
    /// this method resets the whole game manager
    /// </summary>
    public virtual void Reset()
    {
        TimeScaleEvent.Trigger(TimeScaleMethods.Reset, 1f, 0f, false, 0f, true);
        Paused = false;
    }
    /// <summary>
    /// Use this method to decrease the current number of lives
    /// </summary>
    public virtual void LoseLife()
    {
        CurrentLives--;
    }

    /// <summary>
    /// Use this method when a life (or more) is gained
    /// </summary>
    /// <param name="lives">Lives.</param>
    public virtual void GainLives(int lives)
    {
        CurrentLives += lives;
        if (CurrentLives > MaximumLives)
        {
            CurrentLives = MaximumLives;
        }
    }

    /// <summary>
    /// Use this method to increase the max amount of lives, and optionnally the current amount as well
    /// </summary>
    /// <param name="lives">Lives.</param>
    /// <param name="increaseCurrent">If set to <c>true</c> increase current.</param>
    public virtual void AddLives(int lives, bool increaseCurrent)
    {
        MaximumLives += lives;
        if (increaseCurrent)
        {
            CurrentLives += lives;
        }
    }

    /// <summary>
    /// Resets the number of lives to their initial values.
    /// </summary>
    public virtual void ResetLives()
    {
        CurrentLives = _initialCurrentLives;
        MaximumLives = _initialMaximumLives;
    }

    /// <summary>
    /// Pauses the game or unpauses it depending on the current state
    /// </summary>
    public virtual void Pause()
    {
        // if time is not already stopped		
        if (Time.timeScale > 0.0f)
        {
            TimeScaleEvent.Trigger(TimeScaleMethods.For, 0f, 0f, false, 0f, true);
            instance.Paused = true;
            UIManager.instance.SetPauseScreen(true);
            pauseMenuOpen = true;
            LevelManager.instance.CharacterPause(true);
        }
        else
        {
            UnPause();
            LevelManager.instance.CharacterPause(false);
        }
    }

    /// <summary>
    /// Unpauses the game
    /// </summary>
    public virtual void UnPause()
    {
        TimeScaleEvent.Trigger(TimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);
        instance.Paused = false;
        UIManager.instance.SetPauseScreen(false);
        pauseMenuOpen = false;

        LevelManager.instance.CharacterPause(false);
    }

    /// <summary>
    /// Stores the points of entry for the level whose name you pass as a parameter.
    /// </summary>
    /// <param name="levelName">Level name.</param>
    /// <param name="entryIndex">Entry index.</param>
    /// <param name="exitIndex">Exit index.</param>
    public virtual void StorePointsOfEntry(string levelName, int entryIndex)
    {
        if (PointsOfEntry.Count > 0)
        {
            foreach (PointsOfEntryStorage point in PointsOfEntry)
            {
                if (point.LevelName == levelName)
                {
                    point.PointOfEntryIndex = entryIndex;
                    return;
                }
            }
        }

        PointsOfEntry.Add(new PointsOfEntryStorage(levelName, entryIndex));
    }

    /// <summary>
    /// Gets point of entry info for the level whose scene name you pass as a parameter
    /// </summary>
    /// <returns>The points of entry.</returns>
    /// <param name="levelName">Level name.</param>
    public virtual PointsOfEntryStorage GetPointsOfEntry(string levelName)
    {
        if (PointsOfEntry.Count > 0)
        {
            foreach (PointsOfEntryStorage point in PointsOfEntry)
            {
                if (point.LevelName == levelName)
                {
                    return point;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Clears the stored point of entry infos for the level whose name you pass as a parameter
    /// </summary>
    /// <param name="levelName">Level name.</param>
    public virtual void ClearPointOfEntry(string levelName)
    {
        if (PointsOfEntry.Count > 0)
        {
            foreach (PointsOfEntryStorage point in PointsOfEntry)
            {
                if (point.LevelName == levelName)
                {
                    PointsOfEntry.Remove(point);
                }
            }
        }
    }

    /// <summary>
    /// Clears all points of entry.
    /// </summary>
    public virtual void ClearAllPointsOfEntry()
    {
        PointsOfEntry.Clear();
    }

    /// <summary>
    /// Deletes all save files
    /// </summary>
    public virtual void ResetAllSaves()
    {
        //Not yet implemented
        return;
    }

    /// <summary>
    /// Reset the level
    /// </summary>
    protected virtual void ResetLevel()
    {
        UIManager.instance.SetDeathScreen(false);
        instance.Reset();
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Just display the victory screen for now
    /// </summary>
    public virtual void LevelEnd()
    {
        UIManager.instance.SetVictoryScreen(true);
    }

    /// <summary>
    /// Triggers the death screen display after a short delay
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator PlayerDead()
    {
        yield return new WaitForSeconds(DelayBeforeDeathScreen);

        UIManager.instance.SetDeathScreen(true);
    }

    /// <summary>
    /// Stores the selected character for use in upcoming levels
    /// </summary>
    /// <param name="selectedCharacter">Selected character.</param>
    public virtual void StoreSelectedCharacter(Character selectedCharacter)
    {
        StoredCharacter = selectedCharacter;
    }

    /// <summary>
    /// Clears the selected character.
    /// </summary>
    public virtual void ClearSelectedCharacter()
    {
        StoredCharacter = null;
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public virtual void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Catches GameEvents and acts on them, playing the corresponding sounds
    /// </summary>
    /// <param name="GeneralEvenet">MMGameEvent event.</param>
    public virtual void OnExtendedEvent(GeneralEvent GeneralEvenet)
    {

    }

    /// <summary>
    /// Catches TopDownEngineEvents and acts on them, playing the corresponding sounds
    /// </summary>
    /// <param name="GameEvent">TopDownEngineEvent event.</param>
    public virtual void OnExtendedEvent(GameEvent GameEvent)
    {
        switch (GameEvent.EventType)
        {
            case GameEvents.TogglePause:
                if (Paused)
                {
                    GameEvent.Trigger(GameEvents.UnPause, null);
                }
                else
                {
                    GameEvent.Trigger(GameEvents.Pause, null);
                }
                break;
            case GameEvents.Pause:
                Pause();
                break;

            case GameEvents.UnPause:
                UnPause();
                break;
            case GameEvents.LevelReset:
                ResetLevel();
                break;
            case GameEvents.PlayerDeath:
                StartCoroutine(PlayerDead());
                break;
            case GameEvents.LevelEnd:
                LevelEnd();
                break;
        }
    }

    /// <summary>
    /// OnDisable, we start listening to events.
    /// </summary>
    protected virtual void OnEnable()
    {
        this.ExtendedEventStartListening<GeneralEvent>();
        this.ExtendedEventStartListening<GameEvent>();
    }

    /// <summary>
    /// OnDisable, we stop listening to events.
    /// </summary>
    protected virtual void OnDisable()
    {
        this.ExtendedEventStopListening<GeneralEvent>();
        this.ExtendedEventStopListening<GameEvent>();
    }
}
