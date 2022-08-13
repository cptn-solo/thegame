using Assets.Scripts.Services;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class CoroutineUtils
{
    public static IEnumerator DoWithUpdate(float duration, Action<float> action, float startTime = 0f)
    {
        var elapsedTime = startTime / duration;
        while (elapsedTime < duration)
        {
            action(elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        action(1f);
    }

    public static IEnumerator WaitForRealTime(float delay)
    {
        while (true)
        {
            float pauseEndTime = Time.realtimeSinceStartup + delay;
            while (Time.realtimeSinceStartup < pauseEndTime)
                yield return 0;

            break;
        }
    }

    public static IEnumerator Then(this IEnumerator enumerator, Action action)
    {
        yield return enumerator;

        action();
    }

    public static IEnumerator Then(this IEnumerator enumerator, IEnumerator enumerator2)
    {
        yield return enumerator;

        yield return enumerator2;
    }

    public static Coroutine Start(this IEnumerator enumerator, MonoBehaviour holder = null)
    {
        if (holder != null)
            return holder.StartCoroutine(enumerator);

        return AsyncCoroutineRunnerService.Instance.StartCoroutine(enumerator);
    }

    public static void Immediate(this IEnumerator enumerator)
    {
        while (enumerator.MoveNext()) ;
    }
}

public class DoInBackground : CustomYieldInstruction
{
    public override bool keepWaiting => !isFinished;

    private Thread thread;
    private Action task;

    private bool isFinished;

    public DoInBackground(Action task)
    {
        this.task = task;

        thread = new Thread(Work);
        thread.Name = "bgThread" + thread.ManagedThreadId;
        thread.IsBackground = true;
        thread.Start(null);
    }

    private void Work(object args)
    {
        task.Invoke();

        isFinished = true;
    }
}

public class TaskYieldInstruction : CustomYieldInstruction
{
    public Task Task { get; private set; }

    public override bool keepWaiting
    {
        get
        {
            if (Task.Exception != null)
                throw Task.Exception;

            return !Task.IsCompleted;
        }
    }

    public TaskYieldInstruction(Task task) =>
        Task = task ?? throw new ArgumentNullException("task");
}

public class TaskYieldInstruction<T> : TaskYieldInstruction
{
    public new Task<T> Task { get; private set; }

    public T Result => Task.Result;

    public TaskYieldInstruction(Task<T> task) : base(task)
    {
        Task = task;
    }
}

public static class TaskYieldInstructionExtension
{
    public static TaskYieldInstruction AsCoroutine(this Task task)
    {
        if (task == null)
            throw new NullReferenceException();

        return new TaskYieldInstruction(task);
    }

    public static TaskYieldInstruction<T> AsCoroutine<T>(this Task<T> task)
    {
        if (task == null)
            throw new NullReferenceException();

        return new TaskYieldInstruction<T>(task);
    }
}
