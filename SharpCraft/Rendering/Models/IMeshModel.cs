namespace SharpCraft.Rendering.Models;

public interface IMeshModel
{
    void CopyTo(ref Mesh mesh, int vertexOffset);
}