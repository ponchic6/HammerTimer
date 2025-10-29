using System.Collections.Generic;
using Code.Gameplay.Produce.View;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Produce.Shelf.View
{
    public class ShelfView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private List<EnumVisualPair> visuals;
        private ItemsEnum? _currentItem;
        private GameContext _game;

        private void Start()
        {
            _game = Contexts.sharedInstance.game;
        }

        private void Update()
        {
            if (entityBehaviour.Entity.hasGrabbedItem)
            {
                if (_currentItem.HasValue)
                    return;
                
                GameEntity grabbable = _game.GetEntityWithId(entityBehaviour.Entity.grabbedItem.Value);
                _currentItem = grabbable.grabbableItem.Value;
                visuals.ForEach(x => { x.visual.SetActive(x.@enum == _currentItem); });
            }
            else
            {
                if (!_currentItem.HasValue)
                    return;
                visuals.ForEach(x => { x.visual.SetActive(false); });
                _currentItem = null;
            }
        }
    }
}