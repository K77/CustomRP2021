using UnityEngine;
using UnityEngine.Rendering;

// namespace CustomRP.Runtime
// {
    public partial class CameraRenderer
    {
        private ScriptableRenderContext _context;
        private Camera _camera;
        private const string _bufferName = "RenderGamera";
        private CommandBuffer _buffer = new CommandBuffer() { name = _bufferName };
        Lighting lighting = new Lighting();
        private bool useDynamicBatching;
        private bool useGPUInstancing;
        public void Render(ScriptableRenderContext context, Camera camera,
            bool useDynamicBatching, bool useGPUInstancing,
            ShadowSettings shadowSettings)
        {
            _context = context;
            _camera = camera;
            PrepareForSceneWindow();
            if (!Cull(shadowSettings.maxDistance)) return;
            // _buffer.BeginSample(SampleName);
            // ExecuteBuffer();
            // lighting.Setup(context, _cullingResults, shadowSettings);
            // _buffer.EndSample(SampleName);
            Setup();
            DrawVisibleGeometry();
            DrawUnsupportedShaders();
            DrawGizmos();
            Submit();
        }
        
        private CullingResults _cullingResults;
        bool Cull (float maxShadowDistance) {
            if (_camera.TryGetCullingParameters(out ScriptableCullingParameters p)) {
                p.shadowDistance = Mathf.Min(maxShadowDistance,_camera.farClipPlane);
                _cullingResults = _context.Cull(ref p);
                return true;
            }
            return false;
        }
        
        void Setup()
        {
            //把当前摄像机的信息告诉上下文，这样shader中就可以获取到当前帧下摄像机的信息，比如  VP矩阵等
            //同时也会设置当前的Render Target，这样ClearRenderTarget可以直接清除Render  Target中的数据，而不是通过绘制一个全屏的quad来达到同样效果（比较费）
            _context.SetupCameraProperties(_camera);
            //清除当前摄像机Render Target中的内容,包括深度和颜色，ClearRenderTarget内部会   Begin/EndSample(buffer.name)
            _buffer.ClearRenderTarget(true,true,Color.clear);
            //在Profiler和Frame Debugger中开启对Command buffer的监测
            _buffer.BeginSample(_bufferName);
            // context.SetupCameraProperties(camera);
            //提交CommandBuffer并且清空它，在Setup中做这一步的作用应该是确保在后续给CommandBuffer添加指令之前，其内容是空的。
            ExecuteBuffer();
        }

        private static ShaderTagId _unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit"),
            litShaderTagId = new ShaderTagId("CustomLit");
        
        void DrawVisibleGeometry()
        {
            SortingSettings sortingSettings = new SortingSettings(_camera)
            {
                criteria = SortingCriteria.CommonOpaque
            };
            DrawingSettings drawingSettings = new DrawingSettings(_unlitShaderTagId, sortingSettings)
            {
                enableDynamicBatching = useDynamicBatching,
                enableInstancing = false
            };
            drawingSettings.SetShaderPassName(1,litShaderTagId);
            
            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            _context.DrawRenderers(_cullingResults,ref drawingSettings,ref filteringSettings);
            _context.DrawSkybox(_camera);

            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            _context.DrawRenderers(_cullingResults,ref drawingSettings,ref filteringSettings);
        }
        
        void ExecuteBuffer()
        {
            _context.ExecuteCommandBuffer(_buffer);
            _buffer.Clear();
        }
        
        void Submit()
        {
            _buffer.EndSample(_bufferName);
            // _buffer.EndSample("KKK");
            ExecuteBuffer();
            _context.Submit();//called once every frame
        }
    }
// }