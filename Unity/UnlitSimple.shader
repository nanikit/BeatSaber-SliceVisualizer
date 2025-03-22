Shader "Nanikit/Unlit/Simple"
{
  Properties
  {
    _Color ("Color", Color) = (1,1,1,1)
    _Tex ("Texture", 2D) = "white" {}
  }
  SubShader
  {
    Tags { "RenderType" = "Opaque" }
    Cull Off
    LOD 100

    Pass
    {
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_instancing
      #pragma multi_compile_fog

      #include "UnityCG.cginc"

      struct attributes
      {
        float4 vertex : POSITION;
        float4 color : COLOR;
        float2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct v2f
      {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
        float4 color : COLOR;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
      };

      float4 _Color;
      sampler2D _Tex;
      float4 _Tex_ST;

      v2f vert(attributes input)
      {
        v2f output = (v2f)0;

        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_TRANSFER_INSTANCE_ID(input, output);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        output.vertex = UnityObjectToClipPos(input.vertex);
        output.uv = TRANSFORM_TEX(input.uv, _Tex);
        output.color = input.color;

        return output;
      }

      half4 frag(v2f input) : SV_Target
      {
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        half4 baseColor = _Color * tex2D(_Tex, input.uv);
        return half4(baseColor.rgb, 0) * input.color;
      }
      ENDHLSL
    }
  }
}
