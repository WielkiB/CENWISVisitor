using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MyRaycast
{
    static Collider getHitObj(Vector3 pos,Vector3 dir,int layerMask, float precision)
    {
        Collider[] col = Physics.OverlapCapsule(pos, pos + dir, precision, layerMask);
        if (col.Length <= 0)
            return null;
        Collider toRet = col[0];
        float dist = float.MaxValue;
        for (int i = 0; i < col.Length; i++)
        {
            float dist2 = Vector3.Distance(pos, col[i].ClosestPointOnBounds(pos));
            if (dist <= dist2)
                continue;
            dist = dist2;
            toRet = col[i];
        }
        return toRet;
    }

    static GameObject[] getHitObjects(Vector3 pos, Vector3 dir, int layerMask, float precision)
    {
        Collider[] col = Physics.OverlapCapsule(pos, pos + dir, precision, layerMask);
        if (col.Length <= 0)
            return null;
        col.OrderBy(x => Vector3.Distance(x.ClosestPointOnBounds(pos), pos));
        GameObject[] toRet = new GameObject[col.Length];
        for (int i = 0; i < col.Length; i++)
            toRet[i] = col[i].gameObject;
        return toRet;
    }

    public static bool Raycast(Ray ray, out GameObject hit, float maxDistance, LayerMask mask, float precision = 0.01f)
    {
        Vector3 pos = ray.origin;
        const float stepSize = 0.20f;
        float steps = maxDistance / stepSize;
        Vector3 dir = ray.direction * maxDistance / steps;
        for (int i = 0; i < steps; i++) 
        {
            Collider collider = getHitObj(pos, dir, mask.value, precision);
            if (collider)
            {
                hit = collider.gameObject;
                return true;
            }
            pos += dir;
        }
        hit = null;
        return false;
    }

    public static bool Raycast(Vector3 origin, Vector3 dir, out GameObject hit, float maxDistance, LayerMask mask, float precision = 0.01f)
    {
        const float stepSize = 0.20f;
        float steps = maxDistance / stepSize;
        dir = dir.normalized * maxDistance / steps;
        for (int i = 0; i < steps; i++)
        {
            Collider collider = getHitObj(origin, dir, mask.value, precision);
            if (collider)
            {
                hit = collider.gameObject;
                return true;
            }
            origin += dir;
        }
        hit = null;
        return false;
    }


    public static GameObject[] RaycastAll(Ray ray, float maxDistance, LayerMask mask, float precision = 0.01f)
    {
        Vector3 pos = ray.origin;
        const float stepSize = 0.01f;
        float steps = maxDistance / stepSize;
        Vector3 dir = ray.direction * maxDistance / steps;

        List<GameObject> listgms = new List<GameObject>();
        for (int i = 0; i < steps; i++)
        {
            GameObject[] thisStepcolliders = getHitObjects(pos, dir, mask.value, precision);
            pos += dir;
            if (thisStepcolliders == null)
                continue;

            foreach (GameObject gm in thisStepcolliders)
                if (gm && !listgms.Contains(gm))
                    listgms.Add(gm);
        }

        GameObject[] colliders = new GameObject[listgms.Count];
        int e = 0;
        foreach (GameObject gm in listgms)
            colliders[e++] = gm;

        return colliders;
    }
}
