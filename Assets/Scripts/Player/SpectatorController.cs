using Assets.Scripts.Enums;
using Assets.Scripts.Player;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using Assets.Scripts.World;
using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class SpectatorController : MonoBehaviour
{
    public static SpectatorController Instance { get { return _instance; } }
    private static SpectatorController _instance;

    public event Action OnPressKey;
    public event Action<float3> OnMove;
    public event Action<ActionType, DataUnion> OnDoAction;

    [Header("References")]
    [SerializeField] private GameObject bee;
    [SerializeField] private ProgressBarHandler progressBar;
    [SerializeField] private new Camera camera;
    [SerializeField] private GameObject interactUI;
    [SerializeField] private GameObject emptyUI;
    [SerializeField] private TMP_Text polenText;
    [SerializeField] private TMP_Text nectarText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private RectTransform compassArrowTransform;

    [Header("Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float slowSpeed;
    [SerializeField] private float scrollRate;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    [SerializeField] private float sqDistanceToInteract;

    private float _yaw;
    private float _pitch;
    private bool _mouseLocked = false;
    // Pivot variable
    private Vector3 _vector = Vector3.zero;

    private bool _followMode;
    private bool _spectatorMode;
    private float _zoom = 5;

    // Bee state
    private bool _recollecting;
    private int _pollenRecollected;
    private int _nectarRecollected;
    private float _energy = 300;
    private float _currentSpeed;

    private void Awake()
    {
        if(_instance==null)
        {
            _instance = this;
        } else if(_instance != this){
            Destroy(this);
        }
        _followMode = false;
        _spectatorMode = true;
        _currentSpeed = speed;
    }

    private void Update()
    {
        if (MenuHandler.Instance.Paused || MenuHandler.Instance.Loading)
            return;

        if (Input.anyKeyDown)
        {
            OnPressKey?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _followMode = (_followMode || !SelectionHandler.Instance.EntitySelected) ? false : true;
            MenuHandler.Instance.SelectEntity(SelectionHandler.Instance.EntitySelected, _followMode);
            if (_followMode)
            {
                OnDoAction?.Invoke(ActionType.FOLLOW, new DataUnion { EntityType = SelectionHandler.Instance.DictDataTypeData[DataType.ENTITY_TYPE].EntityType });
            }
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            _spectatorMode = !_spectatorMode;
            if (_spectatorMode)
            {
                // Handle change to spectator mode
                bee.SetActive(false);
                interactUI.SetActive(false);
                polenText.gameObject.SetActive(false);
                energyText.gameObject.SetActive(false);
                transform.position = bee.transform.position;
                SoundManager.Instance.PlaySound(SoundType.CLICK_ON_BEE, 1, 0.6f, 0.9f);
                SoundManager.Instance.ChangeVolumeOfSound(1, 0);
            }
            else
            {
                // Handle change to player mode
                bee.SetActive(true);
                bee.transform.position = transform.position /*+ transform.forward * _zoom*/;
                _followMode = false;

                polenText.gameObject.SetActive(true);
                energyText.gameObject.SetActive(true);
                OnDoAction?.Invoke(ActionType.SWITCH_TO_PLAYER_MODE, new DataUnion { Int = 0 });
                SoundManager.Instance.PlaySound(SoundType.CLICK_ON_BEE, 1, 1.2f, 1.4f);
            }
            MenuHandler.Instance.SwitchMode(_spectatorMode);
        }
        if(_followMode || !_spectatorMode)
            HandleZoom();

        if (!_followMode)
            HandleMovement();
    }

    public void MyLateUpdate()
    {
        if (MenuHandler.Instance.Paused || MenuHandler.Instance.Loading)
            return;

        if (_followMode)
            HandleMovement();
    }
    private void HandleZoom()
    {
        _zoom = Math.Clamp(_zoom+ (scrollRate * -Input.mouseScrollDelta.y), minZoom, maxZoom);
        SoundManager.Instance.ChangeVolumeOfSound(1, math.lerp(1, 0, _zoom / maxZoom));
    }

    private void HandleMovement()
    {
        float3 prevPosition = transform.position;
        _yaw += (_spectatorMode?1:-1)*SettingsHandler.Instance.Sensitivity * Input.GetAxis("Mouse X");
        _pitch -= SettingsHandler.Instance.Sensitivity * Input.GetAxis("Mouse Y");
        _pitch = Mathf.Clamp(_pitch, -89, 89);
        _yaw %= 360;
        if(!_spectatorMode)
        {
            // Player mode
            
            if(_energy < 0)
            {
                _currentSpeed = slowSpeed;
                energyText.text = "" + 0;
            } else
            {
                energyText.text = "" + (int)_energy;
                _energy -= Time.deltaTime;
            }

            // Handle player movement
            if (!_recollecting)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    _vector += transform.forward;
                    _vector.y = 0;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    _vector -= transform.forward;
                    _vector.y = 0;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    _vector -= transform.right;
                    _vector.y = 0;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    _vector += transform.right;
                    _vector.y = 0;
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    _vector.y += 1;
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _vector.y -= 1;
                }
                _vector = Vector3.Normalize(_vector);
                bee.transform.position += _vector * _currentSpeed * Time.deltaTime;
                _vector.Set(0, 0, 0);
                float collisionHeight = BeeWorld.Instance.GetHeightAt(bee.transform.position) + 1.0f;
                bee.transform.position = math.clamp(bee.transform.position, new float3(0, collisionHeight, 0), new float3(Globals.CHUNK_SIZE * Globals.WORLD_SIZE, 100, Globals.CHUNK_SIZE * Globals.WORLD_SIZE));
                bee.transform.rotation = Quaternion.Euler(new float3(0, transform.rotation.eulerAngles.y, 0));
            }
            // Handle camera
            float3 deltaPos = new float3(
                                x: math.cos(math.radians(_yaw)) * _zoom,
                                y: math.sin(math.radians(_pitch)) * _zoom,
                                z: math.sin(math.radians(_yaw)) * _zoom
                                );
            float3 goalPosition = (float3)bee.transform.position + deltaPos * new float3((SettingsHandler.Instance.InvertMouseX ? -1 : 1),
                (SettingsHandler.Instance.InvertMouseY ? -1 : 1), (SettingsHandler.Instance.InvertMouseX ? -1 : 1));
            transform.position = math.lerp(transform.position, goalPosition, Time.deltaTime * 7);
            transform.position = goalPosition;
            ClampInWorldTerrain();
            //
            quaternion prevRot = transform.rotation;
            transform.LookAt(bee.transform.position);

            //float3 flowerPosition = BeeWorld.Instance.GetChunkFromWorldPosition(bee.transform.position).FlowerPostion;
            PolenSourceData polenSource = BeeWorld.Instance.GetClosestFlower(bee.transform.position, 1, -1);
            float3 flowerPosition = polenSource.FlowerPosition;
            //Debug.Log("Chunk: " + BeeWorld.WorldToChunk((float3)bee.transform.position) + ", " + flowerPosition);
            if(!_recollecting && math.distancesq(flowerPosition, bee.transform.position)<= sqDistanceToInteract)
            {
                // Show pop-up text
                FlowerData flower;
                lock (BeeWorld.Instance)
                {
                    flower = BeeWorld.Instance.GetFlowerData(polenSource.PolenSourceId, polenSource.ChunkCoord);
                }
                
                if (Input.GetKeyDown(KeyCode.E))
                {
                    
                    if (flower.Pollen > 0 || flower.Nectar > 0)
                    {
                        // Interact
                        interactUI.SetActive(false);
                        progressBar.Show(flowerPosition + new float3(0, 3, 0));
                        StartCoroutine(Recollect(polenSource, 5));
                    }
                    
                }
                else
                {

                    if (flower.Pollen > 0 || flower.Nectar > 0)
                    {
                        interactUI.SetActive(true);
                        emptyUI.SetActive(false);
                    }
                    else
                    {
                        emptyUI.SetActive(true);
                        interactUI.SetActive(false);
                    }
                }                
            } else if(!_recollecting && math.distancesq(bee.transform.position, BeeWorld.Instance.NestPosition)<=sqDistanceToInteract)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Interact
                    interactUI.SetActive(false);
                    progressBar.Show(BeeWorld.Instance.NestPosition + new float3(0, 3, 0));
                    StartCoroutine(Rest(5));
                }
                else
                {
                    interactUI.SetActive(true);
                }
            }
            else if(!_recollecting)
            {
                interactUI.SetActive(false);
                emptyUI.SetActive(false);
            }
            
        }
        else if(_followMode)
        {
            float3 deltaPos = new float3(
                                x: math.cos(math.radians(_yaw)) * _zoom,
                                y: math.sin(math.radians(_pitch)) * _zoom,
                                z: math.sin(math.radians(_yaw)) * _zoom
                                );
            float3 goalPosition = SelectionHandler.Instance.DictDataTypeData[Assets.Scripts.Enums.DataType.POSITION].Float3 + deltaPos * new float3(SettingsHandler.Instance.InvertMouseX ? -1 : 1, 
                SettingsHandler.Instance.InvertMouseY ? -1 : 1, SettingsHandler.Instance.InvertMouseX ? -1 : 1);
            transform.position = math.lerp(transform.position, goalPosition, Time.deltaTime*7);
            transform.position = goalPosition;
            ClampInWorldTerrain();
            //
            quaternion prevRot = transform.rotation;
            transform.LookAt(SelectionHandler.Instance.DictDataTypeData[Assets.Scripts.Enums.DataType.POSITION].Float3);
            //transform.rotation = math.slerp(prevRot, transform.rotation, Time.deltaTime * 30);
            
        } else
        {
            transform.eulerAngles = new Vector3(_pitch, _yaw, 0);
            // TODO: Do not use old input system.
            if (Input.GetKey(KeyCode.W))
            {
                _vector += transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                _vector -= transform.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                _vector -= transform.right;
            }
            if (Input.GetKey(KeyCode.D))
            {
                _vector += transform.right;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                _vector.y += 1;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _vector.y -= 1;
            }
            _vector = Vector3.Normalize(_vector);
            transform.position += _vector * speed * Time.deltaTime;
            ClampInWorldTerrain();
            _vector.Set(0, 0, 0);
        }

        // Update compass to nest
        float3 dirToNest = math.normalize((BeeWorld.Instance.NestPosition - (float3)transform.position) * new float3(1, 0, 1));
        float3 forwardVector = transform.forward * new float3(1,0,1);

        float a = math.atan2(dirToNest.z, dirToNest.x);
        float b = math.atan2(forwardVector.z, forwardVector.x);

        float angle = math.degrees(a - b);
        compassArrowTransform.rotation = Quaternion.Euler(0,0,angle);

        float distSqToNest = math.distancesq(BeeWorld.Instance.NestPosition, transform.position);
        SoundManager.Instance.ChangeVolumeOfSound(SoundType.NEST_BUZZ, math.lerp(0.8f, 0, distSqToNest / Globals.MAX_DISTANCE_FOR_SOUND));

        // Invoke On Move
        if (!prevPosition.Equals(transform.position))
        {
            OnMove?.Invoke(transform.position);
        }
        
    }

    private IEnumerator Recollect(PolenSourceData polenSource, float duration)
    {
        float3 flowerPosition = polenSource.FlowerPosition;
        SoundManager.Instance.PlaySound(SoundType.FINISH_RECOLLECTION, 1, false);
        SoundManager.Instance.ChangePitchOfPlayerBee(UnityEngine.Random.Range(1.2f, 1.5f));
        float elapsed = 0;
        _recollecting = true;
        
        while (elapsed<=duration)
        {
            // Update progress bar
            progressBar.UpdateState(elapsed/duration, camera.transform.position);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _recollecting = false;
        lock (BeeWorld.Instance){
            FlowerData flower = BeeWorld.Instance.GetFlowerData(polenSource.PolenSourceId, polenSource.ChunkCoord);
            int pollenToCollect = Mathf.Min(UnityEngine.Random.Range(10, 30), flower.Pollen < 0 ? 0 : flower.Pollen);
            int nectarToCollect = Mathf.Min(UnityEngine.Random.Range(10, 30), flower.Nectar<0?0:flower.Nectar);
            _pollenRecollected += pollenToCollect;
            _nectarRecollected += nectarToCollect;
            flower.Pollen -= pollenToCollect;
            flower.Nectar -= _nectarRecollected;
            BeeWorld.Instance.SetFlowerData(polenSource.PolenSourceId, polenSource.ChunkCoord, flower);
        }

        polenText.text = "" + _pollenRecollected;
        nectarText.text = "" + _nectarRecollected;

        progressBar.Hide();
        SoundManager.Instance.PlaySound(SoundType.FINISH_RECOLLECTION, 1, 0.6f,0.62f);
        OnDoAction?.Invoke(ActionType.COLLECT, new DataUnion { Int = _pollenRecollected });
        OnDoAction?.Invoke(ActionType.COLLECT_NECTAR, new DataUnion { Int = _nectarRecollected });
        SoundManager.Instance.ChangePitchOfPlayerBee(UnityEngine.Random.Range(0.9f, 1.1f));
    }

    private IEnumerator Rest(float duration)
    {
        SoundManager.Instance.PlaySound(SoundType.CLICK_ON_NEST, 1, false);
        OnDoAction?.Invoke(ActionType.REST, new DataUnion { Int = 0 });
        float elapsed = 0;
        _recollecting = true;
        bee.SetActive(false);
        while (elapsed <= duration)
        {
            // Update progress bar
            progressBar.UpdateState(elapsed / duration, camera.transform.position);
            // Move bee randomly
            elapsed += Time.deltaTime;
            yield return null;
        }
        _recollecting = false;
        bee.SetActive(true);
        progressBar.Hide();
        _currentSpeed = speed;
        _energy = Globals.BEE_MAX_ENERGY;
        BeeWorld.Instance.NestSystem.PolenRequestPlayer = new PolenRequest { Value = _pollenRecollected, IsPollen = true};
        BeeWorld.Instance.NestSystem.NectarRequestPlayer = new PolenRequest { Value = _nectarRecollected, IsPollen = false};
        _pollenRecollected = 0;
        _nectarRecollected = 0;
        polenText.text = "" + _pollenRecollected;
        nectarText.text = "" + _nectarRecollected;
        SoundManager.Instance.PlaySound(SoundType.CLICK_ON_NEST, 1, 0.5f, 0.52f);
    }

    private void ClampInWorldTerrain()
    {
        float collisionHeight = BeeWorld.Instance.GetHeightAt(transform.position) + 1.0f;
        transform.position = math.clamp(transform.position, new float3(0, collisionHeight, 0), new float3(Globals.CHUNK_SIZE * Globals.WORLD_SIZE, 100, Globals.CHUNK_SIZE * Globals.WORLD_SIZE));
    }
}
