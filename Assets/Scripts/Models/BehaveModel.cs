using System.Collections.Generic;
using UnityEngine;

public enum BehaveType
{
    Jump,
    Bark,
    Clench,
    HandShake,
    TailShake,
}

public sealed class BehaveModel
{
    private static readonly BehaveModel _instance = new BehaveModel();

    public static BehaveModel Instance
    {
        get { return _instance; }
    }

    private static Dictionary<BehaveType, bool> behaviors;
    private static readonly Dictionary<BehaveType, string> behaviour2TexPath = new Dictionary<BehaveType, string>
    {
        { BehaveType.Jump, "RuntimeTextures/ActionIcon/jump" },
        { BehaveType.Bark, "RuntimeTextures/ActionIcon/bark" },
        { BehaveType.Clench, "RuntimeTextures/ActionIcon/clench" },
        { BehaveType.HandShake, "RuntimeTextures/ActionIcon/handshake" },
        { BehaveType.TailShake, "RuntimeTextures/ActionIcon/tailshake" },
    };

    private BehaveModel()
    {
        var behaveTypes = System.Enum.GetValues(typeof(BehaveType));
        behaviors = new Dictionary<BehaveType, bool>();
        foreach (BehaveType behaveType in behaveTypes)
        {
            behaviors.Add(behaveType, true);
        }
    }

    public static List<BehaveType> GetAllUnlockedBehaveTypes()
    {
        var result = new List<BehaveType>();
        foreach (KeyValuePair<BehaveType, bool> pair in behaviors)
        {
            if (pair.Value)
            {
                result.Add(pair.Key);
            }
        }
        return result;
    }

    public static string GetBehaviourIconPath(BehaveType behaveType)
    {
        string iconPath;
        if (behaviour2TexPath.TryGetValue(behaveType, out iconPath))
        {
            return iconPath;
        }
        return null;
    }

    public static Texture2D GetBehaviourIconTexture(BehaveType behaveType)
    {
        var iconPath = GetBehaviourIconPath(behaveType);
        if (string.IsNullOrEmpty(iconPath))
        {
            return null;
        }
        return Resources.Load<Texture2D>(iconPath);
    }

    public static void test()
    {
        Debug.Log("BehaveModel.test called.");
    }
}

public static class Models
{
    public static BehaveAccessor behave { get; } = new BehaveAccessor();

    public sealed class BehaveAccessor
    {
        internal BehaveAccessor()
        {
        }

        public BehaveModel Instance
        {
            get { return BehaveModel.Instance; }
        }

        public void test()
        {
            BehaveModel.test();
        }

        public List<BehaveType> GetAllUnlockedBehaveTypes()
        {
            return BehaveModel.GetAllUnlockedBehaveTypes();
        }

        public string GetBehaviourIconPath(BehaveType behaveType)
        {
            return BehaveModel.GetBehaviourIconPath(behaveType);
        }

        public Texture2D GetBehaviourIconTexture(BehaveType behaveType)
        {
            return BehaveModel.GetBehaviourIconTexture(behaveType);
        }
    }
}
