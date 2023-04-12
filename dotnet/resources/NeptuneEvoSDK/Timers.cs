using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkAPI;

namespace Redage.SDK
{
    /// <summary>
    /// 
    /// </summary>
    public static class Timers
    {
        /// <summary>
        /// 
        /// </summary>
        public static ConcurrentDictionary<string, nTimer> TimersData = new ConcurrentDictionary<string, nTimer>();
        /// <summary>
        /// 
        /// </summary>
        public static nLog Log = new nLog("nTimer");
        private static Thread thread;

        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            thread = new Thread(Logic);
            thread.IsBackground = true;
            thread.Name = "nTimer";
            thread.Start();
        }
        private static void Logic()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        foreach (string TimerId in TimersData.Keys.ToList())
                        {
                            try
                            {
                                if (!TimersData.ContainsKey(TimerId)) continue;
                                nTimer timer = TimersData[TimerId];
                                if (timer != null && !timer.isFinished) timer.Elapsed();
                                else if (timer != null && timer.isFinished && TimersData.ContainsKey(timer.ID)) TimersData.TryRemove(timer.ID, out _);
                            }
                            catch (Exception e)
                            {
                                Log.Write($"Logic Foreach Exception: {e.ToString()}");
                            }
                        }
                        Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"Logic While Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"Logic Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Находит и возвращает объект таймера
        /// </summary>
        /// <param name="id">Уникальный идентификатор таймера</param>
        /// <returns>Объект таймера</returns>
        public static nTimer Get(string id)
        {
            try
            {
                if (TimersData.ContainsKey(id)) return TimersData[id];
                return null;
            }
            catch (Exception e)
            {
                Log.Write($"Get Exception: {e.ToString()}");
                return null;
            }
        }

        /// <summary>
        /// Start() запускает таймер и возвращает случайный ID
        /// </summary>
        /// <param name="interval">Интервал срабатывания действия</param>
        /// <param name="action">Лямбда-выражение с действием</param>
        /// <param name="isnapitask">Нужно ли выполнить это в главном потоке</param>
        /// <returns>Уникальный ID таймера</returns>
        public static string Start(int interval, Action action, bool isnapitask = false)
        {
            try
            {
                string id = Guid.NewGuid().ToString();
                nTimer newtimer = new nTimer(action, id, interval, isnapitask_: isnapitask);
                lock (TimersData)
                {
                    TimersData.TryAdd(id, newtimer);
                }
                return id;
            }
            catch (Exception e)
            {
                Log.Write($"Start Exception: {e.ToString()}");
                return null;
            }
        }
        /// <summary>
        /// Start() запускает таймер с уникальным ID
        /// </summary>
        /// <exception>
        /// Exception возникает при передаче уже существующего ID или значения null
        /// </exception>
        /// <param name="id">Уникальный идентификатор таймера</param>
        /// <param name="interval">Интервал срабатывания действия</param>
        /// <param name="action">Лямбда-выражение с действием</param>
        /// <param name="isnapitask">Нужно ли выполнить это в главном потоке</param>
        /// <returns>Уникальный ID таймера</returns>
        public static string Start(string id, int interval, Action action, bool isnapitask = false)
        {
            try
            {
                if (id is null) throw new Exception("Id cannot be null");
                if (TimersData.ContainsKey(id)) throw new Exception("This id is already in use!");
                nTimer newtimer = new nTimer(action, id, interval, isnapitask_: isnapitask);
                lock (TimersData)
                {
                    TimersData.TryAdd(id, newtimer);
                }
                return id;
            }
            catch (Exception e)
            {
                Log.Write($"Start({id}) Exception: {e.ToString()}");
                return null;
            }
        }
        /// <summary>
        /// StartOnce() запускает таймер один раз и возвращает случайный ID
        /// </summary>
        /// <param name="interval">Интервал срабатывания действия</param>
        /// <param name="action">Лямбда-выражение с действием</param>
        /// <param name="isnapitask">Нужно ли выполнить это в главном потоке</param>
        /// <returns>Уникальный ID таймера</returns>
        public static string StartOnce(int interval, Action action, bool isnapitask = false)
        {
            try
            {
                string id = Guid.NewGuid().ToString();
                nTimer newtimer = new nTimer(action, id, interval, true, isnapitask_: isnapitask);
                lock (TimersData)
                {
                    TimersData.TryAdd(id, newtimer);
                }
                return id;
            }
            catch (Exception e)
            {
                Log.Write($"StartOnce Exception: {e.ToString()}");
                return null;
            }
        }
        /// <summary>
        /// StartOnce() запускает таймер один раз и возвращает ID
        /// </summary>
        /// <exception>
        /// Exception возникает при передаче уже существующего ID или значения null
        /// </exception>
        /// <param name="id">Уникальный идентификатор таймера</param>
        /// <param name="interval">Интервал срабатывания действия</param>
        /// <param name="action">Лямбда-выражение с действием</param>
        /// <param name="isnapitask">Нужно ли выполнить это в главном потоке</param>
        /// <returns>Уникальный ID таймера</returns>
        public static string StartOnce(string id, int interval, Action action, bool isnapitask = false)
        {
            try
            {
                if (id is null) throw new Exception("Id cannot be null");
                if (TimersData.ContainsKey(id)) throw new Exception("This id is already in use!");
                nTimer newtimer = new nTimer(action, id, interval, true, isnapitask_: isnapitask);
                lock (TimersData)
                {
                    TimersData.TryAdd(id, newtimer);
                }
                return id;
            }
            catch (Exception e)
            {
                Log.Write($"StartOnce({id}) Exception: {e.ToString()}");
                return null;
            }
        }
        /// <summary>
        /// StartTask() запускает таймер отдельной задачей и возвращает случайный ID
        /// </summary>
        /// <param name="interval">Интервал срабатывания действия</param>
        /// <param name="action">Лямбда-выражение с действием</param>
        /// <returns>Уникальный ID таймера</returns>
        public static string StartTask(int interval, Action action)
        {
            try
            {
                string id = Guid.NewGuid().ToString();
                nTimer newtimer = new nTimer(action, id, interval, false, true);
                lock (TimersData)
                {
                    TimersData.TryAdd(id, newtimer);
                }
                return id;
            }
            catch (Exception e)
            {
                Log.Write($"StartTask Exception: {e.ToString()}");
                return null;
            }
        }
        /// <summary>
        /// StartTask() запускает таймер отдельной задачей и возвращает ID
        /// </summary>
        /// <exception>
        /// Exception возникает при передаче уже существующего ID или значения null
        /// </exception>
        /// <param name="id">Уникальный идентификатор таймера</param>
        /// <param name="interval">Интервал срабатывания действия</param>
        /// <param name="action">Лямбда-выражение с действием</param>
        /// <returns>Уникальный ID таймера</returns>
        public static string StartTask(string id, int interval, Action action)
        {
            try
            {
                if (id is null) throw new Exception("Id cannot be null");
                if (TimersData.ContainsKey(id)) throw new Exception("This id is already in use!");

                nTimer newtimer = new nTimer(action, id, interval, false, true);
                lock (TimersData)
                {
                    TimersData.TryAdd(id, newtimer);
                }
                return id;
            }
            catch (Exception e)
            {
                Log.Write($"StartTask({id}) Exception: {e.ToString()}");
                return null;
            }
        }
        /// <summary>
        /// StartOnceTask() запускает таймер один раз отдельной задачей и возвращает случайный ID
        /// </summary>
        /// <param name="interval">Интервал срабатывания действия</param>
        /// <param name="action">Лямбда-выражение с действием</param>
        /// <returns>Уникальный ID таймера</returns>
        public static string StartOnceTask(int interval, Action action)
        {
            try
            {
                string id = Guid.NewGuid().ToString();
                nTimer newtimer = new nTimer(action, id, interval, true, true);
                lock (TimersData)
                {
                    TimersData.TryAdd(id, newtimer);
                }
                return id;
            }
            catch (Exception e)
            {
                Log.Write($"StartOnceTask Exception: {e.ToString()}");
                return null;
            }
        }
        /// <summary>
        /// StartOnceTask() запускает таймер один раз отдельной задачей и возвращает ID
        /// </summary>
        /// <exception>
        /// Exception возникает при передаче уже существующего ID или значения null
        /// </exception>
        /// <param name="id">Уникальный идентификатор таймера</param>
        /// <param name="interval">Интервал срабатывания действия</param>
        /// <param name="action">Лямбда-выражение с действием</param>
        /// <returns>Уникальный ID таймера</returns>
        public static string StartOnceTask(string id, int interval, Action action)
        {
            try
            {
                if (id is null) throw new Exception("Id cannot be null");
                if (TimersData.ContainsKey(id)) throw new Exception("This id is already in use!");

                nTimer newtimer = new nTimer(action, id, interval, true, true);
                lock (TimersData)
                {
                    TimersData.TryAdd(id, newtimer);
                }
                return id;
            }
            catch (Exception e)
            {
                Log.Write($"StartOnceTask({id}) Exception: {e.ToString()}");
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public static void Stop(string id)
        {
            try
            {
                if (id is null) throw new Exception("Trying to stop timer with NULL ID");
                if (TimersData.ContainsKey(id))
                {
                    TimersData[id].isFinished = true;
                    TimersData.TryRemove(id, out _);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Stop Exception: {e.ToString()}");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void Stats()
        {
            string timers_ = "";
            foreach (nTimer t in TimersData.Values)
            {
                string state = (t.isFinished) ? "stopped" : "active";
                timers_ += $"{t.ID}:{state} ";
            }

            Log.Write(
                $"\nThread State = {thread.ThreadState.ToString()}" +
                $"\nTimers Count = {TimersData.Count}" +
                $"\nTimers = {timers_}" +
                $"\n");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class nTimer
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; }
        /// <summary>
        /// 
        /// </summary>
        public int MS { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Next { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Action action { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool isOnce { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool isTask { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool isNapiTask { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool isFinished { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action_"></param>
        /// <param name="id_"></param>
        /// <param name="ms_"></param>
        /// <param name="isonce_"></param>
        /// <param name="istask_"></param>
        /// <param name="isnapitask_"></param>
        public nTimer(Action action_, string id_, int ms_, bool isonce_ = false, bool istask_ = false, bool isnapitask_ = false)
        {
            action = action_;

            ID = id_;
            MS = ms_;
            Next = DateTime.Now.AddMilliseconds(MS);

            isOnce = isonce_;
            isTask = istask_;
            isNapiTask = isnapitask_;
            isFinished = false;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Elapsed()
        {
            try
            {
                if (this == null || !Timers.TimersData.Values.Contains(this)) return;
                if (isFinished) return;
                if (Next <= DateTime.Now)
                {
                    if (isOnce) isFinished = true;
                    Next = DateTime.Now.AddMilliseconds(MS);
                    Timers.Log.Debug($"Timer.{ID}.Invoke");

                    if (isTask) 
                    {
                        Task.Factory.StartNew(() => 
                        {
                            try
                            {
                                action.Invoke();
                            }
                            catch (Exception e)
                            {
                                Timers.Log.Write($"Elapsed({ID},MS:{MS},ONCE:{isOnce},From:{action.Target?.ToString()}) Task #1 Exception: {e.ToString()}");
                            }
                        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                    }
                    else if (isNapiTask)
                    {
                        NAPI.Task.Run(() =>
                        {
                            try
                            {
                                action.Invoke();
                            }
                            catch (Exception e)
                            {
                                Timers.Log.Write($"Elapsed({ID},MS:{MS},ONCE:{isOnce},From:{action.Target?.ToString()}) Task #2 Exception: {e.ToString()}");
                            }
                        });
                    }
                    else action.Invoke();

                    Timers.Log.Debug($"Timer.{ID}.Completed", nLog.Type.Success);
                }
            }
            catch (Exception e)
            {
                Timers.Log.Write($"Elapsed({ID},MS:{MS},ONCE:{isOnce},From:{action.Target?.ToString()}) Exception: {e.ToString()}");
            }
        }
    }
}
