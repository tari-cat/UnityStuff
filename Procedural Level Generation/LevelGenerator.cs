using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Revision 1.02 // Author: <see href="https://github.com/tari-cat/UnityStuff"/>
/// 
/// <para>The Level Generator takes in an array of <seealso cref="Room"/> prefabs, and does a lot of calculations at runtime to place rooms.</para>
/// <para>The <see cref="depth"/> value is responsible for how recursive the process is. Be careful when above 10 depth as it can be slow.</para>
/// <para>The <see cref="rotateRooms"/> value is responsible for rotating rooms in generation, 90 degrees per rotation, per room prefab.</para>
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("Configuration")]

    public Room[] rooms;

    [Tooltip("The amount of times to generate rooms from one room.")]
    public int depth;

    [Tooltip("Rooms get rotated whenever checking if it can be placed, 4 times. Toggling this off will not rotate rooms during generation.")]
    public bool rotateRooms = true;

    [Tooltip("Don't repeat the same kind of room in a row until x amount of other room prefabs have passed.")]
    public int maxRepeat = 0;

    private readonly List<Room> placedRooms = new List<Room>();
    private readonly List<int> checkedTransforms = new List<int>();

    private void Start()
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

    private void PlaceRoom(int depth, Room firstRoom = null)
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
                PlaceRoom(depth - 1, e, firstRoom);
            }
        }
    }

    public void PlaceRoom(int depth, Transform root, Room source, List<Room> previousRooms = null)
    {
        if (checkedTransforms.Contains(root.GetInstanceID()))
            return;

        checkedTransforms.Add(root.GetInstanceID()); // first time using instance ids, hopefully this is fine?

        List<Room> validatedRooms = new List<Room>();
        Dictionary<Room, Room> validatedRoomsDictionary = new Dictionary<Room, Room>(); // Instantiated Room, Prefab Reference, used for non repeats

        int rotation = rotateRooms ? 4 : 1; // Setting this to 1 (when rotateRooms is off) will only get it through the first loop, therefore not rotating the room

        if (previousRooms == null)
            previousRooms = new List<Room>();

        List<Room> possibleRooms = new List<Room>(rooms);

        if (maxRepeat > 0)
        {
            // Truncate the list size of previous rooms down to the max repeat value
            while (previousRooms.Count > maxRepeat)
            {
                previousRooms.RemoveAt(0);
            }

            // Remove any rooms from the possible rooms list that were previously generated
            foreach (Room r in previousRooms)
            {
                possibleRooms.Remove(r);
            }
        }

        if (possibleRooms.Count <= 0)
        {
            possibleRooms = new List<Room>(rooms);
            Debug.LogWarning("No possible rooms, generating from list of prefabs.");
        }

        // Loop through all possible rooms
        foreach (Room r in possibleRooms)
        {
            if (r == source)
                continue;

            // Rotate the room 90 degrees each time, instantiate after the for loop
            for (int i = 0; i < rotation; i++)
            {
                // Loop through all possible entrances of said room
                for (int j = 0; j < r.entrances.Length; j++)
                {

                    Room room = Instantiate(r, new Vector3(0, 0, 0), Quaternion.Euler(0, i * 90, 0));
                    ConnectRoom(room.entrances[j], root);
                    if (ValidateRoom(room))
                    {
                        validatedRooms.Add(room);
                        validatedRoomsDictionary.Add(room, r);
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
            return;

        // pick one at random from the list, set it
        int rand = Random.Range(0, validatedRooms.Count);
        // pick it, and Destroy the rest
        Room theChosenOne = validatedRooms[rand];

        // Add to placed rooms
        placedRooms.Add(theChosenOne);

        // grab the prefab for later
        Room chosenPrefab = validatedRoomsDictionary[theChosenOne];

        // add this prefab to the list of previously selected rooms
        previousRooms.Add(chosenPrefab);

        // remove it from our list
        validatedRooms.Remove(theChosenOne);

        validatedRoomsDictionary.Clear();

        // destroy the excess objects
        for (int i = 0; i < validatedRooms.Count; i++)
        {
            Destroy(validatedRooms[i].gameObject);
        }

        validatedRooms.Clear();

        // recursively generate rooms
        if (depth > 0)
        {
            foreach (Transform e in theChosenOne.entrances)
            {
                PlaceRoom(depth - 1, e, chosenPrefab, previousRooms);
            }
        }
    }

    public void ConnectRoom(Transform entrance, Transform root)
    {
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