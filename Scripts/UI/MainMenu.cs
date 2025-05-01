using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		GD.Print("C# Main menu initialized");
		
		// Ensure panels are hidden by default
		GetNode<Panel>("RulesPanel").Visible = false;
		GetNode<Panel>("CreditsPanel").Visible = false;
		
		// Connect button signals
		GetNode<Button>("ButtonsContainer/StartGameButton").Pressed += OnStartGameButtonPressed;
		GetNode<Button>("ButtonsContainer/RulesButton").Pressed += OnRulesButtonPressed;
		GetNode<Button>("ButtonsContainer/CreditsButton").Pressed += OnCreditsButtonPressed;
		GetNode<Button>("ButtonsContainer/QuitButton").Pressed += OnQuitButtonPressed;
		
		// Connect back buttons
		GetNode<Button>("RulesPanel/MarginContainer/VBoxContainer/BackButton").Pressed += OnBackButtonPressed;
		GetNode<Button>("CreditsPanel/MarginContainer/VBoxContainer/BackButton").Pressed += OnBackButtonPressed;
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
		GetNode<Panel>("RulesPanel").Visible = true;
	}

	// Show the credits panel
	private void OnCreditsButtonPressed()
	{
		GD.Print("Showing credits panel");
		GetNode<Panel>("CreditsPanel").Visible = true;
	}

	// Handle back button from rules or credits panel
	private void OnBackButtonPressed()
	{
		GD.Print("Returning to main menu");
		// Hide all panels
		GetNode<Panel>("RulesPanel").Visible = false;
		GetNode<Panel>("CreditsPanel").Visible = false;
	}

	// Quit the game
	private void OnQuitButtonPressed()
	{
		GD.Print("Quitting game");
		GetTree().Quit();
	}
}
