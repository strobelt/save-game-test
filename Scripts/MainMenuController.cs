using Godot;

public partial class MainMenuController : Control
{
  public override void _Ready() {
    var startButton = (Button) GetNode("StartGameButton");
    startButton.Pressed += StartGamePressed;

    var loadButton = (Button) GetNode("LoadGameButton");
    loadButton.Pressed += LoadGamePressed;
  }

  private void StartGamePressed() {
    GetTree().ChangeSceneToFile("res://Scenes/Level.tscn");
  }

  private void LoadGamePressed() {
    GetTree().ChangeSceneToFile("res://Scenes/Level.tscn");
  }
}
