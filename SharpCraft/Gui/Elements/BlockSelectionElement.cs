using System.Numerics;
using SharpCraft.Level.Blocks;
using SharpCraft.Rendering;
using SharpCraft.Utilities;

namespace SharpCraft.Gui.Elements;

public class BlockSelectionElement : Element
{
    private const int Size = 48;

    public byte CurrentBlock = 1;
    public Range PreviewRange = 1..BlockRegistry.GetExistingBlockCount();

    public override void Update()
    {
    }

    public override void Draw()
    {
        // var currentBlock = BlockRegistry.Blocks.GetUnsafeRef(CurrentBlock);
        //
        // DrawBlock(16, GetScreenHeight() - 112, 96, currentBlock!);
        //
        // for (var i = PreviewRange.Start.Value; i <= PreviewRange.End.Value; i++)
        // {
        //     var block = BlockRegistry.Blocks.GetUnsafeRef(i)!;
        //
        //     var x = 64 + i * (Size + 8);
        //     var y = GetScreenHeight() - 64;
        //
        //     DrawBlock(x, y, Size, block);
        //
        //     if (CurrentBlock == block.Id)
        //     {
        //         DrawRectangleLinesEx(new Rectangle(x - 4.0f, y - 4.0f, Size + 8.0f, Size + 8.0f), 4.0f, Color.SkyBlue);
        //     }
        // }
    }

//     private void DrawBlock(int x, int y, int size, Block block)
//     {
//         DrawRectangle(x, y, size, size, Color.White);
//         
//         Rlgl.MatrixMode(MatrixMode.Projection);
//         Rlgl.LoadIdentity();
//         Rlgl.Ortho(0.0, GetScreenWidth(), GetScreenHeight(), 0.0, 0.01, 1000.0);
//         
//         Rlgl.MatrixMode(MatrixMode.ModelView);
//         
//         Rlgl.PushMatrix();
//         Rlgl.LoadIdentity();
//         
//         Rlgl.Translatef(x + size * 0.6f, y + size * 0.9f, -200.0f);
//         Rlgl.Scalef(size * 0.5f, size * 0.5f, size * 0.5f);
//         Rlgl.Rotatef(30.0f, 1.0f, 0.0f, 0.0f);
//         Rlgl.Rotatef(30.0f, 0.0f, 1.0f, 0.0f);
//         
//         Rlgl.Scalef(-1.0f, -1.0f, 1.0f);
//         
//         Rlgl.Begin(DrawMode.Quads);
//         Rlgl.SetTexture(ResourceManager.GetTexture("terrain.png").Id);
//         
//         block.Build(RlglBuilder.Instance, 0, 0, 0);
//         
//         Rlgl.SetTexture(0);
//         Rlgl.End();
//         
//         Rlgl.PopMatrix();
// ц    }
}