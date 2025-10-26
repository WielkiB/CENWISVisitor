using TMPro;
using UnityEngine;

public class GameUiManager : MonoBehaviour
{
    private Color32 normalColor = new Color32(255, 255, 255, 255);
    private Color32 grayedColor = new Color32(190, 190, 190, 255);

    public GameObject mountDemountUI;
    [SerializeField]
    private TextMeshProUGUI mountingText;
    [SerializeField]
    private TextMeshProUGUI dismountingText;

    public PauseController pauseController;

    public void ToggleMountDemount(bool isMounting)
    {
        if (isMounting)
        {
            dismountingText.fontStyle = FontStyles.Normal;
            dismountingText.color = grayedColor;

            mountingText.fontStyle = FontStyles.Bold;
            mountingText.color = normalColor;
        }
        else
        {
            mountingText.fontStyle = FontStyles.Normal;
            mountingText.color = grayedColor;

            dismountingText.fontStyle = FontStyles.Bold;
            dismountingText.color = normalColor;
        }
    }
}
