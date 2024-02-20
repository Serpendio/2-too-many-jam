using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTemp : MonoBehaviour
{
    [SerializeField]
    private PlayerInput _playerInput;
    
    // Update is called once per frame
    void Update()
    {
        var move = _playerInput.actions["Move"].ReadValue<Vector2>();
        transform.position += (Vector3)move * (Time.deltaTime * 5f);
    }
}
