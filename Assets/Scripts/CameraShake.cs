using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	// How long the object should shake for.
	public float shake = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	
	Vector3 originalPos;
	
	void Awake()
	{

	}
	
	void OnEnable()
	{
		originalPos = gameObject.transform.localPosition;
	}
	
	void Update()
	{
		if (shake > 0)
		{
			gameObject.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
			
			shake -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shake = 0f;
			gameObject.transform.localPosition = originalPos;
		}
	}
}