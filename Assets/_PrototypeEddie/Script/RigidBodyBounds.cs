using System.Linq;
using UnityEngine;

public class RigidBodyBounds
{
    public static Bounds Get2DBodyBounds(Rigidbody2D Body)
    {
        Bounds totalBounds = new Bounds(Body.position, Vector3.zero);
        if (Body != null)
        {
            var colliders = Body.gameObject.GetComponentsInChildren<Collider2D>();

            foreach (var c in colliders)
            {
                totalBounds.Encapsulate(c.bounds);
            }
        }
        return totalBounds;
    }

    public static Bounds Get3DBodyBounds(Rigidbody body)
    {
        var _ResultBounds = new Bounds(Vector3.zero, Vector3.zero);
        if (!body) return _ResultBounds;
        var gameobj = body.gameObject;
        if (!gameobj) return _ResultBounds;
        Collider[] allColliders = gameobj.GetComponentsInChildren<Collider>();
        if (allColliders.Length > 0)
        {
            _ResultBounds = allColliders[0].bounds;
            for (int i = 1; i < allColliders.Length; i++)
            {
                _ResultBounds.Encapsulate(allColliders[i].bounds);
            }
        }
        return _ResultBounds;
    }
}
