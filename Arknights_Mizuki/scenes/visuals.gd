extends AnimatedSprite2D

func _ready():
	# 连接动画完成信号
	animation_finished.connect(_on_animation_finished)
	# 确保播放待机动画
	play("idle_loop")

func _on_animation_finished():
	# 攻击动画播放完后切换回待机
	if animation != "idle_loop":
		play("idle_loop")
