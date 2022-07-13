using UnityEngine;

/// <summary>
/// Revision 1.00 // Author: <see href="https://github.com/tari-cat/UnityStuff"/>
/// 
/// <para>Each room consists of multiple variables that need to be assigned.</para>
/// <para><see cref="bounds"/> is a Box Collider which should be a trigger, which defines the most outer portion of the room. This is used to prevent intersecting rooms.</para>
/// <para><see cref="entrances"/> is an array of entrances, just taking a Transform each. <seealso cref="Entrance"/> is not a <seealso cref="Component"/>.</para>
/// </summary>
public class Room : MonoBehaviour
{
    [Tooltip("The bounds of the room. Used to not collide with other rooms.")]
    public BoxCollider bounds;

    [Tooltip("The entrances to the room. Used for generation.")]
    public Transform[] entrances;

#if UNITY_EDITOR
    /// <summary>
    /// Used to draw the bounds of the room.
    /// </summary>
    void OnDrawGizmos()
    {
        if (bounds == null)
            return;

        Color prevColor = Gizmos.color;
        Matrix4x4 prevMatrix = Gizmos.matrix;

        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 boxPosition = transform.position;

        // convert from world position to local position 
        boxPosition = transform.InverseTransformPoint(boxPosition) + bounds.center;

        Gizmos.DrawWireCube(boxPosition, bounds.size);

        // restore previous Gizmos settings
        Gizmos.color = prevColor;
        Gizmos.matrix = prevMatrix;
    }
#endif
}
