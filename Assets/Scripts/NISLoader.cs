using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ELFSharp.ELF;
using ELFSharp.ELF.Sections;
using UnityEngine.Experimental.PlayerLoop;
//using UnityScript.Scripting;
using Common;
using Common.Geometry.Data;
using Common.Textures.Data;
using Debug = UnityEngine.Debug;

public class NISLoader : MonoBehaviour
{

	// 40 bytes
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CameraTrackHeader
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
		public byte[] Unknown1;

		public float DurationCarbon;
		public float Duration;
		public short entryCount;
		public byte zero;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
		public string TrackName;
		
		public CameraTrackHeader Clone()
		{
			CameraTrackHeader clone = (CameraTrackHeader)MemberwiseClone();
			clone.Unknown1 = new byte[12];
			for (int i = 0; i < 12; i++)
				clone.Unknown1[i] = Unknown1[i];
			return clone;
		}
	}

	public class CameraTrackEntry
	{
		public CameraTrackEntry(System.Object obj)
		{
			if (obj.GetType() == typeof(CameraTrackEntryMW))
			{
				type = 0;
				obj0 = (CameraTrackEntryMW) obj;
			}
		}

		public int type = -1;
		public CameraTrackEntryMW obj0;

		public byte[] attributes
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.attributes = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.attributes;
				}
			}
		}
		
		public byte[] unk13
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.unk13 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.unk13;
				}
			}
		}
		
		public float Time
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.Time = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.Time;
				}
			}
		}
		
		public float unk5
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.Drift = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.Drift;
				}
			}
		}
		
		public float unk6
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.Drift2 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.Drift2;
				}
			}
		}
		
		public float EyeX
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.EyeX = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.EyeX;
				}
			}
		}
		
		public float EyeZ
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.EyeZ = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.EyeZ;
				}
			}
		}
		
		public float EyeY
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.EyeY = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.EyeY;
				}
			}
		}
		
		public float EyeX2
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.EyeX2 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.EyeX2;
				}
			}
		}
		
		public float EyeZ2
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.EyeZ2 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.EyeZ2;
				}
			}
		}
		
		public float EyeY2
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.EyeY2 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.EyeY2;
				}
			}
		}
		
		public float LookX
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.LookX = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.LookX;
				}
			}
		}
		
		public float LookZ
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.LookZ = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.LookZ;
				}
			}
		}
		
		public float LookY
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.LookY = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.LookY;
				}
			}
		}
		
		public float LookX2
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.LookX2 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.LookX2;
				}
			}
		}
		
		public float LookZ2
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.LookZ2 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.LookZ2;
				}
			}
		}
		
		public float LookY2
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.LookY2 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.LookY2;
				}
			}
		}
		
		public float Tangent
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.Tangent = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.Tangent;
				}
			}
		}
		
		public float Tangent2
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.Tangent2 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.Tangent2;
				}
			}
		}
		
		public float FocalLength
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.FocalLength = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.FocalLength;
				}
			}
		}
		
		public float FocalLength2
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.FocalLength2 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.FocalLength2;
				}
			}
		}
		
		public float Amp
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.Amp = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.Amp;
				}
			}
		}
		
		public float Amp2
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.Amp2 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.Amp2;
				}
			}
		}
		
		public float Freq
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.Freq = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.Freq;
				}
			}
		}
		
		public float Freq2
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.Freq2 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.Freq2;
				}
			}
		}
		
		public float unk9
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.unk9 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.unk9;
				}
			}
		}
		
		public float unk10
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.unk10 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.unk10;
				}
			}
		}
		
		public float unk11
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.unk11 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.unk11;
				}
			}
		}
		
		public float unk12
		{
			set
			{
				switch (type)
				{
					case 0:
						obj0.unk12 = value;
						break;
				}
			}
			get
			{
				switch (type)
				{
					default:
						return obj0.unk12;
				}
			}
		}
	}

	// 132 bytes
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CameraTrackEntryMW
	{
		// 0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15
		// 0D-00-01-01 01-01-00-00 00-00-03-00 1F-BD-00-00
		// 10 - shows police LetterBox when 0x03, 0x02 is fadeout
		// 4 - 0 when car should be tracked, 1 when worldspace, 3 when localspace
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] attributes;

		public float Time;
		public float Drift;
		public float Drift2;
		public float EyeX;
		public float EyeZ;
		public float EyeY;
		public float EyeX2;
		public float EyeZ2;
		public float EyeY2;
		public float LookX;
		public float LookZ;
		public float LookY;
		public float LookX2;
		public float LookZ2;
		public float LookY2;
		public float Tangent;
		public float Tangent2;
		public float FocalLength;
		public float FocalLength2;
		public float unk9;
		public float unk10;
		public float Amp;
		public float Amp2;
		public float Freq;
		public float Freq2;
		public float unk11;
		public float unk12;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] unk13;

		public CameraTrackEntryMW Clone()
		{
			CameraTrackEntryMW clone = (CameraTrackEntryMW)MemberwiseClone();
			clone.attributes = new byte[16];
			for (int i = 0; i < 16; i++)
				clone.attributes[i] = attributes[i];
			clone.unk13 = new byte[8];
			for (int i = 0; i < 8; i++)
				clone.unk13[i] = unk13[i];
			return clone;
		}
	}
	
	// 132 bytes
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CameraTrackEntryCarbon
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] attributes;

		public float Time;
		public float Drift;
		public float Drift2;
		public float EyeX;
		public float EyeZ;
		public float EyeY;
		public float EyeX2;
		public float EyeZ2;
		public float EyeY2;
		public float LookX;
		public float LookZ;
		public float LookY;
		public float LookX2;
		public float LookZ2;
		public float LookY2;
		public float Tangent;
		public float Tangent2;
		public float FocalLength;
		public float FocalLength2;
		public float unk9;
		public float unk10;
		public float Amp;
		public float Amp2;
		public float Freq;
		public float Freq2;
		public float unk11;
		public float unk12;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] unk13;

		public CameraTrackEntryCarbon Clone()
		{
			CameraTrackEntryCarbon clone = (CameraTrackEntryCarbon)MemberwiseClone();
			clone.attributes = new byte[16];
			for (int i = 0; i < 16; i++)
				clone.attributes[i] = attributes[i];
			clone.unk13 = new byte[8];
			for (int i = 0; i < 8; i++)
				clone.unk13[i] = unk13[i];
			return clone;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct NisScene
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] SceneNameHash;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string SceneName;

		public uint DescriptionPointer;
		public short SceneType;
		public short ICEContext;
		public int HaveLayout;
		public int HaveCarAnimation;
		public int VanishFrame;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] unk;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
		public string SeeulatorOverlayName;
	}

	[HideInInspector] public GameObject LetterBox;
	[HideInInspector] public GameObject FadeOut;

	public struct camrec
	{
		public CameraTrackEntry e;

		public camrec(CameraTrackEntry _e)
		{
			e = _e;
		}

		public (camrec, camrec) SplitInTwo(float nexttime)
		{
			camrec camrec1 = new camrec();
			camrec camrec2 = new camrec();
			switch (camrec1.e.type)
			{
				case 0:
					camrec1.e = new CameraTrackEntry(e.obj0.Clone());
					camrec2.e = new CameraTrackEntry(e.obj0.Clone());
					break;
			}
			camrec1.e.unk6 = Mathf.Lerp(e.unk5, e.unk6, 0.5f);
			camrec1.e.EyeX2 = Mathf.Lerp(e.EyeX, e.EyeX2, 0.5f);
			camrec1.e.EyeY2 = Mathf.Lerp(e.EyeY, e.EyeY2, 0.5f);
			camrec1.e.EyeZ2 = Mathf.Lerp(e.EyeZ, e.EyeZ2, 0.5f);
			camrec1.e.LookX2 = Mathf.Lerp(e.LookX, e.LookX2, 0.5f);
			camrec1.e.LookY2 = Mathf.Lerp(e.LookY, e.LookY2, 0.5f);
			camrec1.e.LookZ2 = Mathf.Lerp(e.LookZ, e.LookZ2, 0.5f);
			camrec1.e.Tangent2 = Mathf.Lerp(e.Tangent, e.Tangent2, 0.5f);
			camrec1.e.FocalLength2 = Mathf.Lerp(e.FocalLength, e.FocalLength2, 0.5f);
			camrec1.e.unk10 = Mathf.Lerp(e.unk9, e.unk10, 0.5f);
			camrec1.e.Amp2 = Mathf.Lerp(e.Amp, e.Amp2, 0.5f);
			camrec1.e.Freq2 = Mathf.Lerp(e.Freq, e.Freq2, 0.5f);
			camrec1.e.unk12 = Mathf.Lerp(e.unk11, e.unk12, 0.5f);
			camrec2.e.Time = Mathf.Lerp(e.Time, nexttime, 0.5f);
			camrec2.e.unk5 = camrec1.e.unk6;
			camrec2.e.EyeX = camrec1.e.EyeX2;
			camrec2.e.EyeY = camrec1.e.EyeY2;
			camrec2.e.EyeZ = camrec1.e.EyeZ2;
			camrec2.e.LookX = camrec1.e.LookX2;
			camrec2.e.LookY = camrec1.e.LookY2;
			camrec2.e.LookZ = camrec1.e.LookZ2;
			camrec2.e.Tangent = camrec1.e.Tangent2;
			camrec2.e.FocalLength = camrec1.e.FocalLength2;
			camrec2.e.unk9 = camrec1.e.unk10;
			camrec2.e.Amp = camrec1.e.Amp2;
			camrec2.e.Freq = camrec1.e.Freq2;
			camrec2.e.unk11 = camrec1.e.unk12;
			return (camrec1, camrec2);
		}
	}

	public class NISData
	{
		public string name;
		public string cameraTrackName;
		public SceneType sceneType;
		public List<CameraSpline> cam;
		public List<Animation> animations;
		public List<Skeleton> skeletons;
		public AudioClip audio;
		public AudioClip audio2;
		public float totalDuration;
	}

	public class CameraSpline
	{
		public float start;
		public float end;
		public List<camrec> cam = new List<camrec>();

		int ind(ref float t)
		{
			float globaltime = Mathf.Lerp(start, end, t);
			int index = 0;
			for (int i = 0; i < cam.Count; i += 2)
			{
				if (globaltime >= cam[i].e.Time && (i + 2 >= cam.Count || globaltime <= cam[i + 2].e.Time))
				{
					index = i;
					break;
				}
			}
			camrec rec1 = cam[index];
			camrec rec2 = cam[index + 1];
			float startTime = rec1.e.Time;
			float midTime = rec2.e.Time;
			float endTime = index + 2 >= cam.Count ? end : cam[index + 2].e.Time;
			if (globaltime < midTime)
				t = Mathf.Lerp(0f, 0.5f, Mathf.InverseLerp(startTime, midTime, globaltime));
			else
				t = Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(midTime, endTime, globaltime));
			return index;
		}

		public Vector3 GetEyePos(float t)
		{
			Vector3 p1, p2, p3, p4;
			if (cam.Count == 1)
			{
				p1 = new Vector3(cam[0].e.EyeX, cam[0].e.EyeY, cam[0].e.EyeZ);
				p2 = new Vector3(cam[0].e.EyeX2, cam[0].e.EyeY2, cam[0].e.EyeZ2);
				if (Main.CameraSmoothingEnabled)
					return Vector3.Lerp(p1, p2, Mathf.SmoothStep(0f, 1f, t));
				return Vector3.Lerp(p1, p2, t);
			}
			int i = ind(ref t);
			p1 = new Vector3(cam[i].e.EyeX, cam[i].e.EyeY, cam[i].e.EyeZ);
			p2 = new Vector3(cam[i].e.EyeX2, cam[i].e.EyeY2, cam[i].e.EyeZ2);
			p3 = new Vector3(cam[i + 1].e.EyeX, cam[i + 1].e.EyeY, cam[i + 1].e.EyeZ);
			p4 = new Vector3(cam[i + 1].e.EyeX2, cam[i + 1].e.EyeY2, cam[i + 1].e.EyeZ2);
			return GetPoint(p1, p2, p3, p4, Mathf.SmoothStep(0f, 1f, t));
		}

		public Vector3 GetLookPos(float t)
		{
			Vector3 p1, p2, p3, p4;
			if (cam.Count == 1)
			{
				p1 = new Vector3(cam[0].e.LookX, cam[0].e.LookY, cam[0].e.LookZ);
				p2 = new Vector3(cam[0].e.LookX2, cam[0].e.LookY2, cam[0].e.LookZ2);
				if (Main.CameraSmoothingEnabled)
					return Vector3.Lerp(p1, p2, Mathf.SmoothStep(0f, 1f, t));
				return Vector3.Lerp(p1, p2, t);
			}
			int i = ind(ref t);
			p1 = new Vector3(cam[i].e.LookX, cam[i].e.LookY, cam[i].e.LookZ);
			p2 = new Vector3(cam[i].e.LookX2, cam[i].e.LookY2, cam[i].e.LookZ2);
			p3 = new Vector3(cam[i + 1].e.LookX, cam[i + 1].e.LookY, cam[i + 1].e.LookZ);
			p4 = new Vector3(cam[i + 1].e.LookX2, cam[i + 1].e.LookY2, cam[i + 1].e.LookZ2);
			return GetPoint(p1, p2, p3, p4, Mathf.SmoothStep(0f, 1f, t));
		}

		public float GetAmp(float t)
		{
			float p1, p2, p3, p4;
			if (cam.Count == 1)
			{
				p1 = cam[0].e.Amp;
				p2 = cam[0].e.Amp2;
				if (Main.CameraSmoothingEnabled)
					return Mathf.SmoothStep(p1, p2, t);
				return Mathf.Lerp(p1, p2, t);
			}
			int i = ind(ref t);
			p1 = cam[i].e.Amp;
			p2 = cam[i].e.Amp2;
			p3 = cam[i + 1].e.Amp;
			p4 = cam[i + 1].e.Amp2;
			return Lerp1dCurve(p1, p2, p3, p4, Mathf.SmoothStep(0f, 1f, t));
		}
		
		public float GetFreq(float t)
		{
			float p1, p2, p3, p4;
			if (cam.Count == 1)
			{
				p1 = cam[0].e.Freq;
				p2 = cam[0].e.Freq2;
				if (Main.CameraSmoothingEnabled)
					return Mathf.SmoothStep(p1, p2, t);
				return Mathf.Lerp(p1, p2, t);
			}
			int i = ind(ref t);
			p1 = cam[i].e.Freq;
			p2 = cam[i].e.Freq2;
			p3 = cam[i + 1].e.Freq;
			p4 = cam[i + 1].e.Freq2;
			return Lerp1dCurve(p1, p2, p3, p4, Mathf.SmoothStep(0f, 1f, t));
		}
		
		public float GetTangent(float t)
		{
			float p1, p2, p3, p4;
			if (cam.Count == 1)
			{
				p1 = cam[0].e.Tangent;
				p2 = cam[0].e.Tangent2;
				if (Main.CameraSmoothingEnabled)
					return Mathf.SmoothStep(p1, p2, t);
				return Mathf.Lerp(p1, p2, t);
			}
			int i = ind(ref t);
			p1 = cam[i].e.Tangent;
			p2 = cam[i].e.Tangent2;
			p3 = cam[i + 1].e.Tangent;
			p4 = cam[i + 1].e.Tangent2;
			return Lerp1dCurve(p1, p2, p3, p4, Mathf.SmoothStep(0f, 1f, t));
		}
		
		public float GetFocalLength(float t)
		{
			float p1, p2, p3, p4;
			if (cam.Count == 1)
			{
				p1 = cam[0].e.FocalLength;
				p2 = cam[0].e.FocalLength2;
				if (Main.CameraSmoothingEnabled)
					return Mathf.SmoothStep(p1, p2, t);
				return Mathf.Lerp(p1, p2, t);
			}
			int i = ind(ref t);
			p1 = cam[i].e.FocalLength;
			p2 = cam[i].e.FocalLength2;
			p3 = cam[i + 1].e.FocalLength;
			p4 = cam[i + 1].e.FocalLength2;
			return Lerp1dCurve(p1, p2, p3, p4, Mathf.SmoothStep(0f, 1f, t));
		}

        public float GetTimeScale(float t)
        {
            float p1, p2, p3, p4;
            if (cam.Count == 1)
            {
                p1 = cam[0].e.unk13[4];
                p2 = cam[0].e.unk13[5];
                if (Main.CameraSmoothingEnabled)
                    return Mathf.SmoothStep(p1, p2, t);
                return Mathf.Lerp(p1, p2, t);
            }
            int i = ind(ref t);
            p1 = cam[i].e.unk13[4];
            p2 = cam[i].e.unk13[5];
            p3 = cam[i + 1].e.unk13[4];
            p4 = cam[i + 1].e.unk13[5];
            return Lerp1dCurve(p1, p2, p3, p4, Mathf.SmoothStep(0f, 1f, t));
        }

        Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				oneMinusT * oneMinusT * oneMinusT * p0 +
				3f * oneMinusT * oneMinusT * t * p1 +
				3f * oneMinusT * t * t * p2 +
				t * t * t * p3;
		}
		
		float Lerp1dCurve(float a, float b, float c, float d, float t)
		{
			return a * Mathf.Pow(1f - t, 3) + 3f * b * Mathf.Pow(1f - t, 2) * t + 3f * c * (1f - t) * Mathf.Pow(t, 2) + d * Mathf.Pow(t, 3);
		}
	}

	private Vector3 startpos;
	private Quaternion startrot;

	[HideInInspector] public static bool started;
	private float startTime;
	private AudioSource source;
	private bool syncTimeToSound;
	private float timePassed => syncTimeToSound ? source.time : Time.unscaledTime - startTime;

	public static (float[], float[], float, bool) EvaluateAnim(Animation anim, float curTime)
	{
		if (anim.type == AnimType.ANIM_COMPOUND)
		{
			return EvaluateAnim(anim.subAnimations[0], curTime);
		}
		if (anim.delta == null)
			return (null, null, 0f, false);
		float pos = curTime * 15f;
		int start_pos = Mathf.Clamp((int) Mathf.Floor(pos), 0, anim.delta.Length - 1);
		int end_pos = Mathf.Clamp((int) Mathf.Ceil(pos), 0, anim.delta.Length - 1);
		float subdelta = Mathf.InverseLerp(start_pos, end_pos, pos);
		return (anim.delta[start_pos], anim.delta[end_pos], subdelta, start_pos == end_pos && start_pos == anim.delta.Length - 1);
	}
	
	public static (float[], float[], float, bool) EvaluateAnimAbs(Animation anim, float curTime)
	{
		if (anim.type == AnimType.ANIM_COMPOUND)
		{
			return EvaluateAnimAbs(anim.subAnimations[0], curTime);
		}
		if (anim.delta == null)
			return (null, null, 0f, false);
		float pos = curTime;
		int start_pos = Mathf.Clamp((int) Mathf.Round(pos), 0, anim.delta.Length - 1);
		return (anim.delta[start_pos], anim.delta[start_pos], 0f, false);
	}

	public static void ApplyBoneAnimation(Bone[] bones, Animation anim, float curTime, bool absoluteIndex, Transform trans)
	{
		(float[], float[], float, bool) eval;
		//(float[], float[], float, bool) eval_layout_pos;
		//(float[], float[], float, bool) eval_layout_rot;
		if (absoluteIndex)
		{
			eval = EvaluateAnimAbs(anim.subAnimations[0], curTime);
			//eval_layout_pos = EvaluateAnimAbs(anim.subAnimations[1], curTime);
			//eval_layout_rot = EvaluateAnimAbs(anim.subAnimations[2], curTime);
		}
		else
		{
			eval = EvaluateAnim(anim.subAnimations[0], curTime);
			//eval_layout_pos = EvaluateAnim(anim.subAnimations[1], curTime);
			//eval_layout_rot = EvaluateAnim(anim.subAnimations[2], curTime);
		}
		if (eval.Item1 == null) return;
		int offset;
		for (int boneNum = 0; boneNum < bones.Length; boneNum++)
		{
			offset = boneNum * 4;
			if (offset + 3 > eval.Item1.Length - 1)
				break;
			bones[boneNum].assignedTransform.localRotation = Quaternion.Lerp(new Quaternion(eval.Item1[offset], eval.Item1[offset + 1], eval.Item1[offset + 2], eval.Item1[offset + 3]), new Quaternion(eval.Item2[offset], eval.Item2[offset + 1], eval.Item2[offset + 2], eval.Item2[offset + 3]), eval.Item3);
		}
		//trans.position = Vector3.Lerp(new Vector3(eval_layout_pos.Item1[0], eval_layout_pos.Item1[2], eval_layout_pos.Item1[1]), new Vector3(eval_layout_pos.Item2[0], eval_layout_pos.Item2[2], eval_layout_pos.Item2[1]), eval_layout_pos.Item3);
		//trans.rotation = Quaternion.Euler(0f, Mathf.Lerp(eval_layout_rot.Item1[0], eval_layout_rot.Item2[0], eval_layout_rot.Item3), 0f);
	}

	Dictionary<string, Transform> copCars = new Dictionary<string, Transform>();
	private Dictionary<string, Transform> skinnedModels = new Dictionary<string, Transform>();

	private LayerMask oldCameraMask;

	[HideInInspector]
	public byte[] cam_attrs = new byte[16];

	public static void ApplyCameraMovement(CameraSpline spline, float progression, float t, Transform player_car, Camera target, Transform debugFocus, bool cambeingedited)
	{
		camrec rec = spline.cam[0];
		float f = spline.end;
		for (int i = 0; i < spline.cam.Count; i++)
		{
			if (progression >= spline.cam[i].e.Time)
			{
				f = i < spline.cam.Count - 1 ? spline.cam[i + 1].e.Time : spline.end;
				if (progression > f)
					continue;
				rec = spline.cam[i];
				break;
			}
		}
		progression = Mathf.InverseLerp(spline.start, spline.end, progression);
		if (!cambeingedited)
		{
			Vector3 eyepos = spline.GetEyePos(progression);
			Vector3 lookatpos = spline.GetLookPos(progression);
			target.transform.position = eyepos;
			target.transform.LookAt(lookatpos);
			if (rec.e.attributes[4] == 0x00) // todo add to cxmw
			{
				player_car.Rotate(0f, -90f, 0f);
				target.transform.position = player_car.TransformPoint(target.transform.position);
				target.transform.rotation = player_car.rotation * target.transform.rotation;
				player_car.Rotate(0f, 90f, 0f);
			}
			debugFocus.position = lookatpos;
		}
		float amp = spline.GetAmp(progression);
		float freq = spline.GetFreq(progression);
		target.transform.Rotate(new Vector3(amp * Mathf.Sin(t * freq), amp * Mathf.Sin(t * 2f * freq), spline.GetTangent(progression) * 360f), Space.Self);
		target.focalLength = spline.GetFocalLength(progression);
        Main.timescale = spline.GetTimeScale(progression) * 0.01f;
	}

	// todo add to cxmw
	public static float CalculateNISDuration(List<Animation> anims)
	{
		float maxduration = 0f;
		Action<float> apply = x => maxduration = Mathf.Max(maxduration, x);
		foreach (Animation anim in anims)
		{
			if (anim.delta != null)
				apply((anim.delta.Length - 1) / 15f);
			foreach (Animation child in anim.subAnimations)
				if (child.delta != null)
					apply((child.delta.Length - 1) / 15f);
		}
		return maxduration;
	}

	public static List<CameraSpline> MakeSplines(List<camrec> rec)
	{
		List<CameraSpline> splines = new List<CameraSpline>();
		splines.Add(new CameraSpline());
		splines[0].cam.Add(rec[0]);
		splines[0].start = rec[0].e.Time;
		splines[0].end = rec.Count > 1 ? rec[1].e.Time : 1f;
		camrec laste;
		for (int i = 1; i < rec.Count; i++)
		{
			laste = splines[splines.Count - 1].cam[splines[splines.Count - 1].cam.Count - 1];
			if (new Vector3(laste.e.EyeX2, laste.e.EyeY2, laste.e.EyeZ2) != new Vector3(rec[i].e.EyeX, rec[i].e.EyeY, rec[i].e.EyeZ) || !Main.CameraSmoothingEnabled)
			{
				if (splines[splines.Count - 1].cam.Count > 1 && splines[splines.Count - 1].cam.Count % 2 != 0)
				{
					(camrec, camrec) rpl = laste.SplitInTwo(splines[splines.Count - 1].end);
					splines[splines.Count - 1].cam[splines[splines.Count - 1].cam.Count - 1] = rpl.Item1;
					splines[splines.Count - 1].cam.Add(rpl.Item2);
				}
				splines.Add(new CameraSpline());
				splines[splines.Count - 1].start = rec[i].e.Time;
				splines[splines.Count - 1].end = i < rec.Count - 1 ? rec[i + 1].e.Time : 1f;
			} else {
				splines[splines.Count - 1].start = Mathf.Min(splines[splines.Count - 1].start, rec[i].e.Time);
				splines[splines.Count - 1].end = Mathf.Max(splines[splines.Count - 1].end, i < rec.Count - 1 ? rec[i + 1].e.Time : 1f);
			}
			splines[splines.Count - 1].cam.Add(rec[i]);
		}
		if (splines[splines.Count - 1].cam.Count > 1 && splines[splines.Count - 1].cam.Count % 2 != 0)
		{
			(camrec, camrec) rpl = splines[splines.Count - 1].cam[splines[splines.Count - 1].cam.Count - 1].SplitInTwo(splines[splines.Count - 1].end);
			splines[splines.Count - 1].cam[splines[splines.Count - 1].cam.Count - 1] = rpl.Item1;
			splines[splines.Count - 1].cam.Add(rpl.Item2);
		}
		return splines;
	}

	public static float GetGroundY(Vector3 pos)
	{
		RaycastHit hit;
		if (Physics.Raycast(pos + Vector3.up * 2000f, Vector3.down, out hit, 4000f, 1))
			return hit.point.y;
		return pos.y;
	}

	public static void ApplyCarMovement(Dictionary<string, Transform> players, Animation anim, float t, bool forceY, bool absoluteIndex)
	{
		string animtype = anim.name.Split('_').Last();
		string objname = anim.GetObjectName();
		if (animtype == "s")
			return;
		(float[], float[], float, bool) eval;
		if (absoluteIndex)
			eval = EvaluateAnimAbs(anim, t);
		else
			eval = EvaluateAnim(anim, t);
		if (eval.Item1 == null) return;
		if (!players.ContainsKey(objname))
			return;
		Transform target = players[objname];
		switch (animtype)
		{
			case "t":
				Vector3 target_pos = Vector3.Lerp(new Vector3(eval.Item1[0], eval.Item1[2], eval.Item1[1]), new Vector3(eval.Item2[0], eval.Item2[2], eval.Item2[1]), eval.Item3);
				Vector3 targetposition = new Vector3(target_pos.x, forceY ? GetGroundY(target_pos) : target_pos.y, target_pos.z);
				target.position = targetposition;
				break;
			case "q":
				Vector3 rot = Quaternion.Lerp(new Quaternion(eval.Item1[0], eval.Item1[1], eval.Item1[2], eval.Item1[3]), new Quaternion(eval.Item2[0], eval.Item2[1], eval.Item2[2], eval.Item2[3]), eval.Item3).eulerAngles;
				target.eulerAngles = new Vector3(-rot.y, 90f - rot.z, rot.x);
				break;
		}
	}

	public static NisScene SceneInfo;
	public static string SceneDescription;
	public static int DescriptionOffset;

	public enum SceneType
	{
		NIS_SCENE_INTRO,
		NIS_SCENE_END,
		NIS_SCENE_ARREST,
		NIS_SCENE_GENERIC,
		NIS_SCENE_LOCATION_SPECIFIC,
		NIS_ANIMATED_DRIVER
	}

	public enum AnimType
	{
		ANIM_RAWPOSE, // FnRawPoseChannel
		ANIM_RAWEVENT, // FnRawEventChannel
		ANIM_RAWLINEAR, // FnRawLinearChannel
		ANIM_CYCLE, // FnCycle
		ANIM_EVENTBLENDER, // FnEventBlender
		ANIM_GRAFT, // FnGraft
		ANIM_POSEBLENDER, // FnPoseBlender
		ANIM_POSEMIRROR, // FnPoseMirror
		ANIM_RUNBLENDER, // FnRunBlender
		ANIM_TURNBLENDER, // FnTurnBlender
		ANIM_DELTALERP, // FnDeltaLerpChan
		ANIM_DELTAQUAT, // FnDeltaQuatChan
		ANIM_KEYLERP, // FnKeyLerpChan
		ANIM_KEYQUAT, // FnKeyQuatChan
		ANIM_PHASE, // FnPhaseChan
		ANIM_COMPOUND, // FnCompoundChannel
		ANIM_RAWSTATE, // FnRawStateChan
		ANIM_DELTAQ, // FnDeltaQ
		ANIM_DELTAQFAST, // FnDeltaQFast
		ANIM_DELTASINGLEQ, // FnDeltaSingleQ
		ANIM_DELTAF3, // FnDeltaF3
		ANIM_DELTAF1, // FnDeltaF1
		ANIM_STATELESSQ, // FnStatelessQ
		ANIM_STATELESSF3, // FnStatelessF3
		ANIM_CSISEVENT, // FnCsisEventChannel
		ANIM_POSEANIM, // FnPoseAnim
		ANIM_MAX = 0xff
	}

	public class Animation
	{
		public string name;
		public AnimType type;
		public ushort checkSum;
		public int offset;
		public List<Animation> subAnimations = new List<Animation>();
		public float[][] delta;
		public ushort numDofs;
		public ushort quantBits;
		public float[][] header;
		public ushort unk1;
		public ushort unk2;
		public ushort unk3;

		public string GetObjectName()
		{
			string result = name;
			if (result.EndsWith("_s") || result.EndsWith("_q") || result.EndsWith("_t"))
				result = String.Join("_", result.Split('_').Take(result.Split('_').Length - 1).ToArray());
			if (name.StartsWith(SceneInfo.SceneName) || name.StartsWith(SceneInfo.SceneName.Substring(0, SceneInfo.SceneName.Length / 2)))
				result = String.Join("_", result.Split('_').Skip(1).ToArray());
			if (name.StartsWith("ZPM_"))
				result = String.Join("_", result.Split('_').Skip(1).ToArray());
			return result;
		}
	}

	public class Skeleton
	{
		public string name;
		public Bone[] bones;
		public Mesh attachedMesh;
		public Material[] attachedMaterials;
		public string animationName;
	}

	public class Bone
	{
		public string name;
		public int parent;
		public Vector4 position;
		public Quaternion rotation;
		public Vector3 scale;
		public Matrix4x4 inverseMatrix;
		public Transform assignedTransform;
	}

	public static void GenerateSkinnedObject(Transform target, Skeleton skeleton)
	{
        SkinnedMeshRenderer rend = target.gameObject.AddComponent<SkinnedMeshRenderer>();
        rend.sharedMaterials = skeleton.attachedMaterials;
        Dictionary<string, Transform> boneTransforms = new Dictionary<string, Transform>();
        rend.quality = SkinQuality.Bone1;
        rend.updateWhenOffscreen = true;
        rend.skinnedMotionVectors = true;
        rend.localBounds = new Bounds(new Vector3(0f, 0f, 1f), new Vector3(0.5f, 0.5f, 1f));
        Transform[] bones = new Transform[skeleton.bones.Length];
        for (int boneNum = 0; boneNum < skeleton.bones.Length; boneNum++)
        {
            if (!boneTransforms.ContainsKey(skeleton.bones[boneNum].name))
                boneTransforms.Add(skeleton.bones[boneNum].name, new GameObject(skeleton.bones[boneNum].name).transform);
            Transform trans = boneTransforms[skeleton.bones[boneNum].name];
            if (skeleton.bones[boneNum].parent != -1)
            {
                if (boneTransforms.ContainsKey(skeleton.bones[skeleton.bones[boneNum].parent].name))
                    trans.SetParent(boneTransforms[skeleton.bones[skeleton.bones[boneNum].parent].name]);
                else
                {
                    boneTransforms.Add(skeleton.bones[skeleton.bones[boneNum].parent].name, new GameObject(skeleton.bones[skeleton.bones[boneNum].parent].name).transform);
                    trans.SetParent(boneTransforms[skeleton.bones[skeleton.bones[boneNum].parent].name]);
                }
            }
            else
            {
                trans.SetParent(target);
            }
            trans.localPosition = skeleton.bones[boneNum].position;
            trans.localRotation = skeleton.bones[boneNum].rotation;
            trans.localScale = skeleton.bones[boneNum].scale;
            bones[boneNum] = trans;
            skeleton.bones[boneNum].assignedTransform = trans;
            int spind = trans.name.IndexOf(' ');
            if (spind >= 0)
				trans.name = trans.name.Substring(spind + 1);
        }
        rend.bones = bones;
        // guessing weights for now
        // todo
        if (CurrentGame == GameDetector.Game.MostWanted)
        {
	        BoneWeight[] weights = new BoneWeight[skeleton.attachedMesh.vertexCount];
	        for (int vertexNum = 0; vertexNum < skeleton.attachedMesh.vertexCount; vertexNum++)
	        {
		        int closestBone = 0;
		        float dist = (boneTransforms[skeleton.bones[0].name].position - skeleton.attachedMesh.vertices[vertexNum]).sqrMagnitude;
		        float newDist;
		        for (int boneNum = 0; boneNum < skeleton.bones.Length; boneNum++)
		        {
			        newDist = (boneTransforms[skeleton.bones[boneNum].name].position - skeleton.attachedMesh.vertices[vertexNum]).sqrMagnitude;
			        if (newDist < dist)
			        {
				        dist = newDist;
				        closestBone = boneNum;
			        }
		        }

		        BoneWeight weight = new BoneWeight();
		        weight.weight0 = 1f;
		        weight.boneIndex0 = closestBone;
		        weights[vertexNum] = weight;
	        }
	        skeleton.attachedMesh.boneWeights = weights;
        }
        Matrix4x4[] bindposes = new Matrix4x4[skeleton.bones.Length];
        for (int boneNum = 0; boneNum < skeleton.bones.Length; boneNum++)
            bindposes[boneNum] = boneTransforms[skeleton.bones[boneNum].name].worldToLocalMatrix;
        skeleton.attachedMesh.bindposes = bindposes;
        rend.sharedMesh = skeleton.attachedMesh;
        target.eulerAngles = new Vector3(-90f, 0f, 0f);
	}

	public static GameDetector.Game CurrentGame = GameDetector.Game.Carbon;

	public static (List<Animation>, List<Skeleton>) LoadAnimations(string nisname, string gamepath)
	{
		SceneInfo = new NisScene();
		SceneInfo.SceneName = nisname;
		SceneDescription = "";
		DescriptionOffset = 0;
		List<Animation> animations = new List<Animation>();
		List<Skeleton> skeletons = new List<Skeleton>();
		byte[] bytes;
		string path = "/NIS/Scene_" + nisname + "_BundleB.bun";
		if (File.Exists(gamepath + path))
			bytes = File.ReadAllBytes(gamepath + path);
		else if (File.Exists(gamepath + path.ToUpper()))
			bytes = File.ReadAllBytes(gamepath + path.ToUpper());
		else
			return (new List<Animation>(), new List<Skeleton>());
		Main.f_orig = bytes;
		bool modeldata;
		byte[] data;
		int offset;
		Dictionary<int, string> bonenames = null;
		Skeleton skeleton = null;
		string bonename;
		List<(Mesh, Material[])> models = new List<(Mesh, Material[])>();
		try
		{
			using (MemoryStream stream = new MemoryStream(bytes))
			{
				Dictionary<uint, Material> materialcache = new Dictionary<uint, Material>();
				Shader sh = Shader.Find("Standard");
				ChunkManager chunkManager = new ChunkManager(CurrentGame);
				chunkManager.Open(stream, 0, bytes.Length);
				List<ChunkManager.Chunk> chunks = chunkManager.Chunks;
				Dictionary<uint, Common.Textures.Data.Texture> textures = new Dictionary<uint, Common.Textures.Data.Texture>();
				foreach (var chunk in chunks)
				{
					if (chunk.Resource is TexturePack tpk)
					{
						foreach (var texture in tpk.Textures)
						{
							if (textures.ContainsKey(texture.TexHash))
								continue;
							textures.Add(texture.TexHash, texture);
						}
					}
				}

				foreach (var chunk in chunks)
				{
					if (chunk.Resource is SolidList solidList)
					{
						foreach (var solidObject in solidList.Objects)
						{
							Material[] materials = new Material[solidObject.Materials.Count];
							for (int i = 0; i < materials.Length; i++)
							{
								string n;
								switch (CurrentGame)
								{
									case GameDetector.Game.MostWanted:
										MostWantedMaterial mat = (MostWantedMaterial) solidObject.Materials[i];
										n = mat.Name.Replace("<", "").Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "");
										n += "_" + solidObject.TextureHashes[mat.TextureIndices[0]];
										if (materialcache.ContainsKey(solidObject.TextureHashes[mat.TextureIndices[0]]))
											materials[i] = materialcache[solidObject.TextureHashes[mat.TextureIndices[0]]];
										else
										{
											materials[i] = new Material(sh);
											materials[i].name = n;
											uint h = solidObject.TextureHashes[mat.TextureIndices[0]];
											if (textures.ContainsKey(h))
											{
												TextureFormat format = TextureFormat.ARGB32;
												byte[] im = textures[h].GenerateImage();
												switch (textures[h].CompressionType)
												{
													case TextureCompression.Dxt1:
														format = TextureFormat.DXT1;
														break;
													case TextureCompression.Dxt5:
														format = TextureFormat.DXT5;
														break;
													case TextureCompression.P8:
														continue; // todo
													default:
														Debug.LogError("Unsupported texture format " + textures[h].CompressionType);
														continue;
												}

												try
												{
													Texture2D text = new Texture2D((int) textures[h].Width, (int) textures[h].Height, format, false);
													text.LoadRawTextureData(im);
													text.Apply(false);
													materials[i].mainTexture = text;
													materials[i].mainTextureScale = new Vector2(1f, -1f);
												} catch { }
											}

											materialcache.Add(solidObject.TextureHashes[mat.TextureIndices[0]], materials[i]);
										}
										break;
									case GameDetector.Game.Carbon:
										CarbonMaterial cmat = (CarbonMaterial) solidObject.Materials[i];
										n = cmat.Name.Replace("<", "").Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "");
										materials[i] = new Material(sh);
										materials[i].name = n;
										break;
								}
							}

							Mesh mesh = new Mesh();
							mesh.name = solidObject.Name;
							List<Vector3> vertices = new List<Vector3>(solidObject.Vertices.Length);
							List<Color32> colors32 = new List<Color32>(solidObject.Vertices.Length);
							List<Vector2> uv = new List<Vector2>(solidObject.Vertices.Length);
							List<Vector3> normals = new List<Vector3>(solidObject.Vertices.Length);
							for (int i = 0; i < solidObject.Vertices.Length; i++)
							{
								vertices.Add(new Vector3(solidObject.Vertices[i].X, solidObject.Vertices[i].Y, solidObject.Vertices[i].Z));
								byte[] b = BitConverter.GetBytes(solidObject.Vertices[i].Color);
								colors32.Add(new Color32(b[0], b[1], b[2], b[3]));
								uv.Add(new Vector2(solidObject.Vertices[i].U, solidObject.Vertices[i].V));
								normals.Add(new Vector3(solidObject.Vertices[i].NormalX, solidObject.Vertices[i].NormalY, solidObject.Vertices[i].NormalZ));
							}

							mesh.SetVertices(vertices);
							mesh.SetColors(colors32);
							mesh.SetUVs(0, uv);
							mesh.SetNormals(normals);
							mesh.subMeshCount = materials.Length;
							for (int i = 0; i < materials.Length; i++)
							{
								var faces = solidObject.Faces.Where(f => f.MaterialIndex == i).ToList();
								List<int> tris = new List<int>(faces.Count * 3);
								foreach (var face in faces)
								{
									tris.Add(face.Vtx1);
									tris.Add(face.Vtx3);
									tris.Add(face.Vtx2);
								}

								mesh.SetTriangles(tris, i);
							}

							mesh.RecalculateTangents();
							mesh.RecalculateBounds();
							models.Add((mesh, materials));
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			Debug.Log(e);
		}

		int size;
		int start;
		toollibver = "";
		for (int i = 0; i < bytes.Length; i += 4)
		{
			if (bytes[i] == 0x11 && bytes[i + 1] == 0xB8 && bytes[i + 2] == 0x03 && bytes[i + 3] == 0x00)
			{
				i += 4;
				size = BitConverter.ToInt32(bytes, i);
				i += 4;
				start = i;
				while (bytes[i] == 0x11 && bytes[i + 1] == 0x11 && bytes[i + 2] == 0x11 && bytes[i + 3] == 0x11)
					i += 4;

				int some_length = i + BitConverter.ToInt32(bytes, i) + 16;
				
				i += 16;
				if (bytes[i] == 0x50 && bytes[i + 1] == 0x52 && bytes[i + 2] == 0x41 && bytes[i + 3] == 0x43)
				{
					i += 4;
					int sym_count = BitConverter.ToInt32(bytes, i);
					i += 4;
					int op_count = BitConverter.ToInt32(bytes, i);
					i += 4;
					int prac_unk3 = BitConverter.ToInt32(bytes, i);
					i += 4;

					for (int num = 0; num < op_count; num++)
					{
						//int hash = BitConverter.ToInt32(bytes, i);
						i += 4;
						//int unk1 = BitConverter.ToInt32(bytes, i);
						i += 4;
						//int unk2 = BitConverter.ToInt32(bytes, i);
						i += 4;
						//int unk3 = BitConverter.ToInt32(bytes, i);
						i += 4;
					}

					while (bytes[i] == 0xAA && bytes[i + 1] == 0xAA && bytes[i + 2] == 0xAA && bytes[i + 3] == 0xAA)
						i += 4;

					string[] syms = new string[sym_count];
					for (int num = 0; num < sym_count; num++)
					{
						string str = TakeString(bytes, i);
						i += str.Length + 1;
						syms[num] = str;
					}

					while (i % 4 != 0)
						i++;
					while (bytes[i] == 0xAA && bytes[i + 1] == 0xAA && bytes[i + 2] == 0xAA && bytes[i + 3] == 0xAA)
						i += 4;

					

				} else
					Debug.LogError("PRAC chunk is missing");

				i = start + size;
				while (i % 4 != 0)
					i--;
				i -= 4;
				continue;
			}
			if (bytes[i] == 0x20 && bytes[i + 1] == 0x70 && bytes[i + 2] == 0x03 && bytes[i + 3] == 0x80)
			{
				i += 4;
				// unk
				i += 4;
				// unk
				i += 4;
				// unk
				i += 4;
				SceneInfo = (NisScene) CoordDebug.RawDeserialize(bytes, i, typeof(NisScene));
				i += Marshal.SizeOf(typeof(NisScene));
				if (bytes[i] == 0x00)
					i += 4;
				DescriptionOffset = i;
				SceneDescription = TakeString(bytes, i);
				i += SceneDescription.Length + 1;
				while (i % 4 != 0)
					i--;
				i -= 4;
				continue;
			}
			if (bytes[i] == 0x40 && bytes[i + 1] == 0x70 && bytes[i + 2] == 0x03 && bytes[i + 3] == 0x00)
			{
				if (string.IsNullOrEmpty(SceneDescription) || SceneDescription.Length <= 4)
				{
					DescriptionOffset = 0; // playing safe here
					SceneDescription = TakeString(bytes, i - 5 * 4);
				}
				//i += 4;
				//size = BitConverter.ToInt32(bytes, i);
				// todo
				continue;
			}
			if ((bytes[i] == 0x09 || bytes[i] == 0x10) && bytes[i + 1] == 0x40 && bytes[i + 2] == 0xE3 && bytes[i + 3] == 0x00)
			{
				//Debug.Log("chunk found");
				modeldata = bytes[i] == 0x09;
				if (modeldata)
				{
					bonenames = new Dictionary<int, string>();
					skeleton = null;
				}
				i += 4;
				if (!modeldata)
					Main.ELFChunkSize = i;
				size = BitConverter.ToInt32(bytes, i);
				i += 4;
				start = i;
				while (bytes[i] == 0x11 && bytes[i + 1] == 0x11 && bytes[i + 2] == 0x11 && bytes[i + 3] == 0x11)
					i += 4;
				if (!modeldata)
					Main.ELFChunkStart = i;
				using (Stream stream = new MemoryStream(bytes, i, size - (i - start)))
				{
					IELF reader = ELFReader.Load(stream, true);
					//Debug.Log("loaded elf");
					foreach (ISymbolEntry entry in reader.GetSymTable().Entries)
					{
						//Debug.Log(entry.Name);
						//if (entry.PointedSection != null)
						//	Debug.Log(entry.Name + " " + Convert.ToInt32(entry.Value));
						if (entry.Name.StartsWith("__EAGL_TOOLLIB_VERSION:::"))
						{
							/*if (entry.PointedSection != null)
							{
								data = reader.GetSection(entry.PointedSection.Name).GetContents();
								offset = Convert.ToInt32(entry.Value);
								Debug.Log(BitConverter.ToString(data, offset, 8));
							}*/
							string newver = entry.Name.Split(new[] {":::"}, StringSplitOptions.RemoveEmptyEntries).Last().Split('-').Last();
							if (!toollibver.Contains(newver))
							{
								if (toollibver != "")
									toollibver += " / ";
								toollibver += newver;
							}
							continue;
						}
						if (entry.PointedSection == null)
							continue;
						data = reader.GetSection(entry.PointedSection.Name).GetContents();
						//File.WriteAllBytes("/Users/henrytownsend/Desktop/data", data);
						offset = Convert.ToInt32(entry.Value);
						if (modeldata)
						{
							if (entry.Name.StartsWith("__Skeleton:::"))
							{
								skeleton = new Skeleton();
								skeleton.name = entry.Name.Split(new[] {":::"}, StringSplitOptions.RemoveEmptyEntries).Last();
								//checkSum = BitConverter.ToUInt16(data, offset);
								offset += 2;
								// unk
								offset += 2;
								// unk
								offset += 2;
								// unk
								offset += 2;
								int numOfBones = Convert.ToInt32(BitConverter.ToUInt32(data, offset));
								skeleton.bones = new Bone[numOfBones];
								offset += 4;
								// unk
								offset += 4;
								float boneSX, boneSZ, boneSY;
								float boneQX, boneQY, boneQZ, boneQW;
								float bonePX, bonePZ, bonePY, bonePW;
								for (int boneNum = 0; boneNum < numOfBones; boneNum++)
								{
									skeleton.bones[boneNum] = new Bone();
									if (bonenames.ContainsKey(boneNum))
										skeleton.bones[boneNum].name = bonenames[boneNum];
									boneSX = BitConverter.ToSingle(data, offset);
									offset += 4;
									boneSY = BitConverter.ToSingle(data, offset);
									offset += 4;
									boneSZ = BitConverter.ToSingle(data, offset);
									offset += 4;
									skeleton.bones[boneNum].scale = new Vector3(boneSX, boneSZ, boneSY);
									skeleton.bones[boneNum].parent = BitConverter.ToInt32(data, offset);
									offset += 4;
									boneQX = BitConverter.ToSingle(data, offset);
									offset += 4;
									boneQY = BitConverter.ToSingle(data, offset);
									offset += 4;
									boneQZ = BitConverter.ToSingle(data, offset);
									offset += 4;
									boneQW = BitConverter.ToSingle(data, offset);
									offset += 4;
									skeleton.bones[boneNum].rotation = new Quaternion(boneQX, boneQY, boneQZ, boneQW);
									bonePX = BitConverter.ToSingle(data, offset);
									offset += 4;
									bonePY = BitConverter.ToSingle(data, offset);
									offset += 4;
									bonePZ = BitConverter.ToSingle(data, offset);
									offset += 4;
									bonePW = BitConverter.ToSingle(data, offset);
									skeleton.bones[boneNum].position = new Vector4(bonePX, bonePY, bonePZ, bonePW);
									offset += 4;
									skeleton.bones[boneNum].inverseMatrix = new Matrix4x4();
									for (int x = 0; x < 4; x++)
										for (int y = 0; y < 4; y++)
										{
											skeleton.bones[boneNum].inverseMatrix[x, y] = BitConverter.ToSingle(data, offset);
											offset += 4;
										}
								}
								skeletons.Add(skeleton);
							} else if (entry.Name.StartsWith("__Bone:::"))
							{
								int boneindex = Convert.ToInt32(BitConverter.ToUInt32(data, offset));
								//offset += 4;
								//offset += 4;
								//offset += 4;
								bonename = string.Join(" ", entry.Name.Split(new[] {":::"}, StringSplitOptions.RemoveEmptyEntries).Last());
								if (skeleton == null)
									bonenames.Add(boneindex, bonename);
								else
									skeleton.bones[boneindex].name = bonename;
							}
						}
						else
						{
							if (entry.Name == "__AnimationBank:::bank")
							{
								// unk
								offset += 4;
								int AnimationCount = Convert.ToInt32(BitConverter.ToUInt32(data, offset));
								offset += 4 * 3;
								int AnimationsOffset = Convert.ToInt32(BitConverter.ToUInt32(data, offset));
								offset += 4;
								int AnimationNames = Convert.ToInt32(BitConverter.ToUInt32(data, offset));
								// 4 * 3
								List<int> offset_ignore_list = new List<int>();
								for (int animNum = 0; animNum < AnimationCount; animNum++)
								{
									offset = Convert.ToInt32(BitConverter.ToUInt32(data, AnimationsOffset + animNum * 4));
									Animation anim = new Animation();
									anim.name = TakeString(data, Convert.ToInt32(BitConverter.ToUInt32(data, AnimationNames + animNum * 4)));
									if (offset >= data.Length - 10)
									{
										break;
									}
									if (offset_ignore_list.Contains(offset))
									{
										continue;
									}
									anim.offset = offset;
									ushort tt = BitConverter.ToUInt16(data, offset);
									offset += 2;
									anim.checkSum = BitConverter.ToUInt16(data, offset);
									offset += 2;
									anim.type = (AnimType) tt;
									ParseAnimation(data, anim, animations, offset_ignore_list, ref offset);
									animations.Add(anim);
									if (anim.type != AnimType.ANIM_COMPOUND)
									{
										animNum--;
										anim.name = "";
									}
								}
							}
						}
					}
				}
				i = start + size;
				while (i % 4 != 0)
					i--;
				i -= 4;
			}
		}
		for (int i = 0; i < skeletons.Count; i++)
		{
			if (i >= models.Count)
				break;
			skeletons[i].attachedMesh = models[i].Item1;
			skeletons[i].attachedMaterials = models[i].Item2;
			skeletons[i].animationName = skeletons[i].attachedMesh.name;
		}
		return (animations, skeletons);
	}

	public static string toollibver = "";

	static void ParseAnimationNode(byte[] data, Animation anim, int nodeOffset, int amount, bool reversed_order = false)
	{
		ushort a = BitConverter.ToUInt16(data, nodeOffset);
		nodeOffset += 2;
		ushort b = BitConverter.ToUInt16(data, nodeOffset);
		nodeOffset += 2;
		ushort numDofs = reversed_order ? b : a;
		ushort numQuantBits = reversed_order ? a : b;
		anim.numDofs = numDofs;
		anim.quantBits = numQuantBits;
		if (numQuantBits != 0x10 && numQuantBits != 8)
			Debug.LogError("unsupported amount of quant bits: " + numQuantBits);
		float[][] header = new float[numDofs][];
		for (int x = 0; x < numDofs; x++)
		{
			header[x] = new float[3];
			header[x][0] = BitConverter.ToSingle(data, nodeOffset);
			nodeOffset += 4;
			header[x][1] = BitConverter.ToSingle(data, nodeOffset);
			nodeOffset += 4;
			header[x][2] = BitConverter.ToSingle(data, nodeOffset);
			nodeOffset += 4;
		}
		anim.header = header;
		float[][] result = new float[amount][];
		float delta = 0f;
		for (int i = 0; i < amount; i++)
		{
			result[i] = new float[numDofs];
			for (int x = 0; x < numDofs; x++)
			{
				if (i == 0)
				{
					result[i][x] = header[x][2];
					continue;
				}
				switch (numQuantBits)
				{
					case 8:
						delta = data[nodeOffset++];
						break;
					case 0x10:
						delta = BitConverter.ToUInt16(data, nodeOffset);
						nodeOffset += 2;
						break;
				}
				result[i][x] = result[i - 1][x] + delta * header[x][1] + header[x][0];
			}
		}
		anim.delta = result;
		//Debug.Log(numDofs + " " + BitConverter.ToString(data, nodeOffset, 16));
	}

	static void ParseAnimation(byte[] data, Animation anim, List<Animation> animations, List<int> offset_ignore_list, ref int offset)
	{
		switch (anim.type)
		{
			case AnimType.ANIM_DELTALERP:
				ParseAnimationNode(data, anim, Convert.ToInt32(BitConverter.ToUInt32(data, offset)), BitConverter.ToUInt16(data, offset + 4));
				offset += 4;
				// count
				offset += 2;
				//Debug.Log("ANIM_DELTALERP 2: " + Convert.ToInt32(BitConverter.ToUInt16(data, offset))); // 12 20 12 20
				anim.unk1 = BitConverter.ToUInt16(data, offset);
				offset += 2;
				//Debug.Log("ANIM_DELTALERP 3: " + Convert.ToInt32(BitConverter.ToUInt16(data, offset))); // 13 21 13 21
				anim.unk2 = BitConverter.ToUInt16(data, offset);
				offset += 2;
				//Debug.Log("ANIM_DELTALERP 4: " + Convert.ToInt32(BitConverter.ToUInt16(data, offset))); // 14 22 14 22
				anim.unk3 = BitConverter.ToUInt16(data, offset);
				offset += 2;
				break;
			case AnimType.ANIM_DELTAQUAT:
				ParseAnimationNode(data, anim, Convert.ToInt32(BitConverter.ToUInt32(data, offset)), BitConverter.ToUInt16(data, offset + 4));
				offset += 4;
				// count
				offset += 2;
				//Debug.Log("ANIM_DELTAQUAT 2: " + Convert.ToInt32(BitConverter.ToUInt16(data, offset))); // 16 16 16 16 4
				anim.unk1 = BitConverter.ToUInt16(data, offset);
				offset += 2;
				break;
			case AnimType.ANIM_COMPOUND:
				//Debug.Log("ANIM_COMPOUND " + BitConverter.ToString(data, Convert.ToInt32(BitConverter.ToUInt32(data, offset)), 16));
				// offset for something here
				offset += 4;
				int childCount = Convert.ToInt32(BitConverter.ToUInt16(data, offset));
				offset += 2;
				// child delta count
				anim.unk1 = BitConverter.ToUInt16(data, offset);
				offset += 2;
				for (int childNum = 0; childNum < childCount; childNum++)
				{
					int childOffset = Convert.ToInt32(BitConverter.ToUInt32(data, offset));
					bool found = false;
					for (int i = 0; i < animations.Count; i++)
						if (animations[i].offset == childOffset)
						{
							anim.subAnimations.Add(animations[i]);
							animations.RemoveAt(i);
							found = true;
							break;
						}

					if (!found)
					{
						Animation childAnim = new Animation();
						childAnim.offset = childOffset;
						childAnim.type = (AnimType) BitConverter.ToUInt16(data, childOffset);
						childOffset += 2;
						childAnim.checkSum = BitConverter.ToUInt16(data, childOffset);
						childOffset += 2;
						ParseAnimation(data, childAnim, animations, offset_ignore_list, ref childOffset);
						anim.subAnimations.Add(childAnim);
						offset_ignore_list.Add(childAnim.offset);
					}

					offset += 4;
				}
				break;
			case AnimType.ANIM_DELTAF3:
			case AnimType.ANIM_DELTAF1:
				// nums offset
				offset += 4;
				// 0
				offset += 4;
				//anim.delta = new float[BitConverter.ToUInt16(data, offset)][];
				//anim.delta = new float[1][]; // todo
				offset += 2;
				// 5 1
				offset += 2;
				//int dofs = anim.type == AnimType.ANIM_DELTAF3 ? 3 : 1;
				/*for (int i = 0; i < anim.delta.Length; i++)
				{
					anim.delta[i] = new float[dofs];
					for (int x = 0; x < dofs; x++)
					{
						anim.delta[i][x] = BitConverter.ToSingle(data, offset);
						offset += 4;
					}
				}*/
				Debug.LogError(anim.type + " is not implemented");
				break;
			default:
				Debug.LogError(anim.type + " is not implemented");
				break;
		}
	}

	public static string TakeString(byte[] arr, int offset)
	{
		string output = "";
		char ch;
		for (int i = offset; i < arr.Length; i++)
		{
			ch = (char) arr[i];
			if (ch != '\0')
				output += ch;
			else
				return output;
		}

		throw new Exception("TakeString() reached end of the file?");
	}

	public static byte[] DecompressJZC(byte[] input)
	{
		if (input.Length > 4 && input[0] == 0x4A && input[1] == 0x44 && input[2] == 0x4C && input[3] == 0x5A)
		{
			try
			{
				return DecompressJDLZ(input, 0, BitConverter.ToInt32(new[] {input[12], input[13], input[14], input[15]}, 0));
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
		return input;
	}
		
	static byte[] DecompressJDLZ(byte[] input, int start, int __length) {
		int flags1 = 1, flags2 = 1;
		int t, length;
		int inPos = start + 16, outPos = 0;
		byte[] _length;
		_length = new[] { input[start + 8], input[start + 9], input[start + 10], input[start + 11] };
		byte[] output = new byte[BitConverter.ToInt32(_length, 0)];
		while ((inPos < start + __length) && (outPos < output.Length)) {
			if (flags1 == 1)
				flags1 = input[inPos++] | 0x100;
			if (flags2 == 1)
				flags2 = input[inPos++] | 0x100;
			if ((flags1 & 1) == 1) {
				if ((flags2 & 1) == 1) {
					length = (input[inPos + 1] | ((input[inPos] & 0xF0) << 4)) + 3;
					t = (input[inPos] & 0x0F ) + 1;
				} else {
					t = (input[inPos + 1] | ((input[inPos] & 0xE0) << 3)) + 17;
					length = (input[inPos] & 0x1F) + 3;
				}
				inPos += 2;
				for (int i = 0; i < length; ++i)
					output[outPos + i] = output[outPos + i - t];
				outPos += length;
				flags2 >>= 1;
			} else if (outPos < output.Length)
				output[outPos++] = input[inPos++];
			flags1 >>= 1;
		}
		return output;
	}

	public static (int, List<(CameraTrackHeader, CameraTrackEntry[])>) LoadCameraTrack(string nisname, string gamepath, bool fullpath = false)
	{
		return LookupCamera(gamepath, BinHash(nisname), fullpath);
	}

	public static (int, List<(CameraTrackHeader, CameraTrackEntry[])>) LookupCamera(string gamepath, uint NISHashNeeded, bool fullpath = false)
	{
		byte[] f;
        if (fullpath)
            f = File.ReadAllBytes(gamepath);
        else
        {
            string path = "/GLOBAL/InGameB.lzc";
            if (File.Exists(gamepath + path))
                f = File.ReadAllBytes(gamepath + path);
            else if (File.Exists(gamepath + path.ToUpper()))
                f = File.ReadAllBytes(gamepath + path.ToUpper());
            else
                return (0, new List<(CameraTrackHeader, CameraTrackEntry[])>());
        }
		f = DecompressJZC(f);
		int offset = 0;
		int CameraTrackHeader_size = Marshal.SizeOf(typeof(CameraTrackHeader));
		int idd = 0;
		List<(CameraTrackHeader, CameraTrackEntry[])> output = new List<(CameraTrackHeader, CameraTrackEntry[])>();
		int _offset = 0;
        int size = 0;
        SizeOffset = 0;
		try
		{
			for (int i = 0; i < f.Length; i += 4)
			{
                if (f[i] == 0x00 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x80)
                {
                    i += 4;
                    if (!fullpath)
                        SizeOffset = i;
                }
                if (f[i] == 0x01 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x80)
                {
	                i += 4;
	                if (!fullpath)
		                SizeOffset = i;
                }
                if (f[i] == 0x02 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x80)
                {
	                i += 4;
	                if (!fullpath)
		                SizeOffset = i;
                }
                if (f[i] == 0x03 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x80)
                {
	                i += 4;
	                if (!fullpath)
		                SizeOffset = i;
                }
                if (f[i] == 0x10 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x00)
				{
					_offset = i;
					i += 4;
                    size = BitConverter.ToInt32(f, i);
                    int end = i + size;
					i += 4;
					idd++;
					uint NISHash = BitConverter.ToUInt32(f, i);
					if (NISHashNeeded != NISHash)
					{
						i = end - 4;
						while (i % 4 != 0)
							i--;
						continue;
					}

					i += 4;
					int count = BitConverter.ToInt32(f, i);
                    i += 4;
                    int gametype = 0;
					for (int num = 0; num < count; num++)
					{
						CameraTrackHeader header = (CameraTrackHeader) CoordDebug.RawDeserialize(f, i, typeof(CameraTrackHeader));
						if (header.Duration == 0f && header.DurationCarbon != 0f)
						{
							header.Duration = header.DurationCarbon;
							gametype = 1;
						}
						i += CameraTrackHeader_size;
						CameraTrackEntry[] entries = new CameraTrackEntry[header.entryCount];
						for (int entryNum = 0; entryNum < header.entryCount; entryNum++)
						{
							switch (gametype)
							{
								case 0:
									entries[entryNum] = new CameraTrackEntry((CameraTrackEntryMW) CoordDebug.RawDeserialize(f, i, typeof(CameraTrackEntryMW)));
									i += Marshal.SizeOf(typeof(CameraTrackEntryMW));
									break;
								default:
									throw new Exception("Camera animation format not supported");
							}
						}

						output.Add((header, entries));
					}

					break;
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
		if (output.Count == 0)
		{
			_offset = 0;
			SizeOffset = 0;
		}
		return (_offset, output);
	}

    public static int SizeOffset;

	public static uint BinHash(string k)
	{
		var hash = 0xFFFFFFFFu;
		var bytes = Encoding.ASCII.GetBytes(k);
		for (var i = 0; i < k.Length; i++)
			hash = bytes[i] + 33 * hash;
		return hash;
	}
}