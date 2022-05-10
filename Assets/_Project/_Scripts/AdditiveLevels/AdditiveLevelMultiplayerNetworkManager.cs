using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.Networking;
using Mirror;
using UnityEngine.SceneManagement;
using Mirror.Examples.AdditiveLevels;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Assets._Project._Scripts.AdditiveLevels
{
    public class AdditiveLevelMultiplayerNetworkManager : MultiplayerNetworkServer
    {
        [Header("Additive Scenes - First scene is the start scene")]

        [Scene, Tooltip("Add additive scenes here.\nFirst entry will be players' start scene")]
        // public AssetReference[] additiveScenes;
        public string[] additiveScenes;
        public bool useAddressablesForScenes;
        SceneInstance currentSceneInstance;
        [Header("Fade Control - See child FadeCanvas")]

        // [Tooltip("Reference to FadeInOut script on child FadeCanvas")]
        // public FadeInOut fadeInOut;

        // This is set true after server loads all subscene instances
        bool subscenesLoaded;

        // This is managed in LoadAdditive, UnloadAdditive, and checked in OnClientSceneChanged
        bool isInTransition;
        [Header("Player AssetReference")]
        public AssetReference playerAssetReference;
        bool playerRegistered;

        #region Scene Management

        public override void OnStartClient()
        {
            base.OnStartClient();
            playerRegistered = false;
            Debug.Log($"OnStartClient: Loading player asset");
            playerAssetReference.LoadAssetAsync<GameObject>().Completed += (go) =>
            {
                NetworkClient.RegisterPrefab(go.Result);
                playerRegistered = true;
                Debug.Log($"OnStartClient: Done loading and RegisterPrefab player asset");
            };
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            StartCoroutine(ServerLoadSubScenes());
        }

        /// <summary>
        /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
        /// </summary>
        /// <param name="sceneName">The name of the new scene.</param>
        public override void OnServerSceneChanged(string sceneName)
        {
            // This fires after server fully changes scenes, e.g. offline to online
            // If server has just loaded the Container (online) scene, load the subscenes on server
            if (sceneName == onlineScene)
            {
                // Debug.Log("1. Load Online Scene");
                // Debug.Log("Scene name: " + sceneName);
                // Debug.Log("Online name: " + onlineScene);
                StartCoroutine(ServerLoadSubScenes());
            }
        }
        IEnumerator ServerLoadSubScenes()
        {
            // Debug.Log("2. Loading levels into game");
            foreach (string additiveScene in additiveScenes)
            {
                if (useAddressablesForScenes)
                {
                    AsyncOperationHandle<IList<IResourceLocation>> loadHandle = Addressables.LoadResourceLocationsAsync(additiveScene, typeof(SceneInstance));
                    loadHandle.Completed += loadHandle_Completed;
                    yield return loadHandle;
                    IList<IResourceLocation> locations = loadHandle.Result;
                    string scenePath;

                    #if UNITY_EDITOR
                    bool bundle = false;
                    #endif

                    Debug.Log($"OnFindLocationsForScene : {locations[0].InternalId}");
                    if (Addressables.InternalIdTransformFunc != null)
                    {
                        scenePath = Addressables.InternalIdTransformFunc(locations[0]);
                    }
                    else
                    {
                        scenePath = locations[0].InternalId;
                    }
                    Debug.Log(scenePath);
                    Debug.Log(locations[0].HasDependencies);
                    if (locations[0].HasDependencies)
                    {
                        Debug.Log("Has Dependencies");

                        AsyncOperationHandle<IList<IAssetBundleResource>> depHandle =
                            Addressables.LoadAssetsAsync<IAssetBundleResource>(locations[0].Dependencies,null);
                        depHandle.Completed += depHandle_Completed;
                        yield return depHandle;

                        foreach( IAssetBundleResource resource in depHandle.Result)
                        {
                            if (resource != null && resource.GetAssetBundle() != null)
                            {
                                #if UNITY_EDITOR
                                bundle = true;
                                // break;
                                #endif
                            }
                        }
                    }
                    LoadSceneParameters parameters = new LoadSceneParameters{
                        loadSceneMode = LoadSceneMode.Additive,
                        localPhysicsMode = LocalPhysicsMode.Physics3D
                    };

                    #if UNITY_EDITOR
                    if (bundle)
                    {
                        yield return SceneManager.LoadSceneAsync(scenePath, parameters);
                    }
                    else
                    {
                        yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, parameters);
                    }
                    #else
                    yield return SceneManager.LoadSceneAsync(scenePath, parameters);
                    #endif

                }
                else
                {
                    yield return SceneManager.LoadSceneAsync(additiveScene, new LoadSceneParameters
                    {
                        loadSceneMode = LoadSceneMode.Additive,
                        localPhysicsMode = LocalPhysicsMode.Physics3D // change this to .Physics2D for a 2D game
                    });

                }
            }

            subscenesLoaded = true;
            Debug.Log("3. Loaded levels into game successfully");
        }

        private void loadHandle_Completed(AsyncOperationHandle<IList<IResourceLocation>> obj)
        {
            if (obj.Status == AsyncOperationStatus.Failed)
            {
                Debug.Log("Load Location failed");
            }
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Load Location Succeeded");
            }
        }

        private void depHandle_Completed(AsyncOperationHandle<IList<IAssetBundleResource>> obj)
        {
            if (obj.Status == AsyncOperationStatus.Failed)
            {
                Debug.Log("Load Dependencies failed");
            }
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Load Dependencies Succeeded");
            }
        }

        private void OnFindLocationsForScene(AsyncOperationHandle<IList<IResourceLocation>> obj)
        {
            IList<IResourceLocation> locations = obj.Result;
            string scenePath;
            Debug.Log($"OnFindLocationsForScene : {locations[0].InternalId}");
            if (Addressables.InternalIdTransformFunc != null)
            {
                scenePath = Addressables.InternalIdTransformFunc(locations[0]);
            }
            else 
            {
                scenePath = locations[0].InternalId;
            }
            SceneManager.LoadSceneAsync(scenePath, new LoadSceneParameters{
                loadSceneMode = LoadSceneMode.Additive,
                localPhysicsMode = LocalPhysicsMode.Physics3D
            });
        }

        /// <summary>
        /// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
        /// <para>This allows client to do work / cleanup / prep before the scene changes.</para>
        /// </summary>
        /// <param name="sceneName">Name of the scene that's about to be loaded</param>
        /// <param name="sceneOperation">Scene operation that's about to happen</param>
        /// <param name="customHandling">true to indicate that scene loading will be handled through overrides</param>
        public override void OnClientChangeScene(string sceneName, SceneOperation sceneOperation, bool customHandling)
        {
            Debug.Log($"{System.DateTime.Now:HH:mm:ss:fff} OnClientChangeScene {sceneName} {sceneOperation}");

            if (sceneOperation == SceneOperation.UnloadAdditive)
                StartCoroutine(UnloadAdditive(sceneName));

            if (sceneOperation == SceneOperation.LoadAdditive)
                StartCoroutine(LoadAdditive(sceneName));
        }

        IEnumerator UnloadAdditive(string sceneName)
        {
            isInTransition = true;

            // This will return immediately if already faded in
            // e.g. by LoadAdditive above or by default startup state
            // yield return fadeInOut.FadeIn();

            if (mode == NetworkManagerMode.ClientOnly)
            {
                if (useAddressablesForScenes)
                {
                    yield return Addressables.UnloadSceneAsync(currentSceneInstance);
                }
                else
                {
                    yield return SceneManager.UnloadSceneAsync(sceneName);
                    yield return Resources.UnloadUnusedAssets();
                }
            }

            // Reset these to false when ready to proceed
            NetworkClient.isLoadingScene = false;
            isInTransition = false;

            OnClientSceneChanged();

            // There is no call to FadeOut here on purpose.
            // Expectation is that a LoadAdditive will follow
            // that will call FadeOut after that scene loads.
        }

        IEnumerator LoadAdditive(string sceneName)
        {
            isInTransition = true;

            // This will return immediately if already faded in
            // e.g. by UnloadAdditive above or by default startup state
            // yield return fadeInOut.FadeIn();

            // host client is on server...don't load the additive scene again
            if (mode == NetworkManagerMode.ClientOnly)
            {
                if (useAddressablesForScenes)
                {
                    AsyncOperationHandle<SceneInstance> loadHandle;
                    loadHandle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    loadHandle.Completed += (go) =>
                    {
                        currentSceneInstance = go.Result;
                        
                    };
                    yield return loadHandle;
                }
                else
                {
                    // Start loading the additive subscene
                    loadingSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                    while (loadingSceneAsync != null && !loadingSceneAsync.isDone)
                        yield return null;
                }
            }

            // Reset these to false when ready to proceed
            NetworkClient.isLoadingScene = false;
            isInTransition = false;

            OnClientSceneChanged();

            // yield return fadeInOut.FadeOut();
        }

        

        /// <summary>
        /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
        /// <para>Scene changes can cause player objects to be destroyed. The default implementation of OnClientSceneChanged in the NetworkManager is to add a player object for the connection if no player object exists.</para>
        /// </summary>
        /// <param name="conn">The network connection that the scene change message arrived on.</param>
        public override void OnClientSceneChanged()
        {
            Debug.Log($"{System.DateTime.Now:HH:mm:ss:fff} OnClientSceneChanged {isInTransition}");

            // Only call the base method if not in a transition.
            // This will be called from DoTransition after setting doingTransition to false
            // but will also be called first by Mirror when the scene loading finishes.
            if (!isInTransition)
            {
                base.OnClientSceneChanged();
                Debug.Log("Client Scene Changed");
            }
        }
        #endregion

        #region Server System Callbacks

        /// <summary>
        /// Called on the server when a client is ready.
        /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerReady(NetworkConnection conn)
        {
            //Debug.Log($"OnServerReady {conn} {conn.identity}");
            Debug.Log($"4. OnServerReady {conn} {conn.identity}, loading player");
            // This fires from a Ready message client sends to server after loading the online scene
            base.OnServerReady(conn);

            // conn.authenticationData = NetworkServer.connections.Count;
            Debug.Log("conn.identity null: " + conn.identity == null);
            if (conn.identity == null)
            {
                // StartCoroutine(AddPlayerDelayed(conn));
                StartCoroutine(AddPlayerAssetDelayed(conn));
            }
            Debug.Log($"5. OnServerReady {conn} {conn.identity}, loaded player");
        }

        // This delay is mostly for the host player that loads too fast for the
        // server to have subscenes async loaded from OnServerSceneChanged ahead of it.
        IEnumerator AddPlayerDelayed(NetworkConnection conn)
        {
            Debug.Log($"AddPlayerDelayed {conn} {conn.identity}");
            // Debug.Log("Host go in here");
            // Wait for server to async load all subscenes for game instances
            while (!subscenesLoaded)
                yield return null;

            // Send Scene msg to client telling it to load the first additive scene
            conn.Send(new SceneMessage { sceneName = additiveScenes[0], sceneOperation = SceneOperation.LoadAdditive, customHandling = true });

            // We have Network Start Positions in first additive scene...pick one
            Transform start = GetStartPosition();

            // Instantiate player as child of start position - this will place it in the additive scene
            // This also lets player object "inherit" pos and rot from start position transform
            GameObject player = Instantiate(playerPrefab, start);
            // now set parent null to get it out from under the Start Position object
            player.transform.SetParent(null);

            // Wait for end of frame before adding the player to ensure Scene Message goes first
            yield return new WaitForEndOfFrame();

            // Finally spawn the player object for this connection
            NetworkServer.AddPlayerForConnection(conn, player);
            Debug.Log($"Done AddPlayerDelayed {conn} {conn.identity}");
        }
        IEnumerator AddPlayerAssetDelayed(NetworkConnection conn)
        {
            Debug.Log($"AddPlayerDelayed {conn} {conn.identity}");
            // Debug.Log("Host go in here");
            // Wait for server to async load all subscenes for game instances
            while (!subscenesLoaded)
                yield return null;

            // Send Scene msg to client telling it to load the first additive scene
            conn.Send(new SceneMessage { sceneName = additiveScenes[0], sceneOperation = SceneOperation.LoadAdditive, customHandling = true });

            // We have Network Start Positions in first additive scene...pick one
            Transform start = GetStartPosition();
            yield return new WaitForEndOfFrame();

            // Instantiate player as child of start position - this will place it in the additive scene
            // This also lets player object "inherit" pos and rot from start position transform
            Debug.Log($"AddPlayerAssetDelayed : Start add player asset");
            playerAssetReference.InstantiateAsync(start).Completed += (go) =>
            {
                GameObject player = go.Result;

                player.transform.SetParent(null);
                // player.GetComponent<RuntimeAvatarLoader>().avatarIndex = NetworkServer.connections.Count;
				player.GetComponent<PlayerController>().roomName = additiveScenes[0];
                NetworkServer.AddPlayerForConnection(conn, player);
                Debug.Log($"Done AddPlayerDelayed {conn} {conn.identity}");
            };
        }
        

        #endregion
    }
}