﻿namespace Robots.Grasshopper;

public class TargetParameter : GH_PersistentParam<GH_Target>
{
    public TargetParameter() : base("Target parameter", "Target", "This is a robot target", "Robots", "Parameters") { }
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    protected override System.Drawing.Bitmap Icon => Util.GetIcon("iconTargetParam");
    public override Guid ComponentGuid => new("{BEB590A9-905E-42ED-AB08-3E999EA94553}");
    protected override GH_Target PreferredCast(object data) =>
        data is Target cast ? new GH_Target(cast) : null!;

    protected override GH_GetterResult Prompt_Singular(ref GH_Target value)
    {
        value = new GH_Target();
        return GH_GetterResult.success;
    }

    protected override GH_GetterResult Prompt_Plural(ref List<GH_Target> values)
    {
        values = [];
        return GH_GetterResult.success;
    }
}