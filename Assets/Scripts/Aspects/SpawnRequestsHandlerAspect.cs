using System.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public readonly partial struct SpawnRequestsHandlerAspect : IAspect
{
    public readonly DynamicBuffer<ScanRequest> SpawnRequests;

    public void HandleSpawnRequests(PolenSpawnerConfigComponent comp)
    { 
        
    }

}
