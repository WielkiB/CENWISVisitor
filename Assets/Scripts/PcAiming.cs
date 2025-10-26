using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PcAiming : MonoBehaviour
{
    public GameUiManager uiManager;
    public EqManager eqManager;
    public PlayerAiming playerAiming;
    public SwitchPcLayers switchPcLayers;

    public bool aimingEnabled = true;
    public LayerMask mountedPartMask;
    public LayerMask dismountedPartMask;
    public LayerMask blockMask;
    public int maxDistance = 6;
    public bool isMountingMode = false;

    private GameObject focusedObject;
    private PartManager focusedObjManager;
    private LayerMask currentPartMask;
    
    private void Start()
    {
        currentPartMask = mountedPartMask + blockMask;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!eqManager.gameObject.activeInHierarchy)
            {
                switchPcLayers.CheckAllMounted();

                isMountingMode = !isMountingMode;
                uiManager.ToggleMountDemount(isMountingMode);

                currentPartMask = isMountingMode ? dismountedPartMask + blockMask : mountedPartMask + blockMask;
            }  
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (eqManager.gameObject.activeInHierarchy)
            {
                eqManager.HideEQ();
            }
            else
            {
                RemoveOutline();
                playerAiming.EnterPlayerMode();
                gameObject.SetActive(false);
            }
            
            return;
        }

        // Wy³¹czanie podœwietlania w trybie obrotu kamery i wyœwietlonego EQ
        if (!aimingEnabled || eqManager.gameObject.activeInHierarchy)
        {
            RemoveOutline();
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        GameObject[] hitArray = MyRaycast.RaycastAll(ray, maxDistance, currentPartMask);
        List<GameObject> nonBlockedObjs = new List<GameObject>();

        foreach (var obj in hitArray)
        {
            if (!obj.CompareTag("Block"))
                nonBlockedObjs.Add(obj);
            else
                break;
        }

        if (nonBlockedObjs.Count > 0)
        {
            GameObject hitObject = nonBlockedObjs.First();

            if (isMountingMode)
                hitObject = nonBlockedObjs.Last();

            /*string abc = string.Empty;
            foreach (var obj in nonBlockedObjs)
            {
                abc += obj.name + "    ";
            }
            Debug.Log(abc + " obiekt trafiony: " + hitObject.name);*/

            // Wy³¹czenie obramowania poprzednio wskazanego obiektu
            if (focusedObject != hitObject)
                RemoveOutline();

            // Podœwietlenie nowo wskazanego obiektu
            focusedObject = hitObject;

            // Zabezpieczenie przed b³êdem z brakiem referencji do obiektu
            if (!focusedObject.GetComponent<PartManager>())
                return;

            focusedObjManager = focusedObject.GetComponent<PartManager>();

            focusedObjManager.ChangeMeshVisibility(true);

            // Ustawianie podœwietlenia czêœci na czerwono lub zielono
            if (focusedObject.GetComponent<CollidedParts>() && focusedObject.GetComponent<CollidedParts>().HighlightCollidedObjects(true))
                focusedObjManager.SetOutline(Color.red, false);
            else
                focusedObjManager.SetOutline(Color.green, true);
        }
        else
        {
            RemoveOutline();
        }
    }

    private void RemoveOutline()
    {
        if (focusedObject && focusedObjManager)
        {
            if (focusedObject.GetComponent<CollidedParts>())
                focusedObject.GetComponent<CollidedParts>().HighlightCollidedObjects(false);

            focusedObjManager.DisableOutline();

            if (!focusedObjManager.isMounted)
                focusedObjManager.ChangeMeshVisibility(false);
            
            focusedObject = null;
        }
    }
}
