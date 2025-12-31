using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(MapData mapData, int chunkSize, int worldHeihgt)
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int x = 1; x < chunkSize + 1; x++)
        {
            for (int z = 1; z < chunkSize + 1; z++)
            {
                for (int y = 1; y < worldHeihgt; y++)
                {
                    BlockType blockType = mapData.blocks[x, y, z];
                    Vector3 blockPos = new Vector3(x - 1, y, z - 1);
                    int numFaces = 0;

                    bool isNotBorderBlock = z < chunkSize - 1 && z > 1 && x < chunkSize - 1 && x > 1;

                    if (blockType != BlockType.Air && isNotBorderBlock)
                    {
                        bool renderAllFaces = Block.blocksToRenderAllFaces.Contains(blockType);

                        //front
                        if (mapData.blocks[x, y, z - 1] == BlockType.Air || Block.leafBlocks.Contains(mapData.blocks[x, y, z - 1]) || renderAllFaces)
                        {
                            verts.Add(new Vector3(0, 0, 0) + blockPos);
                            verts.Add(new Vector3(0, 1, 0) + blockPos);
                            verts.Add(new Vector3(1, 1, 0) + blockPos);
                            verts.Add(new Vector3(1, 0, 0) + blockPos);
                            numFaces++;

                            if (Block.blocksWithDifferentFaces.Contains(mapData.blocks[x, y + 1, z]))
                                uvs.AddRange(GetUV(Tile.blocks[mapData.blocks[x, y, z]].bottom));
                            else
                                uvs.AddRange(GetUV(Tile.blocks[mapData.blocks[x, y, z]].side));
                        }

                        //back
                        if (mapData.blocks[x, y, z + 1] == BlockType.Air || Block.leafBlocks.Contains(mapData.blocks[x, y, z + 1]) || renderAllFaces)
                        {
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            numFaces++;

                            if (Block.blocksWithDifferentFaces.Contains(mapData.blocks[x, y + 1, z]))
                                uvs.AddRange(GetUV(Tile.blocks[mapData.blocks[x, y, z]].bottom));
                            else
                                uvs.AddRange(GetUV(Tile.blocks[mapData.blocks[x, y, z]].side));
                        }

                        //right
                        if (mapData.blocks[x + 1, y, z] == BlockType.Air || Block.leafBlocks.Contains(mapData.blocks[x + 1, y, z]) || renderAllFaces)
                        {
                            verts.Add(new Vector3(1, 0, 0) + blockPos);
                            verts.Add(new Vector3(1, 1, 0) + blockPos);
                            verts.Add(new Vector3(1, 1, 1) + blockPos);
                            verts.Add(new Vector3(1, 0, 1) + blockPos);
                            numFaces++;

                            if (Block.blocksWithDifferentFaces.Contains(mapData.blocks[x, y + 1, z]))
                                uvs.AddRange(GetUV(Tile.blocks[mapData.blocks[x, y, z]].bottom));
                            else
                                uvs.AddRange(GetUV(Tile.blocks[mapData.blocks[x, y, z]].side));
                        }

                        //left
                        if (mapData.blocks[x - 1, y, z] == BlockType.Air || Block.leafBlocks.Contains(mapData.blocks[x - 1, y, z]) || renderAllFaces)
                        {
                            verts.Add(new Vector3(0, 0, 1) + blockPos);
                            verts.Add(new Vector3(0, 1, 1) + blockPos);
                            verts.Add(new Vector3(0, 1, 0) + blockPos);
                            verts.Add(new Vector3(0, 0, 0) + blockPos);
                            numFaces++;

                            if (Block.blocksWithDifferentFaces.Contains(mapData.blocks[x, y + 1, z]))
                                uvs.AddRange(GetUV(Tile.blocks[mapData.blocks[x, y, z]].bottom));
                            else
                                uvs.AddRange(GetUV(Tile.blocks[mapData.blocks[x, y, z]].side));
                        }

                        //bottom
                        if (mapData.blocks[x, y - 1, z] == BlockType.Air || Block.leafBlocks.Contains(mapData.blocks[x, y - 1, z]) || renderAllFaces)
                        {
                            verts.Add(new Vector3(0, 0, 0) + blockPos);
                            verts.Add(new Vector3(1, 0, 0) + blockPos);
                            verts.Add(new Vector3(1, 0, 1) + blockPos);
                            verts.Add(new Vector3(0, 0, 1) + blockPos);
                            numFaces++;

                            uvs.AddRange(GetUV(Tile.blocks[mapData.blocks[x, y, z]].bottom));
                        }

                        //top
                        if (mapData.blocks[x, y + 1, z] == BlockType.Air || Block.leafBlocks.Contains(mapData.blocks[x, y + 1, z]) || renderAllFaces)
                        {
                            verts.Add(new Vector3(0, 1, 1) + blockPos);
                            verts.Add(new Vector3(1, 1, 1) + blockPos);
                            verts.Add(new Vector3(1, 1, 0) + blockPos);
                            verts.Add(new Vector3(0, 1, 0) + blockPos);
                            numFaces++;

                            uvs.AddRange(GetUV(Tile.blocks[mapData.blocks[x, y, z]].top));
                        }

                        //create triangles
                        int tl = verts.Count - 4 * numFaces;
                        for (int i = 0; i < numFaces; i++)
                        {
                            tris.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2 });
                            tris.AddRange(new int[] { tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                        }
                    }
                }
            }
        }

        //settings the mesh
        MeshData meshData = new MeshData();
        meshData.vertices = verts.ToArray();
        meshData.triangles = tris.ToArray();
        meshData.uvs = uvs.ToArray();

        return meshData;
    }

    public static Vector2[] GetUV(TileType tileType)
    {
        Vector2 tile = Tile.tileUvs[tileType];
        float divisionFactor = 16f;
        Vector2[] uvs = new Vector2[]
        {
            new Vector2(tile.x/divisionFactor, tile.y/divisionFactor),
            new Vector2(tile.x/divisionFactor, (tile.y+1)/divisionFactor),
            new Vector2((tile.x+1)/divisionFactor, (tile.y+1)/divisionFactor),
            new Vector2((tile.x+1)/divisionFactor, tile.y/divisionFactor)
        };
        return uvs;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    public MeshData()
    {
        
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}