using Assets.Scripts.Components;
using Assets.Scripts.Sound;
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public struct BeeSoundData
    {
        public float DistanceSq;
        public float Pitch;
    }
    public partial class BeeSoundSystem : SystemBase
    {
        private BeeSoundData[] beeSounds = new BeeSoundData[3];
        protected override void OnCreate()
        {
            base.OnCreate();
            Array.Fill(beeSounds, new BeeSoundData { DistanceSq = float.MaxValue, Pitch = 1 });
        }

        protected override void OnUpdate()
        {
            if (BeeWorld.Instance == null )
                return;

            float3 playerPosition = BeeWorld.Instance.PlayerTransform.position;
            int firstFreeSlot = 0;
            int numberOfEmitters = 0;
            //int smallestDistSlot = -1;
            Entities
               .WithAll<SoundEmitter, LocalTransform, BeeStateComponent>()
               .WithNone<IsDead>()
               .ForEach((Entity entity, ref SoundEmitter SoundEmitter, in BeeStateComponent beeState, in LocalTransform transform) =>
               {
                   float distSq = math.distancesq(playerPosition, transform.Position);
                   // Sound
                   if(distSq < Globals.MAX_DISTANCE_FOR_SOUND)
                   {
                       if(firstFreeSlot >= beeSounds.Length)
                       {
                           // Check if can replace one saved
                           float smallestDist = float.MaxValue;
                           int slot = -1;
                           for (int i = 0; i < beeSounds.Length; i++)
                           {
                               if(beeSounds[i].DistanceSq < smallestDist)
                               {
                                   slot = i;
                                   smallestDist = beeSounds[i].DistanceSq;
                               }
                           }
                           if(slot!=-1 && beeSounds[slot].DistanceSq > distSq)
                           {
                               beeSounds[slot].DistanceSq = distSq;
                               beeSounds[slot].Pitch = SoundEmitter.Pitch; // TODO: change pitch depending on bee's pitch
                           }
                       }
                       else
                       {
                           // Save in free slot
                           beeSounds[firstFreeSlot].DistanceSq = distSq;
                           beeSounds[firstFreeSlot].Pitch = SoundEmitter.Pitch; // TODO: change pitch depending on bee's pitch
                           firstFreeSlot++;
                       }
                        
                   }

                   // Particles
                   if(numberOfEmitters <= BeeWorld.Instance.PolenParticleMatrices.Length && beeState.PollenTaken > 0 && distSq < Globals.MAX_DISTANCE_FOR_SOUND)
                   {
                       BeeWorld.Instance.PolenParticleMatrices[numberOfEmitters++] =  Matrix4x4.Translate(transform.Position - new float3(0.1f,0.10f, 0.0f)) * Matrix4x4.Scale(new float3(25, 25, 25)) * Matrix4x4.Rotate(transform.Rotation);
                   }
               }).WithoutBurst().Run();

            BeeWorld.Instance.PolenParticleEmitters = numberOfEmitters;
            for(int i = 0; i < beeSounds.Length; i++)
            {
                SoundManager.Instance.ChangeVolumeAndPitchOfBeeBuzz(i, math.lerp(1, 0, beeSounds[i].DistanceSq / Globals.MAX_DISTANCE_FOR_SOUND), beeSounds[i].Pitch);
                beeSounds[i].DistanceSq = float.MaxValue;
            }            
        }
    }
}
