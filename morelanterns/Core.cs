using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;



[assembly:
    ModInfo("apelanterns",
            Authors = new string[] { "xXx_Ape_xXx" },
            Description = "Adds a variety of decorative and useful lanterns",
            Version = "1.4.0")
]


namespace apelanterns
{

    public sealed class Core : ModSystem
    {

        private ICoreAPI api;

        public static IServerNetworkChannel serverChannel;
        public static IClientNetworkChannel clientChannel;



        public override void Start(ICoreAPI api)
        {
            RegisterClasses(api);
            base.Start(api);
            api.World.Logger.Event("started 'More Lanterns' mod");
            api.Network.RegisterChannel("morelanterns");

        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            this.api = api;
            base.StartClientSide(api);
            clientChannel = api.Network.GetChannel("morelanterns");
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            this.api = api;
            base.StartServerSide(api);
            serverChannel = api.Network.GetChannel("morelanterns");
        }


        private static void RegisterClasses(ICoreAPI api)
        {
            api.RegisterBlockClass("MoreLanterns.BlockFloatingLantern", typeof(BlockFloatingLantern));
            api.RegisterBlockClass("MoreLanterns.BlockDimmable", typeof(BlockDimmable));

            api.RegisterBlockEntityClass("MoreLanterns.BeFloatingLantern", typeof(BEFloatingLantern));

            api.RegisterBlockBehaviorClass("MoreLanterns.BlockName", typeof(BlockBehaviorName));
            api.RegisterBlockBehaviorClass("MoreLanterns.BlockDesc", typeof(BlockBehaviorBlockDescription));
        }

        public override void Dispose()
        {
            base.Dispose();
            serverChannel = null;
            clientChannel = null;
        }
    }
}
