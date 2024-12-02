using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;
public class PostProcessController : MonoBehaviour
{
    public static PostProcessController Instance;
    public enum PostProcessType
    {
        Bloom,
        LensDistortion,
        Vignette
    }
    [SerializeField]Volume Volume;
    Bloom BloomSetting;
    LensDistortion LensDistortionSetting;
    Vignette VignetteSetting;

    private void Start()
    {
        Instance = this;
        Volume.profile.TryGet(out BloomSetting);
        Volume.profile.TryGet(out LensDistortionSetting);
        Volume.profile.TryGet(out VignetteSetting);
    }

    public void SetValue(PostProcessType Type, float Value)
    {
        switch (Type)
        {
            case PostProcessType.Bloom:
                BloomSetting.intensity.value = Value;
                break;
            case PostProcessType.LensDistortion:
                LensDistortionSetting.intensity.value = Value;
                break;
            case PostProcessType.Vignette:
                VignetteSetting.intensity.value = Value;
                break;
        }
    }
    public void ResetPostProcessing()
    {
        BloomSetting.intensity.value = 4;
        LensDistortionSetting.intensity.value = 0;
        VignetteSetting.intensity.value = 0;

    }
}
