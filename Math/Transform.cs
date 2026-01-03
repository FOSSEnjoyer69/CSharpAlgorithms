using System.Numerics;

namespace CSharpAlgorithms.Math;
public class Transform<TPosition, TRotation, TScale> where TPosition : struct, INumber<TPosition> 
                                                     where TRotation : struct, INumber<TRotation> 
                                                     where TScale : struct, INumber<TScale>
{
    public TPosition Position;
    public TRotation Rotation;
    public TScale Scale;
}