using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace apelanterns
{
    public class BlockRotatableLantern : Block
    {
        public override bool DoPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ItemStack byItemStack)
        {
            bool placed = base.DoPlaceBlock(world, byPlayer, blockSel, byItemStack);

            if (!placed) return false;

            BlockEntity be = world.BlockAccessor.GetBlockEntity(blockSel.Position);

            BEBehaviorPlayerFacing facing = be?.GetBehavior<BEBehaviorPlayerFacing>();

            if (facing != null)
            {
                double dx = byPlayer.Entity.Pos.X - (blockSel.Position.X + 0.5);
                double dz = byPlayer.Entity.Pos.Z - (blockSel.Position.Z + 0.5);

                float angle = (float)Math.Atan2(dx, dz);
                float rotationStep = GameMath.PIHALF / 4;

                facing.SetAngle((float)Math.Round(angle / rotationStep) * rotationStep);
            }

            return true;
        }


    }
}
