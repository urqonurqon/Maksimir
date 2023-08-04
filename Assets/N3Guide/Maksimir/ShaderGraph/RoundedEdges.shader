Shader "Custom/Alpha Mask" {
Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Alpha ("Alpha", Range(0,1)) = 1
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        [MaterialToggle] _OutlineSharp ( "Sharp outline", Float ) = 0
        //_StencilComp ("Stencil Comparison", Float) = 8
        //_Stencil ("Stencil ID", Float) = 0
        //_StencilOp ("Stencil Operation", Float) = 0
        //_StencilWriteMask ("Stencil Write Mask", Float) = 255
        //_StencilReadMask ("Stencil Read Mask", Float) = 255
        //_ColorMask ("Color Mask", Float) = 15
        _Radius ("Radius", Range(0,1)) = 1
        _Smoothing ("Smoothing", Range(0,1)) = 1
        
        [MaterialToggle] _TopLeft ( "Top left", Float ) = 0
        [MaterialToggle] _TopRight ( "Top right", Float ) = 0
        [MaterialToggle] _BottomLeft ( "Bottom left", Float ) = 0
        [MaterialToggle] _BottomRight ( "Bottom right", Float ) = 0
        _x_res ("X res", Float) = 1
        _y_res ("Y res", Float) = 1
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Cull Off
        Lighting Off
        ZWrite On
        ZTest [unity_GUIZTestMode]
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Stencil
        {
            Ref 2
            Comp Always

            //Pass [_StencilOp]
            //ReadMask [_StencilReadMask]
            //WriteMask [_StencilWriteMask]
        }
       
        Pass {


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _OutlineColor;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _x_res;
            float _y_res;
            float _Radius;
            float _Smoothing;
            float _Alpha;
            half _TopLeft;
            half _TopRight;
            half _BottomLeft;
            half _BottomRight;
            half _OutlineSharp;
            float changeRange (float x, float min, float max){
                if(x<min) return 0;
                return (x-min)/(max-min);
            }
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                
                return OUT;
            }
            fixed4 frag(v2f IN) : SV_Target
            {
                //half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                float4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);
                float aspect = _MainTex_TexelSize.w / _MainTex_TexelSize.z;
                //half4 color = _TextureSampleAdd;
                //float aspect = _y_res / _x_res;
                float objx = IN.texcoord.x * 2 - 1;
                float objy = IN.texcoord.y * 2 - 1;
                float x;
                float y;
                if (aspect > 1){
                    y = clamp(abs(objy) * aspect  + 1 - aspect,0,1);
                    x = clamp(abs(objx),0,1);
                } else { 
                    float f = 1/aspect;
                    x = clamp(abs(objx) * f  + 1 - f,0,1);
                    y = clamp(abs(objy),0,1);
                }
                x = changeRange(x, 1 -_Radius, 1);
                y = changeRange(y, 1 -_Radius, 1);
                float z;
                if (_TopRight==0 && objx>0 && objy > 0)
                {
                    z = max(x,y);
                    z *=z;
                }
                else if (_TopLeft==0 && objx<0 && objy > 0)
                {
                    z = max(x,y);
                    z *=z;
                }
                else if (_BottomRight==0 && objx>0 && objy < 0)
                {
                    z = max(x,y);
                    z *=z;
                }
                else if (_BottomLeft==0 && objx<0 && objy < 0)
                {
                    z = max(x,y);
                    z *=z;
                }
                else {
                    z = (x*x+y*y);
                }
                if (z > 1) z = 1;
                
                z = 1-changeRange (z,_Smoothing,1);
                if (_OutlineSharp != 0 && z<1 && z>0){
                    z = 1;
                    color.rgb = _OutlineColor.rgb;
                }
                
                return float4(color.rgb,z);
            }  
        ENDCG
    }
}}