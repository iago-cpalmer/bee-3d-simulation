using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class ProgressBarHandler : MonoBehaviour
    {
        [SerializeField] private GameObject progressBar;
        [SerializeField] private MeshRenderer progressMeshRenderer;
        [SerializeField] private MeshRenderer progressMeshRenderer2;
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;

        public void UpdateState(float progress, float3 lookAtPosition)
        {
            progressBar.transform.localScale = new float3(progress, 1,1);
            progressMeshRenderer.material.color = Color.Lerp(startColor, endColor, progress);
            progressMeshRenderer2.material.color = Color.Lerp(startColor, endColor, progress);
            transform.LookAt(lookAtPosition);
            transform.rotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }

        public void UpdateState(float progress)
        {
            progressBar.transform.localScale = new float3(progress, 1, 1);
            progressMeshRenderer.material.color = Color.Lerp(startColor, endColor, progress);
            //progressMeshRenderer2.material.color = Color.Lerp(startColor, endColor, progress);
        }

        public void Show(float3 worldPosition)
        {
            gameObject.transform.position = worldPosition;
           // progressBar.transform.position = (float3)gameObject.transform.position + new float3(0,0, -0.0025f);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
