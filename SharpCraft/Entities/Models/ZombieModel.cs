using System.Numerics;
using SharpCraft.Rendering.Parts;
using SharpCraft.Utilities;

namespace SharpCraft.Entities.Models;

public sealed class ZombieModel : IEntityModel, IDisposable
{
    private const float TextureWidth = 64.0f;
    private const float TextureHeight = 32.0f;

    private readonly ModelPart _head;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ModelPart _body;
    private readonly ModelPart _leftArm;
    private readonly ModelPart _rightArm;
    private readonly ModelPart _leftLeg;
    private readonly ModelPart _rightLeg;

    private readonly ModelPart[] _limbs;
    
    private readonly ZombieEntity _zombie;

    public ZombieModel(ZombieEntity zombie)
    {
        _zombie = zombie;
        
        _head = new ModelPart(0, 0, TextureWidth, TextureHeight);
        _head.AddBox(-4.0f, -8.0f, -4.0f, 8, 8, 8);

        _body = new ModelPart(16, 16, TextureWidth, TextureHeight);
        _body.AddBox(-4.0f, 0.0f, -2.0f, 8, 12, 4);

        _leftArm = new ModelPart(40, 16, TextureWidth, TextureHeight);
        _leftArm.AddBox(-3.0f, -2.0f, -2.0f, 4, 12, 4);
        _leftArm.Position = new Vector3(-5.0f, 2.0f, 0.0f);

        _rightArm = new ModelPart(40, 16, TextureWidth, TextureHeight);
        _rightArm.AddBox(-1.0f, -2.0f, -2.0f, 4, 12, 4);
        _rightArm.Position = new Vector3(5.0f, 2.0f, 0.0f);

        _leftLeg = new ModelPart(0, 16, TextureWidth, TextureHeight);
        _leftLeg.AddBox(-2.0f, 0.0f, -2.0f, 4, 12, 4);
        _leftLeg.Position = new Vector3(-2.0f, 12.0f, 0.0f);

        _rightLeg = new ModelPart(0, 16, TextureWidth, TextureHeight);
        _rightLeg.AddBox(-2.0f, 0.0f, -2.0f, 4, 12, 4);
        _rightLeg.Position = new Vector3(2.0f, 12.0f, 0.0f);

        _limbs =
        [
            _head,
            _body,
            _leftArm, _rightArm,
            _leftLeg, _rightLeg
        ];
    }

    public void Draw(float lastPartTicks)
    {
        Rlgl.PushMatrix();

        var position = _zombie.GetInterpolatedPosition(lastPartTicks);
        Rlgl.Translatef(position.X, position.Y, position.Z);

        // downscale model and invert it on Y axis to look correct
        const float size = 7.0f / 128.0f;
        Rlgl.Scalef(size, -size, size);

        var time = (float)(GetTime() * 10.0 + _zombie.TimeOffset);
        var yOffset = -MathF.Abs(MathF.Sin(time * 0.6662f)) * 5.0f - 23.0f;

        Rlgl.Translatef(0.0f, yOffset, 0.0f);
        
        Rlgl.Rotatef(_zombie.Rotation * RAD2DEG + 180.0f, 0.0f, 1.0f, 0.0f);

        UpdateLimbs(time);

        foreach (var limb in _limbs)
        {
            limb.Draw(Assets.GetTextureMaterial("char.png"));
        }

        Rlgl.PopMatrix();
    }

    private void UpdateLimbs(float time)
    {
        _head.Rotation = new Vector3(
            MathF.Sin(time) * 0.8f,
            MathF.Sin(time * 0.83f), 0.0f
        );

        _leftArm.Rotation = new Vector3(
            MathF.Sin(time * 0.6662f + MathF.PI) * 2.0f,
            0.0f,
            MathF.Sin(time * 0.2312f) + 1.0f
        );

        _rightArm.Rotation = new Vector3(
            MathF.Sin(time * 0.6662f) * 2.0f,
            0.0f,
            MathF.Sin(time * 0.2812f) - 1.0f
        );

        _leftLeg.Rotation = new Vector3(
            MathF.Sin(time * 0.6662f) * 1.4f,
            0.0f,
            0.0f
        );

        _rightLeg.Rotation = new Vector3(
            MathF.Sin(time * 0.6662f + MathF.PI) * 1.4f,
            0.0f,
            0.0f
        );
    }

    public void Dispose()
    {
        foreach (var limb in _limbs)
        {
            limb.Dispose();
        }
    }
}