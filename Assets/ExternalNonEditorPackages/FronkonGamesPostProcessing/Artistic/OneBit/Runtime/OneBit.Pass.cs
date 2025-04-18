////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Artistic.OneBit
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class OneBit
  {
    private sealed class RenderPass : ScriptableRenderPass
    {
      private readonly Settings settings;

      private RenderTargetIdentifier colorBuffer;
      private RenderTextureDescriptor renderTextureDescriptor;

#if UNITY_2022_1_OR_NEWER
      private RTHandle renderTextureHandle0;

      private readonly ProfilingSampler profilingSamples = new(Constants.Asset.AssemblyName);
      private ProfilingScope profilingScope;
#else
      private int renderTextureHandle0;
#endif
      private readonly Material material;

      private static readonly ProfilerMarker ProfilerMarker = new($"{Constants.Asset.AssemblyName}.Pass.Execute");

      private const string CommandBufferName = Constants.Asset.AssemblyName;

      private Gradient gradient = null;
      private Texture2D gradientTexture;

      private static class ShaderIDs
      {
        internal static readonly int Intensity = Shader.PropertyToID("_Intensity");
        internal static readonly int DeltaTime = Shader.PropertyToID("_DeltaTime");

        internal static readonly int Edges = Shader.PropertyToID("_Edges");
        internal static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");
        internal static readonly int NoiseSeed = Shader.PropertyToID("_NoiseSeed");
        internal static readonly int BlendMode = Shader.PropertyToID("_Blend");
        internal static readonly int Color = Shader.PropertyToID("_Color");
        internal static readonly int Color0 = Shader.PropertyToID("_Color0");
        internal static readonly int Color1 = Shader.PropertyToID("_Color1");
        internal static readonly int GradientTexture = Shader.PropertyToID("_GradientTex");
        internal static readonly int LuminanceMin = Shader.PropertyToID("_LumRangeMin");
        internal static readonly int LuminanceMax = Shader.PropertyToID("_LumRangeMax");
        internal static readonly int CircularRadius = Shader.PropertyToID("_GradientRadius");
        internal static readonly int HorizontalOffset = Shader.PropertyToID("_GradientHorizontalOffset");
        internal static readonly int VerticalOffset = Shader.PropertyToID("_GradientVerticalOffset");
        internal static readonly int RedCount = Shader.PropertyToID("_RedCount");
        internal static readonly int GreenCount = Shader.PropertyToID("_GreenCount");
        internal static readonly int BlueCount = Shader.PropertyToID("_BlueCount");
        internal static readonly int InvertColor = Shader.PropertyToID("_InvertColor");

        internal static readonly int Brightness = Shader.PropertyToID("_Brightness");
        internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        internal static readonly int Gamma = Shader.PropertyToID("_Gamma");
        internal static readonly int Hue = Shader.PropertyToID("_Hue");
        internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      private static class Keywords
      {
        internal static readonly string ColorModeSolid = "COLORMODE_SOLID";
        internal static readonly string ColorModeGradient = "COLORMODE_GRADIENT";
        internal static readonly string ColorModeHorizontal = "COLORMODE_HORIZONTAL";
        internal static readonly string ColorModeVertical = "COLORMODE_VERTICAL";
        internal static readonly string ColorModeCircular = "COLORMODE_CIRCULAR";
      }

      /// <summary> Update gradient texture. </summary>
      public void UpdateGradientTexture()
      {
        gradient = settings.gradient;

        const int width = 256;
        const int height = 4;
        gradientTexture = new Texture2D(width, height, TextureFormat.RGB24, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };

        const float inv = 1.0f / (width - 1);
        for (int y = 0; y < height; ++y)
        {
          for (int x = 0; x < width; ++x)
            gradientTexture.SetPixel(x, y, gradient.Evaluate(x * inv));
        }

        gradientTexture.Apply();

        settings.forceGradientTextureUpdate = false;
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass(Settings settings)
      {
        this.settings = settings;

        string shaderPath = $"Shaders/{Constants.Asset.ShaderName}_URP";
        Shader shader = Resources.Load<Shader>(shaderPath);
        if (shader != null)
        {
          if (shader.isSupported == true)
            material = CoreUtils.CreateEngineMaterial(shader);
          else
            Log.Warning($"'{shaderPath}.shader' not supported");
        }
      }

      /// <inheritdoc/>
      public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
      {
        renderTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        renderTextureDescriptor.depthBufferBits = 0;

#if UNITY_2022_1_OR_NEWER
        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;

        RenderingUtils.ReAllocateIfNeeded(ref renderTextureHandle0, renderTextureDescriptor, settings.filterMode, TextureWrapMode.Clamp, false, 1, 0, $"_RTHandle0_{Constants.Asset.Name}");
#else
        colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;

        renderTextureHandle0 = Shader.PropertyToID($"_RTHandle0_{Constants.Asset.Name}");
        cmd.GetTemporaryRT(renderTextureHandle0, renderTextureDescriptor.width, renderTextureDescriptor.height, renderTextureDescriptor.depthBufferBits, settings.filterMode, RenderTextureFormat.ARGB32);
#endif
      }

      /// <inheritdoc/>
      public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
      {
        if (material == null ||
            renderingData.postProcessingEnabled == false ||
            settings.intensity == 0.0f ||
            settings.affectSceneView == false && renderingData.cameraData.isSceneViewCamera == true)
          return;

        CommandBuffer cmd = CommandBufferPool.Get(CommandBufferName);

        if (settings.enableProfiling == true)
#if UNITY_2022_1_OR_NEWER
          profilingScope = new ProfilingScope(cmd, profilingSamples);
#else
          ProfilerMarker.Begin();
#endif

        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, settings.intensity);
        material.SetFloat(ShaderIDs.DeltaTime, settings.ignoreDeltaTimeScale == true ? Time.unscaledDeltaTime : Time.deltaTime);

        material.SetFloat(ShaderIDs.Edges, settings.edges * 100.0f);
        material.SetFloat(ShaderIDs.NoiseStrength, settings.noiseStrength);
        material.SetFloat(ShaderIDs.NoiseSeed, settings.noiseSeed);

        switch (settings.colorMode)
        {
          case ColorModes.Solid:
            material.EnableKeyword(Keywords.ColorModeSolid);
            material.SetColor(ShaderIDs.Color, settings.color);
            break;
          case ColorModes.Gradient:
            if (settings.forceGradientTextureUpdate == true || gradient == null || gradient != settings.gradient)
              UpdateGradientTexture();

            material.EnableKeyword(Keywords.ColorModeGradient);
            material.SetTexture(ShaderIDs.GradientTexture, gradientTexture);
            material.SetFloat(ShaderIDs.LuminanceMin, settings.luminanceMin);
            material.SetFloat(ShaderIDs.LuminanceMax, settings.luminanceMax);
            break;
          case ColorModes.Horizontal:
            material.EnableKeyword(Keywords.ColorModeHorizontal);
            material.SetColor(ShaderIDs.Color0, settings.color0);
            material.SetColor(ShaderIDs.Color1, settings.color1);
            material.SetFloat(ShaderIDs.HorizontalOffset, settings.horizontalOffset - 1.0f);
            break;
          case ColorModes.Vertical:
            material.EnableKeyword(Keywords.ColorModeVertical);
            material.SetColor(ShaderIDs.Color0, settings.color0);
            material.SetColor(ShaderIDs.Color1, settings.color1);
            material.SetFloat(ShaderIDs.VerticalOffset, settings.verticalOffset - 1.0f);
            break;
          case ColorModes.Circular:
            material.EnableKeyword(Keywords.ColorModeCircular);
            material.SetColor(ShaderIDs.Color0, settings.color0);
            material.SetColor(ShaderIDs.Color1, settings.color1);
            material.SetFloat(ShaderIDs.CircularRadius, settings.circularRadius);
            break;
        }

        material.SetInt(ShaderIDs.RedCount, settings.redCount + 1);
        material.SetInt(ShaderIDs.GreenCount, settings.greenCount + 1);
        material.SetInt(ShaderIDs.BlueCount, settings.blueCount + 1);

        material.SetInt(ShaderIDs.InvertColor, settings.invertColor == true ? 1 : 0);

        material.SetInt(ShaderIDs.BlendMode, (int)settings.blendMode);

        material.SetFloat(ShaderIDs.Brightness, settings.brightness);
        material.SetFloat(ShaderIDs.Contrast, settings.contrast);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / settings.gamma);
        material.SetFloat(ShaderIDs.Hue, settings.hue);
        material.SetFloat(ShaderIDs.Saturation, settings.saturation);

#if UNITY_2022_1_OR_NEWER
        cmd.Blit(colorBuffer, renderTextureHandle0, material);
        cmd.Blit(renderTextureHandle0, colorBuffer);
#else
        Blit(cmd, colorBuffer, renderTextureHandle0, material);
        Blit(cmd, renderTextureHandle0, colorBuffer);
#endif

        if (settings.enableProfiling == true)
#if UNITY_2022_1_OR_NEWER
          profilingScope.Dispose();
#else
          ProfilerMarker.End();
#endif

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
      }
    }
  }
}
