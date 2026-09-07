using UnityEngine;

public class _3DPointPing : MonoBehaviour
{
    public GameObject sphere;
    public AnimationCurve scaleAnimationCurve;
    public Light light;
    float time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light.intensity = 0f;
        sphere.transform.localScale = new Vector3(0, 0, 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        time = time + Time.unscaledDeltaTime*10f;
        sphere.transform.localScale = new Vector3(scaleAnimationCurve.Evaluate(time), scaleAnimationCurve.Evaluate(time), scaleAnimationCurve.Evaluate(time));
        light.intensity = scaleAnimationCurve.Evaluate(time);

        if(time > 10f)
        {
            Exit();
        }
    }

    void Exit()
    {
        Destroy(gameObject);    
    }
}
