using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A quest in where the player needs to press keys in  agiven order.
/// </summary>
public class KeyOrderQuest : Quest<KeyOrderQuest, KeyOrderQuestDefinition> {

    private float lastConversationEnd = 0;
    private ConversationPlayer player = null;
    private Queue<Direction> keysToPress;
    private bool firstPlayed = false;

    public KeyOrderQuest(KeyOrderQuestDefinition definition) : base(definition) { }

    protected override void _Start() {
        base._Start();
        Reset();

        SetMovementLocked(true);
        lastConversationEnd = Time.time;
    }

    protected override void _Complete()
    {
        SetMovementLocked(false);
        base._Complete();
    }

    private void Reset()
    {
        keysToPress = new Queue<Direction>();
        definition.KeyOrder.ForEach(d => keysToPress.Enqueue(d));
    }

    public override void Update() {
        if (state != State.STARTED)
            return;
        var keyPress = GetKeyPressed();
        if (keyPress == keysToPress.Peek())
        {
            keysToPress.Dequeue();
            if (keysToPress.Count == 0)
                Complete();
        }
        
        if (!string.IsNullOrEmpty(definition.conversationId))
        {
            if (firstPlayed)
            {
                if (Time.time > lastConversationEnd + definition.repeatDelay)
                    StartConversation();
            } else
            {
                if (Time.time > lastConversationEnd + definition.StartConversationFirstDelay)
                    StartConversation();
            }
        }
    }

	/// <summary>
	/// Get the pressed keys and return the direction the represent.
	/// For mobile users these keys are a one-finger-swipe in a direction
	/// or a two finger 'scrolling' gesture
	/// </summary>
	/// <returns>
	/// The direction represented by the input, or null when no input given
	/// </returns>
    private Direction? GetKeyPressed() {
#if UNITY_ANDROID
		if (Input.touchCount == 1) {
			Vector2 d = Input.GetTouch(0).deltaPosition;
			if (d.y > 0) 
				return Direction.Forward;
			if (d.y < 0) 
				return Direction.Backward;
			if (d.x > 0) 
				return Direction.Right;
			if (d.x < 0) 
				return Direction.Left;
		} else {
			//TODO: Add horizontal scroll for use with external input device
			if (Input.GetAxisRaw("Vertical") > 0.9)
				return Direction.Forward;
			if (Input.GetAxisRaw("Vertical") < -0.9)
				return Direction.Backward;
		}
		return null;
#else
        if (Input.GetAxisRaw("Vertical") > 0.9)
            return Direction.Forward;
        if (Input.GetAxisRaw("Vertical") < -0.9)
            return Direction.Backward;
        if (Input.GetAxisRaw("Horizontal") > 0.9)
            return Direction.Right;
        if (Input.GetAxisRaw("Horizontal") < -0.9)
            return Direction.Left;
        return null;
#endif
    }

    private void StartConversation() {
        if (player != null) return;
        player = ConversationManager.GetConversationPlayer(definition.conversationId);
        player.ConversationEnd += OnConversationEnd;
        player.Start();
    }

    private void OnConversationEnd(ConversationPlayer _player) {
        _player.ConversationEnd -= OnConversationEnd;
        player = null;
        lastConversationEnd = Time.time;
        firstPlayed = true;
    }

    private void SetMovementLocked(bool locked)
    {
        var player = Characters.GetPlayerController();
        if (player != null)
        {
            player.LockMovement = player.LockRotation = locked;
        }
    }
}
