using System;
using System.Text;
using Godot;
using Godot.Collections;

public partial class PauseMenuController : Control
{
  Input.MouseModeEnum previousMouseMode;

  public override void _Ready()
  {
    Hide();
    ProcessMode = Node.ProcessModeEnum.Always;

    var saveGameButton = (Button)GetNode("Buttons/SaveGameButton");
    saveGameButton.Pressed += SaveGamePressed;

    var loadGameButton = (Button)GetNode("Buttons/LoadGameButton");
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
    using var saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
    var saveNodes = GetTree().GetNodesInGroup("Persist");
    foreach (var node in saveNodes)
    {
      if (string.IsNullOrEmpty(node.SceneFilePath))
      {
        GD.Print($"persistent node '{node.Name}' is not an instanced scene, skipped");
        continue;
      }

      if (!node.HasMethod("Save"))
      {
        GD.Print($"persistent node '{node.Name}' is missing a Save() function, skipped");
        continue;
      }

      var nodeData = node.Call("Save").As<Dictionary<string, Variant>>();
      saveFile.StoreVar(nodeData);
    }
  }

  private void LoadGamePressed()
  {
    var saveFilePath = "user://savegame.save";
    if (!FileAccess.FileExists(saveFilePath)) return;

    var saveNodes = GetTree().GetNodesInGroup("Persist");
    foreach (Node saveNode in saveNodes) saveNode.QueueFree();

    using var saveFile = FileAccess.Open(saveFilePath, FileAccess.ModeFlags.Read);

    var nodeData = (Godot.Collections.Dictionary<string, Variant>) saveFile.GetVar();
    var objectScene = GD.Load<PackedScene>(nodeData["SceneName"].ToString());
    var obj = objectScene.Instantiate<Node>();
    GetNode(nodeData["Parent"].ToString()).AddChild(obj);
    obj.Call("Load", nodeData);
  }
}
