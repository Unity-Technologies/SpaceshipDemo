using GameOptionsUtility;
using UnityEngine;
using UnityEngine.Rendering;

public class SpaceshipOptions : GameOption
{
    public class Preferences
    {
        public const string prefix = GameOptions.Preferences.prefix + "Spaceship.";
        public const string screenPercentage = prefix + "ScreenPercentage";
        public const string keyboardScheme = prefix + "FPSKeyboardScheme";
    }

    public enum FPSKeyboardScheme
    {
        WASD = 0,
        IJKL = 1,
        ZQSD = 2
    }


    public FPSKeyboardScheme fpsKeyboardScheme
    {
        get => (FPSKeyboardScheme)PlayerPrefs.GetInt(Preferences.keyboardScheme, (int)FPSKeyboardScheme.WASD);
        set => PlayerPrefs.SetInt(Preferences.keyboardScheme, (int)value);
    }


    public int screenPercentage
    {
        get 
        { 
            if(m_ScreenPercentage == -1) 
                m_ScreenPercentage = PlayerPrefs.GetInt(Preferences.screenPercentage, 100);

            return m_ScreenPercentage;
        }
        set 
        {
            m_ScreenPercentage = value;
            PlayerPrefs.SetInt(Preferences.screenPercentage, m_ScreenPercentage); 
        }
    }

    int m_ScreenPercentage = -1;
    bool init = false;
    public override void Apply()
    {
        if(!init)
        {
            DynamicResolutionHandler.SetDynamicResScaler(SetDynamicResolutionScale, DynamicResScalePolicyType.ReturnsPercentage);
            init = true;
        }

        UpdateFPSControlScheme();
    }

    public FPSKeys fpsKeys { get; private set; }
    public class FPSKeys
    {
        public readonly KeyCode forward;
        public readonly KeyCode back;
        public readonly KeyCode left;
        public readonly KeyCode right;
        public FPSKeys(KeyCode forward, KeyCode left, KeyCode back, KeyCode right)
        {
            this.forward = forward;
            this.back = back;
            this.left = left;
            this.right = right;
        }
    }

    void UpdateFPSControlScheme()
    {
        switch (fpsKeyboardScheme)
        {
            default:
            case FPSKeyboardScheme.WASD:
                fpsKeys = new FPSKeys(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D);
                break;
            case FPSKeyboardScheme.IJKL:
                fpsKeys = new FPSKeys(KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L);
                break;
            case FPSKeyboardScheme.ZQSD:
                fpsKeys = new FPSKeys(KeyCode.Z, KeyCode.Q, KeyCode.S, KeyCode.D);
                break;
        }
    }

    float SetDynamicResolutionScale()
    {
        return screenPercentage;
    }
}
