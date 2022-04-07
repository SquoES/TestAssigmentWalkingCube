// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Cloth"
{
	Properties
	{
		_Noisecount("Noise count", Vector) = (1,1,0,0)
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 32
		_TessPhongStrength( "Phong Tess Strength", Range( 0, 1 ) ) = 0.5
		_Size("Size", Vector) = (1,1,1,0)
		_Directions("Directions", Vector) = (1,0,0,0)
		_Clothtile("Cloth tile", Float) = 1
		_Speed("Speed", Float) = 0.3
		_Height("Height", Float) = 1
		_MainTex("MainTex", 2D) = "white" {}
		_EdgeDistance("Edge Distance", Float) = 0
		_Edgepower("Edge power", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction tessphong:_TessPhongStrength 

		struct appdata_full_custom
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			float4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform float3 _Size;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _EdgeDistance;
		uniform float _Edgepower;
		uniform float _Height;
		uniform float _Speed;
		uniform float2 _Directions;
		uniform float2 _Noisecount;
		uniform float _Clothtile;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _TessValue;
		uniform float _TessPhongStrength;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float4 tessFunction( )
		{
			return _TessValue;
		}

		void vertexDataFunc( inout appdata_full_custom v )
		{
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth71 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_LOD( _CameraDepthTexture, float4( ase_screenPosNorm.xy, 0, 0 ) ));
			float distanceDepth71 = abs( ( screenDepth71 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _EdgeDistance ) );
			float Edge76 = ( distanceDepth71 * _Edgepower );
			float clampResult79 = clamp( Edge76 , 0.0 , _Height );
			float temp_output_20_0 = ( _Time.y * _Speed );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult23 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldUV24 = appendResult23;
			float4 TileUV36 = ( ( WorldUV24 * float4( _Noisecount, 0.0 , 0.0 ) ) * _Clothtile );
			float2 panner16 = ( temp_output_20_0 * _Directions + TileUV36.xy);
			float simplePerlin2D17 = snoise( panner16 );
			simplePerlin2D17 = simplePerlin2D17*0.5 + 0.5;
			float2 temp_cast_2 = (_Directions.x).xx;
			float2 panner39 = ( temp_output_20_0 * temp_cast_2 + ( TileUV36 * float4( 0,0,0,0 ) ).xy);
			float simplePerlin2D40 = snoise( panner39 );
			simplePerlin2D40 = simplePerlin2D40*0.5 + 0.5;
			float Pattern54 = ( simplePerlin2D17 + ( simplePerlin2D40 * 0.25 ) );
			float3 Heigth58 = ( ( _Size * clampResult79 ) * Pattern54 );
			float3 temp_output_59_0 = Heigth58;
			v.vertex.xyz += temp_output_59_0;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth71 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth71 = abs( ( screenDepth71 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _EdgeDistance ) );
			float Edge76 = ( distanceDepth71 * _Edgepower );
			float clampResult79 = clamp( Edge76 , 0.0 , _Height );
			float temp_output_20_0 = ( _Time.y * _Speed );
			float3 ase_worldPos = i.worldPos;
			float4 appendResult23 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldUV24 = appendResult23;
			float4 TileUV36 = ( ( WorldUV24 * float4( _Noisecount, 0.0 , 0.0 ) ) * _Clothtile );
			float2 panner16 = ( temp_output_20_0 * _Directions + TileUV36.xy);
			float simplePerlin2D17 = snoise( panner16 );
			simplePerlin2D17 = simplePerlin2D17*0.5 + 0.5;
			float2 temp_cast_2 = (_Directions.x).xx;
			float2 panner39 = ( temp_output_20_0 * temp_cast_2 + ( TileUV36 * float4( 0,0,0,0 ) ).xy);
			float simplePerlin2D40 = snoise( panner39 );
			simplePerlin2D40 = simplePerlin2D40*0.5 + 0.5;
			float Pattern54 = ( simplePerlin2D17 + ( simplePerlin2D40 * 0.25 ) );
			float3 Heigth58 = ( ( _Size * clampResult79 ) * Pattern54 );
			float3 temp_output_59_0 = Heigth58;
			o.Normal = temp_output_59_0;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			o.Albedo = tex2D( _MainTex, uv_MainTex ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
284;30;862;540;593.5568;90.83185;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;25;-3209.149,-192.275;Inherit;False;589.0478;248.5378;World UV;3;23;22;24;World UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;22;-3159.149,-142.275;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;23;-2983.308,-122.7372;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-2844.101,-122.7372;Inherit;False;WorldUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;38;-3249.571,209.0603;Inherit;False;887.1208;335.1042;Comment;6;26;28;27;29;30;36;TileUV;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;26;-3199.571,259.0602;Inherit;False;24;WorldUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;28;-3184.627,335.7086;Inherit;False;Property;_Noisecount;Noise count;0;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;29;-3026.8,383.3751;Inherit;False;Property;_Clothtile;Cloth tile;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-3019.235,282.9159;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-2843.663,291.1645;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-2586.45,291.6438;Inherit;False;TileUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;-3442.645,1256.821;Inherit;False;36;TileUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-3495.136,1092.846;Inherit;False;Property;_Speed;Speed;9;0;Create;True;0;0;0;False;0;False;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;19;-3510.701,1020.346;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-3338.694,1028.59;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-3258.802,1210.497;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;32;-3310.726,837.3564;Inherit;False;Property;_Directions;Directions;7;0;Create;True;0;0;0;False;0;False;1,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;72;-1832.836,-617.3896;Inherit;False;Property;_EdgeDistance;Edge Distance;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-3503.555,676.9846;Inherit;False;36;TileUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DepthFade;71;-1646.782,-634.909;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-1551.645,-505.2111;Inherit;False;Property;_Edgepower;Edge power;13;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;39;-3080.275,1065.511;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;40;-2808.154,1061.885;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1388.248,-633.7077;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-2639.686,1277.32;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;0.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;16;-3089.696,830.851;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;17;-2810.697,829.851;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;76;-1248.646,-598.8074;Inherit;False;Edge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-2535.426,1123.275;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-2351.199,996.9001;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-2362.739,778.713;Inherit;False;76;Edge;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-2325.678,850.6994;Inherit;False;Property;_Height;Height;10;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-2216.229,1001.995;Inherit;False;Pattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;79;-2191.919,781.192;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;34;-2327.225,622.0027;Inherit;False;Property;_Size;Size;6;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1993.761,717.1415;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-1993.891,875.7943;Inherit;False;54;Pattern;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1832.116,766.0188;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;69;-1766.575,1133.838;Inherit;False;779.9605;402.0926;Comment;5;64;65;66;67;68;Tesselation;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;-1693.452,763.13;Inherit;False;Heigth;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-1670.134,1183.838;Inherit;False;Constant;_Tesselation;Tesselation;9;0;Create;True;0;0;0;False;0;False;100;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-1709.242,1328.049;Inherit;False;Constant;_MinDistTesselation;MinDistTesselation;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;-213.9077,355.2097;Inherit;False;68;Tesselation;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-1716.575,1420.931;Inherit;False;Constant;_MaxDistTesselation;MaxDistTesselation;9;0;Create;True;0;0;0;False;0;False;100;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceBasedTessNode;64;-1459.928,1286.497;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;-244.9221,264.9658;Inherit;False;58;Heigth;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;77;-819.4199,-231.6236;Inherit;False;Constant;_Color0;Color 0;11;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;62;-353.2694,-101.3074;Inherit;True;Property;_MainTex;MainTex;11;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;-1210.615,1301.162;Inherit;False;Tesselation;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector3Node;80;-254.5568,68.16815;Inherit;False;Constant;_Vector0;Vector 0;10;0;Create;True;0;0;0;False;0;False;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Cloth;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;1;32;0;25;True;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;23;0;22;1
WireConnection;23;1;22;3
WireConnection;24;0;23;0
WireConnection;27;0;26;0
WireConnection;27;1;28;0
WireConnection;30;0;27;0
WireConnection;30;1;29;0
WireConnection;36;0;30;0
WireConnection;20;0;19;0
WireConnection;20;1;21;0
WireConnection;47;0;49;0
WireConnection;71;0;72;0
WireConnection;39;0;47;0
WireConnection;39;2;32;1
WireConnection;39;1;20;0
WireConnection;40;0;39;0
WireConnection;74;0;71;0
WireConnection;74;1;75;0
WireConnection;16;0;37;0
WireConnection;16;2;32;0
WireConnection;16;1;20;0
WireConnection;17;0;16;0
WireConnection;76;0;74;0
WireConnection;52;0;40;0
WireConnection;52;1;53;0
WireConnection;48;0;17;0
WireConnection;48;1;52;0
WireConnection;54;0;48;0
WireConnection;79;0;78;0
WireConnection;79;2;55;0
WireConnection;35;0;34;0
WireConnection;35;1;79;0
WireConnection;56;0;35;0
WireConnection;56;1;57;0
WireConnection;58;0;56;0
WireConnection;64;0;65;0
WireConnection;64;1;66;0
WireConnection;64;2;67;0
WireConnection;68;0;64;0
WireConnection;0;0;62;0
WireConnection;0;1;59;0
WireConnection;0;11;59;0
ASEEND*/
//CHKSM=4FD4713A75EB3436B0B434774DC5513FB52F883D