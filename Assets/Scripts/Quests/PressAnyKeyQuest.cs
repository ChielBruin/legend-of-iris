﻿using System;
using UnityEngine;

public class PressAnyKeyQuest : Quest<PressAnyKeyQuest, PressAnyKeyQuestDefinition> {

    private float lastConversationEnd = 0;
    private ConversationPlayer player = null;

    public PressAnyKeyQuest(PressAnyKeyQuestDefinition definition) : base(definition) { }

    protected override void _Start() {
        base._Start();
        StartConversation();
    }

    public override void Update() {
#if UNITY_ANDROID
		if(Input.touchCount == 1) {
			Complete();
			return;
		}
#else
        if (Input.anyKeyDown) {
            Complete();
            return;
        }
#endif
        if (Time.time > lastConversationEnd + definition.repeatDelay)
            StartConversation();
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
    }

}
