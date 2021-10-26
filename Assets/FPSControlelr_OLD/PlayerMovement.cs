// Some stupid rigidbody based movement by Dani

using System;
using System.Collections;
using UnityEngine;

namespace FPSController
{
    public class PlayerMovement : MonoBehaviour
    {
        //Assingables
        [SerializeField] private Transform playerCam;
        [SerializeField] private Transform orientation;

        //Other
        [SerializeField] private Rigidbody rb;

        //Movement
        [SerializeField] private float moveSpeed = 4500;
        private float m_moveModifier = 0.5f;
        private bool grounded;
        [SerializeField] private LayerMask whatIsGround;

        [SerializeField] private float maxSlopeAngle = 35f;

        //Jumping
        private bool readyToJump = true;
        [SerializeField] private float jumpForce = 550f;

        [SerializeField] private float m_maxJumpChargeTime = 1.0f;
        private float m_maxForce = 1;
        private float m_minForce = 0.5f;

        [SerializeField] private GameObject m_bounceCollider;

        //Sliding
        private Vector3 normalVector = Vector3.up;

        private PlayerInput m_input;
        private Inputs m_inputs;

        private Vector3 startPos = Vector3.zero;

        void Start()
        {
            startPos = transform.position;
            m_input = GetComponent<PlayerInput>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            m_input.teleport += teleport;

            UI.PauseAction += Pause;
        }

        private void OnDestroy()
        {
            UI.PauseAction -= Pause;
        }

        private void teleport()
        {
            transform.position = startPos;
            m_justJumped = false;
            m_previousJump = false;
        }

        private bool m_previousJump = false;
        private float m_chargeTime = 0.0f;
        private Vector3 pauseVelocity;
        private void FixedUpdate()
        {
            rb.AddForce(Vector3.down * Time.fixedDeltaTime);

            if (UI.Instance.Paused)
            {
            }
            else
            {
                if (grounded && m_justJumped == false)
                {
                    if (readyToJump)
                    {
                        if (m_inputs.m_jump && m_previousJump == false)
                        {
                            chargeJump();
                        }
                        else if (m_inputs.m_jump == false && m_previousJump)
                        {
                            applyJumpForce();
                        }
                        m_previousJump = m_inputs.m_jump;
                    }
                    else
                    {
                        readyToJump = true;
                        m_previousJump = false;
                    }
                }
                else
                {
                    m_previousJump = false;
                }

                if (grounded && m_justJumped == false)
                {
                    if (m_inputs.m_jump == false) Movement(m_inputs);
                    else StopMovement();
                }
            }
        }

        public void Pause(bool paused)
        {
            if (paused)
            {
                pauseVelocity = rb.velocity;
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }
            else
            {
                rb.velocity = pauseVelocity;
                rb.isKinematic = false;
            }
        }

        private void Update()
        {
            m_inputs = m_input.ReadInputs();
            if (UI.Instance.Paused == false)
            {
                Look(m_inputs);
            }
            else
            {
                if (m_jumpDelay != null) StopCoroutine(m_jumpDelay);
                m_previousJump = false;
                readyToJump = true;
                m_justJumped = false;
            }

            m_bounceCollider.SetActive(grounded == false || m_justJumped);
        }

        //Rotation and look
        [SerializeField] private float sensitivity = 1f;
        private float upDownLook, leftRightLook;
        private float sensMultiplier = 1f;
        private void Look(Inputs inputs)
        {
            float mouseX = inputs.m_look.x * sensitivity * Time.fixedDeltaTime * sensMultiplier;
            float mouseY = inputs.m_look.y * sensitivity * Time.fixedDeltaTime * sensMultiplier;

            //Find current look rotation
            Vector3 rot = playerCam.transform.localRotation.eulerAngles;
            leftRightLook = rot.y + mouseX;

            //Rotate, and also make sure we dont over- or under-rotate.
            upDownLook -= mouseY;
            upDownLook = Mathf.Clamp(upDownLook, -90f, 90f);

            //Perform the rotations
            playerCam.transform.localRotation = Quaternion.Euler(upDownLook, leftRightLook, 0);
            orientation.transform.localRotation = Quaternion.Euler(0, leftRightLook, 0);
        }

        private void Movement(Inputs inputs)
        {
            float xMovement = inputs.m_movement.x, yMovement = inputs.m_movement.y;

            float modifier = inputs.m_crouch ? 1 : m_moveModifier;

            Vector3 forwardMove = orientation.transform.forward * yMovement * Time.fixedDeltaTime * moveSpeed * modifier;
            Vector3 sideMove = orientation.transform.right * xMovement * Time.fixedDeltaTime * moveSpeed * modifier;
            Vector3 velocity = forwardMove + sideMove;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }

        private void StopMovement()
        {
            rb.velocity = Vector3.zero;
        }

        private Coroutine m_jumpDelay;
        private void chargeJump()
        {

            rb.velocity = Vector3.zero;
            m_chargeTime = Time.time;
            m_jumpDelay = StartCoroutine(delayApplyJumpForce());
        }

        private IEnumerator delayApplyJumpForce()
        {
            yield return new WaitForSeconds(m_maxJumpChargeTime);
            applyJumpForce();
        }

        private void applyJumpForce()
        {
            StopAllCoroutines();

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            m_justJumped = true;
            readyToJump = false;

            StartCoroutine(physicsJump());
        }

        private bool m_justJumped = false;
        private IEnumerator physicsJump()
        {
            AudioManager.Instance.Jump();

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            float chargeTime = Time.time - m_chargeTime;
            float percent = chargeTime / m_maxJumpChargeTime;
            float force = percent * (m_maxForce - m_minForce) + m_minForce;

            // MAX Y: 8.3
            // MAX X: 17.5
            rb.AddForce(orientation.transform.up * jumpForce * 2f * force);
            if (m_inputs.m_movement.y > 0)
                rb.AddForce(orientation.transform.forward * jumpForce * force);

            while (grounded)
                yield return new WaitForFixedUpdate();
            m_justJumped = false;
        }

        private bool IsFloor(Vector3 v)
        {
            float angle = Vector3.Angle(Vector3.up, v);
            return angle < maxSlopeAngle;
        }

        private bool cancellingGrounded;

        /// <summary>
        /// Handle ground detection
        /// </summary>
        private void OnCollisionStay(Collision other)
        {
            //Make sure we are only checking for walkable layers
            int layer = other.gameObject.layer;
            if (whatIsGround != (whatIsGround | (1 << layer))) return;

            //Iterate through every collision in a physics update
            for (int i = 0; i < other.contactCount; i++)
            {
                Vector3 normal = other.contacts[i].normal;
                //FLOOR
                if (IsFloor(normal))
                {
                    grounded = true;
                    cancellingGrounded = false;
                    normalVector = normal;
                    CancelInvoke(nameof(StopGrounded));
                }
            }

            //Invoke ground/wall cancel, since we can't check normals with CollisionExit
            float delay = 3f;
            if (!cancellingGrounded)
            {
                cancellingGrounded = true;
                Invoke(nameof(StopGrounded), Time.deltaTime * delay);
            }
        }

        private void StopGrounded()
        {
            grounded = false;
        }

        public void ChangeSensitivity(float sens)
        {
            this.sensitivity = sens;
        }

    }

}