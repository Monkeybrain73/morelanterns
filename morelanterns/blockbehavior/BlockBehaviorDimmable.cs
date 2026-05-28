using System.Linq;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;


namespace apelanterns
{

    public class BlockDimmable: Block
    {

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {

            string curType = world.BlockAccessor.GetBlock(blockSel.Position).Variant["rotator"];


            if (byPlayer.WorldData.EntityControls.CtrlKey)
            {
                if (curType == "l1") curType = "l2";
                else if (curType == "l2") curType = "l3";
                else if (curType == "l3") curType = "l4";
                else if (curType == "l4") curType = "l1";

                MLStaticUtils.SetBlockState(world, blockSel.Position, "rotator", curType);

                return true;
            }

            return base.OnBlockInteractStart(world, byPlayer, blockSel);
        }


        public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
        {
            return base.GetPlacedBlockInteractionHelp(world, selection, forPlayer).Append(new WorldInteraction[] {
                new WorldInteraction()
                {
                    ActionLangCode = "apelanterns:rightclickpickup-help",
                    MouseButton = EnumMouseButton.Right,
                    RequireFreeHand = true,
                },

                new WorldInteraction()
                {
                    ActionLangCode = "apelanterns:blockhelp-behavior-dimmable-ctrl",
                    MouseButton = EnumMouseButton.Right,
                    HotKeyCode = "ctrl",
                    RequireFreeHand = true
                }
            });
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

            dsc.AppendLine("\n" + Lang.Get("apelanterns:dimmable-help"));
        }

    }

    public class MLStaticUtils
    {

        public static void SetBlockState(IWorldAccessor world, BlockPos pos, string newState, string newValue)
        {
            Block curBlock = world.BlockAccessor.GetBlock(pos);
            Block newBlock = world.GetBlock(curBlock.CodeWithVariant(newState, newValue));
            world.BlockAccessor.ExchangeBlock(newBlock.BlockId, pos);

        }

    }

}
