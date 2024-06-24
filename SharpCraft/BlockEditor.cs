using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Framework;
using SharpCraft.Gui.Elements;
using SharpCraft.Level.Blocks;
using SharpCraft.Utilities;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace SharpCraft;

public class BlockEditor(Level.Level level, float maxHitDistance)
{
    private static readonly int LastBlockId = BlockRegistry.GetExistingBlockCount();

    public BlockSelectionElement SelectionElement;
    public byte CurrentBlock = 1;
    
    // private RayCollision _rayCast;

    public void Update(PlayerEntity playerEntity)
    {
        var mouseScroll = Input.ScrollDelta;

        if (mouseScroll > 0)
        {
            CurrentBlock = (byte)(CurrentBlock - 1 < 1 ? LastBlockId : CurrentBlock - 1);
        }
        else if (mouseScroll < 0)
        {
            CurrentBlock = (byte)(CurrentBlock + 1 > LastBlockId ? 1 : CurrentBlock + 1);
        }
        
        SelectionElement.CurrentBlock = CurrentBlock;
        //
        // _rayCast = level.DoRayCast(
        //     GetMouseRay(new Vector2(GetScreenWidth(), GetScreenHeight()) / 2, playerEntity.Camera),
        //     maxHitDistance);
        
        HandleInput();
    }

    private void HandleInput()
    {
        // if (Input.IsMouseButtonPressed(MouseButton.Left) && _rayCast.Hit)
        // {
        //     var hitPoint = _rayCast.Point + _rayCast.Normal / 2;
        //
        //     level.SetBlock(hitPoint, CurrentBlock);
        // }
        //
        // if (Input.IsMouseButtonPressed(MouseButton.Right) && _rayCast.Hit)
        // {
        //     var hitPoint = _rayCast.Point - _rayCast.Normal / 2;
        //
        //     level.SetBlock(hitPoint, 0);
        // }
    }

    public void Draw()
    {
        // if (!_rayCast.Hit) return;
        //
        // var position = (BlockPosition)(_rayCast.Point - _rayCast.Normal / 2.0f);
        //
        // var id = level.GetBlock(position);
        // var block = BlockRegistry.Blocks.GetUnsafeRef(id);
        // if (block == null) return;
        //
        // Program.GL.Disable(EnableCap.DepthTest);
        //
        // var collision = block.GetCollision(position.X, position.Y, position.Z);
        // var size = collision.Max - collision.Min;
        // DrawCubeWiresV(collision.Min + size / 2, size, Color.Black);
        //
        // Program.GL.Enable(EnableCap.DepthTest);
    }
}