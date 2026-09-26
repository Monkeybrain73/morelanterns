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
            tessellator.TesselateBlock(Block, out MeshData mesh);
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
