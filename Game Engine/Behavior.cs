using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine
{
    public abstract class Behavior
    {
        protected GameObject obj;
        public void assign(GameObject g) => obj = g;
        virtual public void Start() { }
        virtual public void Update() { }
        virtual public void LateUpdate() { }
        virtual public void OnDestory() { }

        public void release() => obj.releaseBehavior(this);
    }

    public abstract class Behavior2d : Behavior
    {
       public Sprite objSprite { get => (obj as GameObject2d).sprite; set => (obj as GameObject2d).sprite = value; }
    }

    public abstract class Behavior3d : Behavior {
        public Transform objSprite { get => (obj as GameObject3d).transform; set => (obj as GameObject3d).transform = value; }
    }
}
