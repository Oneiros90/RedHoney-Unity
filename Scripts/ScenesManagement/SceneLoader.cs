using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;


namespace RedHoney.ScenesManagement
{
    using Debug = Log.ContextDebug<SceneLoader>;

    ///////////////////////////////////////////////////////////////////////////
    public class SceneLoader : MonoBehaviour
    {
        [Tooltip("The scene to load")]
        public SceneField Scene;

        [Tooltip("If true, the current loaded scenes will be closed")]
        public bool SingleMode = true;

        [Tooltip("If true, the scene will be loaded as soon as this component starts. " +
            "If false, you will need to load it manually through the " + nameof(LoadScene) + "() function")]
        public bool LoadAutomatically = false;

        [Tooltip("If true, the scene (i.e. its gameobjects) will start as soon as it is loaded. " +
            "If false, you will need to start it manually through the " + nameof(StartLoadedScene) + "() function")]
        public bool StartAutomatically = false;

        [Tooltip("If true, the scene will be activated (i.e. set as main scene) as soon as it starts. " +
            "If false, you will need to activate it manually through the " + nameof(ActivateLoadedScene) + "() function")]
        public bool ActivateAutomatically = false;

        [Tooltip("The event fired when the scene is loaded")]
        public UnityEvent OnSceneLoaded;

        [Tooltip("The event fired when the scene started")]
        public UnityEvent OnSceneStarted;


        private AsyncOperation asyncOperation;


        /// <summary>
        /// Returns true if the scene has been fully loaded and it started
        /// </summary>
        public bool IsStarted => asyncOperation != null && asyncOperation.isDone;

        /// <summary>
        /// Returns true if the scene has been fully loaded
        /// </summary>
        public bool IsFullyLoaded => asyncOperation != null && asyncOperation.progress >= 0.9f;

        /// <summary>
        /// Returns true if the scene is loading
        /// </summary>
        public bool IsLoading => asyncOperation != null && asyncOperation.progress < 0.9f;

        /// <summary>
        /// Returns the loading progress
        /// </summary>
        public float LoadingProgress => asyncOperation == null ? 0.0f : asyncOperation.progress / 0.9f;


        ///////////////////////////////////////////////////////////////////////////
        // Start is called before the first frame update
        public void Start()
        {
            if (LoadAutomatically)
                LoadScene();
        }

        ///////////////////////////////////////////////////////////////////////////
        // 
        public void LoadScene()
        {
            if (Scene == null)
            {
                Debug.LogError("Scene was not set!");
                return;
            }
            if (IsLoading || IsFullyLoaded && !IsStarted)
            {
                Debug.LogWarning("Trying to asynchroudly loading the same scene multiple times!");
                return;
            }

            IEnumerator coroutine()
            {
                // Skip one frame
                yield return null;

                // Setup and start the async scene load
                LoadSceneMode loadMode = SingleMode ? LoadSceneMode.Single : LoadSceneMode.Additive;
                asyncOperation = SceneManager.LoadSceneAsync(Scene, loadMode);
                asyncOperation.allowSceneActivation = StartAutomatically;

                // Wait until is fully loaded
                while (IsLoading)
                    yield return null;

                // Invoke the scene loaded event
                OnSceneLoaded.Invoke();

                // Wait until it starts
                while (!IsStarted)
                    yield return null;

                // Invoke the scene started event
                OnSceneStarted.Invoke();

                // Activate it now if requested
                if (ActivateAutomatically)
                    ActivateLoadedScene();
            }
            StartCoroutine(coroutine());
        }

        ///////////////////////////////////////////////////////////////////////////
        public void StartLoadedScene()
        {
            if (IsFullyLoaded)
                asyncOperation.allowSceneActivation = true;
        }

        ///////////////////////////////////////////////////////////////////////////
        public void ActivateLoadedScene()
        {
            if (IsStarted)
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(Scene));
        }

    }
}