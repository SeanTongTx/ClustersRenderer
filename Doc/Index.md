# ClustersRender 
簇群渲染器
## 简介 
使用GPUInstance渲染大量同模型对象,减少grawcall 。相比粒子系统clusterrender可以精准控制每一个对象。
同时大量渲染对象时不需要创建大量gameobject 减轻场景cpu压力。
!(img/1.jpg)
>Version
## 示例和说明 
!(img/2.jpg)
1.	GPUinstance 限制 必须使用同样的模型网格。
2.	多材质模型通过Materails 添加对应的材质。所有材质必须要符合GPUInstance编译
3.	InstanceCout（1~1022）因为unityGPUInstanceApi在不同平台的特性 1~511个对象占用1个pass。512~1022 占用2个pass。1023对象占用3个pass GPUInstance 限制1个批次最多1023个对象。所以限制在1022个对象。 
4.	ObjectLayout 额外的组件用来控制整个簇群结构属性（参考布局细节）。
5.	Bound ObjectLayout实现了CameraCullingGroup 剔除掉摄影机范围以外的对象。减少GPU渲染。

## ClusterLayout 簇群布局 

!(img/3.jpg)

管理簇群渲染的对象排布。这是一个扩展接口。考虑的大量对象的细节控制，一般对应需求编写对用的Layout布局。例如Demo中  观众台布局。
1.	Templete 模板配置，所有对象默认使用的参数，位置，旋转，缩放自定义属性等。
2.	Overriders 重写配置 通过Index所有标记并重写单一对象的属性。
需要注意的是，修改模板配置需要重置所有对象参数。使用ResetObjectTransfrom editor中自动重置。Override 只修改标记对象属性每帧应用，相对来说更高效。
详细参考
>ClusterLayouts

## ClustersRender.RenderCache 渲染缓存

用以解耦Layout与Render。同时减少渲染数据的帧计算。
现在簇群布局现在依赖渲染缓存，再由缓存递交给渲染器。

## IClusterObject 簇群虚拟物体

用来存储每一个簇群渲染的物体数据。
可以自行扩展。

### Template 
每一个Layout布局都会用到模板数据(Group除外)。
按照模板的设置进行布局。多数情况下使用默认的ClusterObject。

## ShaderCode 

!(img/4.jpg)

ClusterRender必须使用GPUinstance 编译Shader。因此平台需最低支持Opengl es3.0详细查看Unity GPUInstance文档。
*注意材质需要开启Enable GPU Instancing*