using Godot;

public class FlashOnHurt {
	Node2D Owner;
	ShaderMaterial _material;

	public FlashOnHurt(Node2D owner) {
		this.Owner = owner;

		if (Owner.Material != null)
		{
			_material = (ShaderMaterial)Owner.Material;
			_material.SetShaderParameter("active", false);
		}
	}

	public void SetFlash(bool active) {
		if (_material != null)
		{
			_material.SetShaderParameter("active", active);
		}
	}
}