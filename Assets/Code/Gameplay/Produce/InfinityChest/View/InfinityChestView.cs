using Code.Gameplay.Produce.View;
using Code.Infrastructure.StaticData;
using Code.Infrastructure.View;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Produce.InfinityChest.View
{
    public class InfinityChestView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private SpriteRenderer spriteTemplate;
        private ItemSpritesData _itemSpritesData;

        [Inject]
        public void Construct(CommonStaticData commonStaticData)
        {
            _itemSpritesData = commonStaticData.itemSpritesData;
        }

        private void Start()
        {
            UpdateSprite();
            
            entityBehaviour.Entity.OnComponentAdded += OnComponentAdded;
            entityBehaviour.Entity.OnComponentReplaced += OnComponentReplaced;
            entityBehaviour.Entity.OnComponentRemoved += OnComponentRemoved;
        }

        private void OnDestroy()
        {
            if (entityBehaviour.Entity != null)
            {
                entityBehaviour.Entity.OnComponentAdded -= OnComponentAdded;
                entityBehaviour.Entity.OnComponentReplaced -= OnComponentReplaced;
                entityBehaviour.Entity.OnComponentRemoved -= OnComponentRemoved;
            }
        }

        private void OnComponentAdded(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.GrabbedItem)
            {
                UpdateSprite();
            }
        }

        private void OnComponentReplaced(IEntity entity, int index, IComponent previousComponent, IComponent newComponent)
        {
            if (index == GameComponentsLookup.InfinityBox)
            {
                UpdateSprite();
            }
        }

        private void OnComponentRemoved(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.GrabbedItem)
            {
                HideSprite();
            }
        }

        private void UpdateSprite()
        {
            ItemsEnum itemType = entityBehaviour.Entity.infinityBox.Value;

            foreach (ItemEnumSpritesPair pair in _itemSpritesData.spritesPairs)
            {
                if (pair.Enum == itemType)
                {
                    spriteTemplate.sprite = pair.Sprite;
                    return;
                }
            }

            HideSprite();
        }

        private void HideSprite() => 
            spriteTemplate.sprite = null;
    }
}
