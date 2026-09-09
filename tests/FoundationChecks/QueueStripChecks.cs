using DSPSmartQueue;

internal static class QueueStripChecks
{
    private sealed record Task(int Parent, int Recipe, int Count);

    internal static void Run()
    {
        var child = new Task(1, 8, 3);
        var a = new Task(-1, 1, 5);
        var b = new Task(-1, 1, 5); // Equal data, deliberately distinct references.
        var c = new Task(-1, 2, 7);
        var tasks = new List<Task> { child, a, b, c };
        var view = new QueueStrip<Task>(new VisibleRequests<Task>(t => t.Parent, t => t.Recipe,
            t => t.Count, recipe => recipe == 1 ? [2] : [1, 3]));
        var icons = Enumerable.Repeat(99u, 120).ToArray();
        var hover = Enumerable.Repeat(1u, 120).ToArray();
        void Check(bool condition, string label)
        {
            if (!condition) throw new Exception("Queue strip: " + label);
        }
        void Present(int slot)
        {
            view.Prepare(tasks, icons, hover, slot, recipe => (uint)(recipe + 100));
            Check(!view.Ready, "input disabled until upload completes");
            view.Commit();
        }
        Present(2);
        Check(icons.Take(3).SequenceEqual(new uint[] { 101, 101, 102 }) && icons.Skip(3).All(x => x == 0), "mapped icons and cleared tail");
        Check(hover[2] == 1 && hover.Where((_, i) => i != 2).All(x => x == 0), "hover rebuilt after native ordering");
        Check(view.Requests.GetSlot(0).Quantity == 10 && view.Requests.GetSlot(2).Quantity == 7, "display quantities");

        int nativeCalls = 0;
        int mouse = 1;
        tasks.RemoveAt(0); // A completed child shifts both roots.
        Check(view.BeginClick(tasks, mouse, ref mouse, out var state), "valid after completion");
        Check(mouse == 1 && ReferenceEquals(tasks[mouse], b), "duplicate resolved by reference");
        Check(!view.BeginClick(tasks, mouse, ref mouse, out var nested) && !nested.Translated, "nested input blocked while index is translated");
        try { nativeCalls++; tasks.RemoveAt(mouse); } // Stand-in original cancellation, once.
        finally { view.EndClick(ref mouse, state); }
        Check(mouse == 1 && nativeCalls == 1 && !view.Ready, "normal restoration and refresh");
        Present(0);
        mouse = 1;
        tasks.RemoveAt(0); // A preceding root cancelled after rendering.
        Check(view.BeginClick(tasks, mouse, ref mouse, out state) && mouse == 0 && ReferenceEquals(tasks[mouse], c), "preceding cancellation");
        var nativeFailure = new InvalidOperationException("native failure");
        try
        {
            try { nativeCalls++; throw nativeFailure; }
            finally { view.EndClick(ref mouse, state); }
        }
        catch (InvalidOperationException error) { Check(ReferenceEquals(error, nativeFailure), "exception not replaced"); }
        Check(mouse == 1 && nativeCalls == 2 && !view.Ready, "exceptional restoration without retry");

        Present(0);
        tasks.Clear();
        mouse = 0;
        Check(!view.BeginClick(tasks, 0, ref mouse, out state) && !state.Translated && mouse == 0 && !view.Ready, "disappeared target consumed");
        view.EndClick(ref mouse, state);
        tasks.Add(a);
        tasks.AddRange(Enumerable.Range(0, 20).Select(_ => new Task(0, 8, 1)));
        Present(13);
        Check(hover.All(x => x == 0), "empty-slot hover cleared");
        mouse = 13;
        Check(tasks.Count > mouse && !view.BeginClick(tasks, mouse, ref mouse, out state) && mouse == 13,
            "empty filtered slot consumes even when its raw queue index exists");
        Present(-1);
        Check(hover.All(x => x == 0), "outside-strip hover");
        var replacementQueue = new List<Task>(tasks);
        mouse = 0;
        Check(!view.BeginClick(replacementQueue, mouse, ref mouse, out state), "changed queue instance rejected");

        // Upload failure: no Commit, so the previous or partially written view cannot accept input.
        view.Prepare(tasks, icons, hover, 0, recipe => (uint)recipe);
        Check(!view.BeginClick(tasks, 0, ref mouse, out state), "uncommitted presentation blocks input");
        Present(0);
        try
        {
            view.Prepare(tasks, icons, hover, 0, _ => throw new InvalidOperationException("icon failure"));
            throw new Exception("Expected icon failure");
        }
        catch (InvalidOperationException) { }
        Check(!view.Ready && !view.BeginClick(tasks, 0, ref mouse, out state), "failed projection blocks input");
        Present(0);
        Check(view.Ready, "successful refresh restores mapped input");
        Check(nativeCalls == 2, "invalid actions not dispatched");
        Console.WriteLine("PASS: strip buffers/hover, exact identity dispatch, stale/empty/uncommitted input, normal/exceptional index restoration, refresh.");
    }
}
