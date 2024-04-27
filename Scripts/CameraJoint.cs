using Godot;

public partial class CameraJoint : SpringArm3D
{
  [Export]
  public float MouseSensitivity = 0.05f;

  public override void _Ready()
  {
    TopLevel = true;
  }

  public override void _Process(double delta)
  {
    Input.MouseMode = Input.MouseModeEnum.Captured;
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventMouseMotion e)
    {
      var rotation = RotationDegrees;
      rotation.X -= e.Relative.Y * MouseSensitivity;
      rotation.X = Mathf.Clamp(rotation.X, -90.0f, 30.0f);

      rotation.Y -= e.Relative.X * MouseSensitivity;
      rotation.Y = Mathf.Wrap(rotation.Y, 0.0f, 360.0f);

      RotationDegrees = rotation;
    }
  }
}
