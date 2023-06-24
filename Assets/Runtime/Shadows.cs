using UnityEngine;
using UnityEngine.Rendering;
[System.Serializable] public class ShadowSettings {
    public enum TextureSize {_512 = 512, _1024 = 1024, _2048 = 2048, _4096 = 4096 }
    [System.Serializable] public struct Directional { public TextureSize atlasSize; }
    [Min(0f)] public float maxDistance = 100f;
    public Directional directional = new Directional {atlasSize = TextureSize._1024};
}
public class Shadows {
    static int dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas"),
        dirShadowMatricesId = Shader.PropertyToID("_DirectionalShadowMatrices");
		
    static Matrix4x4[]
        dirShadowMatrices = new Matrix4x4[maxShadowedDirectionalLightCount];
    
    const string bufferName = "Shadows";
    const int maxShadowedDirectionalLightCount = 4;
    CommandBuffer buffer = new CommandBuffer {
        name = bufferName
    };
    
    struct ShadowedDirectionalLight {
        public int visibleLightIndex;
    }

    ShadowedDirectionalLight[] ShadowedDirectionalLights =
        new ShadowedDirectionalLight[maxShadowedDirectionalLightCount];

    ScriptableRenderContext context;

    CullingResults cullingResults;

    ShadowSettings settings;

    int ShadowedDirectionalLightCount;
    public void Setup (
        ScriptableRenderContext context, CullingResults cullingResults,
        ShadowSettings settings
    )
    {
        ShadowedDirectionalLightCount = 0;
        this.context = context;
        this.cullingResults = cullingResults;
        this.settings = settings;
    }

    public Vector2 ReserveDirectionalShadows(Light light, int visibleLightIndex)
    {
        if (ShadowedDirectionalLightCount < maxShadowedDirectionalLightCount  &&
            light.shadows != LightShadows.None && light.shadowStrength > 0f &&
            cullingResults.GetShadowCasterBounds(visibleLightIndex, out Bounds b) )
        {
            // ShadowedDirectionalLights[ShadowedDirectionalLightCount++] =
            //     new ShadowedDirectionalLight {
            //         visibleLightIndex = visibleLightIndex
            //     };
            ShadowedDirectionalLights[ShadowedDirectionalLightCount] =
                new ShadowedDirectionalLight {
                    visibleLightIndex = visibleLightIndex
                };
            return new Vector2(
                //shadowStrength越小，影子越淡
                light.shadowStrength, ShadowedDirectionalLightCount++
            );
        }

        return Vector2.zero;
    }
    
    public void Render () {
        if (ShadowedDirectionalLightCount > 0) {
            RenderDirectionalShadows();
        }
    }

    void RenderDirectionalShadows()
    {
        int atlasSize = (int)settings.directional.atlasSize;
        buffer.GetTemporaryRT(
            dirShadowAtlasId, atlasSize, atlasSize,
            32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap
        );    
        buffer.SetRenderTarget(
            dirShadowAtlasId,
            RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
        );
        buffer.ClearRenderTarget(true, false, Color.clear);
        buffer.BeginSample(bufferName);
        ExecuteBuffer();
        
        int split = ShadowedDirectionalLightCount <= 1 ? 1 : 2;
        int tileSize = atlasSize / split;

        for (int i = 0; i < ShadowedDirectionalLightCount; i++) {
            RenderDirectionalShadows(i, split,tileSize);
        }
        buffer.SetGlobalMatrixArray(dirShadowMatricesId, dirShadowMatrices);
        buffer.EndSample(bufferName);
        ExecuteBuffer();
    }

    void RenderDirectionalShadows (int index, int split, int tileSize)
    {
        ShadowedDirectionalLight light = ShadowedDirectionalLights[index];
        var shadowSettings =
            new ShadowDrawingSettings(cullingResults, light.visibleLightIndex);
        
        //1. int activeLightIndex:当前要计算的光源索引。
        //2. int splitIndex:级联索引，阴影级联相关，暂时不深入。
        //3. int splitCount:级联的数量，阴影级联相关，暂时不深入。
        //4. Vector3 splitRatio:级联比率，阴影级联相关，暂时不深入。
        //5. int shadowResolution:阴影贴图（tile）的分辨率。
        //6. float shadowNearPlaneOffset:光源的近平面偏移。
        //7. out Matrix4x4 viewMatrix:计算出的视图矩阵。
        //8. out Matrix4x4 projMatrix:计算出的投影矩阵。
        //9. out Rendering.ShadowSplitData:计算的级联数据，阴影级联相关，暂时不深入。
        cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(
            light.visibleLightIndex, 0, 1, Vector3.zero, 
            tileSize, 0f,
            out Matrix4x4 viewMatrix, out Matrix4x4 projectionMatrix,
            out ShadowSplitData splitData
        );
        
        shadowSettings.splitData = splitData;
        // SetTileViewport(index, split, tileSize);
        dirShadowMatrices[index] = ConvertToAtlasMatrix(
            projectionMatrix * viewMatrix,
            SetTileViewport(index, split, tileSize), split
        );
        // dirShadowMatrices[index] = projectionMatrix * viewMatrix;
        buffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
        
        ExecuteBuffer();
        context.DrawShadows(ref shadowSettings);
    }
    Vector2 SetTileViewport (int index, int split, float tileSize) {
        Vector2 offset = new Vector2(index % split, index / split);
        buffer.SetViewport(new Rect(
            offset.x * tileSize, offset.y * tileSize, tileSize, tileSize
        ));
        return offset;
    }
    
    Matrix4x4 ConvertToAtlasMatrix(Matrix4x4 m, Vector2 offset, int split)
    {
        //如果使用反向Z缓冲区，为Z取反
        if (SystemInfo.usesReversedZBuffer)
        {
            m.m20 = -m.m20;
            m.m21 = -m.m21;
            m.m22 = -m.m22;
            m.m23 = -m.m23;
        }
        //光源裁剪空间坐标范围为[-1,1]，而纹理坐标和深度都是[0,1]，因此，我们将裁剪空间坐标转化到[0,1]内
        //然后将[0,1]下的x,y偏移到光源对应的Tile上
        float scale = 1f / split;
        m.m00 = (0.5f * (m.m00 + m.m30) + offset.x * m.m30) * scale;
        m.m01 = (0.5f * (m.m01 + m.m31) + offset.x * m.m31) * scale;
        m.m02 = (0.5f * (m.m02 + m.m32) + offset.x * m.m32) * scale;
        m.m03 = (0.5f * (m.m03 + m.m33) + offset.x * m.m33) * scale;
        m.m10 = (0.5f * (m.m10 + m.m30) + offset.y * m.m30) * scale;
        m.m11 = (0.5f * (m.m11 + m.m31) + offset.y * m.m31) * scale;
        m.m12 = (0.5f * (m.m12 + m.m32) + offset.y * m.m32) * scale;
        m.m13 = (0.5f * (m.m13 + m.m33) + offset.y * m.m33) * scale;
        m.m20 = 0.5f * (m.m20 + m.m30);
        m.m21 = 0.5f * (m.m21 + m.m31);
        m.m22 = 0.5f * (m.m22 + m.m32);
        m.m23 = 0.5f * (m.m23 + m.m33);
        return m;
    }
    void ExecuteBuffer () {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
    
    public void Cleanup () {
        buffer.ReleaseTemporaryRT(dirShadowAtlasId);
        ExecuteBuffer();
    }
}