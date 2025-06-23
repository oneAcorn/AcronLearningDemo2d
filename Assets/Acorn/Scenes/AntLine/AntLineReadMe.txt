AntLine
	一种指向性的虚线
Step1
	创建一个两边都是透明的图片AntLine.png
	TextureType:Default,勾选Alpha is TransParency
	WrapMode选择Repeat
	点击apply
Step2
	创建一个Material,AntLineMt
	shader:Unlit/Transparent
	选择AntLine.png
Step3
	在场景中创建一个LineRenderer(Effects->Line)
	Material选择刚刚创建的AntLineMt
	Position中将index为1的x改为10(用于之后修改tiling,便于多个repeat),其他为0(适配2d)
	在Material组件中修改tiling,根据需要多少组线段自行决定
	在Material组件中修改offset,查看动态效果,之后会在代码中修改,用于实现动画
