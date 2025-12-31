using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BiomeSettings : UpdatableData
{
    public BlockType baseBlock;
    public bool hasTrees;
    public TreeType treeType;
    public int treeDensity;
}
