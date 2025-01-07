Shader "Shader Graphs/skyshader"
{
    Properties
    {
        [HDR]_Zenith("Zenith", Color) = (0, 0.6039216, 1, 0)
        [HDR]_Horizon("Horizon", Color) = (0.6745098, 0.945098, 1, 0)
        [HDR]_Nadir("Nadir", Color) = (0, 0, 0, 0)
        _ZenithBlend("ZenithBlend", Range(0, 3)) = 0.7
        _NadirBlend("NadirBlend", Range(0, 3)) = 1
        _NadirDistance("NadirDistance", Range(-10, 0)) = -2
        [NoScaleOffset]_MainTex("CloudTexture", 2D) = "white" {}
        _AnimationSpeed("AnimationSpeed", Vector) = (0, 0, 0, 0)
        _DistancePower("DistancePower", Float) = 0.5
        _CloudColor("CloudColor", Color) = (0.759434, 0.9932119, 1, 0)
        _CloudScale("CloudScale", Vector) = (1, 1, 0, 0)
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Background"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Background"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ USE_LEGACY_LIGHTMAPS
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS : INTERP0;
             float3 normalWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _Zenith;
        float4 _Horizon;
        float4 _Nadir;
        float _ZenithBlend;
        float _NadirBlend;
        float _NadirDistance;
        float4 _MainTex_TexelSize;
        float2 _AnimationSpeed;
        float _DistancePower;
        float4 _CloudColor;
        float2 _CloudScale;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }
        
        void Unity_Maximum_float(float A, float B, out float Out)
        {
            Out = max(A, B);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
        Out = A * B;
        }
        
        void Unity_Minimum_float(float A, float B, out float Out)
        {
            Out = min(A, B);
        };
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float
        {
        float3 WorldSpacePosition;
        };
        
        void SG_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float(float4 _Zenith, float4 _Horizon, float4 _Nadir, float _ZenithBlendingPower, float _NadirBlendingPower, float _NadirDistance, Bindings_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float IN, out float4 OutVector4_1)
        {
        float4 _Property_8426b6085f9f48289cfa29ab164a3f7c_Out_0_Vector4 = _Zenith;
        float3 _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3;
        Unity_Normalize_float3(IN.WorldSpacePosition, _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3);
        float _Split_decd5180da21433b95f44e55fc59333c_R_1_Float = _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3[0];
        float _Split_decd5180da21433b95f44e55fc59333c_G_2_Float = _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3[1];
        float _Split_decd5180da21433b95f44e55fc59333c_B_3_Float = _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3[2];
        float _Split_decd5180da21433b95f44e55fc59333c_A_4_Float = 0;
        float _Maximum_d1991a357d4649fa9dca945f789e1991_Out_2_Float;
        Unity_Maximum_float(_Split_decd5180da21433b95f44e55fc59333c_G_2_Float, float(0), _Maximum_d1991a357d4649fa9dca945f789e1991_Out_2_Float);
        float _Property_4727ba508c6b4f3a8ddb354cc9147560_Out_0_Float = _ZenithBlendingPower;
        float _Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float;
        Unity_Power_float(_Maximum_d1991a357d4649fa9dca945f789e1991_Out_2_Float, _Property_4727ba508c6b4f3a8ddb354cc9147560_Out_0_Float, _Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float);
        float4 _Multiply_2cb1c7e10034402581dd231d431faaae_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_8426b6085f9f48289cfa29ab164a3f7c_Out_0_Vector4, (_Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float.xxxx), _Multiply_2cb1c7e10034402581dd231d431faaae_Out_2_Vector4);
        float4 _Property_1b3c6b119ce048e59107b8e174c675bb_Out_0_Vector4 = _Horizon;
        float _Minimum_bda96f6c3bb540fbbfacd894f7b5ebeb_Out_2_Float;
        Unity_Minimum_float(_Split_decd5180da21433b95f44e55fc59333c_G_2_Float, float(0), _Minimum_bda96f6c3bb540fbbfacd894f7b5ebeb_Out_2_Float);
        float _Property_246776fca21e44ed9f247ed449c3812a_Out_0_Float = _NadirDistance;
        float _Multiply_35059d36d57248e3b3a08cf091e2d767_Out_2_Float;
        Unity_Multiply_float_float(_Minimum_bda96f6c3bb540fbbfacd894f7b5ebeb_Out_2_Float, _Property_246776fca21e44ed9f247ed449c3812a_Out_0_Float, _Multiply_35059d36d57248e3b3a08cf091e2d767_Out_2_Float);
        float _Property_8339c13645b1491c9a5b56319bd6632c_Out_0_Float = _NadirBlendingPower;
        float _Power_87669924c0674c36ada8be0e01885978_Out_2_Float;
        Unity_Power_float(_Multiply_35059d36d57248e3b3a08cf091e2d767_Out_2_Float, _Property_8339c13645b1491c9a5b56319bd6632c_Out_0_Float, _Power_87669924c0674c36ada8be0e01885978_Out_2_Float);
        float _Add_d913ea6dd4b8459bb8a9be67c3de46e9_Out_2_Float;
        Unity_Add_float(_Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float, _Power_87669924c0674c36ada8be0e01885978_Out_2_Float, _Add_d913ea6dd4b8459bb8a9be67c3de46e9_Out_2_Float);
        float _Subtract_c0a9eb29663645679a91470bdcdf1741_Out_2_Float;
        Unity_Subtract_float(float(1), _Add_d913ea6dd4b8459bb8a9be67c3de46e9_Out_2_Float, _Subtract_c0a9eb29663645679a91470bdcdf1741_Out_2_Float);
        float4 _Multiply_68d9c4345aea4329a6cd9b9816c75044_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_1b3c6b119ce048e59107b8e174c675bb_Out_0_Vector4, (_Subtract_c0a9eb29663645679a91470bdcdf1741_Out_2_Float.xxxx), _Multiply_68d9c4345aea4329a6cd9b9816c75044_Out_2_Vector4);
        float4 _Add_7cf0fd3094c14b39ae15911290f2d26c_Out_2_Vector4;
        Unity_Add_float4(_Multiply_2cb1c7e10034402581dd231d431faaae_Out_2_Vector4, _Multiply_68d9c4345aea4329a6cd9b9816c75044_Out_2_Vector4, _Add_7cf0fd3094c14b39ae15911290f2d26c_Out_2_Vector4);
        float4 _Property_bfb496d3f00449f39977781ee2ea6d51_Out_0_Vector4 = _Nadir;
        float4 _Multiply_4fe13bd6b8a74b63a2cc7a1dc17df669_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_bfb496d3f00449f39977781ee2ea6d51_Out_0_Vector4, (_Power_87669924c0674c36ada8be0e01885978_Out_2_Float.xxxx), _Multiply_4fe13bd6b8a74b63a2cc7a1dc17df669_Out_2_Vector4);
        float4 _Add_54403ba09d5a4fe39e9ba4e3385e0e36_Out_2_Vector4;
        Unity_Add_float4(_Add_7cf0fd3094c14b39ae15911290f2d26c_Out_2_Vector4, _Multiply_4fe13bd6b8a74b63a2cc7a1dc17df669_Out_2_Vector4, _Add_54403ba09d5a4fe39e9ba4e3385e0e36_Out_2_Vector4);
        OutVector4_1 = _Add_54403ba09d5a4fe39e9ba4e3385e0e36_Out_2_Vector4;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_CloudTexture_298f78d5bf482344ea32b642d0359629_float
        {
        float3 WorldSpacePosition;
        float3 TimeParameters;
        };
        
        void SG_CloudTexture_298f78d5bf482344ea32b642d0359629_float(UnityTexture2D _mainTex, float2 _AnimationSpeed, float _DistanceFadePower, float4 _Color, float2 _Scale, Bindings_CloudTexture_298f78d5bf482344ea32b642d0359629_float IN, out float3 Color_1, out float Alpha_2)
        {
        float4 _Property_a4d8005778f340a7b85b764d88995f4d_Out_0_Vector4 = _Color;
        UnityTexture2D _Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D = _mainTex;
        float2 _Property_d64b6120f71f4b64b47e88fa93a5ace2_Out_0_Vector2 = _AnimationSpeed;
        float2 _Multiply_b90fce15fda5433ab84f77cc44f50a8d_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_d64b6120f71f4b64b47e88fa93a5ace2_Out_0_Vector2, (IN.TimeParameters.x.xx), _Multiply_b90fce15fda5433ab84f77cc44f50a8d_Out_2_Vector2);
        float2 _Property_4cfd6413e4e44935bdd5012060f3d1a3_Out_0_Vector2 = _Scale;
        float3 _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3;
        Unity_Normalize_float3(IN.WorldSpacePosition, _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3);
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_R_1_Float = _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3[0];
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_G_2_Float = _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3[1];
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_B_3_Float = _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3[2];
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_A_4_Float = 0;
        float2 _Vector2_6ea34f81a5b14161adfa9323ef21aa7b_Out_0_Vector2 = float2(_Split_0db94c0c7fa343e38d79c43373f97c1d_R_1_Float, _Split_0db94c0c7fa343e38d79c43373f97c1d_B_3_Float);
        float2 _Divide_d2abbf59f47540c1b4740343173e21a3_Out_2_Vector2;
        Unity_Divide_float2(_Vector2_6ea34f81a5b14161adfa9323ef21aa7b_Out_0_Vector2, (_Split_0db94c0c7fa343e38d79c43373f97c1d_G_2_Float.xx), _Divide_d2abbf59f47540c1b4740343173e21a3_Out_2_Vector2);
        float2 _Multiply_222cd4b5608749d397c8c4fa11465de7_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_4cfd6413e4e44935bdd5012060f3d1a3_Out_0_Vector2, _Divide_d2abbf59f47540c1b4740343173e21a3_Out_2_Vector2, _Multiply_222cd4b5608749d397c8c4fa11465de7_Out_2_Vector2);
        float2 _Add_6576f7a04e7d4db8af5dd0f197f1e47f_Out_2_Vector2;
        Unity_Add_float2(_Multiply_b90fce15fda5433ab84f77cc44f50a8d_Out_2_Vector2, _Multiply_222cd4b5608749d397c8c4fa11465de7_Out_2_Vector2, _Add_6576f7a04e7d4db8af5dd0f197f1e47f_Out_2_Vector2);
        float4 _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D.tex, _Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D.samplerstate, _Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D.GetTransformedUV(_Add_6576f7a04e7d4db8af5dd0f197f1e47f_Out_2_Vector2) );
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_R_4_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.r;
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_G_5_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.g;
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_B_6_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.b;
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_A_7_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.a;
        float4 _Multiply_65aa31db01b34b629c060c6ed721a29b_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_a4d8005778f340a7b85b764d88995f4d_Out_0_Vector4, _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4, _Multiply_65aa31db01b34b629c060c6ed721a29b_Out_2_Vector4);
        float _Maximum_ab46744447254edd8c10a056edd9040b_Out_2_Float;
        Unity_Maximum_float(_Split_0db94c0c7fa343e38d79c43373f97c1d_G_2_Float, float(0), _Maximum_ab46744447254edd8c10a056edd9040b_Out_2_Float);
        float _Property_e42e7e26b42943c29c8e5b9aa72e2cd3_Out_0_Float = _DistanceFadePower;
        float _Power_984f253eb8a24dc7ae63547e33e3a608_Out_2_Float;
        Unity_Power_float(_Maximum_ab46744447254edd8c10a056edd9040b_Out_2_Float, _Property_e42e7e26b42943c29c8e5b9aa72e2cd3_Out_0_Float, _Power_984f253eb8a24dc7ae63547e33e3a608_Out_2_Float);
        float _Float_6ec7d3d8519d44098e06c9d92062a5f7_Out_0_Float = _Power_984f253eb8a24dc7ae63547e33e3a608_Out_2_Float;
        float _Multiply_eac93e3b4ae24a33b7630073b0a81b2f_Out_2_Float;
        Unity_Multiply_float_float(_SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_A_7_Float, _Float_6ec7d3d8519d44098e06c9d92062a5f7_Out_0_Float, _Multiply_eac93e3b4ae24a33b7630073b0a81b2f_Out_2_Float);
        Color_1 = (_Multiply_65aa31db01b34b629c060c6ed721a29b_Out_2_Vector4.xyz);
        Alpha_2 = _Multiply_eac93e3b4ae24a33b7630073b0a81b2f_Out_2_Float;
        }
        
        void Unity_Blend_Overwrite_float3(float3 Base, float3 Blend, out float3 Out, float Opacity)
        {
            Out = lerp(Base, Blend, Opacity);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_16ea6931cb8940cd91a0e12ed8ab9123_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Zenith) : _Zenith;
            float4 _Property_2f52b43c17034e2ca81bd0c869746ede_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Horizon) : _Horizon;
            float4 _Property_667f786bff4c4a6081073ad71856161b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Nadir) : _Nadir;
            float _Property_c7f332c272884477922fd7d260f86e0f_Out_0_Float = _ZenithBlend;
            float _Property_7280485492a744888740a50998a2ef3f_Out_0_Float = _NadirBlend;
            float _Property_0c8fb0a793bd4fa9910ea3e325be81e7_Out_0_Float = _NadirDistance;
            Bindings_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca;
            _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca.WorldSpacePosition = IN.WorldSpacePosition;
            float4 _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca_OutVector4_1_Vector4;
            SG_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float(_Property_16ea6931cb8940cd91a0e12ed8ab9123_Out_0_Vector4, _Property_2f52b43c17034e2ca81bd0c869746ede_Out_0_Vector4, _Property_667f786bff4c4a6081073ad71856161b_Out_0_Vector4, _Property_c7f332c272884477922fd7d260f86e0f_Out_0_Float, _Property_7280485492a744888740a50998a2ef3f_Out_0_Float, _Property_0c8fb0a793bd4fa9910ea3e325be81e7_Out_0_Float, _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca, _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca_OutVector4_1_Vector4);
            UnityTexture2D _Property_1a07b13f792a4d8c870c42bbfd9680d9_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float2 _Property_ae89f3b30b054ebb9832fbf2b597aefb_Out_0_Vector2 = _AnimationSpeed;
            float _Property_f8501c172add4339ac441612f4b1e167_Out_0_Float = _DistancePower;
            float4 _Property_266be4f810364eddbdb3061b605cfc2a_Out_0_Vector4 = _CloudColor;
            float2 _Property_3c626240251d465d80e3e09320c66397_Out_0_Vector2 = _CloudScale;
            Bindings_CloudTexture_298f78d5bf482344ea32b642d0359629_float _CloudTexture_2021ec138a344d2ab27062a734a25be7;
            _CloudTexture_2021ec138a344d2ab27062a734a25be7.WorldSpacePosition = IN.WorldSpacePosition;
            _CloudTexture_2021ec138a344d2ab27062a734a25be7.TimeParameters = IN.TimeParameters;
            float3 _CloudTexture_2021ec138a344d2ab27062a734a25be7_Color_1_Vector3;
            float _CloudTexture_2021ec138a344d2ab27062a734a25be7_Alpha_2_Float;
            SG_CloudTexture_298f78d5bf482344ea32b642d0359629_float(_Property_1a07b13f792a4d8c870c42bbfd9680d9_Out_0_Texture2D, _Property_ae89f3b30b054ebb9832fbf2b597aefb_Out_0_Vector2, _Property_f8501c172add4339ac441612f4b1e167_Out_0_Float, _Property_266be4f810364eddbdb3061b605cfc2a_Out_0_Vector4, _Property_3c626240251d465d80e3e09320c66397_Out_0_Vector2, _CloudTexture_2021ec138a344d2ab27062a734a25be7, _CloudTexture_2021ec138a344d2ab27062a734a25be7_Color_1_Vector3, _CloudTexture_2021ec138a344d2ab27062a734a25be7_Alpha_2_Float);
            float3 _Blend_3c1d65f614fd4089bf1bdd4c73490bfd_Out_2_Vector3;
            Unity_Blend_Overwrite_float3((_BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca_OutVector4_1_Vector4.xyz), _CloudTexture_2021ec138a344d2ab27062a734a25be7_Color_1_Vector3, _Blend_3c1d65f614fd4089bf1bdd4c73490bfd_Out_2_Vector3, _CloudTexture_2021ec138a344d2ab27062a734a25be7_Alpha_2_Float);
            surface.BaseColor = _Blend_3c1d65f614fd4089bf1bdd4c73490bfd_Out_2_Vector3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "MotionVectors"
            Tags
            {
                "LightMode" = "MotionVectors"
            }
        
        // Render State
        Cull Off
        ZTest LEqual
        ZWrite On
        ColorMask RG
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.5
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_MOTION_VECTORS
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _Zenith;
        float4 _Horizon;
        float4 _Nadir;
        float _ZenithBlend;
        float _NadirBlend;
        float _NadirDistance;
        float4 _MainTex_TexelSize;
        float2 _AnimationSpeed;
        float _DistancePower;
        float4 _CloudColor;
        float2 _CloudScale;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        // GraphFunctions: <None>
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/MotionVectorPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Off
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _Zenith;
        float4 _Horizon;
        float4 _Nadir;
        float _ZenithBlend;
        float _NadirBlend;
        float _NadirDistance;
        float4 _MainTex_TexelSize;
        float2 _AnimationSpeed;
        float _DistancePower;
        float4 _CloudColor;
        float2 _CloudScale;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        // GraphFunctions: <None>
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
        
        // Render State
        Cull Off
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _Zenith;
        float4 _Horizon;
        float4 _Nadir;
        float _ZenithBlend;
        float _NadirBlend;
        float _NadirDistance;
        float4 _MainTex_TexelSize;
        float2 _AnimationSpeed;
        float _DistancePower;
        float4 _CloudColor;
        float2 _CloudScale;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        // GraphFunctions: <None>
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }
        
        // Render State
        Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_GBUFFER
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
             float4 probeOcclusion;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if !defined(LIGHTMAP_ON)
             float3 sh : INTERP0;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
             float4 probeOcclusion : INTERP1;
            #endif
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
            output.probeOcclusion = input.probeOcclusion;
            #endif
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
            output.probeOcclusion = input.probeOcclusion;
            #endif
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _Zenith;
        float4 _Horizon;
        float4 _Nadir;
        float _ZenithBlend;
        float _NadirBlend;
        float _NadirDistance;
        float4 _MainTex_TexelSize;
        float2 _AnimationSpeed;
        float _DistancePower;
        float4 _CloudColor;
        float2 _CloudScale;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }
        
        void Unity_Maximum_float(float A, float B, out float Out)
        {
            Out = max(A, B);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
        Out = A * B;
        }
        
        void Unity_Minimum_float(float A, float B, out float Out)
        {
            Out = min(A, B);
        };
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float
        {
        float3 WorldSpacePosition;
        };
        
        void SG_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float(float4 _Zenith, float4 _Horizon, float4 _Nadir, float _ZenithBlendingPower, float _NadirBlendingPower, float _NadirDistance, Bindings_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float IN, out float4 OutVector4_1)
        {
        float4 _Property_8426b6085f9f48289cfa29ab164a3f7c_Out_0_Vector4 = _Zenith;
        float3 _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3;
        Unity_Normalize_float3(IN.WorldSpacePosition, _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3);
        float _Split_decd5180da21433b95f44e55fc59333c_R_1_Float = _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3[0];
        float _Split_decd5180da21433b95f44e55fc59333c_G_2_Float = _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3[1];
        float _Split_decd5180da21433b95f44e55fc59333c_B_3_Float = _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3[2];
        float _Split_decd5180da21433b95f44e55fc59333c_A_4_Float = 0;
        float _Maximum_d1991a357d4649fa9dca945f789e1991_Out_2_Float;
        Unity_Maximum_float(_Split_decd5180da21433b95f44e55fc59333c_G_2_Float, float(0), _Maximum_d1991a357d4649fa9dca945f789e1991_Out_2_Float);
        float _Property_4727ba508c6b4f3a8ddb354cc9147560_Out_0_Float = _ZenithBlendingPower;
        float _Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float;
        Unity_Power_float(_Maximum_d1991a357d4649fa9dca945f789e1991_Out_2_Float, _Property_4727ba508c6b4f3a8ddb354cc9147560_Out_0_Float, _Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float);
        float4 _Multiply_2cb1c7e10034402581dd231d431faaae_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_8426b6085f9f48289cfa29ab164a3f7c_Out_0_Vector4, (_Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float.xxxx), _Multiply_2cb1c7e10034402581dd231d431faaae_Out_2_Vector4);
        float4 _Property_1b3c6b119ce048e59107b8e174c675bb_Out_0_Vector4 = _Horizon;
        float _Minimum_bda96f6c3bb540fbbfacd894f7b5ebeb_Out_2_Float;
        Unity_Minimum_float(_Split_decd5180da21433b95f44e55fc59333c_G_2_Float, float(0), _Minimum_bda96f6c3bb540fbbfacd894f7b5ebeb_Out_2_Float);
        float _Property_246776fca21e44ed9f247ed449c3812a_Out_0_Float = _NadirDistance;
        float _Multiply_35059d36d57248e3b3a08cf091e2d767_Out_2_Float;
        Unity_Multiply_float_float(_Minimum_bda96f6c3bb540fbbfacd894f7b5ebeb_Out_2_Float, _Property_246776fca21e44ed9f247ed449c3812a_Out_0_Float, _Multiply_35059d36d57248e3b3a08cf091e2d767_Out_2_Float);
        float _Property_8339c13645b1491c9a5b56319bd6632c_Out_0_Float = _NadirBlendingPower;
        float _Power_87669924c0674c36ada8be0e01885978_Out_2_Float;
        Unity_Power_float(_Multiply_35059d36d57248e3b3a08cf091e2d767_Out_2_Float, _Property_8339c13645b1491c9a5b56319bd6632c_Out_0_Float, _Power_87669924c0674c36ada8be0e01885978_Out_2_Float);
        float _Add_d913ea6dd4b8459bb8a9be67c3de46e9_Out_2_Float;
        Unity_Add_float(_Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float, _Power_87669924c0674c36ada8be0e01885978_Out_2_Float, _Add_d913ea6dd4b8459bb8a9be67c3de46e9_Out_2_Float);
        float _Subtract_c0a9eb29663645679a91470bdcdf1741_Out_2_Float;
        Unity_Subtract_float(float(1), _Add_d913ea6dd4b8459bb8a9be67c3de46e9_Out_2_Float, _Subtract_c0a9eb29663645679a91470bdcdf1741_Out_2_Float);
        float4 _Multiply_68d9c4345aea4329a6cd9b9816c75044_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_1b3c6b119ce048e59107b8e174c675bb_Out_0_Vector4, (_Subtract_c0a9eb29663645679a91470bdcdf1741_Out_2_Float.xxxx), _Multiply_68d9c4345aea4329a6cd9b9816c75044_Out_2_Vector4);
        float4 _Add_7cf0fd3094c14b39ae15911290f2d26c_Out_2_Vector4;
        Unity_Add_float4(_Multiply_2cb1c7e10034402581dd231d431faaae_Out_2_Vector4, _Multiply_68d9c4345aea4329a6cd9b9816c75044_Out_2_Vector4, _Add_7cf0fd3094c14b39ae15911290f2d26c_Out_2_Vector4);
        float4 _Property_bfb496d3f00449f39977781ee2ea6d51_Out_0_Vector4 = _Nadir;
        float4 _Multiply_4fe13bd6b8a74b63a2cc7a1dc17df669_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_bfb496d3f00449f39977781ee2ea6d51_Out_0_Vector4, (_Power_87669924c0674c36ada8be0e01885978_Out_2_Float.xxxx), _Multiply_4fe13bd6b8a74b63a2cc7a1dc17df669_Out_2_Vector4);
        float4 _Add_54403ba09d5a4fe39e9ba4e3385e0e36_Out_2_Vector4;
        Unity_Add_float4(_Add_7cf0fd3094c14b39ae15911290f2d26c_Out_2_Vector4, _Multiply_4fe13bd6b8a74b63a2cc7a1dc17df669_Out_2_Vector4, _Add_54403ba09d5a4fe39e9ba4e3385e0e36_Out_2_Vector4);
        OutVector4_1 = _Add_54403ba09d5a4fe39e9ba4e3385e0e36_Out_2_Vector4;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_CloudTexture_298f78d5bf482344ea32b642d0359629_float
        {
        float3 WorldSpacePosition;
        float3 TimeParameters;
        };
        
        void SG_CloudTexture_298f78d5bf482344ea32b642d0359629_float(UnityTexture2D _mainTex, float2 _AnimationSpeed, float _DistanceFadePower, float4 _Color, float2 _Scale, Bindings_CloudTexture_298f78d5bf482344ea32b642d0359629_float IN, out float3 Color_1, out float Alpha_2)
        {
        float4 _Property_a4d8005778f340a7b85b764d88995f4d_Out_0_Vector4 = _Color;
        UnityTexture2D _Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D = _mainTex;
        float2 _Property_d64b6120f71f4b64b47e88fa93a5ace2_Out_0_Vector2 = _AnimationSpeed;
        float2 _Multiply_b90fce15fda5433ab84f77cc44f50a8d_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_d64b6120f71f4b64b47e88fa93a5ace2_Out_0_Vector2, (IN.TimeParameters.x.xx), _Multiply_b90fce15fda5433ab84f77cc44f50a8d_Out_2_Vector2);
        float2 _Property_4cfd6413e4e44935bdd5012060f3d1a3_Out_0_Vector2 = _Scale;
        float3 _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3;
        Unity_Normalize_float3(IN.WorldSpacePosition, _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3);
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_R_1_Float = _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3[0];
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_G_2_Float = _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3[1];
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_B_3_Float = _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3[2];
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_A_4_Float = 0;
        float2 _Vector2_6ea34f81a5b14161adfa9323ef21aa7b_Out_0_Vector2 = float2(_Split_0db94c0c7fa343e38d79c43373f97c1d_R_1_Float, _Split_0db94c0c7fa343e38d79c43373f97c1d_B_3_Float);
        float2 _Divide_d2abbf59f47540c1b4740343173e21a3_Out_2_Vector2;
        Unity_Divide_float2(_Vector2_6ea34f81a5b14161adfa9323ef21aa7b_Out_0_Vector2, (_Split_0db94c0c7fa343e38d79c43373f97c1d_G_2_Float.xx), _Divide_d2abbf59f47540c1b4740343173e21a3_Out_2_Vector2);
        float2 _Multiply_222cd4b5608749d397c8c4fa11465de7_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_4cfd6413e4e44935bdd5012060f3d1a3_Out_0_Vector2, _Divide_d2abbf59f47540c1b4740343173e21a3_Out_2_Vector2, _Multiply_222cd4b5608749d397c8c4fa11465de7_Out_2_Vector2);
        float2 _Add_6576f7a04e7d4db8af5dd0f197f1e47f_Out_2_Vector2;
        Unity_Add_float2(_Multiply_b90fce15fda5433ab84f77cc44f50a8d_Out_2_Vector2, _Multiply_222cd4b5608749d397c8c4fa11465de7_Out_2_Vector2, _Add_6576f7a04e7d4db8af5dd0f197f1e47f_Out_2_Vector2);
        float4 _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D.tex, _Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D.samplerstate, _Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D.GetTransformedUV(_Add_6576f7a04e7d4db8af5dd0f197f1e47f_Out_2_Vector2) );
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_R_4_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.r;
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_G_5_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.g;
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_B_6_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.b;
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_A_7_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.a;
        float4 _Multiply_65aa31db01b34b629c060c6ed721a29b_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_a4d8005778f340a7b85b764d88995f4d_Out_0_Vector4, _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4, _Multiply_65aa31db01b34b629c060c6ed721a29b_Out_2_Vector4);
        float _Maximum_ab46744447254edd8c10a056edd9040b_Out_2_Float;
        Unity_Maximum_float(_Split_0db94c0c7fa343e38d79c43373f97c1d_G_2_Float, float(0), _Maximum_ab46744447254edd8c10a056edd9040b_Out_2_Float);
        float _Property_e42e7e26b42943c29c8e5b9aa72e2cd3_Out_0_Float = _DistanceFadePower;
        float _Power_984f253eb8a24dc7ae63547e33e3a608_Out_2_Float;
        Unity_Power_float(_Maximum_ab46744447254edd8c10a056edd9040b_Out_2_Float, _Property_e42e7e26b42943c29c8e5b9aa72e2cd3_Out_0_Float, _Power_984f253eb8a24dc7ae63547e33e3a608_Out_2_Float);
        float _Float_6ec7d3d8519d44098e06c9d92062a5f7_Out_0_Float = _Power_984f253eb8a24dc7ae63547e33e3a608_Out_2_Float;
        float _Multiply_eac93e3b4ae24a33b7630073b0a81b2f_Out_2_Float;
        Unity_Multiply_float_float(_SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_A_7_Float, _Float_6ec7d3d8519d44098e06c9d92062a5f7_Out_0_Float, _Multiply_eac93e3b4ae24a33b7630073b0a81b2f_Out_2_Float);
        Color_1 = (_Multiply_65aa31db01b34b629c060c6ed721a29b_Out_2_Vector4.xyz);
        Alpha_2 = _Multiply_eac93e3b4ae24a33b7630073b0a81b2f_Out_2_Float;
        }
        
        void Unity_Blend_Overwrite_float3(float3 Base, float3 Blend, out float3 Out, float Opacity)
        {
            Out = lerp(Base, Blend, Opacity);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_16ea6931cb8940cd91a0e12ed8ab9123_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Zenith) : _Zenith;
            float4 _Property_2f52b43c17034e2ca81bd0c869746ede_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Horizon) : _Horizon;
            float4 _Property_667f786bff4c4a6081073ad71856161b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Nadir) : _Nadir;
            float _Property_c7f332c272884477922fd7d260f86e0f_Out_0_Float = _ZenithBlend;
            float _Property_7280485492a744888740a50998a2ef3f_Out_0_Float = _NadirBlend;
            float _Property_0c8fb0a793bd4fa9910ea3e325be81e7_Out_0_Float = _NadirDistance;
            Bindings_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca;
            _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca.WorldSpacePosition = IN.WorldSpacePosition;
            float4 _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca_OutVector4_1_Vector4;
            SG_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float(_Property_16ea6931cb8940cd91a0e12ed8ab9123_Out_0_Vector4, _Property_2f52b43c17034e2ca81bd0c869746ede_Out_0_Vector4, _Property_667f786bff4c4a6081073ad71856161b_Out_0_Vector4, _Property_c7f332c272884477922fd7d260f86e0f_Out_0_Float, _Property_7280485492a744888740a50998a2ef3f_Out_0_Float, _Property_0c8fb0a793bd4fa9910ea3e325be81e7_Out_0_Float, _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca, _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca_OutVector4_1_Vector4);
            UnityTexture2D _Property_1a07b13f792a4d8c870c42bbfd9680d9_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float2 _Property_ae89f3b30b054ebb9832fbf2b597aefb_Out_0_Vector2 = _AnimationSpeed;
            float _Property_f8501c172add4339ac441612f4b1e167_Out_0_Float = _DistancePower;
            float4 _Property_266be4f810364eddbdb3061b605cfc2a_Out_0_Vector4 = _CloudColor;
            float2 _Property_3c626240251d465d80e3e09320c66397_Out_0_Vector2 = _CloudScale;
            Bindings_CloudTexture_298f78d5bf482344ea32b642d0359629_float _CloudTexture_2021ec138a344d2ab27062a734a25be7;
            _CloudTexture_2021ec138a344d2ab27062a734a25be7.WorldSpacePosition = IN.WorldSpacePosition;
            _CloudTexture_2021ec138a344d2ab27062a734a25be7.TimeParameters = IN.TimeParameters;
            float3 _CloudTexture_2021ec138a344d2ab27062a734a25be7_Color_1_Vector3;
            float _CloudTexture_2021ec138a344d2ab27062a734a25be7_Alpha_2_Float;
            SG_CloudTexture_298f78d5bf482344ea32b642d0359629_float(_Property_1a07b13f792a4d8c870c42bbfd9680d9_Out_0_Texture2D, _Property_ae89f3b30b054ebb9832fbf2b597aefb_Out_0_Vector2, _Property_f8501c172add4339ac441612f4b1e167_Out_0_Float, _Property_266be4f810364eddbdb3061b605cfc2a_Out_0_Vector4, _Property_3c626240251d465d80e3e09320c66397_Out_0_Vector2, _CloudTexture_2021ec138a344d2ab27062a734a25be7, _CloudTexture_2021ec138a344d2ab27062a734a25be7_Color_1_Vector3, _CloudTexture_2021ec138a344d2ab27062a734a25be7_Alpha_2_Float);
            float3 _Blend_3c1d65f614fd4089bf1bdd4c73490bfd_Out_2_Vector3;
            Unity_Blend_Overwrite_float3((_BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca_OutVector4_1_Vector4.xyz), _CloudTexture_2021ec138a344d2ab27062a734a25be7_Color_1_Vector3, _Blend_3c1d65f614fd4089bf1bdd4c73490bfd_Out_2_Vector3, _CloudTexture_2021ec138a344d2ab27062a734a25be7_Alpha_2_Float);
            surface.BaseColor = _Blend_3c1d65f614fd4089bf1bdd4c73490bfd_Out_2_Vector3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitGBufferPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _Zenith;
        float4 _Horizon;
        float4 _Nadir;
        float _ZenithBlend;
        float _NadirBlend;
        float _NadirDistance;
        float4 _MainTex_TexelSize;
        float2 _AnimationSpeed;
        float _DistancePower;
        float4 _CloudColor;
        float2 _CloudScale;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        // GraphFunctions: <None>
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _Zenith;
        float4 _Horizon;
        float4 _Nadir;
        float _ZenithBlend;
        float _NadirBlend;
        float _NadirDistance;
        float4 _MainTex_TexelSize;
        float2 _AnimationSpeed;
        float _DistancePower;
        float4 _CloudColor;
        float2 _CloudScale;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }
        
        void Unity_Maximum_float(float A, float B, out float Out)
        {
            Out = max(A, B);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
        Out = A * B;
        }
        
        void Unity_Minimum_float(float A, float B, out float Out)
        {
            Out = min(A, B);
        };
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float
        {
        float3 WorldSpacePosition;
        };
        
        void SG_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float(float4 _Zenith, float4 _Horizon, float4 _Nadir, float _ZenithBlendingPower, float _NadirBlendingPower, float _NadirDistance, Bindings_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float IN, out float4 OutVector4_1)
        {
        float4 _Property_8426b6085f9f48289cfa29ab164a3f7c_Out_0_Vector4 = _Zenith;
        float3 _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3;
        Unity_Normalize_float3(IN.WorldSpacePosition, _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3);
        float _Split_decd5180da21433b95f44e55fc59333c_R_1_Float = _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3[0];
        float _Split_decd5180da21433b95f44e55fc59333c_G_2_Float = _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3[1];
        float _Split_decd5180da21433b95f44e55fc59333c_B_3_Float = _Normalize_88e73648d5b7464d8fb5f89f3b457c73_Out_1_Vector3[2];
        float _Split_decd5180da21433b95f44e55fc59333c_A_4_Float = 0;
        float _Maximum_d1991a357d4649fa9dca945f789e1991_Out_2_Float;
        Unity_Maximum_float(_Split_decd5180da21433b95f44e55fc59333c_G_2_Float, float(0), _Maximum_d1991a357d4649fa9dca945f789e1991_Out_2_Float);
        float _Property_4727ba508c6b4f3a8ddb354cc9147560_Out_0_Float = _ZenithBlendingPower;
        float _Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float;
        Unity_Power_float(_Maximum_d1991a357d4649fa9dca945f789e1991_Out_2_Float, _Property_4727ba508c6b4f3a8ddb354cc9147560_Out_0_Float, _Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float);
        float4 _Multiply_2cb1c7e10034402581dd231d431faaae_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_8426b6085f9f48289cfa29ab164a3f7c_Out_0_Vector4, (_Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float.xxxx), _Multiply_2cb1c7e10034402581dd231d431faaae_Out_2_Vector4);
        float4 _Property_1b3c6b119ce048e59107b8e174c675bb_Out_0_Vector4 = _Horizon;
        float _Minimum_bda96f6c3bb540fbbfacd894f7b5ebeb_Out_2_Float;
        Unity_Minimum_float(_Split_decd5180da21433b95f44e55fc59333c_G_2_Float, float(0), _Minimum_bda96f6c3bb540fbbfacd894f7b5ebeb_Out_2_Float);
        float _Property_246776fca21e44ed9f247ed449c3812a_Out_0_Float = _NadirDistance;
        float _Multiply_35059d36d57248e3b3a08cf091e2d767_Out_2_Float;
        Unity_Multiply_float_float(_Minimum_bda96f6c3bb540fbbfacd894f7b5ebeb_Out_2_Float, _Property_246776fca21e44ed9f247ed449c3812a_Out_0_Float, _Multiply_35059d36d57248e3b3a08cf091e2d767_Out_2_Float);
        float _Property_8339c13645b1491c9a5b56319bd6632c_Out_0_Float = _NadirBlendingPower;
        float _Power_87669924c0674c36ada8be0e01885978_Out_2_Float;
        Unity_Power_float(_Multiply_35059d36d57248e3b3a08cf091e2d767_Out_2_Float, _Property_8339c13645b1491c9a5b56319bd6632c_Out_0_Float, _Power_87669924c0674c36ada8be0e01885978_Out_2_Float);
        float _Add_d913ea6dd4b8459bb8a9be67c3de46e9_Out_2_Float;
        Unity_Add_float(_Power_58bca6334c814d48a09b2816682ee67e_Out_2_Float, _Power_87669924c0674c36ada8be0e01885978_Out_2_Float, _Add_d913ea6dd4b8459bb8a9be67c3de46e9_Out_2_Float);
        float _Subtract_c0a9eb29663645679a91470bdcdf1741_Out_2_Float;
        Unity_Subtract_float(float(1), _Add_d913ea6dd4b8459bb8a9be67c3de46e9_Out_2_Float, _Subtract_c0a9eb29663645679a91470bdcdf1741_Out_2_Float);
        float4 _Multiply_68d9c4345aea4329a6cd9b9816c75044_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_1b3c6b119ce048e59107b8e174c675bb_Out_0_Vector4, (_Subtract_c0a9eb29663645679a91470bdcdf1741_Out_2_Float.xxxx), _Multiply_68d9c4345aea4329a6cd9b9816c75044_Out_2_Vector4);
        float4 _Add_7cf0fd3094c14b39ae15911290f2d26c_Out_2_Vector4;
        Unity_Add_float4(_Multiply_2cb1c7e10034402581dd231d431faaae_Out_2_Vector4, _Multiply_68d9c4345aea4329a6cd9b9816c75044_Out_2_Vector4, _Add_7cf0fd3094c14b39ae15911290f2d26c_Out_2_Vector4);
        float4 _Property_bfb496d3f00449f39977781ee2ea6d51_Out_0_Vector4 = _Nadir;
        float4 _Multiply_4fe13bd6b8a74b63a2cc7a1dc17df669_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_bfb496d3f00449f39977781ee2ea6d51_Out_0_Vector4, (_Power_87669924c0674c36ada8be0e01885978_Out_2_Float.xxxx), _Multiply_4fe13bd6b8a74b63a2cc7a1dc17df669_Out_2_Vector4);
        float4 _Add_54403ba09d5a4fe39e9ba4e3385e0e36_Out_2_Vector4;
        Unity_Add_float4(_Add_7cf0fd3094c14b39ae15911290f2d26c_Out_2_Vector4, _Multiply_4fe13bd6b8a74b63a2cc7a1dc17df669_Out_2_Vector4, _Add_54403ba09d5a4fe39e9ba4e3385e0e36_Out_2_Vector4);
        OutVector4_1 = _Add_54403ba09d5a4fe39e9ba4e3385e0e36_Out_2_Vector4;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_CloudTexture_298f78d5bf482344ea32b642d0359629_float
        {
        float3 WorldSpacePosition;
        float3 TimeParameters;
        };
        
        void SG_CloudTexture_298f78d5bf482344ea32b642d0359629_float(UnityTexture2D _mainTex, float2 _AnimationSpeed, float _DistanceFadePower, float4 _Color, float2 _Scale, Bindings_CloudTexture_298f78d5bf482344ea32b642d0359629_float IN, out float3 Color_1, out float Alpha_2)
        {
        float4 _Property_a4d8005778f340a7b85b764d88995f4d_Out_0_Vector4 = _Color;
        UnityTexture2D _Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D = _mainTex;
        float2 _Property_d64b6120f71f4b64b47e88fa93a5ace2_Out_0_Vector2 = _AnimationSpeed;
        float2 _Multiply_b90fce15fda5433ab84f77cc44f50a8d_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_d64b6120f71f4b64b47e88fa93a5ace2_Out_0_Vector2, (IN.TimeParameters.x.xx), _Multiply_b90fce15fda5433ab84f77cc44f50a8d_Out_2_Vector2);
        float2 _Property_4cfd6413e4e44935bdd5012060f3d1a3_Out_0_Vector2 = _Scale;
        float3 _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3;
        Unity_Normalize_float3(IN.WorldSpacePosition, _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3);
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_R_1_Float = _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3[0];
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_G_2_Float = _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3[1];
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_B_3_Float = _Normalize_6c888bc5408c4150a32a9290cd414f73_Out_1_Vector3[2];
        float _Split_0db94c0c7fa343e38d79c43373f97c1d_A_4_Float = 0;
        float2 _Vector2_6ea34f81a5b14161adfa9323ef21aa7b_Out_0_Vector2 = float2(_Split_0db94c0c7fa343e38d79c43373f97c1d_R_1_Float, _Split_0db94c0c7fa343e38d79c43373f97c1d_B_3_Float);
        float2 _Divide_d2abbf59f47540c1b4740343173e21a3_Out_2_Vector2;
        Unity_Divide_float2(_Vector2_6ea34f81a5b14161adfa9323ef21aa7b_Out_0_Vector2, (_Split_0db94c0c7fa343e38d79c43373f97c1d_G_2_Float.xx), _Divide_d2abbf59f47540c1b4740343173e21a3_Out_2_Vector2);
        float2 _Multiply_222cd4b5608749d397c8c4fa11465de7_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_4cfd6413e4e44935bdd5012060f3d1a3_Out_0_Vector2, _Divide_d2abbf59f47540c1b4740343173e21a3_Out_2_Vector2, _Multiply_222cd4b5608749d397c8c4fa11465de7_Out_2_Vector2);
        float2 _Add_6576f7a04e7d4db8af5dd0f197f1e47f_Out_2_Vector2;
        Unity_Add_float2(_Multiply_b90fce15fda5433ab84f77cc44f50a8d_Out_2_Vector2, _Multiply_222cd4b5608749d397c8c4fa11465de7_Out_2_Vector2, _Add_6576f7a04e7d4db8af5dd0f197f1e47f_Out_2_Vector2);
        float4 _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D.tex, _Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D.samplerstate, _Property_7fe0806856204a5d818c7a1613111d16_Out_0_Texture2D.GetTransformedUV(_Add_6576f7a04e7d4db8af5dd0f197f1e47f_Out_2_Vector2) );
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_R_4_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.r;
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_G_5_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.g;
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_B_6_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.b;
        float _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_A_7_Float = _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4.a;
        float4 _Multiply_65aa31db01b34b629c060c6ed721a29b_Out_2_Vector4;
        Unity_Multiply_float4_float4(_Property_a4d8005778f340a7b85b764d88995f4d_Out_0_Vector4, _SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_RGBA_0_Vector4, _Multiply_65aa31db01b34b629c060c6ed721a29b_Out_2_Vector4);
        float _Maximum_ab46744447254edd8c10a056edd9040b_Out_2_Float;
        Unity_Maximum_float(_Split_0db94c0c7fa343e38d79c43373f97c1d_G_2_Float, float(0), _Maximum_ab46744447254edd8c10a056edd9040b_Out_2_Float);
        float _Property_e42e7e26b42943c29c8e5b9aa72e2cd3_Out_0_Float = _DistanceFadePower;
        float _Power_984f253eb8a24dc7ae63547e33e3a608_Out_2_Float;
        Unity_Power_float(_Maximum_ab46744447254edd8c10a056edd9040b_Out_2_Float, _Property_e42e7e26b42943c29c8e5b9aa72e2cd3_Out_0_Float, _Power_984f253eb8a24dc7ae63547e33e3a608_Out_2_Float);
        float _Float_6ec7d3d8519d44098e06c9d92062a5f7_Out_0_Float = _Power_984f253eb8a24dc7ae63547e33e3a608_Out_2_Float;
        float _Multiply_eac93e3b4ae24a33b7630073b0a81b2f_Out_2_Float;
        Unity_Multiply_float_float(_SampleTexture2D_71ea0cfc6f984fb69d599a5143c8263a_A_7_Float, _Float_6ec7d3d8519d44098e06c9d92062a5f7_Out_0_Float, _Multiply_eac93e3b4ae24a33b7630073b0a81b2f_Out_2_Float);
        Color_1 = (_Multiply_65aa31db01b34b629c060c6ed721a29b_Out_2_Vector4.xyz);
        Alpha_2 = _Multiply_eac93e3b4ae24a33b7630073b0a81b2f_Out_2_Float;
        }
        
        void Unity_Blend_Overwrite_float3(float3 Base, float3 Blend, out float3 Out, float Opacity)
        {
            Out = lerp(Base, Blend, Opacity);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_16ea6931cb8940cd91a0e12ed8ab9123_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Zenith) : _Zenith;
            float4 _Property_2f52b43c17034e2ca81bd0c869746ede_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Horizon) : _Horizon;
            float4 _Property_667f786bff4c4a6081073ad71856161b_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Nadir) : _Nadir;
            float _Property_c7f332c272884477922fd7d260f86e0f_Out_0_Float = _ZenithBlend;
            float _Property_7280485492a744888740a50998a2ef3f_Out_0_Float = _NadirBlend;
            float _Property_0c8fb0a793bd4fa9910ea3e325be81e7_Out_0_Float = _NadirDistance;
            Bindings_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca;
            _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca.WorldSpacePosition = IN.WorldSpacePosition;
            float4 _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca_OutVector4_1_Vector4;
            SG_BlendSkyboxColors_a6940017eb957d241a39815111abfc7a_float(_Property_16ea6931cb8940cd91a0e12ed8ab9123_Out_0_Vector4, _Property_2f52b43c17034e2ca81bd0c869746ede_Out_0_Vector4, _Property_667f786bff4c4a6081073ad71856161b_Out_0_Vector4, _Property_c7f332c272884477922fd7d260f86e0f_Out_0_Float, _Property_7280485492a744888740a50998a2ef3f_Out_0_Float, _Property_0c8fb0a793bd4fa9910ea3e325be81e7_Out_0_Float, _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca, _BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca_OutVector4_1_Vector4);
            UnityTexture2D _Property_1a07b13f792a4d8c870c42bbfd9680d9_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float2 _Property_ae89f3b30b054ebb9832fbf2b597aefb_Out_0_Vector2 = _AnimationSpeed;
            float _Property_f8501c172add4339ac441612f4b1e167_Out_0_Float = _DistancePower;
            float4 _Property_266be4f810364eddbdb3061b605cfc2a_Out_0_Vector4 = _CloudColor;
            float2 _Property_3c626240251d465d80e3e09320c66397_Out_0_Vector2 = _CloudScale;
            Bindings_CloudTexture_298f78d5bf482344ea32b642d0359629_float _CloudTexture_2021ec138a344d2ab27062a734a25be7;
            _CloudTexture_2021ec138a344d2ab27062a734a25be7.WorldSpacePosition = IN.WorldSpacePosition;
            _CloudTexture_2021ec138a344d2ab27062a734a25be7.TimeParameters = IN.TimeParameters;
            float3 _CloudTexture_2021ec138a344d2ab27062a734a25be7_Color_1_Vector3;
            float _CloudTexture_2021ec138a344d2ab27062a734a25be7_Alpha_2_Float;
            SG_CloudTexture_298f78d5bf482344ea32b642d0359629_float(_Property_1a07b13f792a4d8c870c42bbfd9680d9_Out_0_Texture2D, _Property_ae89f3b30b054ebb9832fbf2b597aefb_Out_0_Vector2, _Property_f8501c172add4339ac441612f4b1e167_Out_0_Float, _Property_266be4f810364eddbdb3061b605cfc2a_Out_0_Vector4, _Property_3c626240251d465d80e3e09320c66397_Out_0_Vector2, _CloudTexture_2021ec138a344d2ab27062a734a25be7, _CloudTexture_2021ec138a344d2ab27062a734a25be7_Color_1_Vector3, _CloudTexture_2021ec138a344d2ab27062a734a25be7_Alpha_2_Float);
            float3 _Blend_3c1d65f614fd4089bf1bdd4c73490bfd_Out_2_Vector3;
            Unity_Blend_Overwrite_float3((_BlendSkyboxColors_dcfd1313d7a84437900cc538437f41ca_OutVector4_1_Vector4.xyz), _CloudTexture_2021ec138a344d2ab27062a734a25be7_Color_1_Vector3, _Blend_3c1d65f614fd4089bf1bdd4c73490bfd_Out_2_Vector3, _CloudTexture_2021ec138a344d2ab27062a734a25be7_Alpha_2_Float);
            surface.BaseColor = _Blend_3c1d65f614fd4089bf1bdd4c73490bfd_Out_2_Vector3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}