using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldGen;

public class ResizeCamera : MonoBehaviour
{
    private void Awake()
    {

        Camera.main.transform.position = new Vector3(0,0,-10);

        Room.OnEnteredRoom.AddListener((room) => {
            var tilemap = room.GetComponent<Tilemap>();
            tilemap.CompressBounds();

            Camera.main.transform.position = tilemap.localBounds.center + Vector3.forward * -10;

            Vector3 bounds = tilemap.localBounds.size;

            var screenAspect = Screen.width / Screen.height;
            var levelAspect = bounds.x / bounds.y;

            if(screenAspect > levelAspect)
            {
                Camera.main.orthographicSize = bounds.y / 2 * levelAspect / screenAspect;
            } else
            {
                Camera.main.orthographicSize = bounds.y / 2;
            }
        });
    }
}
