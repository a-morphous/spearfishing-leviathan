using Godot;
public class NodeUtils {
	public static T GetNearestAncestorNode<T>(Node self) where T : class
		{
			Node parent = self.GetParent();
			while (parent != null)
			{
				if (parent is T)
				{
					return parent as T;
				}
				parent = parent.GetParent();
			}

			return null;
		}
}