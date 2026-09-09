using System;
using System.Collections.Generic;

namespace DSPSmartQueue
{
    // Shared offline-testable presentation/input boundary. No native actions are performed here.
    internal sealed class QueueStrip<T> where T : class
    {
        internal struct ClickState
        {
            internal bool Translated;
            internal int OriginalIndex;
        }

        internal readonly VisibleRequests<T> Requests;
        private IList<T> presentedQueue = Array.Empty<T>();
        internal bool Ready { get; private set; }
        internal bool ClickInProgress { get; private set; }
        internal bool RefreshRequested { get; private set; } = true;

        internal QueueStrip(VisibleRequests<T> requests) { Requests = requests; }

        internal void Prepare(IList<T> tasks, uint[] icons, uint[] hover, int mouseSlot, Func<int, uint> iconForRecipe)
        {
            Invalidate();
            if (icons.Length < VisibleRequests<T>.Capacity || hover.Length < VisibleRequests<T>.Capacity)
                throw new InvalidOperationException("Queue buffers are smaller than the native viewport.");
            Requests.Rebuild(tasks);
            presentedQueue = tasks;
            Array.Clear(icons, 0, icons.Length);
            Array.Clear(hover, 0, hover.Length);
            for (int i = 0; i < Requests.VisibleCount; i++)
                icons[i] = iconForRecipe(Requests.GetSlot(i).RecipeId);
            if (mouseSlot >= 0 && mouseSlot < Requests.VisibleCount && icons[mouseSlot] != 0)
                hover[mouseSlot] = 1;
        }

        // Commit only after the native buffer upload succeeds.
        internal void Commit() { Ready = true; RefreshRequested = false; }
        internal void Invalidate() { Ready = false; RefreshRequested = true; }

        internal void Clear()
        {
            Invalidate();
            ClickInProgress = false;
            presentedQueue = Array.Empty<T>();
            Requests.Clear();
        }

        internal bool BeginClick(IList<T> tasks, int slot, ref int nativeIndex, out ClickState state)
        {
            state = default(ClickState);
            if (!Ready || !ReferenceEquals(tasks, presentedQueue) || slot < 0 || slot >= Requests.VisibleCount)
            {
                Invalidate();
                return false;
            }
            T target = Requests.GetSlot(slot).Task;
            for (int i = 0; i < tasks.Count; i++)
            {
                if (!ReferenceEquals(tasks[i], target)) continue;
                state = new ClickState { Translated = true, OriginalIndex = nativeIndex };
                nativeIndex = i;
                ClickInProgress = true;
                Invalidate();
                return true;
            }
            Invalidate();
            return false;
        }

        internal void EndClick(ref int nativeIndex, ClickState state)
        {
            if (!state.Translated) return;
            nativeIndex = state.OriginalIndex;
            ClickInProgress = false;
            Invalidate(); // Native selection/cancellation may have changed the queue.
        }
    }
}
