namespace imminent_doom.common;

using Godot;

public static class Juice
{
    static ulong _endMs;

    public static async void HitStop(SceneTree tree, float duration = 0.05f, float timeScale = 0.05f)
    {
        ulong end = Time.GetTicksMsec() + (ulong)(duration * 1000f);
        if (end <= _endMs) return;   // a longer stop is already running
        _endMs = end;

        Engine.TimeScale = timeScale;
        // ignoreTimeScale = true (last arg), otherwise the timer would freeze too
        await tree.ToSignal(tree.CreateTimer(duration, true, false, true), SceneTreeTimer.SignalName.Timeout);

        if (_endMs == end) Engine.TimeScale = 1f;   // only reset if no newer stop replaced this one
    }
}