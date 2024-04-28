using Godot;
using Godot.Collections;

public partial class SaveFileHandler : Node
{
  public void SaveGame()
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

  public void LoadGame()
  {
    var saveFilePath = "user://savegame.save";
    if (!FileAccess.FileExists(saveFilePath)) return;

    var saveNodes = GetTree().GetNodesInGroup("Persist");
    foreach (Node saveNode in saveNodes) saveNode.QueueFree();

    using var saveFile = FileAccess.Open(saveFilePath, FileAccess.ModeFlags.Read);

    var nodeData = (Godot.Collections.Dictionary<string, Variant>)saveFile.GetVar();
    var objectScene = GD.Load<PackedScene>(nodeData["SceneName"].ToString());
    var obj = objectScene.Instantiate<Node>();
    GetNode(nodeData["Parent"].ToString()).AddChild(obj);
    obj.Call("Load", nodeData);
  }

  public bool HasSaveFile()
  {
    var saveFilePath = "user://savegame.save";
    return FileAccess.FileExists(saveFilePath);
  }
}
