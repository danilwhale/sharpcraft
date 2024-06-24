using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.Maths;

namespace SharpCraft.Framework;

[StructLayout(LayoutKind.Sequential)]
public sealed unsafe class MatrixStack
{
    public static readonly MatrixStack Shared = new();
    
    public ref Matrix4x4 Projection => ref _projection;
    public ref Matrix4x4 ModelView => ref _modelView;
    public ref Matrix4x4 Current => ref *_currentMatrix;
    
    private Matrix4x4 _projection = Matrix4x4.Identity;
    private Matrix4x4 _modelView = Matrix4x4.Identity;
    private Matrix4x4* _currentMatrix;
    
    private readonly Stack<Matrix4x4> _matrices = new();

    public MatrixStack()
    {
        SetMode(MatrixMode.Projection);
    }

    public void SetMode(MatrixMode mode)
    {
        switch (mode)
        {
            case MatrixMode.Projection:
                fixed (Matrix4x4* pProjection = &_projection)
                {
                    _currentMatrix = pProjection;
                }
                break;
            
            case MatrixMode.ModelView:
                fixed (Matrix4x4* pModelView = &_modelView)
                {
                    _currentMatrix = pModelView;
                }
                break;
        }
    }

    public void Push()
    {
        _matrices.Push(*_currentMatrix);
    }

    public void MultMatrix(Matrix4x4 matrix)
    {
        *_currentMatrix *= matrix;
    }

    public void LoadIdentity()
    {
        *_currentMatrix = Matrix4x4.Identity;
    }

    public void Translate(float x, float y, float z)
    {
        *_currentMatrix *= Matrix4x4.CreateTranslation(x, y, z);
    }

    public void Translate(Vector3 translation)
    {
        *_currentMatrix *= Matrix4x4.CreateTranslation(translation);
    }

    public void Rotate(float angle, float x, float y, float z)
    {
        *_currentMatrix *= Matrix4x4.CreateFromAxisAngle(new Vector3(x, y, z), angle);
    }

    public void Rotate(float angle, Vector3 axis)
    {
        *_currentMatrix *= Matrix4x4.CreateFromAxisAngle(axis, angle);
    }

    public void Rotate(Quaternion rotation)
    {
        *_currentMatrix *= Matrix4x4.CreateFromQuaternion(rotation);
    }

    public void Scale(float x, float y, float z)
    {
        *_currentMatrix *= Matrix4x4.CreateScale(x, y, z);
    }

    public void Scale(Vector3 scale)
    {
        *_currentMatrix *= Matrix4x4.CreateScale(scale);
    }

    public void Pop()
    {
        *_currentMatrix = _matrices.Pop();
    }
}