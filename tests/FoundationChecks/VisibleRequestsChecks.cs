using DSPSmartQueue;

internal static class VisibleRequestsChecks
{
    private sealed class Request(int parent, int recipe, int count)
    {
        internal int Parent = parent;
        internal int Recipe = recipe;
        internal int Count = count;
    }

    internal static void Run()
    {
        var outputs = new Dictionary<int, int[]> { [1] = [2], [2] = [1, 3] };
        int lookups = 0;
        var map = new VisibleRequests<Request>(t => t.Parent, t => t.Recipe, t => t.Count,
            recipe => { lookups++; return outputs.GetValueOrDefault(recipe, Array.Empty<int>()); });

        void Require(bool condition, string label)
        {
            if (!condition) throw new Exception("Visible request check failed: " + label);
        }

        void EmptyFrom(int index)
        {
            for (int i = index; i < VisibleRequests<Request>.Capacity; i++)
            {
                var slot = map.GetSlot(i);
                Require(slot.Task == null && slot.RecipeId == 0 && slot.Quantity == 0, "cleared slot " + i);
            }
        }

        // Exercise the production mapper over tiny data-only reference objects, never game types.
        map.Rebuild(Array.Empty<Request>());
        Require(map.TotalCount == 0 && map.VisibleCount == 0, "empty queue");
        EmptyFrom(0);
        var component = new Request(-1, 1, 3);
        var duplicate = new Request(-1, 1, 5);
        var multi = new Request(-2, 2, 7); // Contract is < 0, not just == -1.
        var missingRecipe = new Request(-1, 99, 9);
        var child = new Request(1, 1, 40);
        var queue = new List<Request> { child, component, new(3, 2, 8), duplicate, multi, missingRecipe };
        var identities = queue.ToArray();
        var values = queue.Select(t => (t.Parent, t.Recipe, t.Count)).ToArray();
        map.Rebuild(queue.AsReadOnly());
        Require(map.TotalCount == 4 && map.VisibleCount == 4, "mixed roots and children");
        Require(ReferenceEquals(map.GetSlot(0).Task, component) && map.GetSlot(0).Quantity == 6, "explicit component/single output");
        Require(ReferenceEquals(map.GetSlot(1).Task, duplicate) && map.GetSlot(1).Quantity == 10, "duplicate identity");
        Require(ReferenceEquals(map.GetSlot(2).Task, multi) && map.GetSlot(2).Quantity == 7, "multiple outputs");
        Require(map.GetSlot(3).RecipeId == 99 && map.GetSlot(3).Quantity == 9, "missing recipe fallback");
        EmptyFrom(4);
        Require(queue.Count == identities.Length && queue.Select((t, i) => ReferenceEquals(t, identities[i])).All(x => x), "list unchanged");
        Require(queue.Select(t => (t.Parent, t.Recipe, t.Count)).SequenceEqual(values), "task fields unchanged");
        Require(outputs[1].SequenceEqual(new[] { 2 }) && outputs[2].SequenceEqual(new[] { 1, 3 }), "recipe data unchanged");

        // Observe a native-style merge/count change; the mapper does not merge or deduplicate.
        component.Count = 11;
        map.Rebuild(new[] { component });
        Require(map.TotalCount == 1 && map.VisibleCount == 1 &&
            ReferenceEquals(map.GetSlot(0).Task, component) && map.GetSlot(0).Quantity == 22, "merged native task");
        EmptyFrom(1);

        var roots = Enumerable.Range(0, 17).Select(i => new Request(-1, 1, i + 1)).ToArray();
        foreach (int rootCount in new[] { 13, 14, 15, 17 })
        {
            var mixed = roots.Take(rootCount).SelectMany(t => new[] { new Request(0, 2, 50), t }).ToArray();
            lookups = 0;
            map.Rebuild(mixed);
            int visible = Math.Min(rootCount, 14);
            Require(map.TotalCount == rootCount && map.VisibleCount == visible, "viewport boundary " + rootCount);
            Require(lookups == visible, "no recipe lookups for hidden/offscreen tasks");
            for (int i = 0; i < visible; i++)
                Require(ReferenceEquals(map.GetSlot(i).Task, roots[i]) && map.GetSlot(i).RecipeId == 1 &&
                    map.GetSlot(i).Quantity == (i + 1) * 2, "native order/identity/quantity " + i);
            EmptyFrom(visible);
        }
        map.Rebuild(new[] { child });
        Require(map.TotalCount == 0 && map.VisibleCount == 0, "children only after full mapping");
        EmptyFrom(0);
        map.Rebuild(new[] { duplicate });
        EmptyFrom(1);
        map.Rebuild(Array.Empty<Request>());
        EmptyFrom(0);

        // Failure must not expose a partly rebuilt map or references from its predecessor.
        map.Rebuild(new[] { component });
        try { map.Rebuild(new Request[] { duplicate, null! }); throw new Exception("Null task accepted"); }
        catch (InvalidOperationException) { }
        Require(map.TotalCount == 0 && map.VisibleCount == 0, "failure clears counts");
        EmptyFrom(0);
        map.Rebuild(new[] { component });
        map.Clear();
        map.Clear();
        Require(map.TotalCount == 0 && map.VisibleCount == 0, "explicit clear");
        EmptyFrom(0);
        Console.WriteLine("PASS: visible requests, viewport boundaries, identity, native quantities, read-only inputs, refresh/clear and failed rebuild.");
    }
}
