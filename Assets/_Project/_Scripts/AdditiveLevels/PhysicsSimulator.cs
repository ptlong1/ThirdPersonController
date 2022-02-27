using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Assets._Project._Scripts.AdditiveLevels
{
    public class PhysicsSimulator : MonoBehaviour
    {
        PhysicsScene physicsScene;
        PhysicsScene2D physicsScene2D;

        bool simulatePhysicsScene;
        bool simulatePhysicsScene2D;

        void Awake()
        {
            if (NetworkServer.active)
            {
                physicsScene = gameObject.scene.GetPhysicsScene();
                simulatePhysicsScene = physicsScene.IsValid() && physicsScene != Physics.defaultPhysicsScene;
                // Debug.Log("Physics Simulator" + (physicsScene != Physics.defaultPhysicsScene).ToString());

                physicsScene2D = gameObject.scene.GetPhysicsScene2D();
                simulatePhysicsScene2D = physicsScene2D.IsValid() && physicsScene2D != Physics2D.defaultPhysicsScene;
            }
            else
            {
                enabled = false;
            }
        }

        void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            if (simulatePhysicsScene)
                physicsScene.Simulate(Time.fixedDeltaTime);

            if (simulatePhysicsScene2D)
                physicsScene2D.Simulate(Time.fixedDeltaTime);
        }
    }
}