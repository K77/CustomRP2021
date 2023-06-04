using UnityEngine;
using UnityEngine.Rendering;

// namespace CustomRP.Runtime
// {
    public class CameraRender
    {
        private ScriptableRenderContext _context;
        private Camera _camera;
        private const string _bufferName = "RenderGamera";
        private CommandBuffer _buffer = new CommandBuffer() { name = _bufferName };
        public void Render(ScriptableRenderContext context, Camera camera)
        {
            _context = context;
            _camera = camera;

            if (!Cull()) return;

            Setup();
            DrawVisibleGeometry();
            Submit();
        }
        
        private CullingResults _cullingResults;
        bool Cull()
        {
            ScriptableCullingParameters p;
            if (_camera.TryGetCullingParameters(out p))
            {
                _cullingResults = _context.Cull(ref p);
                return true;
            }
            return false;
        }
        
        void Setup()
        {
            _context.SetupCameraProperties(_camera);
            _buffer.ClearRenderTarget(true,true,Color.clear);
            _buffer.BeginSample(_bufferName);
            ExecuteBuffer();
        }

        private static ShaderTagId _unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
        void DrawVisibleGeometry()
        {
            SortingSettings sortingSettings = new SortingSettings(_camera)
            {
                criteria = SortingCriteria.CommonOpaque
            };
            DrawingSettings drawingSettings = new DrawingSettings(_unlitShaderTagId, sortingSettings);
            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);
            _context.DrawRenderers(_cullingResults,ref drawingSettings,ref filteringSettings);
            _context.DrawSkybox(_camera);

            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            _context.DrawRenderers(_cullingResults,ref drawingSettings,ref filteringSettings);
        }

        void Submit()
        {
            _buffer.EndSample(_bufferName);
            ExecuteBuffer();
            _context.Submit();//called once every frame
        }

        void ExecuteBuffer()
        {
            _context.ExecuteCommandBuffer(_buffer);
            _buffer.Clear();
        }
    }
// }