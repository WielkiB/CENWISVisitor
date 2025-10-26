using System.Collections.Generic;
using UnityEngine;

public class CollidedParts : MonoBehaviour
{
    public List<GameObject> parts = new List<GameObject>();

    public bool HighlightCollidedObjects(bool newState)
    {
        if (parts.Count == 0)
            return false;

        foreach (GameObject part in parts)
        {
            if (part.GetComponent<CollidedParts>() && part.GetComponent<CollidedParts>().parts.Count > 0)
                part.GetComponent<CollidedParts>().HighlightCollidedObjects(newState);
            else
                part.GetComponent<Outline>().enabled = newState;
        }

        return true;
    }

    public void RemoveFromList(GameObject obj)
    {
        parts.Remove(obj);
    }

    public void AddToList(GameObject obj)
    {
        parts.Add(obj);
    }
}
