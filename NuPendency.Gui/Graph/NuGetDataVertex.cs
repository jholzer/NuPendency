using GraphX.PCL.Common.Models;

namespace NuPendency.Gui.Graph
{
    public class NuGetDataVertex : VertexBase
    {
        /// <summary>
        /// Default parameterless constructor for this class
        /// (required for YAXLib serialization)
        /// </summary>
        public NuGetDataVertex() : this(string.Empty)
        {
        }

        public NuGetDataVertex(string text = "")
        {
            Text = text;
        }

        /// <summary>
        /// Some string property for example purposes
        /// </summary>
        public string Text { get; set; }

        #region Calculated or static props

        public override string ToString()
        {
            return Text;
        }

        #endregion Calculated or static props
    }
}