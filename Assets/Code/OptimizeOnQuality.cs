using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;

public class OptimizeOnQuality : MonoBehaviour
{
    [SerializeField] GameObject postProcessingVolume;

    // Start is called before the first frame update
    void Start()
    {
        // if "Medium" or higher
        if (QualitySettings.GetQualityLevel() >= 2)
            GetComponent<EdgeDetection>().enabled = true;

        // if "High" or higher
        if (QualitySettings.GetQualityLevel() >= 3)
        {
            GetComponent<PostProcessLayer>().enabled = true;
            postProcessingVolume.SetActive(true);
        }
    }
}
