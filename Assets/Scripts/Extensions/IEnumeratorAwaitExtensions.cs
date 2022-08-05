using Assets.Scripts.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using static Assets.Scripts.Extensions.IEnumeratorAwaitExtensions;

namespace Assets.Scripts.Extensions
{
    public static class IEnumeratorAwaitExtensions
    {
        private static AsyncCoroutineRunnerService Runner =>
            AsyncCoroutineRunnerService.Instance;

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForSeconds instruction) =>
            GetAwaiterReturnVoid(instruction);

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForUpdate instruction) =>
            GetAwaiterReturnVoid(instruction);

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForEndOfFrame instruction) =>
            GetAwaiterReturnVoid(instruction);

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForFixedUpdate instruction) =>
            GetAwaiterReturnVoid(instruction);

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForSecondsRealtime instruction) =>
            GetAwaiterReturnVoid(instruction);

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitUntil instruction) =>
            GetAwaiterReturnVoid(instruction);

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitWhile instruction) =>
            GetAwaiterReturnVoid(instruction);

        public static SimpleCoroutineAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation instruction) =>
            GetAwaiterReturnSelf(instruction);

        public static SimpleCoroutineAwaiter<UnityEngine.Object> GetAwaiter(this ResourceRequest instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => Runner.StartCoroutine(InstructionWrappers.ResourceRequest(awaiter, instruction)));

            return awaiter;
        }

        public static SimpleCoroutineAwaiter<AssetBundle> GetAwaiter(this AssetBundleCreateRequest instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter<AssetBundle>();
            RunOnUnityScheduler(() => Runner.StartCoroutine(InstructionWrappers.AssetBundleCreateRequest(awaiter, instruction)));

            return awaiter;
        }

        public static SimpleCoroutineAwaiter<UnityEngine.Object> GetAwaiter(this AssetBundleRequest instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => Runner.StartCoroutine(InstructionWrappers.AssetBundleRequest(awaiter, instruction)));

            return awaiter;
        }

        public static SimpleCoroutineAwaiter<T> GetAwaiter<T>(this IEnumerator<T> coroutine)
        {
            var awaiter = new SimpleCoroutineAwaiter<T>();
            RunOnUnityScheduler(() => Runner.StartCoroutine(new CoroutineWrapper<T>(coroutine, awaiter).Run()));

            return awaiter;
        }

        public static SimpleCoroutineAwaiter<object> GetAwaiter(this IEnumerator coroutine)
        {
            var awaiter = new SimpleCoroutineAwaiter<object>();
            RunOnUnityScheduler(() => Runner.StartCoroutine(new CoroutineWrapper<object>(coroutine, awaiter).Run()));

            return awaiter;
        }

        private static SimpleCoroutineAwaiter GetAwaiterReturnVoid(object instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter();
            RunOnUnityScheduler(() => Runner.StartCoroutine(InstructionWrappers.ReturnVoid(awaiter, instruction)));

            return awaiter;
        }

        private static SimpleCoroutineAwaiter<T> GetAwaiterReturnSelf<T>(T instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter<T>();
            RunOnUnityScheduler(() => Runner.StartCoroutine(InstructionWrappers.ReturnSelf(awaiter, instruction)));

            return awaiter;
        }

        private static void RunOnUnityScheduler(Action action)
        {
            if (SynchronizationContext.Current == SyncContextUtil.UnitySynchronizationContext)
                action();
            else
                SyncContextUtil.UnitySynchronizationContext.Post(_ => action(), null);
        }

        private static void Assert(bool condition)
        {
            if (!condition)
                throw new Exception("Assert hit in UnityAsyncUtil package!");
        }

        public class SimpleCoroutineAwaiter<T> : INotifyCompletion
        {
            bool _isDone;
            Exception _exception;
            Action _continuation;
            T _result;

            public bool IsCompleted => _isDone;

            public T GetResult()
            {
                Assert(_isDone);

                if (_exception != null)
                    ExceptionDispatchInfo.Capture(_exception).Throw();

                return _result;
            }

            public void Complete(T result, Exception e)
            {
                Assert(!_isDone);

                _isDone = true;
                _exception = e;
                _result = result;

                // Always trigger the continuation on the unity thread when awaiting on unity yield
                // instructions
                if (_continuation != null)
                    RunOnUnityScheduler(_continuation);
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                Assert(_continuation == null);
                Assert(!_isDone);

                _continuation = continuation;
            }
        }

        public class SimpleCoroutineAwaiter : INotifyCompletion
        {
            private bool _isDone;
            private Exception _exception;
            private Action _continuation;

            public bool IsCompleted => _isDone;

            public void GetResult()
            {
                Assert(_isDone);

                if (_exception != null)
                    ExceptionDispatchInfo.Capture(_exception).Throw();
            }

            public void Complete(Exception e)
            {
                Assert(!_isDone);

                _isDone = true;
                _exception = e;

                if (_continuation != null)
                    RunOnUnityScheduler(_continuation);
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                Assert(_continuation == null);
                Assert(!_isDone);

                _continuation = continuation;
            }
        }

        private class CoroutineWrapper<T>
        {
            private readonly SimpleCoroutineAwaiter<T> _awaiter;
            private readonly Stack<IEnumerator> _processStack;

            public CoroutineWrapper(IEnumerator coroutine, SimpleCoroutineAwaiter<T> awaiter)
            {
                _processStack = new Stack<IEnumerator>();
                _processStack.Push(coroutine);
                _awaiter = awaiter;
            }

            public IEnumerator Run()
            {
                while (true)
                {
                    var topWorker = _processStack.Peek();

                    bool isDone;

                    try
                    {
                        isDone = !topWorker.MoveNext();
                    }
                    catch (Exception e)
                    {
                        var objectTrace = GenerateObjectTrace(_processStack);

                        if (objectTrace.Any())
                            _awaiter.Complete(default, new Exception(GenerateObjectTraceMessage(objectTrace), e));
                        else
                            _awaiter.Complete(default, e);

                        yield break;
                    }

                    if (isDone)
                    {
                        _processStack.Pop();

                        if (_processStack.Count == 0)
                        {
                            _awaiter.Complete((T)topWorker.Current, null);
                            yield break;
                        }
                    }

                    if (topWorker.Current is IEnumerator enumerator)
                        _processStack.Push(enumerator);
                    else
                        yield return topWorker.Current;
                }
            }

            private string GenerateObjectTraceMessage(List<Type> objTrace)
            {
                var result = new StringBuilder();

                foreach (var objType in objTrace)
                {
                    if (result.Length != 0)
                        result.Append(" -> ");

                    result.Append(objType.ToString());
                }

                result.AppendLine();

                return "Unity Coroutine Object Trace: " + result.ToString();
            }

            private static List<Type> GenerateObjectTrace(IEnumerable<IEnumerator> enumerators)
            {
                var objTrace = new List<Type>();

                foreach (var enumerator in enumerators)
                {
                    var field = enumerator.GetType().GetField("$this", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    if (field == null)
                        continue;

                    var obj = field.GetValue(enumerator);

                    if (obj == null)
                        continue;

                    var objType = obj.GetType();

                    if (!objTrace.Any() || objType != objTrace.Last())
                        objTrace.Add(objType);
                }

                objTrace.Reverse();

                return objTrace;
            }
        }

        private static class InstructionWrappers
        {
            public static IEnumerator ReturnVoid(SimpleCoroutineAwaiter awaiter, object instruction)
            {
                yield return instruction;
                awaiter.Complete(null);
            }

            public static IEnumerator AssetBundleCreateRequest(SimpleCoroutineAwaiter<AssetBundle> awaiter, AssetBundleCreateRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.assetBundle, null);
            }

            public static IEnumerator ReturnSelf<T>(SimpleCoroutineAwaiter<T> awaiter, T instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction, null);
            }

            public static IEnumerator AssetBundleRequest(SimpleCoroutineAwaiter<UnityEngine.Object> awaiter, AssetBundleRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.asset, null);
            }

            public static IEnumerator ResourceRequest(SimpleCoroutineAwaiter<UnityEngine.Object> awaiter, ResourceRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.asset, null);
            }
        }

        public class WaitForUpdate : CustomYieldInstruction
        {
            public override bool keepWaiting => false;
        }

        public class WaitForBackgroundThread
        {
            public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter() =>
                Task.Run(() => { }).ConfigureAwait(false).GetAwaiter();
        }
    }

    public static class SyncContextUtil
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            UnitySynchronizationContext = SynchronizationContext.Current;
            UnityThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static int UnityThreadId
        {
            get; private set;
        }

        public static SynchronizationContext UnitySynchronizationContext
        {
            get; private set;
        }
    }

    public static class Awaiters
    {
        private readonly static WaitForUpdate _waitForUpdate = new WaitForUpdate();
        private readonly static WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private readonly static WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

        public static WaitForUpdate NextFrame =>
            _waitForUpdate;

        public static WaitForFixedUpdate FixedUpdate =>
            _waitForFixedUpdate;

        public static WaitForEndOfFrame EndOfFrame =>
            _waitForEndOfFrame;

        public static WaitForSeconds Seconds(float seconds) =>
            new WaitForSeconds(seconds);

        public static WaitForSecondsRealtime SecondsRealtime(float seconds) =>
            new WaitForSecondsRealtime(seconds);

        public static WaitUntil Until(Func<bool> predicate) =>
            new WaitUntil(predicate);

        public static WaitWhile While(Func<bool> predicate) =>
            new WaitWhile(predicate);
    }
}
