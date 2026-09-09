using System;

namespace DSPSmartQueue
{
    internal static class ForgeRequests
    {
        internal static VisibleRequests<ForgeTask> Create()
        {
            return new VisibleRequests<ForgeTask>(task => task.parentTaskIndex,
                task => task.recipeId, task => task.count, GetResultCounts);
        }

        private static int[] GetResultCounts(int recipeId)
        {
            var recipe = LDB.recipes.Select(recipeId);
            // Native ActiveQueueText leaves the operation count unchanged for an absent recipe.
            return recipe == null ? Array.Empty<int>() : recipe.ResultCounts;
        }
    }
}
