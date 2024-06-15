using Godot;

public partial class PauseMenuController : Control
{
  Input.MouseModeEnum previousMouseMode;

  private SaveFileHandler saveFileHandler;
  private Button loadGameButton;

  public override void _Ready()
  {
    Hide();
    ProcessMode = Node.ProcessModeEnum.Always;

    saveFileHandler = (SaveFileHandler)GetNode("SaveFileHandler");

    var saveGameButton = (Button)GetNode("Buttons/SaveGameButton");
    saveGameButton.Pressed += SaveGamePressed;

    loadGameButton = (Button)GetNode("Buttons/LoadGameButton");
    loadGameButton.Pressed += LoadGamePressed;

    var exitToMenuButton = (Button)GetNode("Buttons/ExitToMenuButton");
    exitToMenuButton.Pressed += ExitToMenuPressed;

    var returnToGameButton = (Button)GetNode("Buttons/ReturnToGameButton");
    returnToGameButton.Pressed += UnPause;
  }

  public override void _Process(double delta)
  {
    if (Input.IsActionJustReleased("pause"))
    {
      if (GetTree().Paused)
      {
        UnPause();
      }
      else
      {
        Input.MouseMode = previousMouseMode;
        GetTree().Paused = true;
        Show();
      }
    }

    loadGameButton.Disabled = !saveFileHandler.HasSaveFile();
  }

  private void UnPause()
  {
    previousMouseMode = Input.MouseMode;
    Hide();
    GetTree().Paused = false;
  }

  private void ExitToMenuPressed()
  {
    UnPause();
    GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
  }

  private void SaveGamePressed()
  {
    UnPause();
    saveFileHandler.SaveGame();
  }

  private void LoadGamePressed()
  {
    UnPause();
    saveFileHandler.LoadGame();
  }
}
