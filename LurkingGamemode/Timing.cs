// This was made by probe4aiur on GitHub. You can access the latest version here: 
// https://gist.github.com/probe4aiur/fc74510ea216d30cbb0b6b884c4ba84c

using UnityEngine;
using Smod2.EventHandlers;
using Smod2.Events;

using System;
using System.Collections.Generic;
using System.Linq;

namespace scp4aiur
{
    /// <summary>
    /// Module for easy and efficient frame-based timers
    /// </summary>
    internal class Timing : IEventHandlerUpdate, IEventHandlerRoundRestart
    {
        private static Action<string> log;

        private static int jobId;
        private static Dictionary<int, QueueItem> jobs;

        public static void Init(Smod2.Plugin plugin, Priority priority = Priority.Normal)
        {
            log = plugin.Error;
            plugin.AddEventHandlers(new Timing(), priority);

            jobId = int.MinValue;
            jobs = new Dictionary<int, QueueItem>();
        }

        /// <summary>
        /// Queues a job.
        /// </summary>
        public static int Run(IEnumerable<float> method, bool persist = false)
        {
            int id = jobId++;
            jobs.Add(id, new QueueItem(method, persist));

            return id;
        }

        /// <summary>
        /// Removes a job from the queue.
        /// </summary>
        /// <param name="id">ID of the job to remove.</param>
        public static bool Remove(int id)
        {
            return jobs.Remove(id);
        }

        /// <summary>
        /// <para>DO NOT USE</para>
        /// <para>This is an event for Smod2 and as such should not be called by any external code </para>
        /// </summary>
        /// <param name="ev"></param>
        public void OnUpdate(UpdateEvent ev)
        {
            int[] removeJobs = jobs.Keys.Where(x => jobs[x].Run()).ToArray();

            foreach (int job in removeJobs)
            {
                jobs.Remove(job);
            }
        }

        /// <summary>
        /// <para>DO NOT USE</para>
        /// <para>This is an event for Smod2 and as such should not be called by any external code </para>
        /// </summary>
        /// <param name="ev"></param>
        public void OnRoundRestart(RoundRestartEvent ev)
        {
            int[] removeJobs = jobs.Where(x => !x.Value.RoundPersist).Select(x => x.Key).ToArray();

            foreach (int job in removeJobs)
            {
                jobs.Remove(job);
            }
        }

        private class QueueItem
        {
            private readonly IEnumerator<float> timer;
            private int waitFrames;
            private float waitTime;

            public bool RoundPersist { get; }

            public QueueItem(IEnumerable<float> timer, bool persist)
            {
                this.timer = timer.GetEnumerator();
                RoundPersist = persist;
            }

            public bool Run()
            {
                try
                {
                    if (waitFrames < 1)
                    {
                        if (waitTime <= 0)
                        {
                            if (!timer.MoveNext())
                            {
                                return true;
                            }

                            if (timer.Current > 0)
                            {
                                waitTime += timer.Current;
                            }
                            else
                            {
                                waitFrames = (int)-timer.Current;
                            }
                        }
                        else
                        {
                            waitTime -= Time.deltaTime;
                        }
                    }
                    else
                    {
                        waitFrames--;
                    }

                    return false;
                }
                catch (Exception e)
                {
                    log($"Exception was thrown by job:\n{e}");
                    return true;
                }
            }
        }
    }
}