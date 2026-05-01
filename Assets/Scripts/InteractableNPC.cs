using UnityEngine;

namespace CS4700
{
    public class InteractableNPC : MonoBehaviour
    {
        private const float InteractionRadius = 3f;
        private const string IsTalkingParam = "IsTalking";

        private Transform _player;
        private bool _inRange;

        private Animator _animator;
        private NPCWander _wander;
        private NPCWaypointPatrol _patrol;

        private string NPCName => gameObject.name;

        private void Start()
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) _player = p.transform;

            _animator = GetComponent<Animator>();
            _wander = GetComponent<NPCWander>();
            _patrol = GetComponent<NPCWaypointPatrol>();

            if (DialogueManager.Instance != null)
                DialogueManager.Instance.OnDialogueClosed += OnDialogueClosed;
        }

        private void OnDestroy()
        {
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.OnDialogueClosed -= OnDialogueClosed;
        }

        private void Update()
        {
            if (_player == null) return;

            float dist = Vector3.Distance(transform.position, _player.position);
            _inRange = dist <= InteractionRadius;

            if (_inRange && Input.GetKeyDown(KeyCode.E))
            {
                Talk();
            }
        }

        private void Talk()
        {
            StopMovement();

            var c = ChronicleOfEchoes.Instance;

            // CALEB → MAIN SCENE
            if (NPCName == "Caleb")
            {
                NarrativeController.Instance.StartOverlookScene();
                return;
            }

            // ABIGAIL
            if (NPCName == "Abigail")
            {
                if (!c.KnowsAbigailBlackmail)
                {
                    DialogueManager.Instance.OpenDialogue(
                        "Please... don’t talk to me. They’re watching.",
                        "Abigail"
                    );
                    c.KnowsAbigailBlackmail = true;
                }
                else if (!c.HasLocket)
                {
                    DialogueManager.Instance.OpenDialogue(
                        "He said he’d take everything... I had no choice...",
                        "Abigail"
                    );
                }
                else
                {
                    DialogueManager.Instance.OpenDialogue(
                        "You found it... maybe it’s finally over...",
                        "Abigail"
                    );
                }
                return;
            }

            // WEAVER (OldManNarrator)
            if (NPCName == "OldManNarrator")
            {
                DialogueManager.Instance.OpenDialogue(GetWeaverHint(), "Weaver");
                return;
            }

            // FEMALE VILLAGER
            if (NPCName == "FemaleVillager")
            {
                if (!c.HasLedger)
                {
                    DialogueManager.Instance.OpenDialogue(
                        "The Clerk keeps records... hidden away.",
                        "Villager"
                    );
                    c.KnowsCalebTargeted = true;
                }
                else
                {
                    DialogueManager.Instance.OpenDialogue(
                        "If those records are real... we were deceived.",
                        "Villager"
                    );
                }
                return;
            }

            // MALE VILLAGER
            if (NPCName == "MaleVillager")
            {
                if (!c.KnowsAbigailBlackmail)
                {
                    DialogueManager.Instance.OpenDialogue(
                        "That girl... something isn’t right.",
                        "Villager"
                    );
                }
                else
                {
                    DialogueManager.Instance.OpenDialogue(
                        "Someone is controlling all this...",
                        "Villager"
                    );
                }
                return;
            }

            // GENERIC NPCS
            if (NPCName == "MaleMedievalNPC" || NPCName == "FemaleMedievalNPC")
            {
                DialogueManager.Instance.OpenDialogue(
                    "The town is cursed... I can feel it.",
                    "Villager"
                );
                return;
            }

            DialogueManager.Instance.OpenDialogue("...", NPCName);
        }

        private void StopMovement()
        {
            if (_wander != null) _wander.enabled = false;
            if (_patrol != null) _patrol.enabled = false;

            _animator?.SetBool(IsTalkingParam, true);
        }

        private void ResumeMovement()
        {
            if (_wander != null) _wander.enabled = true;
            if (_patrol != null) _patrol.enabled = true;

            _animator?.SetBool(IsTalkingParam, false);
        }

        private void OnDialogueClosed()
        {
            ResumeMovement();
        }

        private string GetWeaverHint()
        {
            var c = ChronicleOfEchoes.Instance;

            if (!c.KnowsAbigailBlackmail)
                return "The child bends in the wind.";

            if (!c.HasLedger)
                return "Truth is written, not spoken.";

            if (!c.HasLocket)
                return "Fear has a shape.";

            return "The threads align.";
        }

        private void OnGUI()
        {
            if (!_inRange) return;
            if (DialogueManager.Instance.IsDialogueOpen) return;

            Vector3 screen = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
            if (screen.z < 0) return;

            float x = screen.x - 80;
            float y = Screen.height - screen.y - 15;

            GUI.Box(new Rect(x, y, 160, 30), $"[E] Talk: {NPCName}");
        }
    }
}