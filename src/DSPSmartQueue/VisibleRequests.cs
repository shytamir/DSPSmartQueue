using System;
using System.Collections.Generic;

namespace DSPSmartQueue
{
    // T is the native task reference in production; selectors provide a small offline test seam.
    internal sealed class VisibleRequests<T> where T : class
    {
        internal const int Capacity = 14;

        internal struct Slot
        {
            internal T Task;
            internal int RecipeId;
            internal int Quantity;
        }

        private readonly Slot[] slots = new Slot[Capacity];
        private readonly Func<T, int> parentIndex;
        private readonly Func<T, int> recipeId;
        private readonly Func<T, int> remaining;
        private readonly Func<int, int[]> resultCounts;

        internal int VisibleCount { get; private set; }
        internal int TotalCount { get; private set; }

        internal VisibleRequests(Func<T, int> parentIndex, Func<T, int> recipeId,
            Func<T, int> remaining, Func<int, int[]> resultCounts)
        {
            this.parentIndex = parentIndex ?? throw new ArgumentNullException(nameof(parentIndex));
            this.recipeId = recipeId ?? throw new ArgumentNullException(nameof(recipeId));
            this.remaining = remaining ?? throw new ArgumentNullException(nameof(remaining));
            this.resultCounts = resultCounts ?? throw new ArgumentNullException(nameof(resultCounts));
        }

        internal Slot GetSlot(int index) { return slots[index]; }

        internal void Clear()
        {
            Array.Clear(slots, 0, slots.Length);
            VisibleCount = 0;
            TotalCount = 0;
        }

        internal void Rebuild(IList<T> tasks)
        {
            Clear();
            if (tasks == null) throw new ArgumentNullException(nameof(tasks));
            try
            {
                for (int i = 0; i < tasks.Count; i++)
                {
                    T task = tasks[i];
                    if (task == null) throw new InvalidOperationException("Native queue contains a null task.");
                    if (parentIndex(task) >= 0) continue;
                    TotalCount++;
                    if (VisibleCount == Capacity) continue;
                    int recipe = recipeId(task);
                    int quantity = remaining(task);
                    int[] outputs = resultCounts(recipe);
                    if (outputs.Length == 1) quantity = unchecked(quantity * outputs[0]);
                    slots[VisibleCount++] = new Slot { Task = task, RecipeId = recipe, Quantity = quantity };
                }
            }
            catch
            {
                Clear();
                throw;
            }
        }
    }
}
