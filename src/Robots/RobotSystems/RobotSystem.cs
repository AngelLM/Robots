using System.Globalization;
using Rhino.Geometry;
using static Robots.Util;

namespace Robots;

public enum Manufacturers { ABB, KUKA, UR, Staubli, FrankaEmika, Doosan, Other, All };

public class DefaultPose(List<List<Plane>> planes, List<List<Mesh>> meshes)
{
    public List<List<Plane>> Planes { get; } = planes;
    public List<List<Mesh>> Meshes { get; } = meshes;
}

public abstract class RobotSystem
{
    Plane _basePlane;
    public string Name { get; }
    public Manufacturers Manufacturer { get; }
    public IO IO { get; }
    public ref Plane BasePlane => ref _basePlane;
    public Mesh? Environment { get; }
    public Mesh DisplayMesh { get; } = new();
    public DefaultPose DefaultPose { get; }
    public IRemote? Remote { get; protected set; }
    public int RobotJointCount { get; protected set; } = 6;

    static RobotSystem()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    protected RobotSystem(string name, Manufacturers manufacturer, IO io, Plane basePlane, Mesh? environment, DefaultPose defaultPose)
    {
        Name = name;
        Manufacturer = manufacturer;
        IO = io;
        BasePlane = basePlane;
        Environment = environment;
        DefaultPose = defaultPose;
    }

    /// <summary>
    /// Quaternion interpolation based on: http://www.grasshopper3d.com/group/lobster/forum/topics/lobster-reloaded
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public virtual Plane CartesianLerp(Plane a, Plane b, double t, double min, double max)
    {
        t = (t - min) / (max - min);
        if (double.IsNaN(t)) t = 0;
        var newOrigin = a.Origin * (1 - t) + b.Origin * t;

        var qa = a.ToQuaternion();
        var qb = b.ToQuaternion();
        var q = Slerp(ref qa, ref qb, t);

        return q.ToPlane(newOrigin);
    }

    public abstract List<KinematicSolution> Kinematics(IEnumerable<Target> target, IEnumerable<double[]>? prevJoints = null);
    internal abstract void SaveCode(IProgram program, string folder);
    internal abstract List<List<List<string>>> Code(Program program);
    internal abstract double Payload(int group);
    internal abstract IList<Joint> GetJoints(int group);
    public abstract double DegreeToRadian(double degree, int i, int group = 0);
    public abstract double[] PlaneToNumbers(Plane plane);
    public abstract Plane NumbersToPlane(double[] numbers);

    public override string ToString() => $"{GetType().Name} ({Name})";
}
