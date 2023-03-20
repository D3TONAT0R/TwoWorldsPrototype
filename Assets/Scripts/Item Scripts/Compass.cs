using D3T;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds.Items
{
	[ExecuteInEditMode]
	public class Compass : MonoBehaviour
	{
		public Transform needle;
		public float needleAcceleration = 10f;
		[Min(0.001f)]
		public float needleDrag = 0.1f;

		public Transform targetPoint;
		public Vector3 targetHeading = Vector3.forward;

		public ReadOnlyString angleCheck;

		private float momentum;

		private Vector3 projection;

		// Update is called once per frame
		void Update()
		{
			if(targetPoint)
			{
				targetHeading = (targetPoint.position - transform.position).normalized;
			}

			var projected = Vector3.ProjectOnPlane(transform.InverseTransformVector(targetHeading), Vector3.up).normalized;
			var desiredAngle = GetAngle(projected.XZ());
			var force = projected.XZ().magnitude;

			var euler = needle.localEulerAngles;
			if(Application.isPlaying)
			{
				var diff = Mathf.DeltaAngle(euler.y, desiredAngle);
				momentum += diff * Time.deltaTime * needleAcceleration * force;
				momentum = Mathf.Lerp(momentum, 0, Time.deltaTime * needleDrag);
				euler.y += momentum * Time.deltaTime;
			}
			else
			{
				euler.y = desiredAngle;
			}
			needle.localEulerAngles = euler;
		}

		float GetAngle(Vector2 vector)
		{
			

			float a = Mathf.Asin(vector.x) * Mathf.Rad2Deg;
			if(vector.y < 0) a = 180f - a;
			return a;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawLine(Vector3.zero, projection);
		}
	}
}	