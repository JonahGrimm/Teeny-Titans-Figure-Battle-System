using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CameraState
{
    Split,
    Full
}
public enum Direction
{
    Left,
    Right
}
public enum CinematicType
{
    Projectile,
    Sword
}
//This class is dedicated to the camera transitions between splitscreen and full screen.
//When going to fullscreen, it then passes the desired CinematicType to CutsceneDirector to play the desired cutscene.
public class TransitionCameras : MonoBehaviour
{
    //Variable is made static for CutsceneDirector to know how early to preemptively call Transition() back to normal
    public static float resetTime = 1f;

    public Image rightCamera;
    public Image leftCamera;
    public RectTransform UIobjects;
    public float transitionTime = 2.5f;
    public AnimationCurve outCurve;
    public AnimationCurve inCurve;
    public Vector3 offset = new Vector3(280, 530, 0);
    public float radius = 200f;

    private CutsceneDirector director;
    private CameraState classState = CameraState.Split;
    private bool blockInputs;
    private RectTransform[] movingObjects;
    private Texture fullScreen;
    private Texture right;
    private Texture left;

    //Used for debugging purposes
    //private GameObject touchVisual;

    //Initialize
    void Start()
    {
        leftCamera.transform.SetSiblingIndex(0);
        rightCamera.transform.SetSiblingIndex(1);
        UIobjects.transform.SetSiblingIndex(2);
        movingObjects = new RectTransform[2];
        movingObjects[0] = UIobjects;
        director = FindObjectOfType<CutsceneDirector>() as CutsceneDirector;

        fullScreen = Resources.Load<Texture2D>("Full");
        right = Resources.Load<Texture2D>("Right");
        left = Resources.Load<Texture2D>("Left");
        //touchVisual = Resources.Load<GameObject>("TouchVisualizer");

        ResetTextures();
    }

    /// <summary>Transition to cinematic camera for attack.</summary>
    /// <param name="LoR">Direction, Left or Right, for the camera that will be focused</param>
    /// <param name="type">Type of attack that will be passed to the cinematic class to show the proper animations</param>
    /// <returns>Returns whether the transition was processed sucessfully (Are inputs being blocked?)</returns>
    public bool Transition(Direction LoR, CinematicType type)
    {
        if (blockInputs || classState == CameraState.Full)
            return false;

        //If transitioning to fullscreen left
        if (LoR == Direction.Left)
        {
            leftCamera.material.SetTexture("_Mask", fullScreen);
            leftCamera.transform.SetAsFirstSibling();
            director.PlayCutscene(LoR, type);
        }
        //If transitioning to fullscreen right
        else
        {
            rightCamera.material.SetTexture("_Mask", fullScreen);
            rightCamera.transform.SetAsFirstSibling();
            director.PlayCutscene(LoR, type);
        }

        StartCoroutine(SwitchCameras(LoR, CameraState.Split));
        classState = CameraState.Full;
        return true;
    }

    /// <summary>Transition back to splitscreen. Use overload for transition to cinematic.</summary>
    /// <param name="LoR">Direction, Left or Right, for the camera that is focused</param>
    /// <returns>Returns whether the transition was processed sucessfully (Are inputs being blocked?)</returns>
    public bool Transition(Direction LoR)
    {
        if (blockInputs || classState == CameraState.Split)
            return false;

        StartCoroutine(SwitchCameras(LoR, CameraState.Full));
        classState = CameraState.Split;

        return true;
    }

    /// <summary>Coroutine that switches either to fullscreen or splitscreen view.</summary>
    /// <param name="LoR">Direction, Left or Right, for the camera that will be/is focused</param>
    /// <param name="beforeState">Before transition camera state</param>
    private IEnumerator SwitchCameras(Direction LoR, CameraState beforeState)
    {
        blockInputs = true;

        Vector3 localOffset = offset;
        //If transitioning to the right camera, add left camera to the animator array and INVERT the offset
        if (LoR == Direction.Right)
        {
            movingObjects[1] = leftCamera.rectTransform;
            localOffset *= -1f;
        }
        //If transitioning to the left camera, add right camera to the animator array
        else
            movingObjects[1] = rightCamera.rectTransform;

        //If transitioning to fullscreen
        if (beforeState == CameraState.Split)
        {
            //Interpolate UI and other elements to offscreen position based on a custom curve
            float timeElapsed = 0f;            
            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;
                timeElapsed = Mathf.Clamp(timeElapsed, 0f, transitionTime);
                foreach (RectTransform rt in movingObjects)
                {
                    rt.anchoredPosition = outCurve.Evaluate(timeElapsed / transitionTime) * localOffset;
                }
                yield return null;
                timeElapsed += Time.deltaTime;
            }
        }
        //If transitioning to splitscreen
        if (beforeState == CameraState.Full)
        {
            //Interpolate UI and other elements to default position based on a custom curve
            float timeElapsed = 0f;
            while (timeElapsed < resetTime)
            {
                timeElapsed += Time.deltaTime;
                timeElapsed = Mathf.Clamp(timeElapsed, 0f, resetTime);
                foreach (RectTransform rt in movingObjects)
                {
                    rt.anchoredPosition = inCurve.Evaluate(1 - (timeElapsed / transitionTime)) * localOffset;
                }
                yield return null;
            }
        }

        //If the camera is now back to splitscreen
        if (beforeState == CameraState.Full)
        {
            //Reset the textures to avoid weird overlap and let the players give inputs again
            ResetTextures();
            var manager = FindObjectOfType<GameManager>() as GameManager;
            manager.ResumeMeter();
        }

        blockInputs = false;
    }

    /// <summary>Resets textures to default splitscreen masks</summary>
    private void ResetTextures()
    {
        leftCamera.material.SetTexture("_Mask", left);
        rightCamera.material.SetTexture("_Mask", right);
    }
}
