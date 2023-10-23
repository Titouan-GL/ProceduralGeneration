using System;
using UnityEngine;

public class Structs
{
    [Serializable]
    public struct ThresholdData
    {
        public Batcher batcher;
        public float height;
        public ThresholdData(Threshold batcher, float height)
        {
            this.batcher = batcher;
            this.height = height;
        }
    }
    public struct MapTile
    {
        public Vector3 position;
        public Biome biomeType;
        public float closenessRatio;//how far is it from center ? 0 is on it, 0.5 makes it as close to the center than to another center
        public Decor decor;
        public MapTile(Biome newType, Vector3 newposition, float newCloseness = 0)
        {
            biomeType = newType;
            position = newposition;
            closenessRatio = newCloseness;
            decor = new Decor();
        }
    }
    public struct Decor
    {
        public Vector2Int position;
        public DecorType decorType;
        public Decor(DecorType type = DecorType.None, Vector2Int newposition = new Vector2Int())
        {
            position = newposition;
            decorType = type;
        }
    }

    public struct BiomeLocation
    {
        public Vector2Int position;
        public Biome biomeType;
        public int x;
        public int y;
        public BiomeLocation(Biome newbiome, Vector2Int newposition = new Vector2Int())
        {
            position = newposition;
            biomeType = newbiome;
            x = position.x;
            y = position.y;
        }
    }

    [Serializable]
    public struct DecorData
    {
        public DecorType type;
        public Environment batcher;
        public Vector2 heightRange;
        public float probability;
    }

    [Serializable]
    public enum DecorType
    {
        None,
        Forest_Tree_1,
        Forest_Tree_2
    }
}
