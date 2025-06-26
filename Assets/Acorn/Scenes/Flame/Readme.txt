Step1 准备材质
	画几个火焰的图片(这里是4个火焰放在一起)
	拖到Unity,TextureType选择Sprite
	创建Meterial,shader选择Mobile->Particles->Additive.
	Scene中Create->Effects->Particle System.
	选择Particle的Renderer,Material选择刚刚创建的FlameMt
Step2 分割
	分割,因为4个火焰是画在一起的
	选择Particle的Texture Sheet Animation
	将Tiles改为x=2,y=2.意味着将图片水平,垂直各切一刀.
	Frame over Time的曲线代表随着时间推移(TimeMode为lifetime),粒子该采用哪张图片.
Step3 发射形状
	调整Particle的Shape,angle改为15,Radius改为0,使发射的角度收敛
Step4 存在时间
	Duration改为1
	勾选Prewarm,会让粒子系统在游戏运行时就满状态运行,而不是由少到多的发射
	设置Start Lifetime 和Start Speed 分别为1.这样发射范围和距离就被控制住了。
	设置Simulation Space 为 world。这样在火焰移动的时候，已经发射的火焰就会留在之前的位置。(这里看情况)
	设置Simulation Speed，将火焰燃烧的速度，设置为0.8
Step5 调整火焰的形状
	火焰都是从无到有，再由有变无，所以我们需要设置粒子元素刚发射出来是无限小，然后变大，在消亡的时候，再次变小直到消失。
	找到 Size over Life time栏，设置Size的曲线。
Step6 设置火焰颜色
	设置Color over lifetime
	注意该颜色条从左到右代表时间的起始和结束的颜色变化。
	色条的下面是设置颜色,上面是透明度






Mobile->Particles下的4个Shader区别
1. Additive（加法混合）
特点：
使用 加法混合（Additive Blending），公式为：
Final Color = Source Color + Destination Color
不考虑背景颜色的透明度，直接叠加光源和粒子颜色，适合明亮、发光的粒子效果。
优点：性能较高，适合火焰、光效等高亮度粒子。
缺点：粒子叠加后可能过亮，无法与背景透明度融合。
适用场景：
火焰、闪电、爆炸、光晕等需要“发光”效果的粒子。
不需要与场景颜色混合的纯亮色粒子。
2. Alpha Blended（阿尔法混合）
特点：
使用 标准阿尔法混合（Alpha Blending），公式为：
Final Color = Source Color * Source Alpha + Destination Color * (1 - Source Alpha)
支持半透明效果，粒子颜色会与背景颜色自然融合。
优点：视觉效果自然，适合烟雾、尘埃等半透明粒子。
缺点：渲染顺序敏感，需正确排序以避免 Z-Fighting。
适用场景：
烟雾、雾气、灰尘、花瓣等需要半透明效果的粒子。
需要与场景颜色混合的复杂粒子效果。
3. Multiply（颜色相乘）
特点：
使用 颜色相乘（Multiply Blending），公式为：
Final Color = Source Color * Destination Color
粒子颜色与背景颜色相乘，通常用于模拟阴影或叠加暗色效果。
优点：适合模拟暗色叠加（如阴影、污渍）。
缺点：不支持纯亮色，叠加后颜色会变暗。
适用场景：
阴影、暗色粒子（如烟尘、墨水）、贴图叠加效果。
需要与背景颜色相乘的特殊视觉效果。
4. VertexLit Blended（顶点光照混合）
特点：
顶点光照（Vertex Lit）：光照计算在顶点级别完成，性能较低但适合简单场景。
混合模式（Blended）：支持阿尔法混合，粒子颜色会与背景融合。
优点：支持多光源（顶点级别），适合需要基础光照的粒子。
缺点：光照效果较粗糙（像素级别更细腻）。
适用场景：
需要与场景光照互动的粒子（如动态粒子在复杂场景中的交互）。
对性能敏感但需要基础光照效果的粒子（如灰尘、雨滴）。