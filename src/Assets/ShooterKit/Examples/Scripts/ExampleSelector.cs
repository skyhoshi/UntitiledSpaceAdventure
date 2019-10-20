using UnityEngine;
using System.Collections;

/// <summary>
/// Makes a carousel type object to display different demo objects (could be used as a garage or a slidable menu)
/// This works by spawning the next object off-screen then animating it smoothly into the on-screen position, while delating the old object
/// </summary>
public class ExampleSelector : MonoBehaviour {

    public Transform[] ExampleObjects;

    /// <summary>
    /// Position for the left-most object to load in. This should be "off camera"
    /// </summary>
    public Transform DisplayPositionLeft;

    /// <summary>
    /// Position for the right-most object to load in. This should be "off camera"
    /// </summary>
    public Transform DisplayPositionRight;

    /// <summary>
    /// Position which the current selected item will be shown
    /// </summary>
    public Transform DisplayPositionCenter;

    /// <summary>
    /// Current main camera
    /// </summary>
    public Camera MainCamera;
 
    /// <summary>
    /// How long each transition takes
    /// </summary>
    public float SwitchDuration = 1;

    private Transform   NewObject;
    private Transform   CurrentObject;
    public int          ExampleNo = 0;
    private bool        isMoving = false;

    public float fudgeNumber = 1;

    public void Start()
    {
        CurrentObject = (Transform)Instantiate(ExampleObjects[ExampleNo], DisplayPositionCenter.position, gameObject.transform.rotation);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow) == true && isMoving == false)
        {
            PreviousExample(); 
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow) == true && isMoving == false)
        {
            NextExample(); 
        }
    }

    /// <summary>
    /// Slides the corousel to the right
    /// </summary>
    public void NextExample()
    {
        ExampleNo++;
        if (ExampleNo > ExampleObjects.Length - 1)
        {
            ExampleNo = ExampleObjects.Length - 1;
            return;
        }

        isMoving = true;
        
        NewObject = (Transform)Instantiate(ExampleObjects[ExampleNo], DisplayPositionLeft.position, gameObject.transform.rotation);
        StartCoroutine(MoveExamples(DisplayPositionLeft.position.x));
    }

    /// <summary>
    /// Slides the corousel to the left
    /// </summary>
    public void PreviousExample()
    {
        ExampleNo--;
        if (ExampleNo < 0)
        {
            ExampleNo = 0;
            return;
        }

        isMoving = true;
        
        NewObject = (Transform)Instantiate(ExampleObjects[ExampleNo], DisplayPositionRight.position, gameObject.transform.rotation);

        StartCoroutine(MoveExamples(DisplayPositionRight.position.x));
    }

    /// <summary>
    /// Moves examples to the specified position over a SwitchDuration seconds
    /// </summary>
    /// <param name="displayPosX">Position to move the screen to to view the next example (This value comes from the DisplayPosition variables)</param>
    /// <returns></returns>
    IEnumerator MoveExamples(float displayPosX)
    {
        
        yield return new WaitForSeconds(0.01f);

        float switchTime = SwitchDuration;

        Vector3 newObjPos = NewObject.position;
        Vector3 currentObjPos = CurrentObject.position;
        Vector3 pos;

        Vector3 cameraPos = MainCamera.transform.position;
        while (switchTime > 0)
        {
            yield return new WaitForFixedUpdate();
            
            //pos = NewObject.position;
            //pos.x = Mathf.Lerp(DisplayPositionCenter.position.x, newObjPos.x, switchTime);

            //NewObject.position = pos;

            //pos = CurrentObject.position;
            ////Debug.Log("Moving " + pos.x.ToString() + " t = " + switchTime.ToString());
            //pos.x = Mathf.Lerp(displayPosX, currentObjPos.x, switchTime);
            //CurrentObject.position = pos;

            pos = MainCamera.transform.position;
            pos.x = Mathf.Lerp(cameraPos.x, displayPosX, (SwitchDuration - switchTime)/SwitchDuration);
            //pos.x = Move(switchTime, displayPosX, cameraPos.x, fudgeNumber);
            
            MainCamera.transform.position = pos;

            switchTime -= Time.fixedDeltaTime;
        }

        Destroy(CurrentObject.gameObject);
        CurrentObject = NewObject;

        // Move this object to the center of the camera again
        pos = transform.position;
        pos.x = MainCamera.transform.position.x;
        transform.position = pos;
        
        isMoving = false;

    }

    /// <summary>
    /// Springy Tween
    /// </summary>
    /// <param name="t"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public float Move(float t, float b, float c, float d)
    {
        if (t == 0)
        {
            return b;
        }
        if ((t /= d) == 1)
        {
            return b + c;
        }
        float p = d * .3f;
        float s = p / 4;
        return -(float)(c * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
    }

}
