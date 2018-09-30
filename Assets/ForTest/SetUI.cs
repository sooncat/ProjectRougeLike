using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUI : MonoBehaviour {

    public Image MyImage1;
    public Image MyImage2;
    public Slider MySlider;

	// Use this for initialization
	void Start () {
        
		
	}

    void Update()
    {
        Set(MyImage1.transform.position, MyImage2.transform.position, MySlider);
    }
	
    /// <summary>
    /// slider 由p1指向p2
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="mySlider"></param>
	void Set(Vector3 p1, Vector3 p2, Slider mySlider)
	{
	    float centerX = (p1.x + p2.x) / 2;
        float centerY = (p1.y + p2.y) / 2;

        float deltaY = p2.y - p1.y;
        float deltaX = p2.x - p1.x;
        double arc = System.Math.Atan(deltaY / deltaX);
        double angle = (180 / Mathf.PI) * arc;
        if(deltaX<0)
        {
            angle += 180;
        }

        float distance = Vector3.Distance(p1, p2);

        mySlider.transform.position = new Vector3(centerX, centerY, 0);
        mySlider.transform.localEulerAngles = new Vector3(0,0,(float)angle);
        mySlider.GetComponent<RectTransform>().sizeDelta = new Vector2(distance, 20);
        mySlider.GetComponent<Slider>().value = 0.5f;
	}

}
