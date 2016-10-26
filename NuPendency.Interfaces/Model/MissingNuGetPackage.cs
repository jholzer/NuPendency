namespace NuPendency.Interfaces.Model
{
    public class MissingNuGetPackage : NuGetPackage
    {
        public MissingNuGetPackage(string packageId) : base(packageId, null)
        {
        }
    }
}