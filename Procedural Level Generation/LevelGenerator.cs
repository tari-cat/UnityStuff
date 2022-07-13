using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Revision 1.00 // Author: <see href="https://github.com/tari-cat/UnityStuff"/>
/// 
/// <para>The Level Generator takes in an array of <seealso cref="Room"/> prefabs, and does a lot of calculations at runtime to place rooms.</para>
/// <para>The <see cref="depth"/> value is responsible for how recursive the process is. Be careful when above 10 depth as it can be slow.</para>
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("Configuration")]

    public Room[] rooms;

    [Tooltip("The amount of times to generate rooms from one room.")]
    public int depth;

    private readonly List<Room> placedRooms = new List<Room>();
    private readonly List<int> checkedTransforms = new List<int>();

    public void Start()
    {
        if (rooms.Length > 0)
        {
            PlaceRoom(depth, rooms[0]);
        }
        else
        {
            Debug.LogError("LevelGenerator has no prefabs!");
        }
    }

    public void PlaceRoom(int depth, Room firstRoom = null)
    {
        if (firstRoom == null)
        {
            int rand = Random.Range(0, rooms.Length);
            firstRoom = rooms[rand];
        }
        Room r = Instantiate(firstRoom, new Vector3(0, 0, 0), Quaternion.identity);
        placedRooms.Add(r);

        if (depth > 0)
        {
            foreach (Transform e in r.entrances)
            {
                PlaceRoom(depth - 1, e);
            }
        }
    }

    public void PlaceRoom(int depth, Transform root)
    {
        if (checkedTransforms.Contains(root.GetInstanceID()))
            return;

        checkedTransforms.Add(root.GetInstanceID()); // first time using instance ids, hopefully this is fine?
        List<Room> validatedRooms = new List<Room>();

        // Loop through all possible rooms
        foreach (Room r in rooms)
        {
            // Rotate the room 90 degrees each time, instantiate after the for loop
            for (int i = 0; i < 4; i++)
            {
                // Loop through all possible entrances of said room
                for (int j = 0; j < r.entrances.Length; j++)
                {
                    Room room = Instantiate(r, new Vector3(0, 0, 0), Quaternion.Euler(0, i * 90, 0));
                    ConnectRoom(room.entrances[j], root);
                    if (ValidateRoom(room))
                    {
                        validatedRooms.Add(room);
                    }
                    else
                    {
                        Destroy(room.gameObject);
                        continue;
                    }
                }
            }
        }

        // If no rooms are possible, return here
        if (validatedRooms.Count == 0)
        {
            return;
        }

        // pick one at random from the list, set it
        int rand = Random.Range(0, validatedRooms.Count);
        // pick it, and Destroy the rest
        Room theChosenOne = validatedRooms[rand];

        // Add to placed rooms
        placedRooms.Add(theChosenOne);

        // remove it from our list
        validatedRooms.RemoveAt(rand);

        // destroy the excess objects
        for (int i = 0; i < validatedRooms.Count; i++)
        {
            Destroy(validatedRooms[i].gameObject);
        }

        // recursively generate rooms
        if (depth > 0)
        {
            foreach (Transform e in theChosenOne.entrances)
            {
                PlaceRoom(depth - 1, e);
            }
        }
    }

    public void ConnectRoom(Transform entrance, Transform root)
    {
        //Vector3 rootRoomPosition = root.parent.transform.position;
        Vector3 connector = root.position;

        Vector3 roomPosition = entrance.parent.transform.position;
        Vector3 entrancePosition = entrance.position;
        Vector3 offset = roomPosition - entrancePosition;

        Transform room = entrance.parent;
        room.position = connector + offset;
    }

    public bool ValidateRoom(Room room)
    {
        BoxCollider bc = room.bounds;
        
        foreach (Room r in placedRooms)
        {
            Physics.ComputePenetration(bc, bc.transform.position, bc.transform.rotation,
                r.bounds, r.transform.position, r.transform.rotation,
                out _, out float dist);
            if (dist > 0.05f) // if the rooms are *just barely* touching eachother but not really
            {
                return false;
            }
        }
        return true;
    }
}