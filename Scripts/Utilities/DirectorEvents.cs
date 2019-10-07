using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class DirectorEvents : MonoBehaviour
{
    [Tooltip("The animator containing the state machine")]
    public PlayableDirector Director;

    [Tooltip("The event of entering in the state")]
    public UnityEvent OnPlay;

    [Tooltip("The event of exiting from the state")]
    public UnityEvent OnStop;

    // Start is called before the first frame update
    void Start()
    {
        Director.played += (PlayableDirector _) => OnPlay.Invoke();
        Director.stopped += (PlayableDirector _) => OnStop.Invoke();
    }
}
