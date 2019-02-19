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
    internal class Timing : IEventHandlerUpdate
    {
        private static Action<string> log;

        private static int nextNextTickId;
        private static int nextNextTicksId;
        private static int nextTimersId;

        private static Dictionary<int, Action> nextTick;
        private static Dictionary<int, NextTicksQueue> nextTicks;
        private static Dictionary<int, TimerQueue> timers;

        public Timing(Action<string> log)
        {
            Timing.log = log;

            nextNextTickId = int.MinValue;
            nextNextTicksId = int.MinValue;
            nextTimersId = int.MinValue;

            nextTick = new Dictionary<int, Action>();
            nextTicks = new Dictionary<int, NextTicksQueue>();
            timers = new Dictionary<int, TimerQueue>();
        }

        /// <summary>
        /// Queues a job for the next tick.
        /// </summary>
        /// <param name="action">Job to execute.</param>
        public static int NextTick(Action action)
        {
            int id = nextNextTickId++;
            nextTick.Add(id, action);

            return id;
        }

        /// <summary>
        /// Removes a job from the queue.
        /// </summary>
        /// <param name="id">ID of the job to remove.</param>
        public static bool RemoveNextTick(int id)
        {
            return nextTick.Remove(id);
        }

        /// <summary>
        /// Queues a job to run in a certain amount of ticks.
        /// </summary>
        /// <param name="action">Job to execute.</param>
        /// <param name="ticks">Number of ticks to wait.</param>
        public static int NextTicks(Action action, int ticks)
        {
            int id = nextNextTicksId++;
            nextTicks.Add(id, new NextTicksQueue
            {
                action = action,
                ticksLeft = ticks
            });

            return id;
        }

        /// <summary>
        /// Removes a job from the queue.
        /// </summary>
        /// <param name="id">ID of the job to remove.</param>
        public static bool RemoveNextTicks(int id)
        {
            return nextTicks.Remove(id);
        }

        /// <summary>
        /// Queues a job to run in a certain amount of seconds
        /// </summary>
        /// <param name="action">Job to execute.</param>
        /// <param name="seconds">Number of seconds to wait.</param>
        public static int Timer(Action<float> action, float seconds)
        {
            int id = nextTimersId++;
            timers.Add(id, new TimerQueue
            {
                action = action,
                timeLeft = seconds
            });

            return id;
        }

        /// <summary>
        /// Removes a job from the queue.
        /// </summary>
        /// <param name="id">ID of the job to remove.</param>
        public static bool RemoveTimer(int id)
        {
            return timers.Remove(id);
        }

        /// <summary>
        /// <para>DO NOT USE</para>
        /// <para>This is an event for Smod2 and as such should not be called by any external code </para>
        /// </summary>
        /// <param name="ev"></param>
        public void OnUpdate(UpdateEvent ev)
        {
            if (nextTick.Count > 0)
            {
                foreach (int id in nextTick.Select(x => x.Key).ToArray())
                {
                    Action action = nextTick[id];
                    nextTick.Remove(id); //remove from queue before running so it doesnt loop if exception is thrown

                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        log($"Exception thrown by next-tick job:\n{e}");
                    }
                }
            }

            if (nextTicks.Count > 0)
            {
                foreach (int id in nextTicks.Select(x => x.Key).ToArray())
                {
                    if (--nextTicks[id].ticksLeft == 0)
                    { //if the job is scheduled for this tick
                        Action action = nextTicks[id].action;
                        nextTicks.Remove(id); //remove from queue before running so it doesnt loop if exception is thrown

                        try
                        {
                            action();
                        }
                        catch (Exception e)
                        {
                            log($"Exception thrown by next-ticks job:\n{e}");
                        }
                    }
                }
            }

            if (timers.Count > 0)
            {
                foreach (int id in timers.Select(x => x.Key).ToArray())
                {
                    if ((timers[id].timeLeft -= Time.deltaTime) <= 0)
                    {
                        //if the timer is up
                        Action<float> action = timers[id].action;
                        float timeLeft = timers[id].timeLeft;
                        timers.Remove(id); //remove from queue before running so it doesnt loop if exception is thrown

                        try
                        {
                            action(timeLeft); //run with inaccuracy as argument
                        }
                        catch (Exception e)
                        {
                            log($"Exception thrown by timed job:\n{e}");
                        }
                    }
                }
            }
        }

        private class NextTicksQueue
        {
            public int ticksLeft;
            public Action action;
        }

        private class TimerQueue
        {
            public float timeLeft;
            public Action<float> action;
        }
    }
}