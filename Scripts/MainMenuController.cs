using Godot;

public partial class MainMenuController : Control
{
  public override void _Ready() {
    var startButton = (Button) GetNode("StartButton");
    startButton.Pressed += StartPressed;
  }

  private void StartPressed() {
    GetTree().ChangeSceneToFile("res://Scenes/Level.tscn");
  }
}
