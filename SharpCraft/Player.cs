using System.Numerics;

namespace SharpCraft;

public class Player
{
    private const float MouseSensitivity = 0.075f;
    private const float HalfWidth = 0.3f;
    private const float HalfHeight = 0.9f;

    private Vector3 _lastPosition;
    public Vector3 Position;
    private Vector3 _direction;
    private BoundingBox _bbox;

    public Camera3D Camera;

    private float _yaw;
    private float _pitch;
    private bool _isOnGround;

    private readonly Level _level;

    public Player(Level level)
    {
        _level = level;
        Camera = new Camera3D(Vector3.Zero, Vector3.Zero, Vector3.UnitY, 70.0f, CameraProjection.Perspective);
        MoveToRandom();
    }

    private void MoveToRandom()
    {
        var x = Random.Shared.NextSingle() * _level.Width;
        var y = _level.Height + 10;
        var z = Random.Shared.NextSingle() * _level.Length;
        MoveTo(new Vector3(x, y, z));
    }

    private void MoveTo(Vector3 newPosition)
    {
        Position = newPosition;
        _bbox = new BoundingBox(
            newPosition - new Vector3(HalfWidth, HalfHeight, HalfWidth),
            newPosition + new Vector3(HalfWidth, HalfHeight, HalfWidth)
        );
    }

    public void Rotate(float pitch, float yaw)
    {
        _pitch += pitch * MouseSensitivity;
        _pitch = Math.Clamp(_pitch, -89.0f, 89.0f);
        
        _yaw -= yaw * MouseSensitivity;
    }

    public void MoveCamera(float lastDelta)
    {
        Camera.Position = _lastPosition + (Position - _lastPosition) * lastDelta;

        var rotation = Matrix4x4.CreateFromYawPitchRoll(
            _yaw * DEG2RAD,
            _pitch * DEG2RAD,
            0.0f
        );

        var forward = Vector3.Transform(Vector3.UnitZ, rotation);

        Camera.Target = Camera.Position + forward;
    }

    public void Tick()
    {
        _lastPosition = Position;
        
        if (IsKeyDown(KeyboardKey.R)) MoveToRandom();

        var x = IsKeyDown(KeyboardKey.A) || IsKeyDown(KeyboardKey.Left) ? 1
            : IsKeyDown(KeyboardKey.D) || IsKeyDown(KeyboardKey.Right) ? -1
            : 0;

        var z = IsKeyDown(KeyboardKey.W) || IsKeyDown(KeyboardKey.Up) ? 1
            : IsKeyDown(KeyboardKey.S) || IsKeyDown(KeyboardKey.Down) ? -1
            : 0;

        if (_isOnGround && (IsKeyDown(KeyboardKey.Space) || IsKeyDown(KeyboardKey.LeftSuper)))
        {
            _direction.Y = 0.12f;
        }

        MoveRelative(x, z, _isOnGround ? 0.02f : 0.005f);
        _direction.Y -= 0.005f;
        Move(_direction.X, _direction.Y, _direction.Z);
        
        _direction *= new Vector3(0.91f, 0.98f, 0.91f);

        if (_isOnGround) _direction *= new Vector3(0.8f, 1.0f, 0.8f);
    }

    private void Move(float x, float y, float z)
    {
        var oldX = x;
        var oldY = y;
        var oldZ = z;

        var boxes = _level.GetBoxes(_bbox.Expand(x, y, z));
        
        foreach (var box in boxes)
        {
            x = box.ClipXCollide(_bbox, x);
            y = box.ClipYCollide(_bbox, y);
            z = box.ClipZCollide(_bbox, z);
        }

        _bbox.Move(x, y, z);

        // ReSharper disable CompareOfFloatsByEqualityOperator
        _isOnGround = oldY != y && oldY < 0.0f;

        _direction.X = oldX != x ? 0.0f : _direction.X;
        _direction.Y = oldY != y ? 0.0f : _direction.Y;
        _direction.Z = oldZ != z ? 0.0f : _direction.Z;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        Position = new Vector3(
            (_bbox.Min.X + _bbox.Max.X) / 2.0f,
            _bbox.Min.Y + 1.62f,
            (_bbox.Min.Z + _bbox.Max.Z) / 2.0f
        );
    }

    private void MoveRelative(float x, float z, float speed)
    {
        var dist = x * x + z * z;

        if (dist < 0.01f) return;

        dist = speed / MathF.Sqrt(dist);

        x *= dist;
        z *= dist;

        var sin = MathF.Sin(_yaw * DEG2RAD);
        var cos = MathF.Cos(_yaw * DEG2RAD);

        _direction.X += x * cos + z * sin;
        _direction.Z += z * cos - x * sin;
    }
}