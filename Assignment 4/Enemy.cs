using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment_4 {
    public class Enemy : Behavior3d {

        static Random ran = new Random();

        public static Model enemyModel;

        public static int fieldSize = 50;

        public static TerrainRenderer terrain;

        // Static method for creating new enemies
        public static GameObject3d createEnemy(GameObject3d player) {

            GameObject3d g = GameObject3d.Initialize();
            Enemy e = new Enemy(player);
            g.addBehavior(e);
            g.mesh = enemyModel;
            g.material = new StandardLightingMaterial();
            (g.material as StandardLightingMaterial).diffuseColor = Vector3.Up;

            //SETUPSEARCH
            int size = 100;
            e.search = new AStarSearch(size, size); // size of grid 

            // use heightmap
            foreach (AStarNode node in e.search.Nodes) {
                node.Passable = (terrain.GetAltitude(node.Position) < 0.5);
            }
            //******


            e.reposition();
            g.transform.LocalPosition += Vector3.Up * 2;
            g.transform.LocalScale = Vector3.One * 2;

            

            return g;
        }

        public GameObject3d playerObject;

        public Enemy(GameObject3d player) => playerObject = player;

        public override void Start() {
            base.Start();
        }

        public float catchDistance = 5;

        public int timer = 10;

        public override void Update() {
            if (Vector3.Distance(playerObject.transform.Position, transform.Position) < catchDistance){
                reposition();
                Assignment4.catchCount++;
            }

            timer--; 
            if(timer <= 0) {
                timer = 10;
                if(path.Any()) {
                    transform.LocalPosition = path.First();
                    path.Remove(path.First());
                }

                // THIS IS WHERE WE PUT THE LOGIC FOR REACHING THE CENTER
                else {
                    reposition();
                    Assignment4.failureCount++;
                }
            }
        }

        List<Vector3> path = new List<Vector3>();

        public AStarSearch search;
        int size = 100;
        public void reposition() {
            timer = 10;
            search.Start = search.Nodes[ran.Next(0, size), ran.Next(0, size)]; // random current position
            search.End = search.Nodes[50, 50]; // middle

            //--Main Search Process *************/
            search.Search(); // A search is made here.

            path = new List<Vector3>();
            AStarNode current = search.End;
            while (current != null) {
                path.Insert(0, current.Position);
                current = current.Parent;
            }
            /***********************************/

            transform.LocalPosition = search.Start.Position;
        }
    }
}
