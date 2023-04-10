@tool
extends Polygon2D

var padding = 100

func _ready():
	var use_custom_rect = true;
	var rect = get_viewport_rect();
	rect.size *= DisplayServer.screen_get_scale();
	rect.size.x += padding * 2;
	rect.size.y += padding * 2;
	rect.position.x -= padding;
	rect.position.y -= padding;
	RenderingServer.canvas_item_set_custom_rect(get_canvas_item(), use_custom_rect, rect)
