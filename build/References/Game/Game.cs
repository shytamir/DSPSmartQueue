using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Compile-time declarations only. This assembly is never shipped or executed.
public class ForgeTask
{
    public int parentTaskIndex;
    public int recipeId;
    public int count;
}
public class MechaForge { public List<ForgeTask> tasks; }
public abstract class ManualBehaviour : MonoBehaviour
{
    public bool active { get => throw new NotSupportedException(); }
    public void _Close() => throw new NotSupportedException();
}
public class UIReplicatorWindow : ManualBehaviour
{
    private List<ForgeTask> taskQueue;
    private MechaForge mechaForge;
    private int mouseQueueIndex;
    private uint[] queueIndexArray;
    private uint[] queueStateArray;
    private Text[] queueNumTexts;
    public Text queueCountText;
    public Color mainTaskTextColor;
    private void SetBufferData() => throw new NotSupportedException();
    private void OnQueueMouseDown(BaseEventData evtData) => throw new NotSupportedException();
    private void RefreshQueueIcons() => throw new NotSupportedException();
    private void TestMouseQueueIndex() => throw new NotSupportedException();
    private void ActiveQueueText(int index) => throw new NotSupportedException();
    private void DeactiveQueueText(int index) => throw new NotSupportedException();
    protected void _OnOpen() => throw new NotSupportedException();
    protected void _OnClose() => throw new NotSupportedException();
    protected void _OnFree() => throw new NotSupportedException();
    protected void _OnDestroy() => throw new NotSupportedException();
}
public class GameMain : MonoBehaviour
{
    public static IconSet iconSet { get => throw new NotSupportedException(); }
}
public class IconSet { public uint[] recipeIconIndex; }
public class Proto { }
public class ProtoTable { }
public class ProtoSet<T> : ProtoTable where T : Proto
{
    public T Select(int id) => throw new NotSupportedException();
}
public class RecipeProto : Proto { public int[] ResultCounts; }
public class RecipeProtoSet : ProtoSet<RecipeProto> { }
public static class LDB
{
    public static RecipeProtoSet recipes { get => throw new NotSupportedException(); }
}
