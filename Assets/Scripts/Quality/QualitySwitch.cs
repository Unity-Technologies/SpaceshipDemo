using UnityEngine;
using GameOptionsUtility;

public class QualitySwitch : MonoBehaviour 
{
    [SerializeField]
    GameObject[] targetGameObjects;

    [SerializeField, QualityDropDown ]
    int targetQuality;
    enum Rule
    {
        Equal,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
        Different,
    }

    [SerializeField, Tooltip("The rule that will determine if defined targets become enabled or not, based on comparing Global QualityLevel with targetQuality.")]
    Rule rule;

    private void GameOptions_onApply()
    {
        UpdateQuality(QualitySettings.GetQualityLevel());
    }

    public void UpdateQuality(int newQuality)
    {
        bool newActive = false;

        switch (rule)
        {
            default:
            case Rule.Equal:
                newActive = (newQuality == targetQuality);
                break;
            case Rule.Greater:
                newActive = (newQuality > targetQuality);
                break;
            case Rule.GreaterOrEqual:
                newActive = (newQuality >= targetQuality);
                break;
            case Rule.Less:
                newActive = (newQuality < targetQuality);
                break;
            case Rule.LessOrEqual:
                newActive = (newQuality <= targetQuality);
                break;
            case Rule.Different:
                newActive = (newQuality != targetQuality);
                break;
        }

        foreach (var gameObject in targetGameObjects)
        {
            if (gameObject == null)
                continue;

            if (gameObject.activeSelf != newActive)
                gameObject.SetActive(newActive);
        }
    }

    private void OnEnable()
    {
        GameOptions.onApply += GameOptions_onApply;
        GameOptions_onApply();
    }


    private void OnDisable()
    {
        GameOptions.onApply -= GameOptions_onApply;
    }

}
