using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling
{
    public class PTGrowingManager : MonoBehaviour
    {
        private List<PTGrowing> pTGrowings = new List<PTGrowing>();
        private Queue<Task> taskQueue = new Queue<Task>();
        private bool isProcessingQueue = false;

        public void RegisterPTGrowing(PTGrowing pTGrowing)
        {
            if (!pTGrowings.Contains(pTGrowing))
            {
                pTGrowings.Add(pTGrowing);
            }
        }

        public void UnregisterPTGrowing(PTGrowing pTGrowing)
        {
            if (pTGrowings.Contains(pTGrowing))
            {
                pTGrowings.Remove(pTGrowing);
            }
        }

        private void Update()
        {
            ProcessTaskQueue();
        }

        private void ProcessTaskQueue()
        {
            if (isProcessingQueue || taskQueue.Count == 0)
            {
                return;
            }

            isProcessingQueue = true;

            Task nextTask = taskQueue.Peek();

            PTGrowing pTGrowing = nextTask.pTGrowing;

            if (nextTask.enableNavMeshCut)
            {
                pTGrowing.EnableNavMeshCutInternal();
            }
            else
            {
                pTGrowing.DisableNavMeshCutInternal();
            }
        }

        public void AddTask(PTGrowing pTGrowing, bool enableNavMeshCut)
        {
            Task task = new Task(pTGrowing, enableNavMeshCut);

            if (!taskQueue.Contains(task))
            {
                taskQueue.Enqueue(task);
            }
        }

        public void CompleteTask(PTGrowing pTGrowing)
        {
            if (taskQueue.Count > 0 && taskQueue.Peek().pTGrowing == pTGrowing)
            {
                taskQueue.Dequeue();
            }

            isProcessingQueue = false;
        }

        private class Task
        {
            public PTGrowing pTGrowing;
            public bool enableNavMeshCut;

            public Task(PTGrowing pTGrowing, bool enableNavMeshCut)
            {
                this.pTGrowing = pTGrowing;
                this.enableNavMeshCut = enableNavMeshCut;
            }
        }
    }
}
