using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
//using System.Diagnostics;


namespace apelanterns
{

    public class BEFloatingLantern : BlockEntityDisplayCase, ITexPositionSource
    {
        public new ICoreClientAPI capi;
        public ICoreServerAPI sapi;

        private bool playerIsWalking = false;

        public BEFloatingLantern() 
        {
            
        }

        //attributes
        private string renderSides = "nsew";
        private string sideShape = "";
        private string baseShape = "";
        private string currentConnections = ""; //i.e. "" for none, or  "nsew" for all
        
        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            this.capi = api as ICoreClientAPI;
     
            if (this.Block.Attributes != null)
            {
                if (this.Block.Attributes["sideShape"].Exists)
                { this.sideShape = this.Block.Attributes["sideShape"].AsString(); }

                if (this.Block.Attributes["baseShape"].Exists)
                { this.baseShape = this.Block.Attributes["baseShape"].AsString(); }

                if (this.Block.Attributes["renderSides"].Exists)
                { this.renderSides = this.Block.Attributes["renderSides"].AsString(); }
            }
        }


        public override void OnBlockPlaced(ItemStack byItemStack = null)
        {
            this.currentConnections = "";
            base.OnBlockPlaced(byItemStack);
        }


        internal string GetConnections()
        {
            return this.currentConnections;
        }

        
        internal void PlayerIsWalking(bool walking)
        {
            this.playerIsWalking = walking;
        }
        

        internal bool AddConnection(BlockPos pos, BlockFacing facing)
        {
            var face = char.ToLower(facing.ToString()[0]);
            if (!this.currentConnections.Contains(face))
            {
                this.currentConnections += face;
                return true;
            }
            return false;
        }


        internal bool RemoveConnection(BlockPos pos, BlockFacing facing)
        {
            var face = char.ToLower(facing.ToString()[0]).ToString();
            if (this.currentConnections.Contains(face))
            {
                this.currentConnections = this.currentConnections.Replace(face, string.Empty);
                return true;
            }
            return false;
        }


        public virtual MeshData GenBaseMesh(ICoreClientAPI capi, string shapePath, ITexPositionSource texture)
        {
            Shape shape;
            var tesselator = capi.Tesselator;
            shape = capi.Assets.TryGet(shapePath + ".json").ToObject<Shape>();
            tesselator.TesselateShape(shapePath, shape, out var mesh, texture, null, 0);

            var thisBlock = Api.World.BlockAccessor.GetBlock(this.Pos, BlockLayersAccess.Default);
            if ( thisBlock.LastCodePart() == "we")
            {
                mesh.Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0, 90 * GameMath.DEG2RAD, 0);
            }

            float radY = GameMath.MurmurHash3Mod(Pos.X, Pos.Y, Pos.Z, 16)* 22.5f * GameMath.DEG2RAD;    // random  rotate
            mesh = mesh.Clone().Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0, radY, 0);


            return mesh;
        }


        public virtual MeshData GenMesh(ICoreClientAPI capi, string shapePath, ITexPositionSource texture, char side)
        {
            Shape shape;
            var tesselator = capi.Tesselator;
            shape = capi.Assets.TryGet(shapePath + ".json").ToObject<Shape>();
            tesselator.TesselateShape(shapePath, shape, out var mesh, texture, null, 0);
            if (mesh != null)
            {
                mesh.Translate(0f, 0f, 0f);
                   switch (side)
                    {
                        case 'e':
                        {
                            mesh.Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0, 90 * GameMath.DEG2RAD, 0);
                            mesh.Translate(new Vec3f(0.435f, 0.0018f, 0.0018f));
                            break;
                        }
                        case 'w':
                        {
                            mesh.Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0, 90 * GameMath.DEG2RAD, 0);
                            mesh.Translate(new Vec3f(-0.435f, 0.0015f, 0.0015f));
                            break;
                        }
                        case 'n':
                        {
                            mesh.Translate(new Vec3f(0.0010f, 0.0010f, -0.435f));
                            //mesh.Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 90 * GameMath.DEG2RAD, 0, 0);
                            break;
                        }
                        case 's':
                        {
                            mesh.Translate(new Vec3f(0.0013f, 0.0013f, 0.435f));
                            //mesh.Rotate(new Vec3f(0.5f, 0.5f, 0.5f), -90 * GameMath.DEG2RAD, 0, 0);
                            break;
                        }
                        default:
                        { break; }
                }
            }
            return mesh;
        }


        public virtual string BuildShapePath(string partialPath)
        {
            if (partialPath == "")
            { return ""; }
            var fullPath = "";
            if (!partialPath.Contains(":"))
            { fullPath = this.Block.Code.Domain + ":"; }
            if (!partialPath.Contains("shapes/"))
            { fullPath += "shapes/"; }
            fullPath += partialPath;
            return fullPath;
        }


        private bool IsFloating()
        {
            var belowBlock = Api.World.BlockAccessor.GetBlock(this.Pos.DownCopy(), BlockLayersAccess.Fluid);
            if (belowBlock != null)
            {
                if (belowBlock.Code.Path.Contains("water-"))
                { return true; }
            }
            return false;
        }


        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator)
        {
            MeshData mesh;
            var floating = IsFloating();
            var texture = tesselator.GetTextureSource(this.Block);
            var shapePath = this.BuildShapePath(this.baseShape); //this uses a single down facing side to render all sides
            mesh = this.GenBaseMesh(this.capi, shapePath, texture);
            if (mesh != null)
            {
                if (floating)
                {
                    var waterWave = EnumWindBitModeMask.Water;
                    for (var vertexNum = 0; vertexNum < mesh.GetVerticesCount(); vertexNum++)
                    {
                        mesh.Flags[vertexNum] |= waterWave;
                    }
                }
                mesher.AddMeshData(mesh); 
            }


            //Sides
            if (floating)
            {

                shapePath = this.BuildShapePath(this.sideShape); //this uses a single down facing side to render all sides
                foreach (var side in this.renderSides)
                {
                    mesh = null;
                    if (!this.currentConnections.Contains(side) && shapePath != "")
                    { mesh = this.GenMesh(this.capi, shapePath, texture, side); }
                    if (mesh != null)
                    {
                        var waterWave = EnumWindBitModeMask.Water;
                        for (var vertexNum = 0; vertexNum < mesh.GetVerticesCount(); vertexNum++)
                        {
                            mesh.Flags[vertexNum] |= waterWave;
                        }
                        mesher.AddMeshData(mesh);
                    }
                }
            }
            return true;
        }

        
        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            this.currentConnections = tree.GetString("currentConnections");
        }


        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetString("currentConnections", this.currentConnections);
        }
        
    }
}
