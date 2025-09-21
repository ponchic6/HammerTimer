using System.Collections;
using Code.Infrastructure.Services;
using Entitas;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Zenject;

namespace Tests.Editor
{
    public class MovementInputTests
    {
        private DiContainer _container;
        private IInputService _inputService;
        private GameContext _game;
        private IGroup<GameEntity> _group;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SceneManager.LoadScene("SampleScene");

            _container = ProjectContext.Instance.Container;
            _inputService = _container.Resolve<IInputService>();

            Contexts contexts = Contexts.sharedInstance;
            _game = contexts.game;
            _group = _game.GetGroup(GameMatcher.AllOf(GameMatcher.MovementInput));
        }

        [TearDown]
        public void TearDown()
        {
            // Ensure that no simulated keys leak between tests
            _inputService.ReleaseKey(KeyCode.W);
            _inputService.ReleaseKey(KeyCode.A);
            _inputService.ReleaseKey(KeyCode.S);
            _inputService.ReleaseKey(KeyCode.D);
        }

        private GameEntity GetInputEntity()
        {
            // There should be exactly one entity with MovementInput created by InitializeInputSystem
            GameEntity[] entities = _group.GetEntities();
            Assert.GreaterOrEqual(entities.Length, 1, "No MovementInput entity found. Ensure InitializeInputSystem runs in the scene.");
            return entities[0];
        }

        [UnityTest]
        public IEnumerator Initial_state_is_zero_vector()
        {
            // Wait a frame to allow EcsRunner.Start() to initialize features
            yield return null;

            GameEntity entity = GetInputEntity();
            Vector2 direction = entity.movementInput.direction;
            Assert.AreEqual(Vector2.zero, direction);
        }

        [UnityTest]
        public IEnumerator Pressing_W_sets_direction_up()
        {
            _inputService.HoldKey(KeyCode.W);
            yield return null; // let MovementInputSystem run

            GameEntity entity = GetInputEntity();
            Vector2 direction = entity.movementInput.direction;
            Assert.AreEqual(Vector2.up, direction);
        }

        [UnityTest]
        public IEnumerator Pressing_S_sets_direction_down()
        {
            _inputService.HoldKey(KeyCode.S);
            yield return null;

            GameEntity entity = GetInputEntity();
            Vector2 direction = entity.movementInput.direction;
            Assert.AreEqual(Vector2.down, direction);
        }

        [UnityTest]
        public IEnumerator Pressing_A_sets_direction_left()
        {
            _inputService.HoldKey(KeyCode.A);
            yield return null;

            GameEntity entity = GetInputEntity();
            Vector2 direction = entity.movementInput.direction;
            Assert.AreEqual(Vector2.left, direction);
        }

        [UnityTest]
        public IEnumerator Pressing_D_sets_direction_right()
        {
            _inputService.HoldKey(KeyCode.D);
            yield return null;

            GameEntity entity = GetInputEntity();
            Vector2 direction = entity.movementInput.direction;
            Assert.AreEqual(Vector2.right, direction);
        }

        [UnityTest]
        public IEnumerator Pressing_A_and_S_sets_normalized_diagonal()
        {
            _inputService.HoldKey(KeyCode.A);
            _inputService.HoldKey(KeyCode.S);
            yield return null;

            GameEntity entity = GetInputEntity();
            Vector2 direction = entity.movementInput.direction;

            Vector2 expected = new Vector2(-1f, -1f).normalized;
            Assert.That(Mathf.Approximately(direction.x, expected.x) && Mathf.Approximately(direction.y, expected.y),
                $"Expected {expected} but was {direction}");
        }
    }
}
