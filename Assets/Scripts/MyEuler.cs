using UnityEngine;

public class MyEuler
{
    public static float speed = 0.01f;

    public static Quaternion Euler(float x, float y, float z)
    {
        x *= speed;
        y *= speed;
        z *= speed;
        float sx = Mathf.Sin(x);
        float sy = Mathf.Sin(y);
        float sz = Mathf.Sin(z);

        float cx = Mathf.Cos(x);
        float cy = Mathf.Cos(y);
        float cz = Mathf.Cos(z);

        return new Quaternion(cy * sx * cz + sy * cx * sz,
                              sy * cx * cz - cy * sx * sz,
                              cy * cx * sz - sy * sx * cz,
                              cy * cx * cz - sy * sx * sz);
    }
}
