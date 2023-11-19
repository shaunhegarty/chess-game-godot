using Godot;
using System;

public partial class Camera : Camera3D
{

	const float SHIFT_MULTIPLIER = 2.5f;
	const float ALT_MULTIPLIER = 1.0f / SHIFT_MULTIPLIER;

	[Export(PropertyHint.Range, "0,1,0.1,")]
	float sensitivity = 0.25f;

	// Mouse state
	Vector2 _mouse_position = new(0.0f, 0.0f);
	float _total_pitch = 0.0f;

	// Movement state
	Vector3 _direction = new Vector3(0.0f, 0.0f, 0.0f);
	Vector3 _velocity = new Vector3(0.0f, 0.0f, 0.0f);
	int _acceleration = 30;
	int _deceleration = -10;
	int _vel_multiplier = 4;

	// Keyboard state
	bool _w = false;
	bool _s = false;
	bool _a = false;
	bool _d = false;
	bool _q = false;
	bool _e = false;
	bool _shift = false;
	bool _alt = false;

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion eventMotion) {
			_mouse_position = eventMotion.Relative;
		}

		// Receives mouse button input
        if (@event is InputEventMouseButton eventButton)
		{
			switch(eventButton.ButtonIndex):
				MOUSE_BUTTON_RIGHT: # Only allows rotation if right click down
					Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED if event.pressed else Input.MOUSE_MODE_VISIBLE)
				MOUSE_BUTTON_WHEEL_UP: # Increases max velocity
					_vel_multiplier = clamp(_vel_multiplier* 1.1, 0.2, 20)
				MOUSE_BUTTON_WHEEL_DOWN: # Decereases max velocity
					_vel_multiplier = clamp(_vel_multiplier / 1.1, 0.2, 20)
		}
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

