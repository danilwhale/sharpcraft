using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Rendering;

namespace SharpCraft.Level;

public sealed class DirtyChunkComparer : IComparer<Chunk>
{
    public Player Player = null!;
    public Frustum Frustum = null!;
    
    public int Compare(Chunk? x, Chunk? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        
        if (y is null) return 1;
        if (x is null) return -1;

        var xVisible = Frustum.IsCubeVisible(x.BBox);
        var yVisible = Frustum.IsCubeVisible(y.BBox);
        var visibleComparision = xVisible.CompareTo(yVisible);
        if (visibleComparision != 0) return visibleComparision;

        var xDistance = Vector3.Distance(x.Center, Player.Position);
        var yDistance = Vector3.Distance(y.Center, Player.Position);
        var distanceComparision = xDistance.CompareTo(yDistance);
        if (distanceComparision != 0) return distanceComparision;
        
        return x.DirtyTime.CompareTo(y.DirtyTime);
    }
}