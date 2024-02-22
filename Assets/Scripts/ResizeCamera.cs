using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldGen;

public class ResizeCamera : MonoBehaviour
{
    [SerializeField] Transform player;
    bool followPlayerY = false;
    bool followPlayerX = false;

    Bounds bounds;
    float screenAspect;
    Vector2 cameraExtents;

    bool firstRoom = true; //bool-lock to get startingOrthoSize
    float startingOrthoSize; //default ortho

    private void Awake()
    {
        transform.position = new Vector3(0, 0, -10);

        Room.OnEnteredRoom.AddListener((room) => {

            
            cameraExtents = new Vector2(GetComponent<Camera>().orthographicSize * GetComponent<Camera>().aspect, GetComponent<Camera>().orthographicSize);
            float orthoSize = GetComponent<Camera>().orthographicSize;

            Tilemap tilemap = room.GetComponent<Tilemap>();
            tilemap.CompressBounds();

            transform.position = tilemap.transform.TransformPoint(tilemap.localBounds.center) + Vector3.forward * -10;

            bounds = tilemap.localBounds;

            screenAspect = Screen.width / Screen.height;
            float levelAspect = bounds.size.x / bounds.size.y;


            if (screenAspect > levelAspect) {
                orthoSize = bounds.size.y / 2 * levelAspect / screenAspect;
            }
            else {
                orthoSize = bounds.size.y / 2;
            }
            if (firstRoom) {
                startingOrthoSize = orthoSize;
                firstRoom = false;
            }

            //in case of extreme aspect ratio or very large room - camera should follow player
            followPlayerY = levelAspect < 0.5 || bounds.size.y > 20;
            followPlayerX = levelAspect > 2 || bounds.size.x > 20;

            if (!followPlayerX && !followPlayerY) {
                if (screenAspect > levelAspect) {
                    GetComponent<Camera>().orthographicSize = bounds.size.y / 2 * levelAspect / screenAspect;
                }
                else {
                    GetComponent<Camera>().orthographicSize = bounds.size.y / 2;
                }
            }
            else {
                GetComponent<Camera>().orthographicSize = startingOrthoSize;
            }

        });
    }

    private void Update()
    {
        Vector2 min = bounds.center - bounds.extents;
        Vector2 max = bounds.center + bounds.extents;

        if (followPlayerX) {
            Debug.Log("sdfsdkjfj");
            transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
            float clampedX = Mathf.Clamp(transform.position.x, min.x + cameraExtents.x, max.x - cameraExtents.x);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
        if (followPlayerY) {
            transform.position = new Vector3(transform.position.x, player.position.y, transform.position.z);
            float clampedY = Mathf.Clamp(player.position.y, min.y + cameraExtents.y, max.y - cameraExtents.y);
            transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
        }

    }
}
