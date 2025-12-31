using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeCategorizing
{
    public static BiomeSettings GetBiome(int waterHeight, List<BiomeSettings> biomes, Vector2Int position, int terrainHeight, float[,] erosionMap, float[,] temperatureMap, float[,] humidityMap)
    {
        float erosion = erosionMap[position.x, position.y];
        float temperature = temperatureMap[position.x, position.y];
        float humidity = humidityMap[position.x, position.y];

        if (terrainHeight <= waterHeight)
        {
            return biomes[10]; //Ocean Biome
        }

        //1
        if (terrainHeight <= 325 && erosion >= 0 && erosion <= 0.25)
        {
            if (humidity >= 0.4 && humidity <= 1 && temperature <= 0.5) { return biomes[0]; } //Savanha
            else { return biomes[1]; } //Desert
        }

        //2
        else if (terrainHeight > 325 && erosion >= 0 && erosion <= 0.25)
        {
            if (temperature < 0.25) { return biomes[3]; } //Snowy Taiga
            else if (temperature >= 0.25 && temperature <= 0.4) { return biomes[8]; } //Spruce Forest
            else if (temperature >= 0.3 && temperature <= 0.6) { return biomes[1]; } //Desert
            else if (temperature > 0.6) { return biomes[2]; } //Mesa
        }

        //3
        else if (terrainHeight <= 325 && erosion >= 0.25 && erosion <= 1)
        {
            if (temperature < 0.15) { return biomes[4]; } //Snowy Plains
            else if (humidity < 0.15) { return biomes[5]; } //Plains
            else if (humidity >= 0.15 && humidity < 0.35) { return biomes[6]; } //Oak Forest
            else if (humidity >= 0.35 && humidity < 0.6) { return biomes[7]; } //Birch Forest
            else { return biomes[8]; } //Spruce Forest
        }

        //4
        else
        {
            if (temperature <= 0.3) { return biomes[4]; } //Snowy Plains
            else if (temperature >= 0.6 && humidity >= 0.25) { return biomes[9]; } //Jungle
            else { return biomes[5]; } //Plains
        }
        return biomes[1];
    }
}

