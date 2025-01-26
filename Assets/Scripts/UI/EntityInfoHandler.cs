using Assets.Scripts.Enums;
using Assets.Scripts.Player;
using Assets.Scripts.Systems;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class EntityInfoHandler : MonoBehaviour
    {

        [SerializeField] private GameObject infoPanel;
        [SerializeField] private TextMeshProUGUI infoText;

        private StringBuilder stringBuilder;
        private Dictionary<EntityType, string> EntityTypeName;


        private void Start()
        {

            QualitySettings.maxQueuedFrames = 5;
            //infoPanel = GameObject.FindGameObjectWithTag("InfoPanel");
            EntityTypeName = new Dictionary<EntityType, string>();
            infoPanel.SetActive(false);
            SelectionSystem.Instance.OnSelectEntity += OnSelectEntity;
            SelectionSystem.Instance.OnUnselectEntity += OnUnselectEntity;
            stringBuilder = new StringBuilder();
            EntityTypeName.Add(EntityType.BEE, "Bee");
            EntityTypeName.Add(EntityType.BEE_NEST, "Bee Nest");
        }

        private void Update()
        {
            if(SelectionHandler.Instance.EntitySelected)
            {
                stringBuilder.AppendLine(EntityTypeName[SelectionHandler.Instance.DictDataTypeData[DataType.ENTITY_TYPE].EntityType]);
                stringBuilder.Append("\n");
                switch (SelectionHandler.Instance.DictDataTypeData[DataType.ENTITY_TYPE].EntityType)
                {
                    case EntityType.BEE:
                        stringBuilder.Append("<size=30>");
                        stringBuilder.Append("Energy: ");
                        stringBuilder.Append(math.max((int)SelectionHandler.Instance.DictDataTypeData[DataType.ENERGY].Float, 0));
                        stringBuilder.AppendLine("</size>\n");
                        if((SelectionHandler.Instance.DictDataTypeData[DataType.STATE].BeeState==BeeStates.RECOLLECTING)) {
                            stringBuilder.Append("<size=30>");
                            stringBuilder.Append("State: ");
                            stringBuilder.Append("FORAGING");
                            stringBuilder.AppendLine("</size>\n");
                        }
                        else
                        {
                            stringBuilder.Append("<size=30>");
                            stringBuilder.Append("State: ");
                            stringBuilder.Append(SelectionHandler.Instance.DictDataTypeData[DataType.STATE].BeeState.ToString());
                            stringBuilder.AppendLine("</size>\n");
                        }
                        

                        stringBuilder.Append("<size=30>");
                        stringBuilder.Append("Age: ");
                        float age = SelectionHandler.Instance.DictDataTypeData[DataType.AGE].Float;
                        int days = (int)(age / Globals.DAY_DURATION);
                        int hours = (int)(((age % Globals.DAY_DURATION) / Globals.DAY_DURATION)*24);
                        stringBuilder.Append(days + "d " + hours + "h");
                        stringBuilder.AppendLine("</size>\n");

                        stringBuilder.Append("<size=30>");
                        stringBuilder.Append("Polen: ");
                        stringBuilder.Append(SelectionHandler.Instance.DictDataTypeData[DataType.POLEN_AMOUNT].Int);
                        stringBuilder.Append("</size>" + "\n\n");

                        stringBuilder.Append("<size=30>");
                        stringBuilder.Append("Nectar: ");
                        stringBuilder.Append((int)SelectionHandler.Instance.DictDataTypeData[DataType.NECTAR_AMOUNT].Float);
                        stringBuilder.Append("</size>" + "\n\n");

                        stringBuilder.Append("<size=30>");
                        stringBuilder.Append("Protein: ");
                        stringBuilder.Append(math.max((int)SelectionHandler.Instance.DictDataTypeData[DataType.PROT].Float, 0));
                        stringBuilder.Append("</size>" + "\n\n");

                        stringBuilder.Append("<size=30>");
                        stringBuilder.Append("Carbohydrates: ");
                        stringBuilder.Append(math.max((int)SelectionHandler.Instance.DictDataTypeData[DataType.CARBS].Float, 0));
                        stringBuilder.Append("</size>" + "\n\n");

                        stringBuilder.Append("<size=30>");
                        stringBuilder.Append("Fats: ");
                        stringBuilder.Append((math.max((int)SelectionHandler.Instance.DictDataTypeData[DataType.FATS].Float, 0)));
                        stringBuilder.Append("</size>" + "\n\n");

                        break;
                    case EntityType.BEE_NEST:
                        stringBuilder.Append("<size=30>");
                        stringBuilder.Append("Polen: ");
                        stringBuilder.Append(SelectionHandler.Instance.DictDataTypeData[DataType.POLEN_AMOUNT].Int);
                        stringBuilder.Append("</size>" +"\n\n");
                        stringBuilder.Append("<size=30>");
                        stringBuilder.Append("Nectar: ");
                        stringBuilder.Append(SelectionHandler.Instance.DictDataTypeData[DataType.NECTAR_AMOUNT].Int);
                        stringBuilder.Append("</size>" + "\n\n");
                        stringBuilder.Append("<size=30>");
                        stringBuilder.Append("Population: ");
                        stringBuilder.Append(SelectionHandler.Instance.DictDataTypeData[DataType.POPULATION].Int);
                        stringBuilder.Append("</size>");
                        break;
                }
                infoText.text = stringBuilder.ToString();
                stringBuilder.Clear();
            }
        }
        private void OnDestroy()
        {
            SelectionSystem.Instance.OnSelectEntity -= OnSelectEntity;
            SelectionSystem.Instance.OnUnselectEntity -= OnUnselectEntity;
        }

        private void OnSelectEntity()
        {
            infoPanel.SetActive(true);
        }

        private void OnUnselectEntity()
        {
            infoPanel.SetActive(false);
        }

    }
}
