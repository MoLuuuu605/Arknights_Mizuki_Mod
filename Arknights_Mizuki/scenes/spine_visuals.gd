extends SpineSprite

@export var idle_animation := "Idle"
@export var attack_animation := "Attack"
@export var cast_animation := "Skill_1"
@export var buff_animation := "Skill_2"
@export var summon_animation := "Skill_1"
@export var dead_animation := "Die"
@export var hit_animation := ""

func _ready():
	call_deferred("_play_idle")

func play_trigger(trigger: String):
	var animation_name := _resolve_animation(trigger)
	if animation_name == "":
		return

	var state = get_animation_state()
	if state == null:
		return

	var loop := animation_name == idle_animation
	state.set_animation(animation_name, loop, 0)
	if not loop and idle_animation != "":
		state.add_animation(idle_animation, 0.0, true, 0)

func _play_idle():
	if idle_animation == "":
		return
	var state = get_animation_state()
	if state != null:
		state.set_animation(idle_animation, true, 0)

func _resolve_animation(trigger: String) -> String:
	match trigger:
		"Idle":
			return idle_animation
		"Attack":
			return attack_animation
		"Cast":
			return cast_animation
		"Buff":
			return buff_animation
		"Summon":
			return summon_animation
		"Dead", "DeadTrigger":
			return dead_animation
		"Hit":
			return hit_animation
		_:
			return trigger
