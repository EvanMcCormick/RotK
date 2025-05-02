using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		GD.Print("C# Main menu initialized");
		
		// Debug: Verify button existence
		VerifyButtonsExist();
		
		// Ensure panels are hidden by default
		GetNode<PopupPanel>("RulesPanel").Hide();
		GetNode<PopupPanel>("CreditsPanel").Hide();
		
		// Start background music through the MusicManager
		StartBackgroundMusic();
		
		// Make sure the menu is ready to receive input
		SetupInputHandling();
	}

	// Set up proper input handling for the menu
	private void SetupInputHandling()
	{
		// Make this control process input
		ProcessMode = ProcessModeEnum.Always;
		
		// Set mouse filter to pass through
		MouseFilter = MouseFilterEnum.Pass;
		
		// Make sure this control can get focus
		FocusMode = FocusModeEnum.All;
		
		 // Set focus neighbors for buttons
		Button startButton = GetNode<Button>("ButtonsContainer/StartGameButton");
		Button rulesButton = GetNode<Button>("ButtonsContainer/RulesButton");
		Button creditsButton = GetNode<Button>("ButtonsContainer/CreditsButton");
		Button quitButton = GetNode<Button>("ButtonsContainer/QuitButton");
		
		if (startButton != null && rulesButton != null)
		{
			startButton.FocusNeighborBottom = rulesButton.GetPath();
			rulesButton.FocusNeighborTop = startButton.GetPath();
		}
		
		if (rulesButton != null && creditsButton != null)
		{
			rulesButton.FocusNeighborBottom = creditsButton.GetPath();
			creditsButton.FocusNeighborTop = rulesButton.GetPath();
		}
		
		if (creditsButton != null && quitButton != null)
		{
			creditsButton.FocusNeighborBottom = quitButton.GetPath();
			quitButton.FocusNeighborTop = creditsButton.GetPath();
		}
		
		 // Make sure no button has focus initially
		if (startButton != null)
		{
			startButton.ReleaseFocus();
			
			// Remove automatic focus highlighting from buttons
			startButton.FocusMode = FocusModeEnum.Click;
			if (rulesButton != null) rulesButton.FocusMode = FocusModeEnum.Click;
			if (creditsButton != null) creditsButton.FocusMode = FocusModeEnum.Click;  
			if (quitButton != null) quitButton.FocusMode = FocusModeEnum.Click;
		}
	}
	
	// Override _Input to debug input events
	public override void _Input(InputEvent @event)
	{
		// Only log mouse button events to avoid console spam
		if (@event is InputEventMouseButton mouseEvent)
		{
			GD.Print($"Mouse button event: Button={mouseEvent.ButtonIndex}, Pressed={mouseEvent.Pressed}, Position={mouseEvent.Position}");
			
			// Try manual button handling
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				CheckButtonClicks(mouseEvent.Position);
			}
		}
	}
	
	// Manually check if any buttons were clicked
	private void CheckButtonClicks(Vector2 clickPosition)
	{
		// Check main menu buttons
		TryClickButton("ButtonsContainer/StartGameButton", clickPosition, OnStartGameButtonPressed);
		TryClickButton("ButtonsContainer/RulesButton", clickPosition, OnRulesButtonPressed);
		TryClickButton("ButtonsContainer/CreditsButton", clickPosition, OnCreditsButtonPressed);
		TryClickButton("ButtonsContainer/QuitButton", clickPosition, OnQuitButtonPressed);
		
		// Check back buttons on panels
		TryClickButton("RulesPanel/MarginContainer/VBoxContainer/BackButton", clickPosition, OnBackButtonPressed);
		TryClickButton("CreditsPanel/MarginContainer/VBoxContainer/BackButton", clickPosition, OnBackButtonPressed);
	}
	
	// Try to manually handle a button click
	private void TryClickButton(string path, Vector2 clickPosition, Action clickAction)
	{
		Button button = GetNodeOrNull<Button>(path);
		if (button != null)
		{
			Rect2 rect = button.GetGlobalRect();
			bool clicked = rect.HasPoint(clickPosition);
			
			if (clicked)
			{
				GD.Print($"Manual click detected on {path}");
				clickAction?.Invoke();
			}
		}
	}
	
	// Debug method to verify buttons exist and have proper signal connections
	private void VerifyButtonsExist()
	{
		// Check each button and log its existence
		Button startButton = GetNode<Button>("ButtonsContainer/StartGameButton");
		Button rulesButton = GetNode<Button>("ButtonsContainer/RulesButton");
		Button creditsButton = GetNode<Button>("ButtonsContainer/CreditsButton");
		Button quitButton = GetNode<Button>("ButtonsContainer/QuitButton");
		
		GD.Print($"Start Button found: {startButton != null}");
		GD.Print($"Rules Button found: {rulesButton != null}");
		GD.Print($"Credits Button found: {creditsButton != null}");
		GD.Print($"Quit Button found: {quitButton != null}");
		
		// Log if panel buttons exist too
		Button rulesBackButton = GetNode<Button>("RulesPanel/MarginContainer/VBoxContainer/BackButton");
		Button creditsBackButton = GetNode<Button>("CreditsPanel/MarginContainer/VBoxContainer/BackButton");
		
		GD.Print($"Rules Back Button found: {rulesBackButton != null}");
		GD.Print($"Credits Back Button found: {creditsBackButton != null}");
		
		// Force-enable mouse and input on all buttons
		SetupButtonInput(startButton);
		SetupButtonInput(rulesButton);
		SetupButtonInput(creditsButton);
		SetupButtonInput(quitButton);
		SetupButtonInput(rulesBackButton);
		SetupButtonInput(creditsBackButton);
	}
	
	// Setup input properties for a button to ensure it can receive input
	private void SetupButtonInput(Button button)
	{
		if (button == null) return;
		
		// Make sure button can get focus and process input, but only on click
		button.FocusMode = FocusModeEnum.Click;
		button.MouseFilter = MouseFilterEnum.Stop;
		button.ProcessMode = ProcessModeEnum.Always;
		
		// Force enable input
		button.SetMeta("_edit_use_anchors_", true);
		
		// Add direct press handling
		button.ButtonDown += () => GD.Print($"{button.Name} pressed down directly");
		button.ButtonUp += () => GD.Print($"{button.Name} released directly");
	}
	
	// Start the background music using MusicManager singleton
	private void StartBackgroundMusic()
	{
		if (MusicManager.Instance != null)
		{
			// Start the first track in the playlist
			GD.Print("Attempting to play music via MusicManager");
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
