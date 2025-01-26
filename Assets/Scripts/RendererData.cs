using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public class RendererData
    {
        public RenderParams[] RenderParams;
        public RenderParams[] ParticleRenderParams;

        [Header("Terrain settings")]
        [SerializeField] public Material[] ChunkMaterial;

        [Space(10)]
        [Header("Grass settings")]
        [SerializeField] public Mesh[] GrassLODMeshes;
        [SerializeField] public Material GrassMaterial;

        [Space(10)]
        [Header("Decoration object settings")]
        [SerializeField] public WorldObject[] DecorationalObjects;

        [Space(10)]
        [Header("Particles")]
        public Mesh[] ParticleMesh;
        public Material[] particleMaterial;

        public void Init()
        {
            // Compute total number of materials
            int totalMaterialsOfDecorationalObjects = 0;
            
            for(int j = 0; j < DecorationalObjects.Length; j++)
            {
                totalMaterialsOfDecorationalObjects += DecorationalObjects[j].Materials.Length;
            }
            int i = 0;
            // Terrain
            RenderParams = new RenderParams[ChunkMaterial.Length + 1 + totalMaterialsOfDecorationalObjects]; // 0: Chunk materials, 1: Grass material, >1: decorational objects materials
            
            for(int t = 0; t < ChunkMaterial.Length; t++)
            {
                RenderParams[i] = new RenderParams(ChunkMaterial[t]);
                RenderParams[i].receiveShadows = true;
                RenderParams[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                i++;
            }
            // Grass 
            RenderParams[i] = new RenderParams(GrassMaterial);
            RenderParams[i].receiveShadows = true;
            RenderParams[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            i++;
            
            // Decoration objects
            int r = ChunkMaterial.Length + 1;
            for (int j = 0; j < DecorationalObjects.Length; j++)
            {
                for(int l = 0; l < DecorationalObjects[j].Materials.Length; l++)
                {
                    RenderParams[r] = new RenderParams(DecorationalObjects[j].Materials[l]);
                    RenderParams[r].receiveShadows = true;
                    RenderParams[r].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    r++;
                }
                
            }

            // Particles
            ParticleRenderParams = new RenderParams[particleMaterial.Length];
            for(int l = 0; l < ParticleRenderParams.Length; l++)
            {
                ParticleRenderParams[l] = new RenderParams(particleMaterial[l]);
                ParticleRenderParams[l].receiveShadows = false;
                ParticleRenderParams[l].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }

        public void RenderParticles(int particleId, ref Matrix4x4[] data, int emitters)
        {
            if(emitters!=0 && emitters <= 1024)
                Graphics.RenderMeshInstanced(ParticleRenderParams[particleId], ParticleMesh[particleId], 0, data, emitters);
        }

        public void RenderInstance(Mesh instancedMesh, int renderParamsId, ref Matrix4x4[] instanceData)
        {
            
            if (instanceData.Length > 1024)
            {
                Graphics.RenderMeshInstanced(RenderParams[renderParamsId], instancedMesh, 0, instanceData, 1024, 0);
                Graphics.RenderMeshInstanced(RenderParams[renderParamsId], instancedMesh, 0, instanceData, 1024, 1024);
                /*
                Graphics.RenderMeshInstanced(RenderParams[renderParamsId], instancedMesh, 0, instanceData, 1024, 2048);
                Graphics.RenderMeshInstanced(RenderParams[renderParamsId], instancedMesh, 0, instanceData, 1024, 3072);*/

            } else
            {
                for (int i = 0; i < instancedMesh.subMeshCount; i++)
                {
                    Graphics.RenderMeshInstanced(RenderParams[renderParamsId], instancedMesh, i, instanceData, instanceData.Length, 0);
                }

            }
        }
    }
}
