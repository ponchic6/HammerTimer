using System;
using Code.Gameplay.Produce.Moulding;
using UnityEngine;

namespace Code.Gameplay.Produce.View
{
    [Serializable]
    public class EnumVisualPair
    {
        public ItemsEnum @enum;
        public GameObject visual;
    }
    
    [Serializable]
    public class MoldEnumVisualPair
    {
        public MoldEnum @enum;
        public GameObject visual;
    }
}