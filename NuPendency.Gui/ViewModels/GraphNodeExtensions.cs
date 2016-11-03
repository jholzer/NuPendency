namespace NuPendency.Gui.ViewModels
{
    public static class GraphNodeExtensions
    {
        public static void DeHighlight(this GraphNode node)
        {
            if (!node.IsHighlighted())
                return;
            node.Selected = SelectionMode.NotSelected;
        }

        public static void DeSelect(this GraphNode node)
        {
            if (!node.IsSelected())
                return;
            node.Selected = SelectionMode.NotSelected;
        }

        public static void Highlight(this GraphNode node)
        {
            node.Selected = SelectionMode.Highlighted;
        }

        public static bool IsHighlighted(this GraphNode node)
        {
            return node.Selected == SelectionMode.Highlighted;
        }

        public static bool IsSelected(this GraphNode node)
        {
            return node.Selected == SelectionMode.Selected;
        }

        public static void Select(this GraphNode node)
        {
            node.Selected = SelectionMode.Selected;
        }
    }
}