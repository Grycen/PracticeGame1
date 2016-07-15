﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;

/// <summary>
/// Manages the loading up of the gameplay prefab and controls the gameplay events.
/// </summary>
public class GameplayManager : MonoBehaviourSingleton<GameplayManager>
{
    // Speed at which the moving track of incoming objects moves towards the player during the infinite runner gameplay.
    public float GameplayTrackSpeed = 0.001f;

    public bool GameplayPaused = false;

    private bool _GameplayStarted = false;
    private bool _GameplayControlsListenerOn = false;
    private bool _StartGameCountdownFinished = false;

    private GameObject _ThirdPersonController;
    private ThirdPersonUserControl _ThirdPersonUserControlScript;

    private static float STARTING_COUNTDOWN_TIME = 5.0f;
    private float _TimeLeft = STARTING_COUNTDOWN_TIME;

    private const string PRIMARY_GAMEPLAY_AUDIO_SOURCE = "GameplayPrefab(Clone)";

    public int TimeLeftOnCountdownInSeconds = 5;

    private string GAMEPLAY_PREFAB_PATH = "Prefabs/3D/GameplayPrefab";

    private string GAMEPLAY_PREFAB_NAME = "GameplayPrefab(Clone)";

    // Use this for initialization
    void Start()
    {
        Debug.Log("GameplayManager is active.");
    }

    void Update()
    {
        if(_GameplayControlsListenerOn)
        {
            // Escape key - Toggle on/off pause.
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseGameplay();
            }
        }

        // Countdown timer to begin game.
        if(!GameplayPaused && !_StartGameCountdownFinished && _GameplayStarted)
        {
            _TimeLeft -= Time.deltaTime;

            if(_TimeLeft < 0)
            {
                // Timer is finished begin gameplay and enable user controls.

                _StartGameCountdownFinished = true;

                //_ThirdPersonUserControlScript.enabled = true;

                Time.timeScale = 1.0f;
            }
            else
            {
                //_ThirdPersonUserControlScript.enabled = false;

                TimeLeftOnCountdownInSeconds = Mathf.RoundToInt(_TimeLeft);
            }
        }
    }

    public void CreateGameplayInstance()
    {
        Debug.Log("Gameplay instance is being created.");

        GameplayPaused = false;
        Time.timeScale = 1.0f;
        _StartGameCountdownFinished = false;
        _TimeLeft = STARTING_COUNTDOWN_TIME;

        UIManager.Instance.ToggleFrontendUI(false);

        CreateGameplayPrefab();

        _GameplayStarted = true;
    }

    private void CreateGameplayPrefab()
    {
        // Check for previous GameplayPrefab instance if one still exists and destroy it.
        GameObject previousGameplayPrefab = GameObject.Find(GAMEPLAY_PREFAB_NAME);
        if (previousGameplayPrefab != null)
        {
            DestroyImmediate(previousGameplayPrefab);
        }

        // Create new GameplayPrefab instance.
        GameObject gameplayPrefab = Instantiate(Resources.Load(GAMEPLAY_PREFAB_PATH, typeof(GameObject))) as GameObject;

        if (gameplayPrefab != null)
        {
            Debug.LogFormat("Gameplay prefab instanced.");

            gameplayPrefab.transform.position = Vector3.zero;
        }
        else
        {
            Debug.LogErrorFormat("Gameplay prefab failed to instance!");
        }

        _ThirdPersonController = GameObject.Find("ThirdPersonController");
        if(_ThirdPersonController != null)
        {
            _ThirdPersonUserControlScript = _ThirdPersonController.GetComponent<ThirdPersonUserControl>();
        }

        // Setup the gameplay UI.
        SetupInGameUI();

        ToggleGameplayControlsListener(true);

        // Set the primary gameplay audio source for the AudioManager to use.
        StartCoroutine("DelayedSetupGameplayAudioSource");
    }

    public IEnumerator DelayedSetupGameplayAudioSource()
    {
        yield return new WaitForSeconds(2.0f);

        SetupGameplayAudioSource();
    }

    public void SetupGameplayAudioSource()
    {
        GameObject audioSourceObj = GameObject.Find(PRIMARY_GAMEPLAY_AUDIO_SOURCE);
        if (audioSourceObj != null)
        {
            AudioSource gameplayAudioSource = audioSourceObj.GetComponent<AudioSource>();
            if (gameplayAudioSource != null)
            {
                AudioManager.Instance.SetGameplayAudioSource(gameplayAudioSource);
            }
        }
    }

    private void DestroyGameplayPrefab()
    {
        GameObject gameplayPrefab = GameObject.Find(GAMEPLAY_PREFAB_NAME);
        if(gameplayPrefab != null)
        {
            DestroyImmediate(gameplayPrefab);
        }
    }

    /// <summary>
    /// Interacts with UIManager script to establish gameplay UI (Hud overlay, load any pre-game screens, load any necessary popups).
    /// </summary>
    private void SetupInGameUI()
    {
        // Switch to in-game overlay.
        UIManager.Instance.LoadOverlay(UIManager.UIOverlays.GameplayHud, true);
        // Load the PauseGameplayScreen but don't set to active.
        UIManager.Instance.LoadScreen(UIManager.UIScreens.PauseGameplayScreen, false);
    }

    public void ExitGameAndReturnToFrontend()
    {
        UIManager.Instance.ToggleFrontendUI(true);

        DestroyGameplayPrefab();
    }

    private void ToggleGameplayControlsListener(bool isActive)
    {
        _GameplayControlsListenerOn = isActive;
    }
    
    public void OpenGameOverPopup()
    {
        TogglePauseGameplay(false);
        UIManager.Instance.LoadAndShowUniquePopup(UIManager.UIPopups.GameOverPopup);
    }

    /// <summary>
    /// Toggle on or off pause. If paused, it pauses time and movement in worldspace gameplay and freezes timer or any other time-based effects.
    /// </summary>
    public void TogglePauseGameplay(bool showPauseScreen = true)
    {
        // Toggle between paused or unpaused.
        if (GameplayPaused)
        {
            // Resume gameplay and unpause.
            Time.timeScale = 1.0f;

            Debug.Log("Resuming gameplay and unpausing!");

            GameplayPaused = false;
        }
        else
        {
            // Bring up the pause menu and pause gameplay.
            Time.timeScale = 0.0f;

            Debug.Log("Pausing the gameplay!");

            GameplayPaused = true;
        }

        if (showPauseScreen)
        {
            UIManager.Instance.ToggleScreenActive(UIManager.UIScreens.PauseGameplayScreen, GameplayPaused);
        }
    }
}
