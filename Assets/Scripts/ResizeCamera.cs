using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldGen;

public class ResizeCamera : MonoBehaviour
{
    [SerializeField] GameObject player;
    bool followPlayerY = false;
    bool followPlayerX = false;

    Vector3 levelBoundsWorldSpace;
    Bounds bounds;
    Vector3 sizeBounds;
    float screenAspect;

    private void Awake()
    {

        transform.position = new Vector3(0,0,-10);

        Room.OnEnteredRoom.AddListener((room) => {

            Tilemap tilemap = room.GetComponent<Tilemap>();
            tilemap.CompressBounds();

            transform.position = tilemap.transform.TransformPoint(tilemap.localBounds.center) + Vector3.forward * -10;

            bounds = tilemap.localBounds;
            sizeBounds = bounds.size;

            screenAspect = Screen.width / Screen.height;
            float levelAspect = sizeBounds.x / sizeBounds.y;

            //extreme aspect ratio case - camera should follow player
            followPlayerY = levelAspect < 0.5;
            followPlayerX = levelAspect > 2;

            if (!followPlayerX && !followPlayerY) {
                if(screenAspect > levelAspect) {
                    GetComponent<Camera>().orthographicSize = sizeBounds.y / 2 * levelAspect / screenAspect;
                } else {
                    GetComponent<Camera>().orthographicSize = sizeBounds.y / 2;
                }
            }

        });
    }

    private void Update()
    {
        Vector2 min = bounds.center - bounds.size / 2;
        Vector2 max = bounds.center + bounds.size / 2;
        Vector2 cameraExtents = new Vector2(GetComponent<Camera>().orthographicSize * screenAspect, GetComponent<Camera>().orthographicSize);
        if (followPlayerX) {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

            //Clamp
            float clampedX = Mathf.Clamp(transform.position.x, min.x + cameraExtents.x, max.x - cameraExtents.x);

            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
        else if (followPlayerY) {
            transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);

            //Clamp
            float clampedY = Mathf.Clamp(transform.position.y, min.y + cameraExtents.y, max.y - cameraExtents.y);

            transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
        }
    }
}
