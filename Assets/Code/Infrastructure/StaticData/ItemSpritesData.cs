using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.StaticData
{
    [CreateAssetMenu(fileName = "ItemSpritesData", menuName = "StaticData/ItemSpritesData")]
    public class ItemSpritesData : ScriptableObject
    {
        public List<ItemEnumSpritesPair> spritesPairs;
    }
}