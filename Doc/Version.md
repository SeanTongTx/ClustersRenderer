# ClusterLayouts/Version
版本记录

## Preview 

### Preview 090423
1·重构裁剪部分代码。缓存增加可视化编组，现在可视化排序为插入排序法

### Preview 090421
1·重构了整体结构。
现在按照mvc结构 构建Layout-cache-render模型。
所有的数据操作通过cache提交到render中。
2·重构了cache数据结构。
现在缓存更新模式变为排序，从而移除了更新缓存时的gc。