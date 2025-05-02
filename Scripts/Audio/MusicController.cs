using Godot;
using System;

/// <summary>
/// UI controller for music playback - provides controls for volume, 
/// next/previous track, shuffle, etc.
/// </summary>
public partial class MusicController : Control
{
    // UI Control nodes
    private HSlider _volumeSlider;
    private Button _playPauseButton;
    private Button _nextButton;
    private Button _prevButton;
    private Button _shuffleButton;
    private Label _trackNameLabel;
    private PopupPanel _musicControlPanel;
    private Button _musicControlButton;
    
    // Keep track of state
    private bool _isExpanded = false;
    
    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        // Reference UI elements
        _volumeSlider = GetNode<HSlider>("MusicControlPanel/VBoxContainer/VolumeControl/VolumeSlider");
        _playPauseButton = GetNode<Button>("MusicControlPanel/VBoxContainer/ButtonsContainer/PlayPauseButton");
        _nextButton = GetNode<Button>("MusicControlPanel/VBoxContainer/ButtonsContainer/NextButton");
        _prevButton = GetNode<Button>("MusicControlPanel/VBoxContainer/ButtonsContainer/PrevButton");
        _shuffleButton = GetNode<Button>("MusicControlPanel/VBoxContainer/ButtonsContainer/ShuffleButton");
        _trackNameLabel = GetNode<Label>("MusicControlPanel/VBoxContainer/TrackName");
        _musicControlPanel = GetNode<PopupPanel>("MusicControlPanel");
        _musicControlButton = GetNode<Button>("MusicControlButton");
        
        // Connect signals
        _volumeSlider.ValueChanged += OnVolumeChanged;
        _playPauseButton.Pressed += OnPlayPausePressed;
        _nextButton.Pressed += OnNextTrackPressed;
        _prevButton.Pressed += OnPrevTrackPressed;
        _shuffleButton.Pressed += OnShufflePressed;
        _musicControlButton.Pressed += OnMusicControlButtonPressed;
        
        // Initialize the UI state
        InitializeUI();
    }
    
    // Initialize UI components based on MusicManager state
    private void InitializeUI()
    {
        if (MusicManager.Instance != null)
        {
            // Set initial volume slider position
            float volume = MusicManager.Instance.GetMusicVolume();
            _volumeSlider.Value = volume;
            
            // Set button texts/icons
            UpdatePlayPauseButtonText();
            
            // Set initial track name (if any)
            UpdateTrackNameLabel();
        }
    }
    
    // Update the play/pause button text based on current state
    private void UpdatePlayPauseButtonText()
    {
        if (MusicManager.Instance != null)
        {
            bool isPlaying = true; // Assuming the music manager is playing by default
            
            // Update button text
            _playPauseButton.Text = isPlaying ? "Pause" : "Play";
        }
    }
    
    // Update the track name label with the current track
    private void UpdateTrackNameLabel()
    {
        if (MusicManager.Instance != null)
        {
            // For now, just show a generic name since we don't have track titles stored
            _trackNameLabel.Text = "Kingdom Music";
        }
    }
    
    // Handle volume slider changes
    private void OnVolumeChanged(double value)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetMusicVolume((float)value);
        }
    }
    
    // Handle play/pause button press
    private void OnPlayPausePressed()
    {
        if (MusicManager.Instance != null)
        {
            // Toggle music enabled state
            bool isEnabled = !_playPauseButton.Text.Equals("Play");
            MusicManager.Instance.SetMusicEnabled(!isEnabled);
            
            // Update button text
            UpdatePlayPauseButtonText();
        }
    }
    
    // Handle next track button press
    private void OnNextTrackPressed()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayNextTrack();
            UpdateTrackNameLabel();
        }
    }
    
    // Handle previous track button press
    private void OnPrevTrackPressed()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayPreviousTrack();
            UpdateTrackNameLabel();
        }
    }
    
    // Handle shuffle button press
    private void OnShufflePressed()
    {
        if (MusicManager.Instance != null)
        {
            bool shuffleEnabled = _shuffleButton.Text.Contains("On");
            MusicManager.Instance.SetShuffle(!shuffleEnabled);
            
            // Update button text
            _shuffleButton.Text = shuffleEnabled ? "Shuffle: Off" : "Shuffle: On";
        }
    }
    
    // Toggle the music control panel
    private void OnMusicControlButtonPressed()
    {
        _isExpanded = !_isExpanded;
        
        if (_isExpanded)
        {
            // Position the popup near the button
            // Fixed: Using Vector2I instead of Vector2 for position
            Vector2 buttonPos = _musicControlButton.GlobalPosition;
            Vector2 panelSize = _musicControlPanel.Size;
            
            // Convert to Vector2I for the Position property
            Vector2I position = new Vector2I(
                (int)(buttonPos.X - panelSize.X + _musicControlButton.Size.X), 
                (int)(buttonPos.Y + _musicControlButton.Size.Y)
            );
            
            // Show the panel
            _musicControlPanel.Position = position;
            _musicControlPanel.Popup();
        }
        else
        {
            // Hide the panel
            _musicControlPanel.Hide();
        }
    }
}