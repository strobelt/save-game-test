using Godot;
using Godot.Collections;

public partial class Player : CharacterBody3D
{
  [Export]
  public float Speed { get; set; } = 7.0f;

  [Export]
  public float JumpStrength { get; set; } = 20.0f;

  [Export]
  public float Gravity { get; set; } = 50.0f;

  private Vector3 _targetVelocity = Vector3.Down;

  private SpringArm3D springArm;
  private Node3D model;

  public override void _Ready()
  {
    springArm = GetNode<SpringArm3D>("SpringArm3D");
    model = GetNode<Node3D>("CharacterShape");
  }

  public override void _PhysicsProcess(double delta)
  {
    var direction = Vector3.Zero;

    if (Input.IsActionPressed("move_right"))
      direction.X += 1.0f;
    if (Input.IsActionPressed("move_left"))
      direction.X -= 1.0f;
    if (Input.IsActionPressed("move_back"))
      direction.Z += 1.0f;
    if (Input.IsActionPressed("move_forward"))
      direction.Z -= 1.0f;

    direction = direction.Rotated(Vector3.Up, springArm.Rotation.Y).Normalized();

    if (direction != Vector3.Zero)
      model.LookAt(Position - direction, Vector3.Up);

    _targetVelocity.X = direction.X * Speed;
    _targetVelocity.Z = direction.Z * Speed;

    if (IsOnFloor() && Input.IsActionPressed("jump"))
      _targetVelocity.Y += JumpStrength;

    if (!IsOnFloor())
      _targetVelocity.Y -= Gravity * (float)delta;

    Velocity = _targetVelocity;
    MoveAndSlide();
  }

  public override void _Process(double delta)
  {
    springArm.Position = Position;
  }

  public Dictionary<string, Variant> Save() => new() {
      {"SceneName", SceneFilePath},
      {"Parent", GetParent().GetPath()},
      {"PlayerTransform", Transform},
      {"SpringArmTransform", springArm.Transform},
      {"CameraTransform", springArm.GetNode<Node3D>("Camera3D").Transform}
  };

  public void Load(Dictionary<string, Variant> savedData)
  {
    Transform = (Transform3D)savedData["PlayerTransform"];
    springArm.Transform = (Transform3D)savedData["SpringArmTransform"];
    springArm.GetNode<Node3D>("Camera3D").Transform = (Transform3D)savedData["CameraTransform"];
    AddToGroup("Persist");
  }
}
