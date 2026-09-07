using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Deleted PlayerInteract dublicate");
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Settings")]
    public bool debugMode;

    [Header ("Other")]

    //VFX
    public VolumeProfile globalVolume;

    //Volume components
    public ChromaticAberration chromaticAberration;
    public LensDistortion lensDistortion;

    [HideInInspector]public Map currentMap;
    bool timeStopped;
    float timeStopEffectWeight;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        ChromaticAberration cA;
        globalVolume.TryGet<ChromaticAberration>(out cA);
        chromaticAberration = cA;

        LensDistortion lD;
        globalVolume.TryGet<LensDistortion>(out lD);
        lensDistortion = lD;
    }

    // Update is called once per frame
    void Update()
    {
        TimeStopping();

        if(timeStopped && timeStopEffectWeight < 0.90f)
        {
            timeStopEffectWeight = Mathf.Lerp(timeStopEffectWeight, 1.0f, Time.unscaledDeltaTime * 10f);
        }

        if(!timeStopped && timeStopEffectWeight >0.10f)
        {
            timeStopEffectWeight = Mathf.Lerp(timeStopEffectWeight, 0.0f, Time.unscaledDeltaTime * 10f);
        }

        chromaticAberration.intensity.value = timeStopEffectWeight;
        lensDistortion.intensity.value = timeStopEffectWeight * 0.1f;
    }

    void TimeStopping()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!timeStopped)
            {
                timeStopped = true;
                Time.timeScale = 0;
            }
            else
            {
                timeStopped = false;
                Time.timeScale = 1;
            }
        }
    }
}
