using Assets.Scripts.Systems;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Sound
{
    public enum SoundType
    {
        AMBIENT,
        BEE_BUZZ,
        NEST_BUZZ,
        CLICK_ON_NEST,
        CLICK_ON_BEE,
        FINISH_RECOLLECTION,
        BUTTON_CLICK,
        COMPLETE_QUEST,
        COMPLETE_LEVEL
    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance {  get { return _instance; } }
        private static SoundManager _instance;

        [SerializeField] private float2 pitchRange;
        [SerializeField] private AudioClip[] clips;
        private AudioSource[] _source;
        private AudioSource[] _beeSources;

        private AudioSource _ambient;

        private void Awake()
        {
            if(_instance==null)
            {
                _instance = this;
            } else
            {
                Destroy(this);
            }
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            _source = new AudioSource[5];
            for(int i = 0; i < _source.Length; i++)
            {
                _source[i] = gameObject.AddComponent<AudioSource>();
            }

            // Bee sources
            _beeSources = new AudioSource[3];
            for (int i = 0; i < _beeSources.Length; i++)
            {
                _beeSources[i] = gameObject.AddComponent<AudioSource>();
                _beeSources[i].volume = 0;
                _beeSources[i].pitch = 1;
                _beeSources[i].clip = clips[(int)SoundType.BEE_BUZZ];
                _beeSources[i].loop = true;
                _beeSources[i].Play();
            }

            // Ambient sound
            _ambient = gameObject.AddComponent<AudioSource>();
            _ambient.loop = true;
            _ambient.clip = clips[(int)SoundType.AMBIENT];
            _ambient.volume = 0;
            _ambient.Play();

            // Nest sound
            PlayLoopSound(SoundType.NEST_BUZZ);

            // Bee sound for player
            PlayLoopSound(SoundType.BEE_BUZZ);
        }

        public void PauseAllSounds(bool pause)
        {
            for(int i = 0; i < _source.Length-1; i++)
            {
                if(pause)
                {
                    _source[i].Pause();
                } else
                {
                    _source[i].UnPause();
                }
                
            }

            for (int i = 0; i < _beeSources.Length; i++)
            {
                if (pause)
                {
                    _beeSources[i].Pause();
                } else
                {
                    _beeSources[i].UnPause();
                }
                
            }
        }

        public void RemoveSounds()
        {
            for (int i = 0; i < _source.Length; i++)
            {
                _source[i].volume = 0;
            }
            Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BeeSoundSystem>().Enabled = false;
            Globals.SIMULATION_PAUSED = true;
            for (int i = 0; i < _beeSources.Length; i++)
            {
                _beeSources[i].volume = 0;
            }

            _ambient.volume = 0;

        }
        public void PlaySound(SoundType sound, float volume, bool randomPitch)
        {
            if (sound == SoundType.BUTTON_CLICK)
            {
                _source[_source.Length - 1].volume = volume;
                _source[_source.Length - 1].pitch = randomPitch ? (UnityEngine.Random.Range(pitchRange.x, pitchRange.y)) : 1;
                _source[_source.Length - 1].PlayOneShot(clips[(int)sound]);
                return;
            }

            for (int i = 0; i < _source.Length-1; i++)
            {
                if(!_source[i].isPlaying || i == _source.Length-1)
                {
                    _source[i].volume = volume;
                    _source[i].pitch = randomPitch ? (UnityEngine.Random.Range(pitchRange.x, pitchRange.y)) : 1;
                    _source[i].PlayOneShot(clips[(int)sound]);
                    break;
                }
            }
        }

        public void PlaySound(SoundType sound, float volume, float minPitch, float maxPitch)
        {
            if(sound==SoundType.BUTTON_CLICK)
            {
                _source[_source.Length-1].volume = volume;
                _source[_source.Length-1].pitch = UnityEngine.Random.Range(minPitch, maxPitch);
                _source[_source.Length-1].PlayOneShot(clips[(int)sound]);
                return;
            }
            for (int i = 0; i < _source.Length-1; i++)
            {
                if (!_source[i].isPlaying || i == _source.Length - 1)
                {
                    _source[i].volume = volume;
                    _source[i].pitch = UnityEngine.Random.Range(minPitch, maxPitch);
                    _source[i].PlayOneShot(clips[(int)sound]);
                    break;
                }
            }
        }

        public void ChangeVolumeOfSound(SoundType sound, float volume)
        {
            if (!Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BeeSoundSystem>().Enabled)
            {
                _source[0].volume = 0;
                return;
            }
                
            // Source is 0 because it's the bee buzz sound
            _source[0].volume = volume;
        }
        public void ChangeVolumeOfSound(int source, float volume)
        {
            if (!Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BeeSoundSystem>().Enabled)
            {
                _source[source].volume = 0;
            } else
            {
                _source[source].volume = volume;
            }
                
        }

        public void ChangePitchOfPlayerBee(float pitch)
        {
            if (!Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BeeSoundSystem>().Enabled)
            {
                _source[1].volume = 0;
                return;
            }

            _source[1].pitch = pitch;
        }

        public void ChangeVolumeAndPitchOfBeeBuzz(int source, float volume, float pitch)
        {
            if (Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BeeSoundSystem>().Enabled)
            {
                _beeSources[source].volume = volume;
                _beeSources[source].pitch = pitch;
            } else
            {
                _beeSources[source].volume = 0;
            }
            
        }

        public void PlayLoopSound(SoundType sound)
        {
            for (int i = 0; i < _source.Length - 1; i++)
            {
                if (!_source[i].isPlaying || i == _source.Length - 1)
                {
                    _source[i].volume = 0;
                    _source[i].pitch = 1;
                    _source[i].clip = clips[(int)sound];
                    _source[i].loop = true;
                    _source[i].Play();
                    break;
                }
            }

        }

        public void PauseAmbient(bool pause)
        {
            StopCoroutine(FadeInOutAmbient(1, !pause));
            StartCoroutine(FadeInOutAmbient(1, pause));
        }

        IEnumerator FadeInOutAmbient(float duration, bool fadeOut)
        {
            float elapsed = 0;
            while(elapsed<=duration)
            {
                if(fadeOut)
                {
                    _ambient.volume = math.lerp(1,0, elapsed / duration);
                } else
                {
                    _ambient.volume = math.lerp(0, 1, elapsed / duration);
                }
               
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}

