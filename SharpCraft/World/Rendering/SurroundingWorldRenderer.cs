using SharpCraft.Rendering;
using SharpCraft.Utilities;

namespace SharpCraft.World.Rendering;

public sealed class SurroundingWorldRenderer : IDisposable
{
    static SurroundingWorldRenderer()
    {
        Assets.SetMaterialShader("rock.png", WorldShader.Shader);
        Assets.SetMaterialShader("water.png", WorldShader.Shader);
    }
    
    private readonly World _world;

    private readonly MeshBuilder _groundBuilder = new();
    private readonly MeshBuilder _waterBuilder = new();

    public SurroundingWorldRenderer(World world)
    {
        _world = world;

        CreateGroundMesh();
        CreateWaterMesh();
    }

    private void CreateGroundMesh()
    {
        const int groundLevel = 32 - 2;

        _groundBuilder.Begin(DrawMode.Quads);
        _groundBuilder.SetColor(255, 255, 255);


        // generate top planes
        for (var x = -640; x < _world.Width + 640; x += 128)
        {
            for (var z = -640; z < _world.Depth + 640; z += 128)
            {
                var y = groundLevel;
                if (x >= 0 && z >= 0 && x < _world.Width && z < _world.Depth)
                {
                    y = 0;
                }
                
                // @formatter:off
                _groundBuilder.AddVertexWithUv(x,          y, z + 128.0f, 0.0f,   128.0f);
                _groundBuilder.AddVertexWithUv(x + 128.0f, y, z + 128.0f, 128.0f, 128.0f);
                _groundBuilder.AddVertexWithUv(x + 128.0f, y, z,          128.0f, 0.0f);
                _groundBuilder.AddVertexWithUv(x,          y, z,          0.0f,   0.0f);
                // @formatter:on
            }
        }

        // generate planes on X axis
        for (var x = 0; x < _world.Width; x += 128)
        {
            // @formatter:off
            _groundBuilder.AddVertexWithUv(x,          0.0f,        0.0f, 0.0f,   0.0f);
            _groundBuilder.AddVertexWithUv(x + 128.0f, 0.0f,        0.0f, 128.0f, 0.0f);
            _groundBuilder.AddVertexWithUv(x + 128.0f, groundLevel, 0.0f, 128.0f, groundLevel);
            _groundBuilder.AddVertexWithUv(x,          groundLevel, 0.0f, 0.0f,   groundLevel);
            
            _groundBuilder.AddVertexWithUv(x,          groundLevel, _world.Depth, 0.0f,   groundLevel);
            _groundBuilder.AddVertexWithUv(x + 128.0f, groundLevel, _world.Depth, 128.0f, groundLevel);
            _groundBuilder.AddVertexWithUv(x + 128.0f, 0.0f,        _world.Depth, 128.0f, 0.0f);
            _groundBuilder.AddVertexWithUv(x,          0.0f,        _world.Depth, 0.0f,   0.0f);
            // @formatter:on
        }

        // generate planes on Z axis
        for (var z = 0; z < _world.Depth; z += 128)
        {
            // @formatter:off
            _groundBuilder.AddVertexWithUv(0.0f, groundLevel, z,          0.0f,   0.0f);
            _groundBuilder.AddVertexWithUv(0.0f, groundLevel, z + 128.0f, 128.0f, 0.0f);
            _groundBuilder.AddVertexWithUv(0.0f, 0.0f,        z + 128.0f, 128.0f, groundLevel);
            _groundBuilder.AddVertexWithUv(0.0f, 0.0f,        z,          0.0f,   groundLevel);
            
            _groundBuilder.AddVertexWithUv(_world.Width, 0.0f,        z,          0.0f,   groundLevel);
            _groundBuilder.AddVertexWithUv(_world.Width, 0.0f,        z + 128.0f, 128.0f, groundLevel);
            _groundBuilder.AddVertexWithUv(_world.Width, groundLevel, z + 128.0f, 128.0f, 0.0f);
            _groundBuilder.AddVertexWithUv(_world.Width, groundLevel, z,          0.0f,   0.0f);
            // @formatter:on
        }

        _groundBuilder.End();
    }

    private void CreateWaterMesh()
    {
        _waterBuilder.Begin(DrawMode.Quads);
        _waterBuilder.SetColor(255, 255, 255);

        const int waterLevel = 32;
        const float y = waterLevel - 0.1f;

        for (var x = -640; x < _world.Width + 640; x += 128)
        {
            for (var z = -640; z < _world.Depth + 640; z += 128)
            {
                if (!(x < 0 || z < 0 || x >= _world.Width || z >= _world.Depth))
                {
                    continue;
                }
                
                // @formatter:off
                _waterBuilder.AddVertexWithUv(x,       y, z + 128, 0.0f,   128.0f);
                _waterBuilder.AddVertexWithUv(x + 128, y, z + 128, 128.0f, 128.0f);
                _waterBuilder.AddVertexWithUv(x + 128, y, z,       128.0f, 0.0f);
                _waterBuilder.AddVertexWithUv(x,       y, z,       0.0f,   0.0f);
                
                _waterBuilder.AddVertexWithUv(x,       y, z,       0.0f,   0.0f);
                _waterBuilder.AddVertexWithUv(x + 128, y, z,       128.0f, 0.0f);
                _waterBuilder.AddVertexWithUv(x + 128, y, z + 128, 128.0f, 128.0f);
                _waterBuilder.AddVertexWithUv(x,       y, z + 128, 0.0f,   128.0f);
                // @formatter:on
            }
        }

        _waterBuilder.End();
    }

    public void DrawGround()
    {
        _groundBuilder.Mesh.Draw(Assets.GetTextureMaterial("rock.png"));
    }

    public void DrawWater()
    {
        _waterBuilder.Mesh.Draw(Assets.GetTextureMaterial("water.png"));
    }

    public void Dispose()
    {
        _groundBuilder.Dispose();
        _waterBuilder.Dispose();
    }
}