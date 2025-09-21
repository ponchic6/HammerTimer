using Entitas;
using UnityEngine;

namespace Code.Infrastructure.View
{
    [Game] public class View : IComponent { public EntityBehaviour Value;}
    [Game] public class ViewPath : IComponent { public string Value; }
    [Game] public class ViewPrefab : IComponent { public EntityBehaviour Value; }
    [Game] public class ViewPrefabWithParent : IComponent { public EntityBehaviour Value; public GameObject Parent; }
    [Game] public class InitialTransform : IComponent { public Vector3 Position; public Quaternion Rotation;}
}