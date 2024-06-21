namespace SharpCraft.Level;

public interface ILevelSerializable
{
    void Read(Stream stream);
    void Write(Stream stream);
}