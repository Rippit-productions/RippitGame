using UnityEngine;

/// <summary>
/// Manages the player's ability to swing from objects.
/// </summary>
public class PlayerSwing : MonoBehaviour
{
    /// <summary>
    /// Whether the player is currently swinging.
    /// </summary>
    public bool isSwinging = true;
    public HingeJoint2D joint;
    private RaycastHit2D _hit;

    /// <summary>
    /// Line renderer to visualize the swinging rope.
    /// </summary>
    public LineRenderer lineRenderer;
    
    /// <summary>
    /// Min and max of the joint's angle limits : testing limits
    /// </summary>

    public float minAngle = -90f;
    public float maxAngle = 90f;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        if (isSwinging && joint != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            Vector2 anchorWorldPosition = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);
            lineRenderer.SetPosition(1, anchorWorldPosition);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }

    }


    /// <summary>
    /// Raycasts upward to check for objects to swing from and attaches to them.
    /// </summary>
    public void AttachToSwingable()
    {
        Debug.DrawRay(transform.position, Vector2.up * 20, Color.red);
        _hit = Physics2D.Raycast(transform.position, Vector2.up, 20, LayerMask.GetMask("Swingable"));
        if (_hit.collider != null)
        {
            Debug.Log("Hit " + _hit.collider.gameObject.name);
            AttachToTarget(_hit.point, _hit.collider.gameObject);
        }
    }

    /// <summary>
    /// Attaches the player to the given target at the specified point using a hinge joint.
    /// </summary>
    /// <param name="hitPoint">The point to attach to.</param>
    /// <param name="target">The target object to attach to.</param>
  private void AttachToTarget(Vector2 hitPoint, GameObject target)
{
    joint = target.AddComponent<HingeJoint2D>();
    joint.connectedBody = gameObject.GetComponent<Rigidbody2D>();
    joint.anchor = target.transform.InverseTransformPoint(hitPoint);
    joint.connectedAnchor = transform.InverseTransformPoint(hitPoint);
    joint.enableCollision = true;

    // Set the joint's angle limits to allow for swinging.
    JointAngleLimits2D limits = joint.limits;

    limits.min = minAngle;
    limits.max = maxAngle;

    joint.limits = limits;

    isSwinging = true;
}


    /// <summary>
    /// Extends the rope while swinging, moving the player away from the anchor.
    /// </summary>
    public void ExtendRope()
    {
        if (joint != null)
        {
            Vector2 anchorWorldPosition = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);
            Vector2 directionFromAnchorToPlayer = (Vector2)transform.position - anchorWorldPosition;
            transform.position += (Vector3)directionFromAnchorToPlayer.normalized * 0.1f;
        }
    }

    /// <summary>
    /// Shortens the rope while swinging, moving the player toward the anchor.
    /// </summary>
    public void ShortenRope()
    {
        if (joint != null)
        {
            Vector2 anchorWorldPosition = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);
            Vector2 directionFromAnchorToPlayer = (Vector2)transform.position - anchorWorldPosition;
            transform.position -= (Vector3)directionFromAnchorToPlayer.normalized * 0.1f;
        }
    }

    /// <summary>
    /// Detaches the player from any swingable objects, disabling swinging.
    /// </summary>
    public void DetachFromSwingable()
    {
        Destroy(joint);
        isSwinging = false;
        Debug.Log("Detached");
        lineRenderer.positionCount = 0;
    }
}
