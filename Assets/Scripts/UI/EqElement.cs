using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EqElement : MonoBehaviour
{
    public EqManager manager;
    public EqManager.EqValuesStruct valuesStruct;
    public GameObject selectButton;

    [SerializeField] private TextMeshProUGUI partNameText;
    [SerializeField] private Image partImage;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Image barFillImage;

    public void RefreshUI()
    {
        partNameText.text = valuesStruct.name;
        conditionText.text = valuesStruct.condition.ToString() + "%";

        float percentProgress = (float)valuesStruct.condition / 100;
        progressBar.value = percentProgress;

        Color barColor = Color.Lerp(Color.red, Color.green, percentProgress);
        barFillImage.color = barColor;

        if (valuesStruct.sprite)
            partImage.sprite = valuesStruct.sprite;
    }

    public void MountThis()
    {
        manager.MountElement(valuesStruct);
    }
}
