using Assets.Scripts.WorldScripts.NoiseGenerators;
using Unity.Mathematics;
using UnityEngine;
namespace Assets.Scripts.World.WorldGenerators
{
    public class NormalChunkGenerator
    {
        private BeeWorld _world;
        private FastNoiseLite _continentalnessNoise;
        private FastNoiseLite _erosionNoise;
        private FastNoiseLite _floraNoise;

        public NormalChunkGenerator(BeeWorld world)
        {
            _world = world;
            // Set up noises
            _continentalnessNoise = new FastNoiseLite(_world.Seed);
            _continentalnessNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            _continentalnessNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            _continentalnessNoise.SetFrequency(0.5f);
            _continentalnessNoise.SetFractalGain(Globals.CONTINENTALNESS_PERSISTENCE);
            _continentalnessNoise.SetFractalLacunarity(Globals.CONTINENTALNESS_LACUNARITY);
            _continentalnessNoise.SetFractalOctaves(Globals.CONTINENTALNESS_OCTAVES);
            _continentalnessNoise.SetScale(Globals.CONTINENTALNESS_SCALE);
            _continentalnessNoise.SetOffset(Globals.CONTINENTALNESS_OFFSET);

            _erosionNoise = new FastNoiseLite(_world.Seed);
            _erosionNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            _erosionNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            _erosionNoise.SetFrequency(0.5f);
            _erosionNoise.SetFractalGain(Globals.EROSION_PERSISTENCE);
            _erosionNoise.SetFractalLacunarity(Globals.EROSION_LACUNARITY);
            _erosionNoise.SetFractalOctaves(Globals.EROSION_OCTAVES);
            _erosionNoise.SetScale(Globals.EROSION_SCALE);
            _erosionNoise.SetOffset(Globals.EROSION_OFFSET);

            _floraNoise = new FastNoiseLite(_world.Seed);
            _floraNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            _floraNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            _floraNoise.SetFrequency(0.5f);
            _floraNoise.SetFractalGain(Globals.EROSION_PERSISTENCE);
            _floraNoise.SetFractalLacunarity(Globals.EROSION_LACUNARITY);
            _floraNoise.SetFractalOctaves(Globals.EROSION_OCTAVES);
            _floraNoise.SetScale(Globals.EROSION_SCALE);
            _floraNoise.SetOffset(Globals.EROSION_OFFSET);

        }
        public void GenerateChunk(int xChunk, int zChunk, ref Chunk chunk)
        {
            for (int x = 0; x < Globals.CHUNK_SIZE; x++)
            {
                for (int z = 0; z < Globals.CHUNK_SIZE; z++)
                {
                    int3 global = (int3)BeeWorld.GetWorldFromLocalPosition(new int2(xChunk, zChunk), new float3(x,0,z));
                    float height = GetHeightAt(global.x, global.z);
                    chunk.SetHeightAt(x, z, height);
                    //GenFlowers(global.x, global.z, height);
                }
            }
        }
        private void GenFlowers(int x, int z, float y)
        {
            float flora = _floraNoise.GetNoise(x, z);
            if(flora > 0.1f && flora < 0.15f)
            {
                // there is a flower
                //_world.SpawnerSystem.AddSpawnRequest(new ScanRequest { Position = new float3(x,y,z), Type = flora<0?0:1});
            }
        }
        private float GetErosionBase(int x, int z, float currentHeight)
        {
            float erosion = _erosionNoise.GetNoise(x, z);
            float finalErosionHeight;
            if (erosion < 0.0f)
            {
                erosion = Mathf.Lerp(0, erosion, Mathf.InverseLerp(Globals.WATER_LEVEL - 4, Globals.WATER_LEVEL, currentHeight));
                finalErosionHeight = Mathf.Lerp(22f, 5f, Mathf.InverseLerp(-1, 0, erosion));

            }
            else
            {
                finalErosionHeight = Mathf.Lerp(5f, 2f, Mathf.InverseLerp(0f, 1f, erosion));
            }
            if (currentHeight > Globals.WATER_LEVEL)
            {
                finalErosionHeight -= (currentHeight - Globals.WATER_LEVEL);
            }

            return finalErosionHeight;
        }

        private float GenContinentalBase(int x, int z)
        {
            float continentalness = _continentalnessNoise.GetNoise(x, z);
            if (continentalness < 0.0f)
            {
                if (continentalness < -0.4f)
                {
                    // ocean
                    return Mathf.Lerp(5f, Globals.WATER_LEVEL - 10f, Mathf.InverseLerp(-1f, -0.4f, continentalness));
                }
                else
                {
                    // sea
                    return Mathf.Lerp(Globals.WATER_LEVEL - 10f, Globals.WATER_LEVEL, Mathf.InverseLerp(-0.3f, 0.0f, continentalness));
                }
            }
            else
            {
                return Mathf.Lerp(Globals.WATER_LEVEL, Globals.WATER_LEVEL + 5f, Mathf.InverseLerp(0.0f, 1f, continentalness));
            }
        }

        /**
         * Returns the base height depending on the continentalness
         */
        public float GetHeightAt(int x, int z)
        {
            float currentHeight = GenContinentalBase(x, z);
            currentHeight = currentHeight + GetErosionBase(x, z, currentHeight);
            return currentHeight;
        }
    }
}
