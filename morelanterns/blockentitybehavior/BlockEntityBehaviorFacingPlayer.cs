using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace apelanterns
{
    public class BEBehaviorPlayerFacing : BlockEntityBehavior
    {
        public float MeshAngle { get; private set; }

        public BEBehaviorPlayerFacing(BlockEntity blockEntity) : base(blockEntity)
        {
        }

        public void SetAngle(float angle)
        {
            MeshAngle = angle;
            Blockentity.MarkDirty(true);
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessellator)
        {
            int alternateCount = 0;

            if (Block.Textures != null && Block.Textures.TryGetValue("stone", out CompositeTexture stoneTexture))
            {
                alternateCount = stoneTexture.Alternates?.Length ?? 0;
            }

            int alternateNumber = 0;

            if (alternateCount > 0)
            {
                alternateNumber = 1 + GameMath.MurmurHash3Mod(Pos.X, Pos.Y, Pos.Z, alternateCount);
            }

            ITexPositionSource textureSource = tessellator.GetTextureSource(Block, alternateNumber);

            tessellator.TesselateShape("rotatable-lantern", Block.Code, Block.Shape, out MeshData mesh, textureSource);
            mesh.Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0, MeshAngle, 0);
            mesher.AddMeshData(mesh);

            return true;
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetFloat("playerFacingAngle", MeshAngle);
        }

        public override void FromTreeAttributes(
            ITreeAttribute tree,
            IWorldAccessor world)
        {
            base.FromTreeAttributes(tree, world);
            MeshAngle = tree.GetFloat("playerFacingAngle");
        }
    }
}