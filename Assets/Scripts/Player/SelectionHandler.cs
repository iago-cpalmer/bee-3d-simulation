using Assets.Scripts.Enums;
using Assets.Scripts.Sound;
using Assets.Scripts.Systems;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class SelectionHandler : MonoBehaviour
    {
        public static SelectionHandler Instance { get { return _instance; } }
        private static SelectionHandler _instance;

        public bool EntitySelected { get { return _entitySelected; } }
        public event Action<float3, float3, float> OnClick;
        public Dictionary<DataType, DataUnion> DictDataTypeData;
        
        public event Action<EntityType> OnSelectEntity;
        public event Action OnUnselectEntity;
       

        [SerializeField] private float selectionDistance;
        [SerializeField] private Transform selectionMarker;
        [SerializeField] private Transform selectionMarkerTop;
        [SerializeField] private new Camera camera;

        private bool _entitySelected;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(this);
                return;
            }
            DictDataTypeData = new Dictionary<DataType, DataUnion>();
            foreach(DataType d in Enum.GetValues(typeof(DataType)))
            {
                DictDataTypeData.Add(d, new DataUnion { Quaternion=Quaternion.identity});
            }
        }
        private void Start()
        {
            SelectionSystem.Instance.OnSelectEntity += SelectEntity;
            SelectionSystem.Instance.OnUnselectEntity += UnselectEntity;
        }

        private void Update()
        {
            if (MenuHandler.Instance.Paused || MenuHandler.Instance.Loading)
            {
                return;
            }
                
            if (Input.GetMouseButtonDown(0))
            {
                OnClick?.Invoke(camera.transform.position, camera.transform.forward, selectionDistance);
            }
            
        }

        public void MyLateUpdate()
        {
            if (MenuHandler.Instance.Paused || MenuHandler.Instance.Loading)
                return;
            //if(_entitySelected)
            if (_entitySelected)
            {
                DictDataTypeData[DataType.POSITION] = new DataUnion { Float3 = SelectionSystem.Instance.GetEntityPos() };
                selectionMarker.position = DictDataTypeData[DataType.POSITION].Float3 + new float3(0, 1, 0);
                selectionMarkerTop.LookAt(camera.transform, new float3(0, 1, 0));
            }
        }

        private void SelectEntity()
        {
            //Debug.Log("Selected entity");
            selectionMarker.gameObject.SetActive(true);
            _entitySelected = true;
            MenuHandler.Instance.SelectEntity(true, false);
            OnSelectEntity?.Invoke(DictDataTypeData[DataType.ENTITY_TYPE].EntityType);
            if (DictDataTypeData[DataType.ENTITY_TYPE].EntityType==EntityType.BEE_NEST)
            {
                SoundManager.Instance.PlaySound(SoundType.CLICK_ON_NEST, 1, true);
            } else
            {
                SoundManager.Instance.PlaySound(SoundType.CLICK_ON_BEE, 1, 0.6f, 0.9f);
            }
        }

        private void UnselectEntity()
        {
            //Debug.Log("Unselected entity");
            selectionMarker.gameObject.SetActive(false);
            _entitySelected = false;
            MenuHandler.Instance.SelectEntity(false, false);
            OnUnselectEntity?.Invoke();
        }

        private void OnDestroy()
        {
            SelectionSystem.Instance.OnSelectEntity -= SelectEntity;
            SelectionSystem.Instance.OnUnselectEntity -= UnselectEntity;
        }
    }
}
