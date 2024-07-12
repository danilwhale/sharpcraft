using SharpCraft.Entities;
using SharpCraft.Rendering;
using SharpCraft.Tiles;
using SharpCraft.Utilities;
using SharpCraft.World.Rendering;

namespace SharpCraft.Gui;

public sealed class TilePreviewElement(Player player) : Element
{
    public override void Update()
    {
        if (System == null) return;

        Position.X = System.ViewWidth - 16;
        Position.Y = 16;
    }

    public override void Draw()
    {
        Rlgl.Scalef(16.0f, 16.0f, 16.0f);

        Rlgl.Rotatef(30.0f, 1.0f, 0.0f, 0.0f);
        Rlgl.Rotatef(45.0f, 0.0f, 1.0f, 0.0f);

        Rlgl.Translatef(-1.5f, 0.5f, -0.5f);
        
        Rlgl.Scalef(-1.0f, -1.0f, 1.0f);

        Rlgl.Begin(DrawMode.Quads);
        Rlgl.SetTexture(Assets.GetTexture("terrain.png").Id);

        var tile = Registries.Tiles.Registry[player.CurrentTile];
        tile?.Build(RlglVertexBuilder.Instance, null, -2, 0, 0);

        Rlgl.SetTexture(Rlgl.GetTextureIdDefault());
        Rlgl.End();
    }
}