using DSPSmartQueue;

internal static class QueueRecoveryChecks
{
    internal static void Run()
    {
        void Check(bool condition, string label)
        {
            if (!condition) throw new Exception("Queue recovery: " + label);
        }
        var gate = new QueueRecovery();
        int unpatched = 0, cleared = 0, restores = 0, closes = 0;
        // Partial hook installation failed before activation: no native UI needs restoration.
        gate.Stop(() => throw new Exception("Must not restore untouched UI"), () => throw new Exception(),
            () => unpatched++, () => cleared++);
        Check(gate.NativeInputAllowed && !gate.ProjectionEnabled && unpatched == 1 && cleared == 1, "initialization failure cleanup");
        gate.Activate();
        var icons = new uint[] { 101, 102, 0 }; // Partially projected UI.
        gate.Recover(() =>
        {
            Check(!gate.NativeInputAllowed && !gate.ProjectionEnabled, "input held during native restore");
            icons[0] = 8; icons[1] = 9; icons[2] = 10;
            restores++;
        }, () => closes++);
        Check(gate.NativeInputAllowed && icons.SequenceEqual(new uint[] { 8, 9, 10 }) && closes == 0, "native view before raw input");
        gate.Recover(() => restores++, () => closes++);
        Check(restores == 1, "no recovery retry loop");

        gate.Activate();
        bool visible = true;
        gate.Recover(() => { icons[0] = 99; throw new InvalidOperationException("failed halfway"); }, () =>
        {
            Check(!gate.NativeInputAllowed, "input held until close completes");
            visible = false; closes++;
        });
        Check(gate.NativeInputAllowed && !visible, "close removes partial view before native input");

        gate.Activate();
        try
        {
            gate.Stop(() => throw new InvalidOperationException("restore"), () => throw new InvalidOperationException("close"),
                () => unpatched++, () => cleared++);
            throw new Exception("Expected blocked recovery");
        }
        catch (AggregateException) { }
        Check(!gate.NativeInputAllowed && !gate.ProjectionEnabled && unpatched == 1 && cleared == 2,
            "double failure retains guard but clears task references");
        gate.Stop(() => { }, () => throw new Exception(), () => unpatched++, () => cleared++);
        gate.Stop(() => throw new Exception(), () => throw new Exception(), () => unpatched++, () => cleared++);
        Check(gate.NativeInputAllowed && unpatched == 3 && cleared == 4, "explicit teardown and repeated cleanup safe");

        // Native task references cannot survive close/reopen, session replacement, or an in-flight close.
        var task = new object();
        var queue = new List<object> { task };
        var strip = new QueueStrip<object>(new VisibleRequests<object>(_ => -1, _ => 1, _ => 1, _ => [1]));
        void Present(IList<object> source)
        {
            strip.Prepare(source, new uint[14], new uint[14], 0, _ => 1);
            strip.Commit();
        }
        void IsCleared()
        {
            Check(!strip.Ready && !strip.ClickInProgress && strip.Requests.VisibleCount == 0 && strip.Requests.TotalCount == 0,
                "mapping and counters reset");
            for (int i = 0; i < 14; i++) Check(strip.Requests.GetSlot(i).Task == null, "no retained task reference");
        }
        Present(queue);
        strip.Clear(); IsCleared();
        int mouse = 0;
        Check(!strip.BeginClick(queue, 0, ref mouse, out _), "reopen cannot use old projection");
        var newQueue = new List<object> { new object() };
        Present(newQueue);
        Check(!strip.BeginClick(queue, 0, ref mouse, out _), "old session cannot target new mapping");
        Present(newQueue);
        Check(strip.BeginClick(newQueue, 0, ref mouse, out var click), "click begins");
        strip.Clear(); IsCleared(); // Native close while original handler is on stack.
        mouse = 9;
        strip.EndClick(ref mouse, click);
        Check(mouse == 0 && !strip.Ready, "originating click still restores after close");
        strip.Clear(); strip.Clear(); IsCleared();
        Console.WriteLine("PASS: initialization cleanup, native recovery ordering, partial failure/close fallback, blocked double failure, repeated shutdown and session mapping cleanup.");
    }
}
