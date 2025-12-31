using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum BlockType { Air, Grass, Snow, Sand,
                        OakWood, AcaciaWood, SpruceWood, JungleWood, BirchWood,
                        DeepslateCoal, DeepslateIron, DeepslateCopper, DeepslateGold, DeepslateRedstone, DeepslateEmerald, DeepslateLapiz, DeepslateDiamond,
                        Coal, Iron, Copper, Gold, Redstone, Emerald, Lapiz, Diamond,
                        Stone, Diorite, Andesite, Granite, Deepslate, Terracotta,
                        Bedrock,
                        SpruceLeaf, OakLeaf, BirchLeaf, AcaciaLeaf, JungleLeaf,
                        Water,
                        Cactus
                      };

public enum TileType { Blank,
                       GrassSide, GrassTop, GrassBottom, SnowSide, SnowTop, SandSide,
                       OakWoodTop, OakWoodSide, AcaciaWoodTop, AcaciaWoodSide, SpruceWoodTop, SpruceWoodSide, JungleWoodTop, JungleWoodSide, BirchWoodTop, BirchWoodSide,
                       DeepslateCoalSide, DeepslateIronSide, DeepslateCopperSide, DeepslateGoldSide, DeepslateRedstoneSide, DeepslateEmeraldSide, DeepslateLapizSide, DeepslateDiamondSide,
                       CoalSide, IronSide, CopperSide, GoldSide, RedstoneSide, EmeraldSide, LapizSide, DiamondSide,
                       StoneSide, DioriteSide, AndesiteSide, GraniteSide, DeepslateSide, TerracottaSide,
                       BedrockSide,
                       SpruceLeafSide, OakLeafSide, BirchLeafSide, AcaciaLeafSide, JungleLeafSide,
                       WaterSide,
                       CactusSide, CactusTop, CactusBottom
                     };

public class Block
{
    public TileType top;
    public TileType bottom;
    public TileType side;

    public static List<BlockType> leafBlocks = new List<BlockType> { BlockType.AcaciaLeaf, BlockType.JungleLeaf, BlockType.OakLeaf, BlockType.BirchLeaf, BlockType.SpruceLeaf };
    public static List<BlockType> woodBlocks = new List<BlockType> { BlockType.AcaciaWood, BlockType.JungleWood, BlockType.OakWood, BlockType.BirchWood, BlockType.SpruceWood };

    public static List<BlockType> blocksWithDifferentFaces = new List<BlockType> { BlockType.Grass, BlockType.Snow };

    public static List<BlockType> oreBlocks = new List<BlockType> { BlockType.Coal, BlockType.Copper, BlockType.Iron, BlockType.Gold, BlockType.Redstone, BlockType.Emerald, BlockType.Lapiz, BlockType.Diamond };
    public static List<BlockType> deepslateOreBlocks = new List<BlockType> { BlockType.DeepslateCoal, BlockType.DeepslateCopper, BlockType.DeepslateIron, BlockType.DeepslateGold, BlockType.DeepslateRedstone, BlockType.DeepslateEmerald, BlockType.DeepslateLapiz, BlockType.DeepslateDiamond };

    public static List<BlockType> blocksToRenderAllFaces = leafBlocks.Concat(oreBlocks).Concat(deepslateOreBlocks).Distinct().ToList();

    public Block(TileType top, TileType bottom = TileType.Blank, TileType side = TileType.Blank)
    {
        this.top = top;
        this.bottom = (bottom == TileType.Blank) ? top : bottom;
        this.side = (side == TileType.Blank) ? top : side;
    }
}

public static class Tile
{
    public static Dictionary<BlockType, Block> blocks = new Dictionary<BlockType, Block>()
    {
        { BlockType.Grass, new Block(TileType.GrassTop, TileType.GrassBottom, TileType.GrassSide) },
        { BlockType.Snow, new Block(TileType.SnowTop, TileType.GrassBottom, TileType.SnowSide) },
        { BlockType.Sand, new Block(TileType.SandSide) },

        { BlockType.OakWood, new Block(TileType.OakWoodTop, TileType.OakWoodTop, TileType.OakWoodSide) },
        { BlockType.AcaciaWood, new Block(TileType.AcaciaWoodTop, TileType.AcaciaWoodTop, TileType.AcaciaWoodSide) },
        { BlockType.SpruceWood, new Block(TileType.SpruceWoodTop, TileType.SpruceWoodTop, TileType.SpruceWoodSide) },
        { BlockType.JungleWood, new Block(TileType.JungleWoodTop, TileType.JungleWoodTop, TileType.JungleWoodSide) },
        { BlockType.BirchWood, new Block(TileType.BirchWoodTop, TileType.BirchWoodTop, TileType.BirchWoodSide) },

        { BlockType.DeepslateCoal, new Block(TileType.DeepslateCoalSide) },
        { BlockType.DeepslateIron, new Block(TileType.DeepslateIronSide) },
        { BlockType.DeepslateCopper, new Block(TileType.DeepslateCopperSide) },
        { BlockType.DeepslateGold, new Block(TileType.DeepslateGoldSide) },
        { BlockType.DeepslateRedstone, new Block(TileType.DeepslateRedstoneSide) },
        { BlockType.DeepslateEmerald, new Block(TileType.DeepslateEmeraldSide) },
        { BlockType.DeepslateLapiz, new Block(TileType.DeepslateLapizSide) },
        { BlockType.DeepslateDiamond, new Block(TileType.DeepslateDiamondSide) },

        { BlockType.Coal, new Block(TileType.CoalSide) },
        { BlockType.Iron, new Block(TileType.IronSide) },
        { BlockType.Copper, new Block(TileType.CopperSide) },
        { BlockType.Gold, new Block(TileType.GoldSide) },
        { BlockType.Redstone, new Block(TileType.RedstoneSide) },
        { BlockType.Emerald, new Block(TileType.EmeraldSide) },
        { BlockType.Lapiz, new Block(TileType.LapizSide) },
        { BlockType.Diamond, new Block(TileType.DiamondSide) },

        { BlockType.Stone, new Block(TileType.StoneSide) },
        { BlockType.Diorite, new Block(TileType.DioriteSide) },
        { BlockType.Andesite, new Block(TileType.AndesiteSide) },
        { BlockType.Granite, new Block(TileType.GraniteSide) },
        { BlockType.Deepslate, new Block(TileType.DeepslateSide) },
        {BlockType.Terracotta, new Block(TileType.TerracottaSide) },

        { BlockType.Bedrock, new Block(TileType.BedrockSide) },

        { BlockType.SpruceLeaf, new Block(TileType.SpruceLeafSide) },
        { BlockType.OakLeaf, new Block(TileType.OakLeafSide) },
        { BlockType.BirchLeaf, new Block(TileType.BirchLeafSide) },
        { BlockType.AcaciaLeaf, new Block(TileType.AcaciaLeafSide) },
        { BlockType.JungleLeaf, new Block(TileType.JungleLeafSide) },

        { BlockType.Water, new Block(TileType.WaterSide) },

        { BlockType.Cactus, new Block(TileType.CactusTop, TileType.CactusBottom, TileType.CactusSide) }
    };

    public static Dictionary<TileType, Vector2> tileUvs = new Dictionary<TileType, Vector2>()
    {
        { TileType.Blank, new Vector2(1,1) },

        { TileType.GrassSide, new Vector2(0, 0) },
        { TileType.GrassTop, new Vector2(1, 0) },
        { TileType.GrassBottom, new Vector2(2, 0) },
        { TileType.SnowSide, new Vector2(3, 0) },
        { TileType.SnowTop, new Vector2(4, 0) },
        { TileType.SandSide, new Vector2(5, 0) },

        { TileType.OakWoodTop, new Vector2(0, 1) },
        { TileType.OakWoodSide, new Vector2(1, 1) },
        { TileType.AcaciaWoodTop, new Vector2(2, 1) },
        { TileType.AcaciaWoodSide, new Vector2(3, 1) },
        { TileType.SpruceWoodTop, new Vector2(4, 1) },
        { TileType.SpruceWoodSide, new Vector2(5, 1) },
        { TileType.JungleWoodTop, new Vector2(6, 1) },
        { TileType.JungleWoodSide, new Vector2(7, 1) },
        { TileType.BirchWoodTop, new Vector2(8, 1) },
        { TileType.BirchWoodSide, new Vector2(9, 1) },

        { TileType.DeepslateCoalSide, new Vector2(0, 2) },
        { TileType.DeepslateIronSide, new Vector2(1, 2) },
        { TileType.DeepslateCopperSide, new Vector2(2, 2) },
        { TileType.DeepslateGoldSide, new Vector2(3, 2) },
        { TileType.DeepslateRedstoneSide, new Vector2(4, 2) },
        { TileType.DeepslateEmeraldSide, new Vector2(5, 2) },
        { TileType.DeepslateLapizSide, new Vector2(6, 2) },
        { TileType.DeepslateDiamondSide, new Vector2(7, 2) },

        { TileType.CoalSide, new Vector2(0, 3) },
        { TileType.IronSide, new Vector2(1, 3) },
        { TileType.CopperSide, new Vector2(2, 3) },
        { TileType.GoldSide, new Vector2(3, 3) },
        { TileType.RedstoneSide, new Vector2(4, 3) },
        { TileType.EmeraldSide, new Vector2(5, 3) },
        { TileType.LapizSide, new Vector2(6, 3) },
        { TileType.DiamondSide, new Vector2(7, 3) },

        { TileType.StoneSide, new Vector2(0, 4) },
        { TileType.DioriteSide, new Vector2(1, 4) },
        { TileType.AndesiteSide, new Vector2(2, 4) },
        { TileType.GraniteSide, new Vector2(3, 4) },
        { TileType.DeepslateSide, new Vector2(4, 4) },
        {TileType.TerracottaSide, new Vector2(5,4) },

        { TileType.BedrockSide, new Vector2(0, 5) },

        { TileType.SpruceLeafSide, new Vector2(0, 6) },
        { TileType.OakLeafSide, new Vector2(1, 6) },
        { TileType.BirchLeafSide, new Vector2(2, 6) },
        { TileType.AcaciaLeafSide, new Vector2(3, 6) },
        { TileType.JungleLeafSide, new Vector2(4, 6) },

        { TileType.WaterSide, new Vector2(0, 7) },

        { TileType.CactusTop, new Vector2(0,8) },
        { TileType.CactusSide, new Vector2(1,8) },
        { TileType.CactusBottom, new Vector2(2,8) }
    };
}

//Order of leaves in texture atlas : 
//spruce, oak, birch, acacia, jungle
