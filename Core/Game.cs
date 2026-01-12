using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameFrameWork
{
    public partial class Game
    {
        private List<GameObject> objects = new List<GameObject>();
        private List<GameObject> pendingAdd = new List<GameObject>();

        public List<GameObject> Objects => objects;

        public void AddObject(GameObject obj) => pendingAdd.Add(obj);

        public void ProcessPendingObjects()
        {
            if (pendingAdd.Count > 0)
            {
                objects.AddRange(pendingAdd);
                pendingAdd.Clear();
            }
        }

        public void Update(GameTime gameTime)
        {
            ProcessPendingObjects();
            
            var activeObjects = objects.Where(o => o.IsActive).ToList();
            foreach (var obj in activeObjects)
                obj.Update(gameTime);
        }

        public void Draw(Graphics g)
        {
            var activeObjects = objects.Where(o => o.IsActive).ToList();
            foreach (var obj in activeObjects)
                obj.Draw(g);
        }

        public void Cleanup() => objects.RemoveAll(o => !o.IsActive);
    }
}