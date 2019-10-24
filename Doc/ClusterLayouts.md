# ClusterLayouts 
簇群布局
## 简介 
默认实现布局器。如果要扩展可以参考LayoutSquare
!(img/ClusterLayouts/1.jpg)
## LayoutSquare 方阵布局 
按照固定列数，动态行数按照对象中心点排列成方正。如图
!(img/ClusterLayouts/2.jpg)
### 随机 
可选随机分布。打开Random。使RandomRange生效。
随机数据，来自Template模板中所有参数会一RandomRange/100取随机值。
RandomSeed 使用固定随机种子。

## LayoutAudience 观众布局 
手动输入每一行数量，实际的实例数量=所有行相加。
可以摆出梯形，菱形等正多边形。提供了偏移参数，可以让布局分行交错。
!(img/ClusterLayouts/3.jpg)

## <color="yellow">LayoutGroup 布局组</color> 
组合布局，没有实际的布局逻辑。负责将多个布局合并为组，交给同一个渲染器渲染。
因此可以通过多个布局对象摆出复杂的布局，同时保持Drawcall最优化。