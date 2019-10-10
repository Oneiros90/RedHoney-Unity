using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

///////////////////////////////////////////////////////////////////////////
[CreateAssetMenu(fileName = "StateMachine", menuName = "State Machine/State Machine", order = 1)]
public class StateMachine : ScriptableObject
{
    [Tooltip("The animator controller representing the state machine")]
    public RuntimeAnimatorController Controller;

    [HideInInspector]
    public Animator Animator;

    private StateMachineNode currentNode;
    private Queue<StateMachineNode> nodesWaitingToEnter, nodesWaitingToExit;

    ///////////////////////////////////////////////////////////////////////////
    public void Initialize(Animator animator)
    {
        Animator = animator;

        currentNode = null;
        nodesWaitingToEnter = new Queue<StateMachineNode>();
        nodesWaitingToExit = new Queue<StateMachineNode>();

        // Initialize the nodes
        var nodes = Animator.GetBehaviours<StateMachineNode>();
            foreach (StateMachineNode node in nodes)
                node.Init(this);
    }

    ///////////////////////////////////////////////////////////////////////////
    public void TryToEnter(StateMachineNode node)
    {
        if (currentNode == null)
        {
            currentNode = node;
            currentNode.OnEnter.Invoke();
            if (nodesWaitingToExit.Count > 0 && nodesWaitingToExit.Peek() == node)
                TryToExit(nodesWaitingToExit.Dequeue());
        }
        else if (currentNode == node)
        {
            Debug.LogWarning($"State node {node.name} is trying to enter but it is already active!");
        }
        else if (currentNode != node)
        {
            nodesWaitingToEnter.Enqueue(node);
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    public void TryToExit(StateMachineNode node)
    {
        if (currentNode == null)
        {
            Debug.LogWarning($"State node {node.name} is trying to exit but there is no active nodes!");
        }
        else if (currentNode == node)
        {
            currentNode.OnExit.Invoke();
            currentNode = null;
            if (nodesWaitingToEnter.Count > 0)
                TryToEnter(nodesWaitingToEnter.Dequeue());
        }
        else if (currentNode != node)
        {
            nodesWaitingToExit.Enqueue(node);
        }
    }




    ///////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR

    public SortedDictionary<string, T> FindAllBehaviours<T>() where T : StateMachineBehaviour
    {
        return FindAllBehaviours<T>(Controller);
    }

    public static SortedDictionary<string, T> FindAllBehaviours<T>(RuntimeAnimatorController runtimeController) where T : StateMachineBehaviour
    {
        var behaviours = new SortedDictionary<string, T>();

        AnimatorController controller = runtimeController as AnimatorController;

        if (controller != null)
        {
            // This utility local function fills the behaviours dictionary
            void lookForBehaviour(StateMachineBehaviour bhv, params string[] nameComponents)
            {
                if (bhv != null && bhv is T)
                {
                    string behaviourName = string.Join(".", nameComponents.Where(c => c.Length > 0).ToArray());
                    behaviours.Add(behaviourName, bhv as T);
                }
            }

            // This utility local function deep-scans all possible behaviours in a state machine
            void scanStateMachine(string layerName, AnimatorStateMachine stateMachine)
            {
                // Scan behaviours on the state machine itself (only for sub-state machine)
                foreach (var behaviour in stateMachine.behaviours)
                    lookForBehaviour(behaviour, layerName, stateMachine.name + " (sub-state)");

                // Scan each state
                foreach (ChildAnimatorState state in stateMachine.states)
                    foreach (var behaviour in state.state.behaviours)
                        lookForBehaviour(behaviour, layerName, stateMachine.name, state.state.name);

                // Scan each sub-state machine
                foreach (var subStateMachine in stateMachine.stateMachines)
                    scanStateMachine(stateMachine.name, subStateMachine.stateMachine);
            }

            // Start a recursive scan from each layer of the controller
            foreach (AnimatorControllerLayer layer in controller.layers)
                scanStateMachine("", layer.stateMachine);
        }
        return behaviours;
    }
#endif
}
