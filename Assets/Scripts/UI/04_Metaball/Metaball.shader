﻿/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

Shader "FancyScrollViewGallery/Metaball"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "UnityUI.cginc"
    #include "Assets/Scripts/UI/Common/Common.cginc"
    #include "Metaball.hlsl"

    #pragma multi_compile __ UNITY_UI_CLIP_RECT
    #pragma multi_compile __ UNITY_UI_ALPHACLIP

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
        float2 uiCoord  : TEXCOORD0;
        float4 worldPosition : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    sampler2D _MainTex;
    fixed4 _Color;
    fixed4 _TextureSampleAdd;
    float4 _ClipRect;
    float4 _MainTex_ST;

    v2f vert(appdata_t v)
    {
        v2f OUT;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
        OUT.worldPosition = v.vertex;
        OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

        OUT.uiCoord = ui_coord(TRANSFORM_TEX(v.texcoord, _MainTex));

        OUT.color = v.color * _Color;
        return OUT;
    }

    fixed4 frag(v2f i) : SV_Target
    {
		half4 color = metaball(i.uiCoord);

		color += _TextureSampleAdd;
		color *= i.color;

		// Define a threshold for white color detection
		half threshold = 0.95;

		// If the color is close to white, set alpha to 0
		if (color.r > threshold && color.g > threshold && color.b > threshold)
		{
			color.a = 0.0;
		}

		#ifdef UNITY_UI_CLIP_RECT
		color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
		#endif

		#ifdef UNITY_UI_ALPHACLIP
		clip(color.a - 0.001);
		#endif

		return color;
    }
    ENDCG

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

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            ENDCG
        }
    }
}
