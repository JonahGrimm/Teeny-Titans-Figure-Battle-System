using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

//The controller of what cutscene plays after the camera transitions
public class CutsceneDirector : MonoBehaviour
{
    //Both left and right directors should share the same length
    //Should probably have thought of a better way to communicate this in the inspector...
    public PlayableDirector[] leftDirectors;
    public PlayableDirector[] rightDirectors;

    private PlayableDirector[,] directors;
    private TransitionCameras trans;

    //Initialize
    private void Start()
    {
        trans = FindObjectOfType<TransitionCameras>() as TransitionCameras;

        //This could would need to be refactored in the future if the system was to be fully realized
        directors = new PlayableDirector[2, leftDirectors.Length];
        directors[0, 0] = leftDirectors[0];
        directors[0, 1] = leftDirectors[1];
        directors[1, 0] = rightDirectors[0];
        directors[1, 1] = rightDirectors[1];
    }

    ///<summary>Start a coroutine that will play the correct cutscene and wait for it to finish</summary>
    public void PlayCutscene(Direction focusedSide, CinematicType cinematicType)
    {
        StartCoroutine(PlayAndWaitForCutscene(focusedSide, cinematicType));
    }

    ///<summary>Plays the appropriate cutscene and waits for its completion</summary>
    private IEnumerator PlayAndWaitForCutscene(Direction side, CinematicType cinematic)
    {
        //Play the corresponding timeline
        directors[(int)side, (int)cinematic].Play();
        //Waits for cutscene to finish but also takes into account the transition back to splitscreen time
        yield return new WaitForSeconds((float)directors[(int)side, (int)cinematic].duration - TransitionCameras.resetTime);
        //Calls the transition back
        trans.Transition(side);
    }
}
