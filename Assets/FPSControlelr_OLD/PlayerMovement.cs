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
        private bool grounded;
        [SerializeField] private LayerMask whatIsGround;

        [SerializeField] private float maxSlopeAngle = 35f;

        //Jumping
        private bool readyToJump = true;
        [SerializeField] private float jumpForce = 550f;

        [SerializeField] private float m_maxJumpChargeTime = 1.0f;

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
        }

        private void teleport()
        {
            transform.position = startPos;
            m_justJumped = false;
            m_previousJump = false;
        }

        private bool m_previousJump = false;
        private float m_chargeTime = 0.0f;
        private void FixedUpdate()
        {
            rb.AddForce(Vector3.down * Time.fixedDeltaTime);

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

        private void Update()
        {
            m_inputs = m_input.ReadInputs();
            Look(m_inputs);

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

            Vector3 forwardMove = orientation.transform.forward * yMovement * Time.fixedDeltaTime * moveSpeed;
            Vector3 sideMove = orientation.transform.right * xMovement * Time.fixedDeltaTime * moveSpeed;
            Vector3 velocity = forwardMove + sideMove;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }

        private void StopMovement()
        {
            rb.velocity = Vector3.zero;
        }

        private void chargeJump()
        {
            orientation.transform.localScale = Vector3.one * (3 / 4);
            orientation.transform.position = new Vector3(orientation.transform.position.x, orientation.transform.position.y - (0.5f * transform.localScale.y), orientation.transform.position.z);
            playerCam.transform.position = new Vector3(playerCam.transform.position.x, playerCam.transform.position.y - (0.5f * transform.localScale.y), playerCam.transform.position.z);

            rb.velocity = Vector3.zero;
            m_chargeTime = Time.time;
            StartCoroutine(delayApplyJumpForce());
        }

        private IEnumerator delayApplyJumpForce()
        {
            yield return new WaitForSeconds(m_maxJumpChargeTime);
            applyJumpForce();
        }

        private void applyJumpForce()
        {
            StopAllCoroutines();

            orientation.transform.localScale = Vector3.one;
            orientation.transform.position = new Vector3(orientation.transform.position.x, orientation.transform.position.y + (0.5f * transform.localScale.y), orientation.transform.position.z);
            playerCam.transform.position = new Vector3(playerCam.transform.position.x, playerCam.transform.position.y + (0.5f * transform.localScale.y), playerCam.transform.position.z);

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            m_justJumped = true;
            readyToJump = false;

            StartCoroutine(physicsJump());
        }

        private bool m_justJumped = false;
        private IEnumerator physicsJump()
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            float chargeTime = Mathf.Max(Time.time - m_chargeTime, 0.1f);

            // MAX Y: 8.3
            // MAX X: 17.5
            rb.AddForce(orientation.transform.up * jumpForce * 2f * (chargeTime / m_maxJumpChargeTime));
            if (m_inputs.m_movement.y > 0)
                rb.AddForce(orientation.transform.forward * jumpForce * (chargeTime / m_maxJumpChargeTime));

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

    }

}