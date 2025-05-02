using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		GD.Print("C# Main menu initialized");
		
		// Ensure panels are hidden by default
		GetNode<PopupPanel>("RulesPanel").Hide();
		GetNode<PopupPanel>("CreditsPanel").Hide();
		
		// Connect button signals
		GetNode<Button>("ButtonsContainer/StartGameButton").Pressed += OnStartGameButtonPressed;
		GetNode<Button>("ButtonsContainer/RulesButton").Pressed += OnRulesButtonPressed;
		GetNode<Button>("ButtonsContainer/CreditsButton").Pressed += OnCreditsButtonPressed;
		GetNode<Button>("ButtonsContainer/QuitButton").Pressed += OnQuitButtonPressed;
		
		// Connect back buttons
		GetNode<Button>("RulesPanel/MarginContainer/VBoxContainer/BackButton").Pressed += OnBackButtonPressed;
		GetNode<Button>("CreditsPanel/MarginContainer/VBoxContainer/BackButton").Pressed += OnBackButtonPressed;
		
		// Start background music through the MusicManager
		StartBackgroundMusic();
	}
	
	// Start the background music using MusicManager singleton
	private void StartBackgroundMusic()
	{
		if (MusicManager.Instance != null)
		{
			// Start the first track in the playlist
			MusicManager.Instance.PlayNextTrack(false);
		}
		else
		{
			GD.PrintErr("MusicManager singleton not found! Make sure it's properly set up as an autoload.");
		}
	}

	// Start the game - loads the game board scene
	private void OnStartGameButtonPressed()
	{
		GD.Print("Starting new game...");
		// Game setup will go here, such as:
		// - Player selection (2-6 players)
		// - Setting up the game board
		// - Initialize cards (Activity, Scripture, Fruit of the Spirit, Crown, Star)
		// TODO: Replace with actual scene loading
		// GetTree().ChangeSceneToFile("res://Scenes/Game/GameBoard.tscn");
	}

	// Show the rules panel
	private void OnRulesButtonPressed()
	{
		GD.Print("Showing rules panel");
		PopupPanel rulesPanel = GetNode<PopupPanel>("RulesPanel");
		
		// Center the panel on the screen
		// Fix: Use explicit conversion from Vector2 to Vector2I
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		Vector2I screenSizeI = new Vector2I((int)screenSize.X, (int)screenSize.Y);
		
		rulesPanel.Position = new Vector2I(
			(screenSizeI.X - rulesPanel.Size.X) / 2,
			(screenSizeI.Y - rulesPanel.Size.Y) / 2
		);
		
		rulesPanel.Popup();
		GD.Print("Rules panel opened");
	}

	// Show the credits panel
	private void OnCreditsButtonPressed()
	{
		GD.Print("Showing credits panel");
		PopupPanel creditsPanel = GetNode<PopupPanel>("CreditsPanel");
		
		// Center the panel on the screen
		// Fix: Use explicit conversion from Vector2 to Vector2I
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		Vector2I screenSizeI = new Vector2I((int)screenSize.X, (int)screenSize.Y);
		
		creditsPanel.Position = new Vector2I(
			(screenSizeI.X - creditsPanel.Size.X) / 2,
			(screenSizeI.Y - creditsPanel.Size.Y) / 2
		);
		
		creditsPanel.Popup();
		GD.Print("Credits panel opened");
	}

	// Handle back button from rules or credits panel
	private void OnBackButtonPressed()
	{
		GD.Print("Returning to main menu");
		// Hide all panels
		GetNode<PopupPanel>("RulesPanel").Hide();
		GetNode<PopupPanel>("CreditsPanel").Hide();
	}

	// Quit the game
	private void OnQuitButtonPressed()
	{
		GD.Print("Quitting game");
		GetTree().Quit();
	}
}
