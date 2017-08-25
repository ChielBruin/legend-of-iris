using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;


/// <summary>
/// The manager of the questing system.
/// </summary>
public class QuestManager : MonoBehaviour {

	public List<GameObject> startingQuests = new List<GameObject>();
    public List<GameObject> cheatQuests = new List<GameObject>();

	public List<Quest> quests = new List<Quest>();

    private List<Quest> activeQuests = new List<Quest>();

	
	/// <summary>
	/// Initilize the quest manager
	/// Attaches handlers to all quests and strats the starting quests.
	/// </summary>
	void Start () {
		// Register handlers on all quests for logging purposes
		Quest.onAnyQuestStart += OnQuestEvent;
		Quest.onAnyQuestComplete += OnQuestEvent;

		// Start all starting quests
		foreach (GameObject o in startingQuests) {
			foreach (QuestDefinition d in o.GetComponents<QuestDefinition>()) {
				Quest q = d.Create();
				quests.Add(q);
				q.Start();
			}
		}
        Quest.onAnyQuestStart += s => activeQuests.Add(s);
        Quest.onAnyQuestComplete += s => activeQuests.Remove(s);
        Quest.onAnyQuestComplete += delegate(Quest q)
        {
            TimerManager.RegisterEvent("Quest started: " + q.definition.gameObject.name);
        };
	}


	/// <summary>
	/// Update all active quests and complete some when some cheaty combos are pressed.
	/// </summary>
    void Update()
    {
        // Send the quests update notifications, so they can keep internal timers etc.
		foreach (var quest in quests) {
            if (quest.state == Quest.State.STARTED) quest.Update();
        }

        var cheatPressed = GetCheatPressed();
        if (cheatPressed.HasValue)
        {
            //Debug.Log("Cheat, started quest " + cheatPressed.Value);
            if (cheatQuests.Count > cheatPressed.Value)
                StartQuest(cheatQuests[cheatPressed.Value]);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Return))
        {
            foreach (var q in activeQuests.ToArray())
            {
                if (!(q is SerialQuest) && !(q is ParallelQuest))
                {
                    q.Complete();
                }
            }
        }
    }
	
	
	/// <summary>
	/// Send the quests fixed update notifications, so they can keep internal timers etc.
	/// </summary>
    void FixedUpdate()
    {
        foreach (var quest in quests)
        {
            if (quest.state == Quest.State.STARTED) quest.FixedUpdate();
        }
    }

	/// <summary>
	/// Start the quests contained in the given GameOgbject
	/// </summary>
    public void StartQuest(GameObject questGO)
    {
        foreach(var quest in questGO.GetComponents<QuestDefinition>())
        {
            foreach (QuestDefinition d in quest.GetComponents<QuestDefinition>())
            {
                Quest q = d.Create();
                quests.Add(q);
                q.Start();
            }
        }
    }

	/// <summary>
	/// Check if an input is given that corresponds to activating cheatcodes
	/// </summary>
	/// <returns>The entered cheatcode, or null when none is entered.</returns>
    private int? GetCheatPressed() {
#if UNITY_ANDROID
		// TODO: a 4 or 5 finger tap is not very intuitive
		if (Input.touchCount > 3) {
			return Input.touchCount - 3;
		} else {
			return null;
		}
#else
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.F2))
                return 0;
            if (Input.GetKeyDown(KeyCode.F3))
                return 1;
        }
        return null;
#endif
    }

	/// <summary>
	/// Handler for quest state changes.
	/// </summary>
	private static void OnQuestEvent(Quest quest) {
		Debug.Log(quest.state + ": " + quest.definition.gameObject.name);
	}

}
