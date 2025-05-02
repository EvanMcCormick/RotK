using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Global music manager that persists between scenes and handles music playback,
/// volume control, fading and playlist management for the entire game.
/// </summary>
public partial class MusicManager : Node
{
    // Audio players for crossfading between tracks
    private AudioStreamPlayer _musicPlayer1;
    private AudioStreamPlayer _musicPlayer2;
    
    // Track which player is currently active
    private AudioStreamPlayer _currentPlayer;
    private AudioStreamPlayer _nextPlayer;
    
    // Music settings
    private float _masterVolume = 1.0f;
    private float _musicVolume = 0.5f; // Default to 50% volume
    private bool _isMusicEnabled = true;
    
    // Fade settings
    private bool _isFading = false;
    private float _fadeInDuration = 1.0f; // seconds
    private float _fadeOutDuration = 1.0f; // seconds
    private float _fadeTimer = 0.0f;
    private float _targetVolume = 0.0f;
    
    // Playlist system
    private List<string> _playlist = new List<string>();
    private int _currentTrackIndex = 0;
    private bool _shufflePlaylist = false;
    private Random _random = new Random();
    
    // Singleton instance
    private static MusicManager _instance;
    
    // Property to access the singleton instance
    public static MusicManager Instance
    {
        get { return _instance; }
    }
    
    public override void _EnterTree()
    {
        // FIX: Check if this is the autoloaded singleton instance
        if (_instance == null)
        {
            _instance = this;
            // Make the music manager persist between scenes
            ProcessMode = ProcessModeEnum.Always;
        }
        else if (this != _instance)
        {
            // This is a duplicate - remove it
            QueueFree();
        }
    }
    
    public override void _Ready()
    {
        // Only initialize if this is the singleton instance
        if (this != _instance)
        {
            return;
        }
        
        // Create audio players
        _musicPlayer1 = new AudioStreamPlayer();
        _musicPlayer2 = new AudioStreamPlayer();
        
        // Set initial properties
        _musicPlayer1.VolumeDb = -80.0f; // Start silent
        _musicPlayer2.VolumeDb = -80.0f; // Start silent
        
        // Add players as children of this node
        AddChild(_musicPlayer1);
        AddChild(_musicPlayer2);
        
        // Connect signals
        _musicPlayer1.Finished += OnMusicFinished;
        _musicPlayer2.Finished += OnMusicFinished;
        
        // Set initial player references
        _currentPlayer = _musicPlayer1;
        _nextPlayer = _musicPlayer2;
        
        // Initialize the playlist with available music
        InitializePlaylist();
    }
    
    public override void _Process(double delta)
    {
        // Handle fading between tracks
        if (_isFading)
        {
            _fadeTimer += (float)delta;
            
            // Calculate fade progress
            float progress = 0;
            
            // Fading in or out?
            if (_targetVolume > -80.0f)
            {
                // Fading in
                progress = Math.Min(_fadeTimer / _fadeInDuration, 1.0f);
                float currentDb = Mathf.Lerp(-80.0f, LinearToDb(_musicVolume * _masterVolume), progress);
                _nextPlayer.VolumeDb = currentDb;
                
                // Fade complete
                if (progress >= 1.0f)
                {
                    _isFading = false;
                    _currentPlayer = _nextPlayer;
                }
            }
            else
            {
                // Fading out
                progress = Math.Min(_fadeTimer / _fadeOutDuration, 1.0f);
                float currentDb = Mathf.Lerp(LinearToDb(_musicVolume * _masterVolume), -80.0f, progress);
                _currentPlayer.VolumeDb = currentDb;
                
                // Fade complete
                if (progress >= 1.0f)
                {
                    _isFading = false;
                    _currentPlayer.Stop();
                }
            }
        }
    }
    
    /// <summary>
    /// Initialize the playlist with all available music
    /// </summary>
    private void InitializePlaylist()
    {
        _playlist.Clear();
        
        // Add all available music files to the playlist
        _playlist.Add("res://Assets/music/ROTK1.mp3");
        _playlist.Add("res://Assets/music/ROTK2.mp3");
        
        // Shuffle playlist if enabled
        if (_shufflePlaylist)
        {
            ShufflePlaylist();
        }
    }
    
    /// <summary>
    /// Play the specified music file with optional fade-in
    /// </summary>
    public void PlayMusic(string musicPath, bool fadeIn = true)
    {
        if (!_isMusicEnabled || string.IsNullOrEmpty(musicPath))
        {
            return;
        }
        
        // Load the music resource
        var music = ResourceLoader.Load<AudioStream>(musicPath);
        if (music == null)
        {
            GD.PrintErr($"Failed to load music: {musicPath}");
            return;
        }
        
        // Swap players (current becomes next for the next transition)
        AudioStreamPlayer temp = _currentPlayer;
        _currentPlayer = _nextPlayer;
        _nextPlayer = temp;
        
        // Stop any existing playback on the next player
        _nextPlayer.Stop();
        
        // Set up the next player
        _nextPlayer.Stream = music;
        
        // Start silent if fading in
        if (fadeIn)
        {
            _nextPlayer.VolumeDb = -80.0f;
        }
        else
        {
            _nextPlayer.VolumeDb = LinearToDb(_musicVolume * _masterVolume);
        }
        
        // Start playback
        _nextPlayer.Play();
        
        // Set up fading if needed
        if (fadeIn)
        {
            _isFading = true;
            _fadeTimer = 0.0f;
            _targetVolume = _musicVolume * _masterVolume;
        }
    }
    
    /// <summary>
    /// Play the next track in the playlist
    /// </summary>
    public void PlayNextTrack(bool fadeIn = true)
    {
        if (_playlist.Count == 0)
        {
            return;
        }
        
        // Advance to the next track or wrap around
        if (_shufflePlaylist)
        {
            _currentTrackIndex = _random.Next(_playlist.Count);
        }
        else
        {
            _currentTrackIndex = (_currentTrackIndex + 1) % _playlist.Count;
        }
        
        // Play the selected track
        PlayMusic(_playlist[_currentTrackIndex], fadeIn);
    }
    
    /// <summary>
    /// Play the previous track in the playlist
    /// </summary>
    public void PlayPreviousTrack(bool fadeIn = true)
    {
        if (_playlist.Count == 0)
        {
            return;
        }
        
        // Go back to the previous track or wrap around
        if (_shufflePlaylist)
        {
            _currentTrackIndex = _random.Next(_playlist.Count);
        }
        else
        {
            _currentTrackIndex = (_currentTrackIndex - 1 + _playlist.Count) % _playlist.Count;
        }
        
        // Play the selected track
        PlayMusic(_playlist[_currentTrackIndex], fadeIn);
    }
    
    /// <summary>
    /// Stop the currently playing music with optional fade-out
    /// </summary>
    public void StopMusic(bool fadeOut = true)
    {
        if (fadeOut)
        {
            _isFading = true;
            _fadeTimer = 0.0f;
            _targetVolume = 0.0f;
        }
        else
        {
            _currentPlayer.Stop();
        }
    }
    
    /// <summary>
    /// Set the music volume (0.0 to 1.0)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
        
        // Update volume of current player if not fading
        if (!_isFading && _isMusicEnabled)
        {
            _currentPlayer.VolumeDb = LinearToDb(_musicVolume * _masterVolume);
        }
    }
    
    /// <summary>
    /// Get the current music volume (0.0 to 1.0)
    /// </summary>
    public float GetMusicVolume()
    {
        return _musicVolume;
    }
    
    /// <summary>
    /// Enable or disable music playback
    /// </summary>
    public void SetMusicEnabled(bool enabled)
    {
        _isMusicEnabled = enabled;
        
        if (!_isMusicEnabled)
        {
            // Immediately stop all music
            _musicPlayer1.Stop();
            _musicPlayer2.Stop();
        }
        else if (_currentPlayer.Stream != null)
        {
            // Resume playback if we have a current track
            _currentPlayer.Play();
            _currentPlayer.VolumeDb = LinearToDb(_musicVolume * _masterVolume);
        }
        else
        {
            // Start playing if we don't have a current track
            PlayNextTrack(false);
        }
    }
    
    /// <summary>
    /// Toggle shuffle mode for playlist
    /// </summary>
    public void SetShuffle(bool enabled)
    {
        _shufflePlaylist = enabled;
        
        if (_shufflePlaylist)
        {
            ShufflePlaylist();
        }
    }
    
    /// <summary>
    /// Set fade durations for transitions
    /// </summary>
    public void SetFadeDurations(float fadeIn, float fadeOut)
    {
        _fadeInDuration = Mathf.Max(0.1f, fadeIn);
        _fadeOutDuration = Mathf.Max(0.1f, fadeOut);
    }
    
    /// <summary>
    /// Shuffle the current playlist
    /// </summary>
    private void ShufflePlaylist()
    {
        // Fisher-Yates shuffle
        for (int i = _playlist.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            string temp = _playlist[i];
            _playlist[i] = _playlist[j];
            _playlist[j] = temp;
        }
    }
    
    /// <summary>
    /// Handle when music playback finishes
    /// </summary>
    private void OnMusicFinished()
    {
        // Automatically play next track when current one finishes
        if (_isMusicEnabled)
        {
            PlayNextTrack();
        }
    }
    
    /// <summary>
    /// Convert a linear volume (0.0 to 1.0) to decibels
    /// </summary>
    private float LinearToDb(float linear)
    {
        if (linear <= 0.0f)
        {
            return -80.0f; // Silent
        }
        
        return 20.0f * Mathf.Log(linear) / Mathf.Log(10.0f);
    }
}