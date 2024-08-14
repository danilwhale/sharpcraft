namespace SharpCraft.Rendering.Models;

public interface IMeshPart
{
    void CopyTo(ref Mesh mesh, int vertexOffset);
}