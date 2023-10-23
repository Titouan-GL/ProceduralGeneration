using System;
using System.Collections.Generic;
using UnityEngine;
using static Structs;

[Serializable]
public class Biome
{
    public string name;
    public float offset;
    public float scale;
    public int probabilityStrength = 10;

    public List<ThresholdData> thresholds;
    public List<DecorData> decors;

}
