using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThirdPersonDash : MonoBehaviour
{
    private CharacterController _controller;
    public Transform camTransform;

    public float dashSpeed;
    public float maxStamina;
    private float _stamina;
    private bool _isDashing = false;
    public Image staminaBar;
    private bool _wait = false;
    private bool _buffer = true;

    private void OnEnable()
    {
        ThirdPersonMovement.onReset += ResetStamina;
    }

    private void Start()
    {

        _controller = GetComponent<CharacterController>();
        _stamina = maxStamina;
    }

    private void Update()
    {
        if (_isDashing && _stamina > 0)
        {
            DashMovement();
            _stamina -= 2 * Time.deltaTime;
            _buffer = true;
        }
        if(_stamina < 0) _stamina = 0;
        if (_stamina > maxStamina) _stamina = maxStamina;
        staminaBar.fillAmount = _stamina /maxStamina;
        if(_stamina < maxStamina  && !_isDashing)
        {
            if(_buffer && !_wait) StartCoroutine(SecondBuffer());
            if (!_wait) _stamina += 2 * Time.deltaTime;
        }
    }

    public void DashMovement()
    {
        transform.rotation = Quaternion.Euler(0f, camTransform.eulerAngles.y, 0f);
        _controller.Move(camTransform.forward * dashSpeed * Time.deltaTime);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        _isDashing = context.performed;
    }

    IEnumerator SecondBuffer()
    {
        _wait = true;
        yield return new WaitForSeconds(1);
        _stamina += 0.01f;
        _buffer = false;
        _wait = false;
        yield break;
    }

    private void ResetStamina(bool doReset)
    {
        if (doReset) _stamina = maxStamina;
    }
}
