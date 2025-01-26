using TMPro;
using UnityEngine;

public class DebugUIHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text fpsText;
    private bool _active;

    private void Start()
    {
        QualitySettings.maxQueuedFrames = 5;
        fpsText.gameObject.SetActive(_active);
    }
    void Update()
    {
        if(_active)
        {
            fpsText.text = "FPS: " + Mathf.FloorToInt(1 / Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            _active = _active ? false : true;
            fpsText.gameObject.SetActive(_active);
        }     
    }
}
