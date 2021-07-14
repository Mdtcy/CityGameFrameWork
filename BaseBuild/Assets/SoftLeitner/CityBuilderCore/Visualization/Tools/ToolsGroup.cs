using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CityBuilderCore
{
    /// <summary>
    /// has one current tool and a selection of other tools that are shown on mouse over<br/>
    /// when one of the other tools gets activated it becomes the current tool<br/>
    /// </summary>
    public class ToolsGroup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject Current;
        public GameObject Other;

        private BaseTool _currentTool;
        private List<BaseTool> _otherTools;

        private void Start()
        {
            _currentTool = Current.GetComponentInChildren<BaseTool>();
            _otherTools = Other.GetComponentsInChildren<BaseTool>().ToList();

            _currentTool.Activating.AddListener(toolActivating);
            foreach (var tool in _otherTools)
            {
                tool.Activating.AddListener(toolActivating);
            }

            Other.SetActive(false);
        }

        private void toolActivating(BaseTool tool)
        {
            if (tool == _currentTool)
                return;

            _currentTool.transform.SetParent(Other.transform);
            _otherTools.Add(_currentTool);
            _otherTools.Remove(tool);
            _currentTool = tool;
            _currentTool.transform.SetParent(Current.transform);

            Other.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Other.transform.HasActiveChildren())
                return;

            Other.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Other.SetActive(false);
        }
    }
}