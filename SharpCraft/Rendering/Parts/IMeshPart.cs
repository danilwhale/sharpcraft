namespace SharpCraft.Rendering.Parts;

public interface IMeshPart
{
    void CopyTo(ref Mesh mesh, int vertexOffset);
}