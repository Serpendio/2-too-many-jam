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

    Camera camera;
    [SerializeField] float lerpSpeed;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        transform.position = new Vector3(0, 0, -10);

        Room.OnEnteredRoom.AddListener((room) => {

            
            cameraExtents = new Vector2(camera.orthographicSize * camera.aspect, camera.orthographicSize);
            float orthoSize = camera.orthographicSize;

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
                    camera.orthographicSize = bounds.size.y / 2 * levelAspect / screenAspect;
                }
                else {
                    camera.orthographicSize = bounds.size.y / 2;
                }
            }
            else {
                camera.orthographicSize = startingOrthoSize;

                //Move camera to player without interpolation avoid lerping from room center to player upon entering room
                UpdateCameraPosition(false);
            }

        });
    }

    private void Update() {
        UpdateCameraPosition(true);
    }



    private void UpdateCameraPosition(bool interpolate) {
        Vector2 min = bounds.center - bounds.extents;
        Vector2 max = bounds.center + bounds.extents;

        float clampedX = Mathf.Clamp(player.position.x, min.x + cameraExtents.x, max.x - cameraExtents.x);
        float clampedY = Mathf.Clamp(player.position.y, min.y + cameraExtents.y, max.y - cameraExtents.y);

        if (interpolate) {
            float interpolation = lerpSpeed * Time.deltaTime;
            if (followPlayerX) { transform.position = Vector3.Lerp(transform.position, new Vector3(clampedX, transform.position.y, transform.position.z), interpolation); }
            if (followPlayerY) { transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, clampedY, transform.position.z), interpolation); }
        }
        else {
            if (followPlayerX) { transform.position = new Vector3(clampedX, transform.position.y, transform.position.z); }
            if (followPlayerY) { transform.position = new Vector3(transform.position.x, clampedY, transform.position.z); }
        }

    }
}
