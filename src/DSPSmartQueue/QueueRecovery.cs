using System;

namespace DSPSmartQueue
{
    // Queue-specific gate: raw input is safe only after native restoration or native closure.
    internal sealed class QueueRecovery
    {
        internal bool ProjectionEnabled { get; private set; }
        internal bool NativeInputAllowed { get; private set; } = true;

        internal void Activate()
        {
            NativeInputAllowed = false;
            ProjectionEnabled = true;
        }

        internal void Recover(Action restoreNative, Action closeNative)
        {
            ProjectionEnabled = false;
            if (NativeInputAllowed) return;
            try { restoreNative(); }
            catch (Exception restoreFailure)
            {
                try { closeNative(); }
                catch (Exception closeFailure)
                {
                    throw new AggregateException("Native queue could neither be restored nor closed; input remains blocked.",
                        restoreFailure, closeFailure);
                }
            }
            NativeInputAllowed = true;
        }

        internal void Stop(Action restoreNative, Action closeNative, Action removeOwnedHooks, Action clearMapping)
        {
            try
            {
                Recover(restoreNative, closeNative);
                removeOwnedHooks();
            }
            finally { clearMapping(); }
        }
    }
}
