using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Linq;
using System.Net.NetworkInformation;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { ContinentalityMap, ErosionMap, PeaksAndValleysMap, TemperatureMap, HumidityMap, Mesh };
    public DrawMode drawMode;
    public Noise.NormalizeMode normalizeMode;

    public int editorPreviewMapChunkSize = 200;
    public const int mapChunkSize = 16 + 3;
    public int waterLevel = 150;
    public const int chunkHeight = 500;
    [Range(0.00001f, 100)]
    public float squashingFactor;
    [Range(0,6)]
    public int editorPreviewLOD;

    public int seed = 1;

    public NoiseSettings continentalitySettings;
    public NoiseSettings erosionSettings;
    public NoiseSettings peaksAndValleysSettings;
    public NoiseSettings temperatureSettings;
    public NoiseSettings humiditySettings;

    public float meshHeightMultiplier;

    public bool autoUpdate;

    public List<BiomeSettings> biomes = new List<BiomeSettings>();

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public static Dictionary<BlockType, int> oreProbability = new Dictionary<BlockType, int> { { BlockType.Coal, 300000 } };

    private void Start()
    {
        continentalitySettings.seed = seed;
        erosionSettings.seed = seed + 1;
        peaksAndValleysSettings.seed = seed + 2;
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero, editorPreviewMapChunkSize);
        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.ContinentalityMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.continentality));
        }
        else if (drawMode == DrawMode.ErosionMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.erosion));
        }
        else if (drawMode == DrawMode.PeaksAndValleysMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.peaksAndValleys));
        }
        else if (drawMode == DrawMode.TemperatureMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.temperature));
        }
        else if (drawMode == DrawMode.HumidityMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.humidity));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData, editorPreviewMapChunkSize, chunkHeight));//, TextureGenerator.TextureFromColorMap(mapData.colorMap, chunkSize, chunkSize));
        }
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying) DrawMapInEditor();
    }

    private void OnValidate()
    {
        if (continentalitySettings != null)
        {
            continentalitySettings.OnValuesUpdated -= OnValuesUpdated;
            continentalitySettings.OnValuesUpdated += OnValuesUpdated;
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center, mapChunkSize);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData, mapChunkSize, chunkHeight);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData(Vector2 center, int chunkSize)
    {
        float[,] continentality = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed,
                                                         continentalitySettings.scale,
                                                         continentalitySettings.octaves,
                                                         continentalitySettings.persistance,
                                                         continentalitySettings.lacunarity,
                                                         center + continentalitySettings.offset,
                                                         normalizeMode);

        float[,] erosion = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed,
                                                  erosionSettings.scale,
                                                  erosionSettings.octaves,
                                                  erosionSettings.persistance,
                                                  erosionSettings.lacunarity, 
                                                  center + erosionSettings.offset,
                                                  normalizeMode);

        float[,] peaksAndValleys = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed,
                                                  peaksAndValleysSettings.scale,
                                                  peaksAndValleysSettings.octaves,
                                                  peaksAndValleysSettings.persistance,
                                                  peaksAndValleysSettings.lacunarity,
                                                  center + peaksAndValleysSettings.offset,
                                                  normalizeMode);

        float[,] temperature = Noise.GenerateNoiseMap(chunkSize, chunkSize, temperatureSettings.seed,
                                                  temperatureSettings.scale,
                                                  temperatureSettings.octaves,
                                                  temperatureSettings.persistance,
                                                  temperatureSettings.lacunarity,
                                                  center + temperatureSettings.offset,
                                                  normalizeMode);

        float[,] humidity = Noise.GenerateNoiseMap(chunkSize, chunkSize, humiditySettings.seed,
                                                  humiditySettings.scale,
                                                  humiditySettings.octaves,
                                                  humiditySettings.persistance,
                                                  humiditySettings.lacunarity,
                                                  center + humiditySettings.offset,
                                                  normalizeMode);

        BlockType[,,] blocks = GenerateBlockArray(continentality, erosion, peaksAndValleys, temperature, humidity, chunkSize, center);

        return new MapData(continentality, erosion, peaksAndValleys, temperature, humidity, blocks);
    }

    public BlockType[,,] GenerateBlockArray(float[,] continentality, float[,] erosion, float[,] peaksAndValleys, float[,] temperature, float[,] humidity, int chunkSize, Vector2 centre)
    {
        AnimationCurve _continentalityCurve = new AnimationCurve(continentalitySettings.heightCurve.keys);
        AnimationCurve _erosionCurve = new AnimationCurve(erosionSettings.heightCurve.keys);
        AnimationCurve _peaksAndValleysCurve = new AnimationCurve(peaksAndValleysSettings.heightCurve.keys);

        System.Random prng = new System.Random(continentalitySettings.seed + (int)centre.x * (int)centre.x + (int)centre.y);

        BlockType[,,] blocks;
        blocks = new BlockType[chunkSize + 2, chunkHeight, chunkSize + 2];

        BiomeSettings[,] biomeMap = new BiomeSettings[chunkSize, chunkSize];
        float[,] terrainHeights = new float[chunkSize, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                for (int y = 150; y < chunkHeight - 1; y++)
                {
                    float adjustedContinentalness = _continentalityCurve.Evaluate(continentality[x, z]);
                    float adjustedErosion = _erosionCurve.Evaluate(erosion[x, z]);
                    float adjustedPeaksValleys = _peaksAndValleysCurve.Evaluate(peaksAndValleys[x, z]);

                    float continentalnessWeight = .7f;
                    float erosionWeight = .6f;
                    float peaksValleysWeight = .6f;

                    float terrainHeight = continentalnessWeight * adjustedContinentalness+
                                          erosionWeight * adjustedErosion +
                                          peaksValleysWeight * adjustedPeaksValleys;

                    terrainHeight = terrainHeight * meshHeightMultiplier - 150;
                    terrainHeights[x, z] = terrainHeight;

                    int stoneHeight = prng.Next(2, 5);

                    BiomeSettings biome = BiomeCategorizing.GetBiome(waterLevel, biomes, new Vector2Int(x, z), (int)terrainHeight, erosion, temperature, humidity);
                    biomeMap[x, z] = biome;

                    if (y < terrainHeight && y >= terrainHeight - stoneHeight)
                    {
                        blocks[x, y - 150, z] = biome.baseBlock;
                    }
                    else if (y < terrainHeight - stoneHeight)
                    {
                        blocks[x, y - 150, z] = BlockType.Stone;
                    }
                    else
                    {
                        blocks[x, y - 150, z] = BlockType.Air;
                    }
                }
            }
        }

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                for (int y = 0; y < terrainHeights[x, z] - 5; y++)
                {
                    //Ore Generation
                    for (int i = 0; i < oreProbability.Count; i++)
                    {
                        //if (prng.Next(0, oreProbability.Values.ElementAt(i)) == 1)
                        //{ PropagateOreVein(blocks, oreProbability.Keys.ElementAt(i), new Vector3Int(x, y - 150, z)); }
                    }

                    //Cave Generation
                    if (y > 150)
                    {
                        float dstFromCenter = y - 250;
                        float adjustedDst = Mathf.InverseLerp(-250, 250, dstFromCenter) * 2 - 1;

                        float densityValue = Noise.OctavePerlin3D(x + centre.x, y, z + centre.y, 4, 3, 400);
                        float adjustedDensity = densityValue + adjustedDst * 0.6f;

                        if (adjustedDensity > 0.1f) { blocks[x, y - 150, z] = BlockType.Air; }
                        //else { blocks[x, y - 50, z] = BlockType.Stone; }
                    }
                }

                //Tree Generation
                if (biomeMap[x, z].hasTrees)
                {
                    bool isNotBorderBlock = z < chunkSize - 3 && z > 3 && x < chunkSize - 3 && x > 3;
                    if (prng.Next(0, biomeMap[x, z].treeDensity) == 1 && isNotBorderBlock)
                    {
                        blocks = TreeGenerator.GenerateTree(blocks, seed, chunkHeight, x, z, TreeGenerator.treeDict[biomeMap[x, z].treeType]);
                    }
                }
            }
        }

        return blocks;
    }

    public BlockType[,,] PropagateOreVein(BlockType[,,] blocks, BlockType ore, Vector3Int position)
    {
        blocks[position.x, position.y, position.z] = ore;
        return blocks;
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

public struct MapData
{
    public readonly float[,] continentality;
    public readonly float[,] erosion;
    public readonly float[,] peaksAndValleys;
    public readonly float[,] temperature;
    public readonly float[,] humidity;

    public readonly BlockType[,,] blocks;

    public MapData(float[,] continentality, float[,] erosion, float[,] peaksAndValleys, float[,] temperature, float[,] humidity, BlockType[,,] blocks)
    {
        this.continentality = continentality;
        this.erosion = erosion;
        this.peaksAndValleys = peaksAndValleys;
        this.temperature = temperature;
        this.humidity = humidity;
        this.blocks = blocks;
    }
}