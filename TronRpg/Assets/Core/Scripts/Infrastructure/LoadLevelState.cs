using Core.Scripts.CameraLogic;
using Core.Scripts.Logic;
using UnityEngine;

namespace Core.Scripts.Infrastructure
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private const string InitialPointTag = "InitialPoint";
        private const string HeroPlayerPath = "Hero/Player";
        private const string CameraPath = "Camera/Camera Rig";

        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;

        public LoadLevelState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, LoadingCurtain loadingCurtain)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
        }

        public void Enter(string sceneName)
        {
            _loadingCurtain.Show();
            _sceneLoader.Load(sceneName, onLoaded: OnLoaded);
        }


        public void Exit()
        {
            _loadingCurtain.Hide();
        }

        private void OnLoaded()
        {
            var initialPoint = GameObject.FindGameObjectWithTag(InitialPointTag);


            var hero = Instantiate(HeroPlayerPath, initialPoint.transform.position);

            CameraFollow(hero);

            _gameStateMachine.Enter<GameLoopState>();
        }

        private static void CameraFollow(GameObject hero)
        {
            Instantiate(CameraPath)
                .GetComponent<CameraFollow>()
                .Follow(hero);
        }

        private static GameObject Instantiate(string path)
        {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab);
        }

        private static GameObject Instantiate(string path, Vector3 at)
        {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab, at, Quaternion.identity);
        }
    }
}