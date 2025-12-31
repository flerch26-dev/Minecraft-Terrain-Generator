using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreeType { Oak, Birch, Spruce, Acacia, Jungle, Cactus };

public static class TreeGenerator
{
    public static Tree OakTree = new Tree(BlockType.OakWood, BlockType.OakLeaf, 2, 1, 2, 2);
    public static Tree AcaciaTree = new Tree(BlockType.AcaciaWood, BlockType.AcaciaLeaf, 4, 1, 3, 1);
    public static Tree JungleTree = new Tree(BlockType.JungleWood, BlockType.JungleLeaf, 6, 2, 4, 2);
    public static Tree SpruceTree = new Tree(BlockType.SpruceWood, BlockType.SpruceLeaf, 3,1, 2, 3);
    public static Tree BirchTree = new Tree(BlockType.BirchWood, BlockType.BirchLeaf, 2, 1, 2, 2);
    public static Tree Cactus = new Tree(BlockType.Cactus, BlockType.Air, 3, 1, 0, 0);

    public static Tree[] trees = new Tree[6] { OakTree, AcaciaTree, JungleTree, SpruceTree, BirchTree, Cactus };

    public static Dictionary<TreeType, Tree> treeDict = new Dictionary<TreeType, Tree>() { { TreeType.Oak, OakTree }, { TreeType.Birch, BirchTree }, { TreeType.Spruce, SpruceTree }, { TreeType.Acacia, AcaciaTree }, { TreeType.Jungle, JungleTree }, { TreeType.Cactus, Cactus } };

    public static BlockType[,,] GenerateTree(BlockType[,,] blocks, int seed, int chunkHeight, int x, int z, Tree treeType)
    {
        int landHeight = GetLandHeight(blocks, chunkHeight, x, z);
        
        if (landHeight < 80 || blocks[x, landHeight, z] == BlockType.Water || blocks[x, landHeight, z] == BlockType.Stone)
        {
            return blocks;
        }

        int treeTrunkHeight = treeType.trunkHeight;
        int treeTrunkWidth = treeType.trunkWidth;
        int highestWoodBlock = landHeight + treeTrunkHeight;

        GenerateTrunk(blocks, landHeight, x, z, treeType.woodType, treeTrunkHeight, treeTrunkWidth);
        GenerateLeaves(blocks, highestWoodBlock, x, z, treeType);
        
        return blocks;
    }

    static int GetLandHeight(BlockType[,,] blocks, int chunkHeight, int x, int z)
    {
        int landHeight = 0;

        for (int y = chunkHeight - 1; y > 0; y--)
        {
            bool canSPawn = blocks[x, y, z] != BlockType.Air &&
                            !Block.woodBlocks.Contains(blocks[x, y, z]) &&
                            !Block.leafBlocks.Contains(blocks[x, y, z]);
            if (canSPawn)
            {
                landHeight = y;
                break;
            }
        }

        return landHeight;
    }

    static void GenerateTrunk(BlockType[,,] blocks, int landHeight, int x, int z, BlockType woodType, int treeTrunkHeight, int treeTrunkWidth)
    {
        for (int xOffset = 0; xOffset < treeTrunkWidth; xOffset++)
        {
            for (int zOffset = 0; zOffset < treeTrunkWidth; zOffset++)
            {
                for (int y = 0; y <= treeTrunkHeight; y++)
                {
                    blocks[x + xOffset, landHeight + y, z + zOffset] = woodType;
                }
            }
        }
    }

    //Maybe come back later and make a function for each specific tree to vary leaves even more
    static void GenerateLeaves(BlockType[,,] blocks, int highestWoodBlock, int x, int z, Tree treeType)
    {
        int addLeaf = 0;
        if (treeType.trunkWidth == 2) { addLeaf = 1; }

        for (int i = -treeType.leafWidth + addLeaf; i <= treeType.leafWidth; i++)
        {
            for (int j = -treeType.leafWidth + addLeaf; j <= treeType.leafWidth; j++)
            {
                for (int k = 1; k <= treeType.leafHeight; k++)
                {
                    if (((i == -treeType.leafWidth + addLeaf && j == -treeType.leafWidth + addLeaf) ||
                         (i == treeType.leafWidth && j == -treeType.leafWidth + addLeaf) ||
                         (i == -treeType.leafWidth + addLeaf && j == treeType.leafWidth) ||
                         (i == treeType.leafWidth && j == treeType.leafWidth)) && k != 1) { }
                    else
                    {
                        if (blocks[x + i, highestWoodBlock + k, z + j] == BlockType.Air)
                        {
                            blocks[x + i, highestWoodBlock + k, z + j] = treeType.leafType;
                        }
                    }
                }
            }
        }

        blocks[x, highestWoodBlock + treeType.leafHeight + 1, z] = treeType.leafType;
        if (treeType.trunkWidth == 2)
        {
            blocks[x + 1, highestWoodBlock + treeType.leafHeight + 1, z] = treeType.leafType;
            blocks[x, highestWoodBlock + treeType.leafHeight + 1, z + 1] = treeType.leafType;
            blocks[x + 1, highestWoodBlock + treeType.leafHeight + 1, z + 1] = treeType.leafType;
        }
    }
}

public class Tree
{
    public BlockType woodType;
    public BlockType leafType;

    public int trunkHeight;
    public int trunkWidth;

    public int leafWidth;
    public int leafHeight;

    public Tree(BlockType woodType, BlockType leafType, int trunkHeight, int trunkWidth, int leafWidth, int leafHeight)
    {
        this.woodType = woodType;
        this.leafType = leafType;

        this.trunkHeight = trunkHeight;
        this.trunkWidth = trunkWidth;

        this.leafWidth = leafWidth;
        this.leafHeight = leafHeight;
    }
}