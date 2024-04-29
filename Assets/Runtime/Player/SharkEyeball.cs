using UnityEngine;

namespace Runtime.Player
{
    public class SharkEyeball : SharkBinder
    {
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            var right = shark.shark.input.moving ? (Vector3)(shark.shark.goalPosition - shark.shark.body.position).normalized : shark.transform.right;
            var up = -Vector3.forward;

            transform.rotation = Quaternion.LookRotation(right, up);
        }
    }
}