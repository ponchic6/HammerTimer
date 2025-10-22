using System.IO;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Code.Infrastructure.View
{
    [Game] public class View : IComponent { public EntityBehaviour Value;}
    [Game] public class ViewPath : IComponent { public string Value; }
    [Game] public class ViewPrefab : IComponent { public EntityBehaviour Value; }
    [Game] public class ViewPrefabWithParent : IComponent { public EntityBehaviour Value; public GameObject Parent; }

    [Game]
    public class InitialTransform : IComponent, IComponentSerializable
    {
        public Vector3 Position; public Quaternion Rotation;
        
        public byte[] SerializeComponent()
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write(Position.x);
                bw.Write(Position.y);
                bw.Write(Position.z);
                bw.Write(Rotation.x);
                bw.Write(Rotation.y);
                bw.Write(Rotation.z);
                bw.Write(Rotation.w);

                return ms.ToArray();
            }
        }    
    }
}