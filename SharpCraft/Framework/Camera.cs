using System.Numerics;
using Silk.NET.Maths;

namespace SharpCraft.Framework;

public struct Camera(Vector3 position, Vector3 target, Vector3 up, float fovY)
{
    public Vector3 Position = position;
    public Vector3 Target = target;
    public Vector3 Up = up;
    public float FovY = fovY;

    public void Begin(MatrixStack matrices)
    {
        matrices.SetMode(MatrixMode.Projection);

        matrices.Push();
        matrices.LoadIdentity();
        matrices.MultMatrix(Matrix4x4.CreatePerspectiveFieldOfView(
            float.DegreesToRadians(FovY),
            Program.MainWindow.Size.X / (float)Program.MainWindow.Size.Y,
            0.01f,
            1000.0f
        ));
        
        matrices.SetMode(MatrixMode.ModelView);
        
        matrices.LoadIdentity();
        matrices.MultMatrix(Matrix4x4.CreateLookAt(Position, Target, Up));
    }

    public static void End(MatrixStack matrices)
    {
        matrices.SetMode(MatrixMode.Projection);
        
        matrices.Pop();
        
        matrices.SetMode(MatrixMode.ModelView);
        
        matrices.LoadIdentity();
    }
}