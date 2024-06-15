using Godot;
using Godot.Collections;

public partial class SaveFileHandler : Node
{
  public void SaveGame()
  {
    using var saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);

    var sceneData = new Dictionary<string, Variant>() {
      {"SceneFilePath", GetTree().CurrentScene.SceneFilePath}
    };

    saveFile.StoreVar(sceneData);

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

    using var saveFile = FileAccess.Open(saveFilePath, FileAccess.ModeFlags.Read);

    Node loadingScene = null;
    var root = GetTree().Root;

    while (saveFile.GetPosition() < saveFile.GetLength())
    {
      var nodeData = (Godot.Collections.Dictionary<string, Variant>)saveFile.GetVar();

      if (loadingScene == null)
      {
        var sceneFile = GD.Load<PackedScene>(nodeData["SceneFilePath"].ToString());
        GD.Print(nodeData["SceneFilePath"].ToString());
        var currentScene = GetTree().CurrentScene;
        GD.Print(currentScene);
        root.RemoveChild(currentScene);
        loadingScene = sceneFile.Instantiate();
        /*
        var saveNodes = loadingScene.GetTree().GetNodesInGroup("Persist");
        foreach (Node saveNode in saveNodes) {
          GD.Print(saveNode.Name);
          saveNode.QueueFree();
        }
        */
      }
      else
      {
        var objectScene = GD.Load<PackedScene>(nodeData["SceneName"].ToString());
        var obj = objectScene.Instantiate<Node>();
        loadingScene.AddChild(obj);
        obj.Call("Load", nodeData);
      }
    }
    root.AddChild(loadingScene);
    GD.Print(GetTree().CurrentScene);
  }

  public bool HasSaveFile()
  {
    var saveFilePath = "user://savegame.save";
    return FileAccess.FileExists(saveFilePath);
  }
}
