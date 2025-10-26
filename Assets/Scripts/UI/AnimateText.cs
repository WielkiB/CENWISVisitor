using System.Collections;
using TMPro;
using UnityEngine;

public class AnimateText : MonoBehaviour
{
    public string textCard;

    public PauseController pauseControllerRef;
    public TextMeshProUGUI storyTextBar;

    public float letterWaitTime = 0.03f;

    public void ShowStoryBar()
    {
        storyTextBar.text = string.Empty;
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        storyTextBar.text = string.Empty;

        // pêtla iteruje po literach s³ów opisu
        for (int j = 0; j < textCard.Length; j++)
        {
            // sprawdza czy natrafiono na znacznik nowej linii
            if (textCard.IndexOf("<br>", j) == j)
            {
                // je¿eli warunek jest spe³niony to dodaje znacznik nowej linii
                storyTextBar.text += "<br>";
                j += 3;
            }
            else
            {
                // dokleja do tekstu kolejn¹ literê
                storyTextBar.text += textCard[j];
            }

            // opóŸnienie do wyœwietlenia nastêpnego znaku
            yield return new WaitForSeconds(letterWaitTime);
        }

        yield return null;
    }
}
