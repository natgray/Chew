﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MalbersAnimations.Utilities;
using System;

namespace MalbersAnimations
{
    //Animal Logic
    public partial class Animal
    {
        float YFix;

        void Awake()
        {
            AnimalMesh = GetComponentInChildren<Renderer>();
            anim = GetComponent<Animator>();
            _transform = transform;

            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Int, Hash.Type))
            {
                Anim.SetInteger(Hash.Type, animalTypeID);                         //Adjust the layer for the curret animal Type this is of offseting the bones to another pose
            }
            WaterLayer = LayerMask.GetMask("Water");
        }

        void Start()
        {
            SetStart();
        }

  

        protected virtual void SetStart()
        {
            DeltaPosition = Vector3.zero;

            if (_RigidBody) _RigidBody.isKinematic = false;                     //Some People set it as Kinematic and falling stop working (Just to make sure)
            Anim.updateMode = AnimatorUpdateMode.Normal;                        //Make Sure the Update Mode is on Normal Mode

            isInAir = false;

            scaleFactor = _transform.localScale.y;                              //TOTALLY SCALABE animal

            MovementReleased = true;

            SetPivots();

            switch (StartSpeed)                                     //Set Start Speed
            {
                case Ground.walk: Speed1 = true;
                    break;
                case Ground.trot: Speed2 = true;
                    break;
                case Ground.run: Speed3 = true;
                    break;
                default:
                    break;
            }

            Attack_Triggers = GetComponentsInChildren<AttackTrigger>(true).ToList();        //Save all Attack Triggers.

            OptionalAnimatorParameters();                                                   //Enable Optional Animator Parameters on the Animator Controller;

            Start_Flying();

            FrameCounter = UnityEngine.Random.Range(0, 10000);  //Make every animal start on a diferent frame rate

            //OnAnimationChange.AddListener(OnAnimationStateEnter); //Check everytime an animation state has change
        }

        public virtual void SetPivots()
        {
            pivots = GetComponentsInChildren<Pivots>().ToList();                //Pivots are Strategically Transform objects use to cast rays used to calculate the terrain inclination 

            if (pivots != null)
            {
                pivot_Hip = pivots.Find(p => p.name.ToUpper().Contains("HIP"));
                pivot_Chest = pivots.Find(p => p.name.ToUpper().Contains("CHEST"));
                pivot_Water = pivots.Find(p => p.name.ToUpper().Contains("WATER"));
            }
        }

        /// <summary>
        /// Setting the animal that flies when start
        /// </summary>
        protected virtual void Start_Flying()
        {
            if (hasFly && StartFlying && canFly)
            {
                stand = false;
                Fly = true;
                Anim.Play("Fly", 0);
                IsInAir = true;
                if (_RigidBody) _RigidBody.useGravity = false;
            }
        }

        /// <summary>
        ///  //Enable Optional Animator Parameters on the Animator Controller;
        /// </summary>
        protected void OptionalAnimatorParameters()
        {
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Swim)) hasSwim = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Dodge)) hasDodge = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Fly)) hasFly = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Attack2)) hasAttack2 = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Stunned)) hasStun = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Bool, Hash.Underwater)) hasUnderwater = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Float, Hash.UpDown)) hasUpDown = true;
            if (MalbersTools.FindAnimatorParameter(Anim, AnimatorControllerParameterType.Float, Hash.Slope)) hasSlope = true;
        }

        /// <summary>
        /// Link all Parameters to the animator
        /// </summary>
        public virtual void LinkingAnimator()
        {
            if (!Death)
            {
                Anim.SetFloat(Hash.Vertical, vertical);
                Anim.SetFloat(Hash.Horizontal, horizontal);
                Anim.SetBool(Hash.Stand, stand);
                Anim.SetBool(Hash.Shift, Shift);
                Anim.SetBool(Hash._Jump, jump);
                Anim.SetBool(Hash.Attack1, attack1);
                Anim.SetBool(Hash.Damaged, damaged);
                Anim.SetBool(Hash.Action, action);
                Anim.SetInteger(Hash.IDAction, actionID);
                Anim.SetInteger(Hash.IDInt, IDInt);                //The problem is that is always zero if you change it externally;
                


                //Optional Animator Parameters
                if (hasSlope) Anim.SetFloat(Hash.Slope, Slope);
                if (hasStun) Anim.SetBool(Hash.Stunned, stun);
                if (hasAttack2) Anim.SetBool(Hash.Attack2, attack2);
                if (hasUpDown) Anim.SetFloat(Hash.UpDown, movementAxis.y);
                if (hasDodge) Anim.SetBool(Hash.Dodge, dodge);
                if (hasFly && canFly) Anim.SetBool(Hash.Fly, Fly);
                if (hasSwim && canSwim) Anim.SetBool(Hash.Swim, swim);
                if (hasUnderwater && CanGoUnderWater) Anim.SetBool(Hash.Underwater, underwater);
            }

            Anim.SetBool(Hash.Fall, fall);  //Update  fall either if is death or not

            OnSyncAnimator.Invoke(); //Ready to Sync all the parameters with external scripts ... (Riding System)
        }

        /// <summary>
        /// Gets the movement from the Input Script or AI
        /// </summary>
        /// <param name="move">Direction VEctor</param>
        /// <param name="active">Active = true means that is taking the direction Move</param>
        public virtual void Move(Vector3 move, bool active = true)
        {
            MovementReleased = move.x == 0 && move.z == 0;

            if (active)
            {
                // convert the world relative moveInput vector into a local-relative
                // turn amount and forward amount required to head in the desired
                // direction.
                if (move.magnitude > 1f) move.Normalize();
                move = _transform.InverseTransformDirection(move);

                if (!Fly && !underwater)
                {
                    move = Vector3.ProjectOnPlane(move, /*-Physics.gravity*/ SurfaceNormal).normalized;       //If is not underwater and not flying remove the Y axis and added to the other ones
                }

                float turnAmount = Mathf.Atan2(move.x, move.z);                   //Convert it to relative

                float forwardAmount = move.z;

                if (forwardAmount > 0) forwardAmount = 1;               //It will remove slowing Down when rotating
                if (forwardAmount < 0) forwardAmount = -1;               //It will remove slowing Down when rotating


                movementAxis = new Vector3(turnAmount, movementAxis.y, Mathf.Abs(forwardAmount));

                if (!Jump && !Down)                   //Up & Down movement while flying or swiming;
                {
                    if (Fly || underwater)
                    {
                        float Ylimit = move.y;

                        Ylimit = Ylimit > 0 ? Ylimit * YAxisPositiveMultiplier : Ylimit * YAxisNegativeMultiplier;
                        movementAxis.y = Mathf.Lerp(movementAxis.y, Ylimit, Time.deltaTime);
                    }
                }

                if (!stand && AnimState != AnimTag.Action && AnimState != AnimTag.Sleep)
               //if (!stand && !RealAnimation(Hash.Action,Hash.Tag_Sleep)) //if is not stand and is not making an action
                {
                    DeltaRotation *= Quaternion.Euler(0, movementAxis.x * Time.deltaTime * TurnMultiplier, 0);
                    DeltaPosition += _transform.DeltaPositionFromRotate(AnimalMesh.bounds.center, UpVector, movementAxis.x * Time.deltaTime * TurnMultiplier);
                    //_transform.RotateAround(animalMesh.bounds.center, UpVector, movementAxis.x * Time.deltaTime * TurnMultiplier); //Add Extra rotation when is not standing
                }


                if (AnimState == AnimTag.Action) movementAxis = Vector3.zero;    // No movement when is making an action
            }
            else
            {
                movementAxis = new Vector3(move.x, movementAxis.y, move.z);         //Do not convert to Direction Input Mode (Camera or AI)
            }
        }

        ///─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Add more Rotations to the current Turn Animations 
        /// </summary>
        protected virtual void AdditionalTurn(float time)
        {
            float Turn = currentSpeed.rotation;

            float clampDirection = Mathf.Clamp(horizontal, -1, 1) * (movementAxis.z >= 0 ? 1 : -1);  //Add +Rotation when going Forward and -Rotation when going backwards


            DeltaRotation *= Quaternion.Euler(0, Turn * 2 * clampDirection * time, 0);

            if (Fly || swim || stun || AnimState == AnimTag.Action) return;      //Skip the code below if is in any of this states


            if (IsJumping || fall)                                                //More Rotation when jumping and falling... in air rotation
            {
                float amount = airRotation * horizontal * time * (movementAxis.z >= 0 ? 1 : -1);

                DeltaRotation *= Quaternion.Euler(0, amount, 0);
                DeltaPosition += _transform.DeltaPositionFromRotate(AnimalMesh.bounds.center, T_Up, amount);
            }
        }

        ///─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Add more Speed to the current Move animations
        /// </summary>
        protected virtual void AdditionalSpeed(float time)
        {
            if (hasSwim && swim) currentSpeed = swimSpeed;                                        //Change values to Swim
            else if (hasFly && fly) currentSpeed = flySpeed;                                         //Change values to Fly
            else if (IsJumping || fall) {/*EMPTY*/ }
            else if (Speed3 || (Speed2 && Shift)) currentSpeed = runSpeed;
            else if (Speed2 || (Speed1 && Shift)) currentSpeed = trotSpeed;
            else if (Speed1) currentSpeed = walkSpeed;
          

            if (AnimState != AnimTag.Locomotion)                   //Reset to the Default if is not on the Locomotion State
            {
                currentSpeed.animator = 1;
                currentSpeed.rotation = 0;
            }

            if (vertical < 0) currentSpeed.position = walkSpeed.position;      //If is going backwards use the walkspeed Position

            currentSpeed.position = currentSpeed.position * ScaleFactor * Mathf.Clamp(Mathf.Abs(vertical), -1, 1); //Add the Scale to the Aditional Speed          

            Vector3 forward = T_Forward * vertical;
       

            if (forward.magnitude > 1) forward.Normalize();

            DeltaPosition += forward * currentSpeed.position / 5f * time;

            Anim.speed = Mathf.Lerp(Anim.speed, currentSpeed.animator * animatorSpeed, time * currentSpeed.lerpAnimator);               //Changue the velocity of the animator
        }

        /// <summary>
        /// Updates the MovementAxis.Y.
        /// </summary>
        /// <param name="smoothness">Smoothness</param>
        public virtual void YAxisMovement(float smoothness, float time)
        {
            if (jump)
            {
                movementAxis.y = Mathf.Lerp(movementAxis.y, LockUp ? 0 : 1, time * smoothness);
            }
            else if (down)
            {
                movementAxis.y = Mathf.Lerp(movementAxis.y, -1, time * smoothness);
            }
            else
            {
                movementAxis.y = Mathf.Lerp(movementAxis.y, 0, time * smoothness);
            }


            if (Mathf.Abs(movementAxis.y) < 0.001f) movementAxis.y = 0;         //Remove extra float values...

        }

        ///─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Updates Movement on Platforms
        /// </summary>
        private void UpdatePlatformMovement(bool update)
        {
            if (platform == null) return;

            if (AnimState == AnimTag.Jump || AnimState == AnimTag.NoAlign) { platform = null; return; }                          //Do not calculate if you are jumping or making an action

            if (!update) //FIXED UPDATE
            {
                FixedDeltaPos = platform.position - platform_Pos;    // Keep the same relative position.
                platform_Pos = platform.position;
                //FixedDeltaPos.y = 0;
            }
            else //UPDATE
            {
                var deltaAngle = platform.eulerAngles.y - platform_formAngle;       //The diference between the this and the last frame

                if (deltaAngle == 0) return;                                        // no rotation founded.. Skip the code below

                DeltaRotation *= Quaternion.Euler(0, deltaAngle, 0);
                DeltaPosition += _transform.DeltaPositionFromRotate(platform.position, Vector3.up, deltaAngle);             //Move Position to the relative rotation pivot of the platform..

                platform_formAngle = platform.eulerAngles.y;
            }
        }



        /// <summary>
        /// Mayor Raycasting stuff to align and calculate the ground from the animal ****IMPORTANT***
        /// </summary>
        protected void RayCasting()
        {
            if (AnimState != AnimTag.Jump
                && AnimState != AnimTag.JumpEnd
                && AnimState != AnimTag.Recover
                && AnimState != AnimTag.Fall)
            {
                if (FrameCounter % PivotsRayInterval != 0) return;         //Skip to reduce aditional raycasting
            }

            UpVector = -Physics.gravity;
            scaleFactor = _transform.localScale.y;                       //Keep Updating the Scale Every Frame
            _Height = height * scaleFactor;                             //multiply the Height by the scale

            backray = frontray = false;

            hit_Chest = NULLRayCast;                               //Clean the Raycasts every time 
            hit_Hip = NULLRayCast;                                 //Clean the Raycasts every time 

            hit_Chest.distance = hit_Hip.distance = _Height;       //Reset the Distances to the Heigth of the animal (IMPORTANT)

            if (Pivot_Hip != null) //Ray From the Hip to the ground
            {
                if (Physics.Raycast(Pivot_Hip.GetPivot, -T_Up, out hit_Hip, scaleFactor * Pivot_Hip.multiplier, GroundLayer))
                {
                    if (debug) Debug.DrawRay(hit_Hip.point, hit_Hip.normal * 0.2f, Color.blue);
                   
                    backray = true;

                    if (platform == null && AnimState != AnimTag.Jump)               //Platforming logic
                    {
                        platform = hit_Hip.transform;
                        platform_Pos = platform.position;
                        platform_formAngle = platform.eulerAngles.y;
                    }
                }
                else { platform = null; }
            }

            //Ray From Chest to the ground ***Use the pivot for calculate the ray... but the origin position to calculate the distance
            if (Physics.Raycast(Main_Pivot_Point, -T_Up, out hit_Chest, Chest_Pivot_Multiplier, GroundLayer))
            {
                if (debug) Debug.DrawRay(hit_Chest.point, hit_Chest.normal * 0.2f, Color.red);

                float frontSlopeAngle = Vector3.Angle(hit_Chest.normal, Vector3.up);

                if (frontSlopeAngle < maxAngleSlope) frontray = true;

                //if ((platform == null || platform != hit_Chest.transform) && AnimState != AnimTag.Jump)      //Platforming
                //{
                //    platform = hit_Chest.transform;
                //    platform_Pos = platform.position;
                //    platform_formAngle = platform.eulerAngles.y;
                //}

            }
            //else { platform = null; }

            if (debug && frontray && backray)
            {
                Debug.DrawLine(hit_Hip.point, hit_Chest.point, Color.yellow);
            }

            if (!frontray && Stand) //Hack if is there's no ground beneath the animal and is on the Stand Sate;
            {
                fall = true;

                if (pivot_Hip && backray) fall = false;
            }

            FIXdistance = hit_Hip.distance;
            if (!backray) FIXdistance = hit_Chest.distance;         //if is landing on the front feets
        }

        protected virtual void AlignRotation(bool value, float time)
        {
            Quaternion AlignRot = Quaternion.FromToRotation(T_Up, SurfaceNormal) * _transform.rotation;  //Calculate the orientation to Terrain 
            Quaternion Inverse_Rot = Quaternion.Inverse(_transform.rotation);
            Quaternion Delta;

            if (value)
            {
                Delta = Quaternion.Inverse(_transform.rotation) * AlignRot;
            }
            else
            {
                var UpRot = Quaternion.FromToRotation(T_Up, UpVector) * _transform.rotation;      //Calculate with the UPVECTOR instead of the terrain normal
                Delta = Inverse_Rot * UpRot;
            }

            Delta = Quaternion.Slerp(DeltaRotation, DeltaRotation * Delta, time * AlingToGround / 2); //Calculate the Delta Align Rotation
            DeltaRotation *= Delta;
        }

        protected virtual void FixRotation(float time)
        {
            //───────────────────────────────────────────────CHECK FOR ANIMATIONS THAT WILL SKIP THE REST OF THE METHOD ───────────────────────────────────────────────────────────────────────────────────
            if (swim || fly || underwater) return;
            //───────────────────────────────────────────────Terrain Rotation Adjusment───────────────────────────────────────────────────────────────────────────────────
            //Calculate the Align vector of the terrain
            if (IsInAir || slope < -1 || AnimState == AnimTag.NoAlign || !backray || (backray && !frontray))
            {
                if (slope < 0 || AnimState == AnimTag.Fall)
                    AlignRotation(false, time);
            }
            else
            {
                AlignRotation(true, time);
            }
        }

        internal virtual void RaycastWater()
        {
            if (Physics.Raycast(pivot_Water.transform.position, -T_Up, out WaterHitCenter, scaleFactor * pivot_Water.multiplier*1.5f, WaterLayer))
            {
                waterLevel = WaterHitCenter.point.y;                //Get the water level when find water

                isInWater = true;                                   //Has found a water layer.. so Set isInWater to true
            }
            else
            {
                if (isInWater && AnimState != AnimTag.SwimJump)     //if is in water but is jumping set to is inwater to false
                {
                    isInWater = false;
                    //waterLevel = LowWaterLevel;
                }
            }
        }

        /// <summary>
        /// Swim Logic
        /// </summary>
        protected virtual void Swimming(float time)
        {
            if (!hasSwim || !canSwim) return;                   //If doesnt have swimm animation don't do the swimming calculations
            if (underwater) return;                             //if we are underwater this behavior does not need to be calcultate **Important**
            if (Stand || !pivot_Water) return;                  //Skip if where doing nothing | If there's no water Pivot do nothing


            if (FrameCounter % WaterRayInterval == 0)  RaycastWater();      //DO the Water Raycast
          

            if (isInWater)                                                  //if we hit water
            {
                if ((hit_Chest.distance < (_Height * 0.8f) && movementAxis.z > 0 && hit_Chest.transform != null)    //Exit the water walking forward  and it has found land
                    || (hit_Hip.distance < (_Height * 0.8f) && movementAxis.z < 0 && hit_Hip.transform != null))     //Exit the water walking backward and it has found land
                {
                    if (AnimState != AnimTag.Recover)       //Don't come out of the water if you are playing entering water
                    {
                        Swim = false;
                        return;
                    }
                }
                if (!swim)
                {
                    if (Pivot_Chest.Y <= Waterlevel) //Enter the water if the water is above chest level
                    {
                        Swim = true;
                        currentSpeed = swimSpeed;
                        OnSwim.Invoke();
                        _RigidBody.constraints = Still_Constraints;
                    }
                }


            }

            if (swim)
            {
                fall = isInAir = fly = false;  // Reset all other states

                float angleWater = Vector3.Angle(T_Up, WaterHitCenter.normal);  //Calculates the angle of the water

                Quaternion finalRot = Quaternion.FromToRotation(T_Up, WaterHitCenter.normal) * _transform.rotation;        //Calculate the rotation forward for the water
                Quaternion deltaFixRotation = Quaternion.Inverse(_transform.rotation) * finalRot;

                if (angleWater > 0.5f)
                {
                    deltaFixRotation = Quaternion.Slerp(DeltaRotation, DeltaRotation * deltaFixRotation, time * 10f);
                }

                DeltaRotation *= deltaFixRotation;

                //-------------------Go UnderWater---------------
                if (CanGoUnderWater && Down && !IsJumping && AnimState != AnimTag.SwimJump)
                {
                    underwater = true;
                }
            }
        }


        ///─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Terrain Aligment Logic
        /// </summary>
        protected virtual void FixPosition(float time)
        {
            if (swim) return;

            float difference = _Height - FIXdistance;

            if (FIXdistance > _Height)              //Snap To Terrain  -HIGHER
            {
                if (!isInAir && !swim)
                {
                    YFix = YFix +
                       (((AnimState == AnimTag.Locomotion || Stand) && difference > 0.01f) ?
                        difference : 
                        difference * time * SnapToGround);
                }
            }
            else                                    //Snap To Terrain  -LOWER
            {
                if (!fall && !IsInAir)
                {
                    YFix = YFix + ((difference < 0.01f || Stand) ?
                        difference :
                        difference * time * SnapToGround);
                }
            }
            FIXdistance += YFix;
        }


        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Fall Logic
        /// </summary>
        protected virtual void Falling()
        {
            // fall_Point = Chest_Pivot_Point;
            // if (!Fly)
            fall_Point = Main_Pivot_Point + (T_Forward * (Shift ? GroundSpeed + 1 : GroundSpeed) * FallRayDistance * ScaleFactor); //Calculate ahead the falling ray

            if (FrameCounter % FallRayInterval != 0) return;         //Skip to reduce aditional raycasting

            //Don't Calcultate Fall Ray if the animal on any of these states    
            if (AnimState == AnimTag.Sleep || 
                AnimState == AnimTag.Action || 
                AnimState == AnimTag.Swim || 
                AnimState == AnimTag.Idle) return;


            float Multiplier = Chest_Pivot_Multiplier;

            if (AnimState == AnimTag.Jump || AnimState == AnimTag.Fall || AnimState == AnimTag.Fly)
            {
                Multiplier *= FallRayMultiplier;
            }

            //Set the Fall Ray a bit farther from the front feet.
            if (Physics.Raycast(fall_Point, -T_Up, out FallHit, Multiplier, GroundLayer))
            {
                if (debug)
                {
                    Debug.DrawRay(fall_Point, -T_Up * Multiplier, Color.magenta);
                    MalbersTools.DebugPlane(FallHit.point, 0.1f, Color.magenta, true);
                }

                float fallSlopeAngle = Vector3.Angle(FallHit.normal, Vector3.up);
                fallSlopeAngle *= Vector3.Dot(T_ForwardW, FallHit.normal) > 0 ? 1 : -1; //Calcualte the Fall Angle Positive or Negative


                if ((fallSlopeAngle > maxAngleSlope) || (!frontray && !backray)) //found a ray but there's no BACK/FrontRay
                {
                    fall = true;
                    return;
                }

                fall = false;

                if (AnimState == AnimTag.Fly && land)
                {
                    SetFly(false);                                          //Reset Fly when the animal is near the ground
                    IsInAir = false;
                    groundSpeed = LastGroundSpeed;                          //Restore the GroundSpeed to the original
                }

                if (AnimState == AnimTag.SwimJump) Swim = false;            //in case is jumping from the water to the ground ***Important
            }
            else
            {
                fall = true;
                if (debug)
                {
                    MalbersTools.DebugPlane(fall_Point + (-T_Up * Multiplier), 0.1f, Color.gray, true);
                    Debug.DrawRay(fall_Point, -T_Up * Multiplier, Color.gray);
                }
            }
        }

        // Check for a behind Cliff so it will stop going backwards.
        protected virtual bool IsFallingBackwards(float ammount)
        {
            if (FrameCounter % FallRayInterval != 0) return false;         //Skip to reduce aditional raycasting

            RaycastHit BackRay = new RaycastHit();
            var point = Pivot_Hip ? Pivot_Hip.transform.position : _transform.position + new Vector3(0, _Height, 0);
            var multiplier = Pivot_Hip ? Pivot_Hip.multiplier * FallRayMultiplier : FallRayMultiplier;

            Vector3 FallingVectorBack = point + T_Forward * -1 * ammount;

            if (debug) Debug.DrawRay(FallingVectorBack, -T_Up * multiplier * scaleFactor, Color.white);                 //Draw a White Ray

            if (Physics.Raycast(FallingVectorBack, -T_Up, out BackRay, scaleFactor * multiplier, GroundLayer))
            {
                if (BackRay.normal.y < 0.6)  // if the back ray if in Big Angle don't walk 
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (!swim && movementAxis.z < 0) return true; //Check if the animal is moving backwards
            }
            return false;
        }


        /// <summary>
        /// Movement Trot Walk Run (Velocity changes)
        /// </summary>
        protected virtual void MovementSystem(float s1 = 1, float s2 = 2, float s3 = 3)
        {
            float maxspeed = groundSpeed;           //Do not override the groundSpeed

            var H_Smooth = 1+currentSpeed.lerpRotation;
            var V_Smooth = 1+currentSpeed.lerpPosition;

            maxspeed = swim || underwater ? 1 : maxspeed;

            if (Shift) maxspeed++;                                                  //Increase the Speed with Shift pressed

            if (!Fly && !Swim && !IsJumping )                                       //Don't check for slopes when swimming or flying
            {
                if (SlowSlopes && slope >= 0.5 && maxspeed > 1) maxspeed--;         //SlowDown When going UpHill

                if (slope >= 1)                                                     //Prevent to go uphill
                {
                    maxspeed = 0;
                    V_Smooth = 10;
                }
            }

            if (Fly)
            {
                YAxisMovement(upDownSmoothness, Time.deltaTime);                //Controls the Fly Movement Up and Down
            }

            if (movementAxis.z < 0)                                             //if is walking backwards check for a cliff
            {
                if (!swim && !Fly && !fall && IsFallingBackwards(BackFallRayDistance))
                {
                    maxspeed = 0;
                    V_Smooth = 10;
                }
            }

            vertical = Mathf.Lerp(vertical, movementAxis.z * maxspeed, Time.deltaTime * V_Smooth);             //smooth the Vertical direction Move.Z
            horizontal = Mathf.Lerp(horizontal, movementAxis.x * (Shift ? 2 : 1), Time.deltaTime * H_Smooth);  //smooth the Horizontal direction Move.X

            if (Mathf.Abs(horizontal)>0.1f || (Mathf.Abs(vertical) > 0.2f))   //Check if the Character is Standing
                stand = false;
            else stand = true;

            if (!MovementReleased) stand = false;
           

            if (jump || damaged || stun || fall || swim || fly || isInAir || (tired >= GotoSleep && GotoSleep != 0)) stand = false; //Stand False when doing some action

            if (tired >= GotoSleep) tired = 0;                  //Reset Time Out

            if (!stand) tired = 0;                              //Reset Tired if is moving;

            if (!swim && !fly) movementAxis.y = 0;              //Reset Movement in Y if is not swimming or flying
        }

        /// <summary>
        /// Distance calculated for the RayCasts Hip-Chest or Water Pivot
        /// </summary>
        protected float FIXdistance;

        void FixedUpdate()
        {
            if (fly || underwater) return;              //Dont calculate the methods below  if we are swimming

            float time = Time.fixedDeltaTime;

            if (swim && AnimState != AnimTag.SwimJump )      //if is not Swim Jumping then aling with the water (THIS IS NEEDED TO BE DONE IN FIXEDUPDATE)
            {
                YFix = ((Waterlevel - _Height + waterLine) - _transform.position.y) * time * 5f;   //Smoothy Aling position with the Water
            }

            FixPosition(time);
            UpdatePlatformMovement(false);

            FixedDeltaPos.y += YFix;
            _transform.position += FixedDeltaPos;

            YFix = 0;
            FixedDeltaPos = Vector3.zero;
        }

        void Update()
        {
            UpdateSettings();

            if (CanGoUnderWater && underwater) return;           //Dont calculate the methods below  if we are swimming

            RayCasting();

            var time = Time.deltaTime;

            AdditionalSpeed(time);                                //Apply Speed movement Turn movement ON UPDATE
            AdditionalTurn(time);                                 //Apply Additional Turn movement ON UPDATE
            Swimming(time);                                       //Calculate Swimming Logic when not falling on Update
            FixRotation(time);                                    //Apply Extra rotation (for aligning to the terrain)
            UpdatePlatformMovement(true);
            Falling();
          
        }

        /// <summary>
        /// Things to do every Frame
        /// </summary>
        public virtual void UpdateSettings()
        {
            CurrentAnimState = Anim.GetCurrentAnimatorStateInfo(0).tagHash;         //Store the Current Animation State on the Base Layer
            NextAnimState = Anim.GetNextAnimatorStateInfo(0).tagHash;               //Store the Next    Animation State on the Base Layer

            if (LastAnimationTag != AnimState) LastAnimationTag = AnimState;       //Check if there's an animation Change and INVOKE THE EVENT 

            T_Up = _transform.up;
            T_Right = _transform.right;
            T_Forward = _transform.forward;

            FrameCounter++;                                  //This is used for reducing the raycasting 
            FrameCounter %= 100000;                         //This is used for reducing the raycasting 
        }

        void LateUpdate()
        {
            MovementSystem(movementS1, movementS2, movementS3);
            LinkingAnimator();              //Set all Animator Parameters
        }

        private void OnAnimatorMove()
        {
            var time = Time.deltaTime;

            if (Anim.applyRootMotion)       //Only Update the RigidBody Velocity when is on RootMotion Mode IMPORTANT
            {
                _RigidBody.velocity = (Anim.deltaPosition + DeltaPosition) / time;
            }

            _transform.rotation *= Anim.deltaRotation * DeltaRotation; //Apply the Rotation |I did not find another way ;(  |


            DeltaPosition = Vector3.zero;
            DeltaRotation = Quaternion.identity;

            LastPosition = _transform.position;      //Store the last position of the animal... (Check for the fall and swim states);
        }

        protected virtual void OnAnimationStateEnter(int animTag)
        {
            if (animTag == AnimTag.Locomotion) IsInAir = false; //Double Check that when we enter to locomotion we Lock the RB Constraints
        }

        void OnEnable()
        {
            if (Animals == null) Animals = new List<Animal>();
            Animals.Add(this);          //Save the the Animal on the current List
        }

        void OnDisable()
        {
            Animals.Remove(this);   //Remove all this animal from the Overall AnimalList
        }

        
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (debug && Application.isPlaying)
            {
                Gizmos.color = Color.magenta;
                float sc = _transform.localScale.y;

                if (backray)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(hit_Hip.point, 0.03f * sc);
                }
                if (frontray)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(hit_Chest.point, 0.03f * sc);
                }

              
                GUIStyle newStyle = new GUIStyle();
                newStyle.normal.textColor = RootMotion ? Color.green : Color.black;
                newStyle.fontStyle = _rigidbody.constraints == RigidbodyConstraints.FreezeRotation ? FontStyle.BoldAndItalic : FontStyle.Bold;

                UnityEditor.Handles.Label(_transform.position,"Rootmotion", newStyle);
            }
        }
#endif
    }
}