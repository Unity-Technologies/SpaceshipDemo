using UnityEngine;
using UnityEngine.UI;
using GameOptionsUtility;
using GameOptionsUtility.HDRP;
using UnityEngine.Rendering.HighDefinition;

public class UpsamplingDebug : MonoBehaviour
{
    [SerializeField]
    Text aaMethod;
    [SerializeField]
    Text percentage;
    [SerializeField]
    Text upsampling;

    private void OnEnable()
    {
        Refresh();
        GameOptions.onApply += GameOptions_onApply;
    }

    private void GameOptions_onApply()
    {
        Refresh();
    }

    private void OnDisable()
    {
        GameOptions.onApply -= GameOptions_onApply;
    }

    void Refresh()
    {
        var camOption = GameOption.Get<HDRPCameraOption>();
        var grpOption = GameOption.Get<SpaceshipOptions>();

        aaMethod.text = camOption.antiAliasing.ToString();
        percentage.text = $"{grpOption.screenPercentage}% {(grpOption.screenPercentage == 100? "(Native)":"")}";
        if(grpOption.screenPercentage == 100)
        {
            upsampling.text = $"Disabled ({grpOption.upsamplingMethod})";
        }
        else
            upsampling.text = grpOption.upsamplingMethod.ToString();
    }


    private void Update()
    {
        int shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? -1 : 1;

        if(Input.GetKeyDown(KeyCode.F1))
        {
            var camOption = GameOption.Get<HDRPCameraOption>();
            int val = (int)camOption.antiAliasing + 1 * shift;
            if (val > 3)
                val = 0;
            if (val < 0)
                val = 3;
            camOption.antiAliasing = (HDAdditionalCameraData.AntialiasingMode)val;
            GameOptions.Apply();
            Refresh();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            var grpOption = GameOption.Get<SpaceshipOptions>();
            int val = grpOption.screenPercentage + 10 * shift;
            if (val > 100)
                val = 50;
            if (val < 50)
                val = 100;
            grpOption.screenPercentage = val;
            GameOptions.Apply();
            Refresh();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            var grpOption = GameOption.Get<SpaceshipOptions>(); 
            int val = (int)grpOption.upsamplingMethod + 1 * shift;
            if (val > 4)
                val = 0;
            if (val < 0)
                val = 4;

            grpOption.upsamplingMethod = (SpaceshipOptions.UpsamplingMethod)val;
            GameOptions.Apply();
            Refresh();
        }
    }
}
