using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThirdPersonDash : MonoBehaviour
{
    [Header("References")]
    private CharacterController _controller;
    public Transform camTransform;
    public Image staminaBar;

    [Header("Movement")]
    public float dashSpeed;
    public float maxStamina;
    private float _stamina;

    [Header("Detection")]
    private bool _isDashing = false;
    private bool _wait = false;
    private bool _buffer = true;

    private void OnEnable()
    {
        //when position is reset, refill stamina
        ThirdPersonMovement.onReset += ResetStamina;
    }

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _stamina = maxStamina;
    }

    private void Update()
    {
        if (ButtonManager.startScreen && GameObject.FindFirstObjectByType<ButtonManager>() != null) return;
        if (_isDashing && _stamina > 0)
        {
            DashMovement();
            //lose stamina over time
            //buffer to wait before refilling
            _stamina -= 2 * Time.deltaTime;
            _buffer = true;
        }

        //keep stamina within range
        if(_stamina < 0) _stamina = 0;
        if (_stamina > maxStamina) _stamina = maxStamina;

        //fill bar accordingly
        staminaBar.fillAmount = _stamina /maxStamina;

        //refill after buffering (using coroutine logic)
        if(_stamina < maxStamina  && !_isDashing)
        {
            if(_buffer && !_wait) StartCoroutine(SecondBuffer());
            if (!_wait) _stamina += 2 * Time.deltaTime;
        }
    }

    public void DashMovement()
    {
        //rotation matches camera when dashing to we always move forward
        transform.rotation = Quaternion.Euler(0f, camTransform.eulerAngles.y, 0f);
        _controller.Move(camTransform.forward * dashSpeed * Time.deltaTime);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        _isDashing = context.performed;
    }

    IEnumerator SecondBuffer()
    {
        //wait before we start to add stamina in update
        _wait = true;
        yield return new WaitForSeconds(1);
        //then stop both to fill stamina
        _buffer = false;
        _wait = false;
        yield break;
    }

    private void ResetStamina(bool doReset)
    {
        if (doReset) _stamina = maxStamina;
    }
}
