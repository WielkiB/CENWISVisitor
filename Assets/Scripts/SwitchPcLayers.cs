using System.Collections.Generic;
using UnityEngine;

public class SwitchPcLayers : MonoBehaviour
{
    public GameUiManager uiManager;

    [SerializeField] BoxCollider leftPanelCol;
    [SerializeField] BoxCollider rightPanelCol;

    [SerializeField] private List<PartManager> leftPanelDependencies;
    [SerializeField] private List<PartManager> rightPanelDependencies;

    // Sprawdzanie, czy zamontowane s� wszystkie podzespo�y wewn�trzne dla danej komory obudowy
    // W�wczas umo�liwiany jest monta� danego panelu
    public void CheckAllMounted()
    {
        bool allMounted = true;

        foreach (var part in leftPanelDependencies)
        {
            if (!part.isMounted)
            {
                allMounted = false;
                break;
            }
        }

        if (allMounted)
            leftPanelCol.enabled = true;
        else if (!leftPanelCol.gameObject.GetComponent<PartManager>().isMounted)
            leftPanelCol.enabled = false;

        allMounted = true;

        foreach (var part in rightPanelDependencies)
        {
            if (!part.isMounted)
            {
                allMounted = false;
                break;
            }
        }

        if (allMounted)
            rightPanelCol.enabled = true;
        else if (!rightPanelCol.gameObject.GetComponent<PartManager>().isMounted)
            rightPanelCol.enabled = false;
    }
}
