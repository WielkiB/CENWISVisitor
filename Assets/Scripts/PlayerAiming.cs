using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    public GameUiManager uiManager;
    public EqManager eqManager;
    public GameObject playerObject;
    public GameObject crosshair;

    public LayerMask mask;
    public int maxDistance = 6;

    private GameObject focusedObject;

    private void EnterPcMode()
    {
        focusedObject.GetComponent<Outline>().enabled = false;
        playerObject.GetComponent<MeshRenderer>().enabled = false;
        playerObject.GetComponent<PlayerController>().enabled = false;
        crosshair.SetActive(false);

        focusedObject.transform.GetChild(1).gameObject.SetActive(true);
        gameObject.SetActive(false);

        uiManager.mountDemountUI.SetActive(true);
    }

    public void EnterPlayerMode()
    {
        uiManager.mountDemountUI.SetActive(false);

        playerObject.GetComponent<MeshRenderer>().enabled = true;
        playerObject.GetComponent<PlayerController>().enabled = true;
        crosshair.SetActive(true);

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (MyRaycast.Raycast(transform.position, transform.forward, out var hit, maxDistance, mask))
        {
            focusedObject = hit;
            focusedObject.GetComponent<Outline>().enabled = true;
        }
        else if (focusedObject != null)
        {
            focusedObject.GetComponent<Outline>().enabled = false;
            focusedObject = null;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && focusedObject != null && !eqManager.catalogShowing)
        {
            EnterPcMode();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (eqManager.catalogShowing)
                eqManager.HideEQ();
            else
                eqManager.ShowCatalog();

            playerObject.GetComponent<PlayerController>().enabled = !eqManager.catalogShowing;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (eqManager.catalogShowing)
                eqManager.HideEQ();
            else
                uiManager.pauseController.ChangePauseState(true);

            playerObject.GetComponent<PlayerController>().enabled = !eqManager.catalogShowing;
        }
    }
}
