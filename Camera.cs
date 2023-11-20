using Godot;
using Godot.Collections;
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
    float _vel_multiplier = 4.0f;

    // Keyboard state
    bool _w = false;
    bool _s = false;
    bool _a = false;
    bool _d = false;
    bool _q = false;
    bool _e = false;
    bool _shift = false;
    bool _alt = false;

    public override void _Process(double delta)
    {
        base._Process(delta);
        _UpdateMouselook();
        _UpdateMovement((float)delta);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion eventMotion)
        {
            _mouse_position = eventMotion.Relative;
        }

        // Receives mouse button input
        if (@event is InputEventMouseButton eventButton)
        {

            if (eventButton.ButtonIndex == MouseButton.Right)
            {
                Input.MouseMode = eventButton.Pressed ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
            }
            else if (eventButton.ButtonIndex == MouseButton.WheelUp)
            {
                _vel_multiplier = Mathf.Clamp(_vel_multiplier * 1.1f, 0.2f, 20f);
            }
            else if (eventButton.ButtonIndex == MouseButton.WheelDown)
            {
                _vel_multiplier = Mathf.Clamp(_vel_multiplier / 1.1f, 0.2f, 20f);
            }
        }

        if (@event is InputEventKey eventKey)
        {
            switch(eventKey.Keycode)
            {
                case Key.W:
                    _w = eventKey.Pressed;
                    break;
                case Key.S:
                    _s = eventKey.Pressed;
                    break;
                case Key.D:
                    _d = eventKey.Pressed;
                    break;
                case Key.A:
                    _a = eventKey.Pressed;
                    break;
                case Key.Q:
                    _q = eventKey.Pressed;
                    break;
                case Key.E:
                    _e = eventKey.Pressed;
                    break;
                case Key.Shift:
                    _shift = eventKey.Pressed;
                    break;
                case Key.Alt:
                    _alt = eventKey.Pressed;
                    break;
            }
        }

    }

    private void _UpdateMovement(float delta)
    {
        _direction = new Vector3(
            Convert.ToSingle(_d) - System.Convert.ToSingle(_a),
            System.Convert.ToSingle(_e) - System.Convert.ToSingle(_q),
            System.Convert.ToSingle(_s) - System.Convert.ToSingle(_w)
        );

        // Computes the change in velocity due to desired direction and "drag"
        // The "drag" is a constant acceleration on the camera to bring it's velocity to 0
        var offset = _direction.Normalized() * _acceleration * _vel_multiplier * delta + _velocity.Normalized() * _deceleration * _vel_multiplier * delta;
        float speed_multi = 1f;

        speed_multi = _shift ? speed_multi * SHIFT_MULTIPLIER : speed_multi;
        speed_multi = _alt ? speed_multi * ALT_MULTIPLIER : speed_multi;

        // Checks if we should bother translating the camera
        if (_direction == Vector3.Zero && offset.LengthSquared() > _velocity.LengthSquared()) {
            // Sets the velocity to 0 to prevent jittering due to imperfect deceleration
            _velocity = Vector3.Zero;

        } else {
		    // Clamps speed to stay within maximum value (_vel_multiplier)
		    _velocity.X = Mathf.Clamp(_velocity.X + offset.X, -_vel_multiplier, _vel_multiplier);
            _velocity.Y = Mathf.Clamp(_velocity.Y + offset.Y, -_vel_multiplier, _vel_multiplier);
            _velocity.Z = Mathf.Clamp(_velocity.Z + offset.Z, -_vel_multiplier, _vel_multiplier);
            Translate(_velocity * delta * speed_multi);
        }
    }

    // Updates mouse look 
    private void _UpdateMouselook()
    {

	    // Only rotates mouse if the mouse is captured
	    if (Input.MouseMode== Input.MouseModeEnum.Captured) {
            _mouse_position *= sensitivity;
            var yaw = _mouse_position.X;
            var pitch = _mouse_position.Y;
            _mouse_position = new(0, 0);

            // Prevents looking up/down too far
            pitch = Mathf.Clamp(pitch, -90 - _total_pitch, 90 - _total_pitch);

            _total_pitch += pitch;

            RotateY(Mathf.DegToRad(-yaw));
            RotateObjectLocal(new Vector3(1, 0, 0), Mathf.DegToRad(-pitch));
        }
    }
}


