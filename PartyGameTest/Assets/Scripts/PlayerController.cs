using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aula.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Player speed")]
        [SerializeField]
        private float m_HorizontalSpeed;
        [SerializeField]
        private float m_VerticalSpeed;
        [SerializeField]
        private float m_RotationSpeed;

        [Header("Player control")]
        [SerializeField]
        private bool m_IsTankMovement = true;
        [SerializeField]
        private bool m_IsRotationWithMouse = true;

        [Header("Player Bools")]
        [SerializeField]
        private bool m_IsFirstPlayer;
        [SerializeField]
        private bool m_IsSecondPlayer;
        [SerializeField]
        private bool m_IsThirdPlayer;

        [Header("Special Ability")]
        [SerializeField]
        private GameObject m_ObjectToThrow;
        [SerializeField]
        private GameObject m_ObjectOrigin;
        [SerializeField]
        private GameObject m_CanThrowStatus;
        [SerializeField]
        private bool m_CanThrowObject;
        [SerializeField]
        private float m_TimeToReloadAbility;
        [SerializeField]
        private float m_TimeToResetObjectPosition;
        [SerializeField]
        private float m_TimeToResetRotation;
        [SerializeField]
        private LayerMask m_ClickMask;

        private Rigidbody m_Rigidbody;
        private Vector3 m_InputValues;
        private float m_HorizontalValue;

        public bool IsTankMovement
        {
            get { return m_IsTankMovement; }
            set { m_IsTankMovement = value; }
        }

        public bool IsRotationWithMouse
        {
            get { return m_IsRotationWithMouse; }
            set { m_IsRotationWithMouse = value; }
        }

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_ObjectToThrow.SetActive(false);
            m_CanThrowObject = true;
        }

        void Start()
        {
            m_InputValues = Vector3.zero;
        }

        void Update()
        {
            if (m_IsFirstPlayer)
            {
                m_InputValues.z = Input.GetAxis("Player1Vertical") * m_VerticalSpeed * Time.deltaTime;
            }
            else if (m_IsSecondPlayer)
            {
                m_InputValues.z = Input.GetAxis("Player2Vertical") * m_VerticalSpeed * Time.deltaTime;
            }
            else if (m_IsThirdPlayer)
            {
                m_InputValues.z = Input.GetAxis("JoystickLeftAnalogicVertical") * m_VerticalSpeed * Time.deltaTime;
            }

            if (m_IsTankMovement)
            {
                if (m_IsFirstPlayer)
                {
                    m_HorizontalValue = m_IsRotationWithMouse ? Input.GetAxis("Mouse X") : Input.GetAxis("Player1Horizontal");
                    m_InputValues.x = m_HorizontalValue * m_RotationSpeed;
                }
                else if (m_IsSecondPlayer)
                {
                    m_HorizontalValue = m_IsRotationWithMouse ? Input.GetAxis("Mouse X") : Input.GetAxis("Player2Horizontal");
                    m_InputValues.x = m_HorizontalValue * m_RotationSpeed;
                }
                else if (m_IsThirdPlayer)
                {
                    m_HorizontalValue = Input.GetAxis("JoystickRightAnalogicHorizontal");
                    m_InputValues.x = m_HorizontalValue * m_RotationSpeed;
                }
            }
            else
            {
                if (m_IsFirstPlayer)
                {
                    m_HorizontalValue = Input.GetAxis("Player1Horizontal");
                    m_InputValues.x = m_HorizontalValue * m_HorizontalSpeed * Time.deltaTime;
                }
                else if (m_IsSecondPlayer)
                {
                    m_HorizontalValue = Input.GetAxis("Player2Horizontal");
                    m_InputValues.x = m_HorizontalValue * m_HorizontalSpeed * Time.deltaTime;
                }
                else if (m_IsThirdPlayer)
                {
                    m_HorizontalValue = Input.GetAxis("JoystickLeftAnalogicHorizontal");
                    m_InputValues.x = m_HorizontalValue * m_HorizontalSpeed * Time.deltaTime;
                }
            }

            if (!m_ObjectToThrow.activeSelf && (m_IsFirstPlayer || m_IsThirdPlayer))
            {
                m_ObjectToThrow.transform.position = m_ObjectOrigin.transform.position;
                m_ObjectToThrow.transform.rotation = transform.rotation;
            }
            else if (!m_ObjectToThrow.activeSelf && m_IsSecondPlayer)
            {
                m_ObjectToThrow.transform.position = m_ObjectOrigin.transform.position;
                m_ObjectToThrow.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            }

            if (m_IsFirstPlayer && Input.GetMouseButtonDown(0))
            {
                ThrowObject();
            }
            else if (m_IsSecondPlayer && Input.GetMouseButtonDown(1))
            {
                ThrowObject();
            }
            else if (m_IsThirdPlayer && Input.GetButtonDown("Fire1Joystick"))
            {
                ThrowObject();
            }
        }

        void FixedUpdate()
        {
            if (m_IsTankMovement)
            {
                m_Rigidbody.MovePosition(transform.position + transform.forward * m_InputValues.z);
                m_Rigidbody.MoveRotation(m_Rigidbody.rotation * Quaternion.Euler(0.0f, m_InputValues.x, 0.0f));
            }
            else
            {
                m_Rigidbody.MovePosition(transform.position + m_InputValues);
            }
        }

        void ThrowObject()
        {
            if (m_CanThrowObject)
            {
                if (m_IsFirstPlayer || m_IsThirdPlayer)
                {
                    m_ObjectToThrow.SetActive(true);
                    m_CanThrowObject = false;
                    m_CanThrowStatus.SetActive(false);
                    StartCoroutine(ResetObjectPosition());
                    StartCoroutine(CanThrowAgain());
                }

                if (m_IsSecondPlayer)
                {
                    Vector3 clickPosition;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_ClickMask))
                    {
                        clickPosition = hit.point;
                        m_ObjectToThrow.transform.position = clickPosition;
                        m_ObjectToThrow.transform.position += new Vector3(0.0f, 10.0f, 0.0f);
                        m_ObjectToThrow.SetActive(true);
                        m_CanThrowObject = false;
                        m_CanThrowStatus.SetActive(false);
                        StartCoroutine(ResetObjectPosition());
                        StartCoroutine(CanThrowAgain());
                    }
                }
            }
        }

        private IEnumerator ResetObjectPosition()
        {
            yield return new WaitForSeconds(m_TimeToResetObjectPosition);
            m_ObjectToThrow.SetActive(false);
            m_ObjectToThrow.transform.position = m_ObjectOrigin.transform.position;
        }

        private IEnumerator CanThrowAgain()
        {
            yield return new WaitForSeconds(m_TimeToReloadAbility);
            m_CanThrowObject = true;
            m_CanThrowStatus.SetActive(true);
        }

        private IEnumerator ResetDamage()
        {
            yield return new WaitForSeconds(m_TimeToResetRotation);
            transform.position += new Vector3(0.0f, 2.0f, 0.0f);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }

        void OnCollisionEnter(Collision other)
        {
            if (m_IsFirstPlayer)
            {
                if(other.gameObject.tag == "GreenObject" || other.gameObject.tag == "YellowObject")
                {
                    TookDamage();
                }
            }

            if (m_IsSecondPlayer)
            {
                if(other.gameObject.tag == "RedObject" || other.gameObject.tag == "YellowObject")
                {
                    TookDamage();
                }
            }

            if (m_IsThirdPlayer)
            {
                if(other.gameObject.tag == "RedObject" || other.gameObject.tag == "GreenObject")
                {
                    TookDamage();
                }
            }
        }

        void TookDamage()
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
            StartCoroutine(ResetDamage());
        }
    }
}