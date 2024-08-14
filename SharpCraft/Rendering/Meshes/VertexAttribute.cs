namespace SharpCraft.Rendering.Meshes;

public readonly struct VertexAttribute(
    VertexAttributeType type,
    VertexAttributeLocation location,
    int size,
    bool normalize,
    nint offsetBytes
)
{
    public readonly VertexAttributeType Type = type;
    public readonly VertexAttributeLocation Location = location;
    public readonly int Size = size;
    public readonly bool Normalize = normalize;
    public readonly nint OffsetBytes = offsetBytes;
}