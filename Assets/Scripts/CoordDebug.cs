
//#if UNITY_EDITOR
using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
//using System.Text;
//using System.Globalization;
//using System.Linq;
using UnityEditor;
using System.Runtime.InteropServices;

//using UnityEngine.Assertions;

[ExecuteInEditMode]
public class CoordDebug : MonoBehaviour
{
    public string path;
    public string path2;
    public bool coords_extract;
    public bool apply;
    public Transform UnsortedParent;
    public bool PutBackUnsorted;
    public bool LoadParticles;
    public Transform ParticleParent;
    public bool RemoveDuplicates;

    public bool RenameModels;

    public bool CalculateHashes;

    // 4D410300
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BarrierSpline
    {
        public float X1;
        public float Z1;
        public float X2;
        public float Z2;
        public byte BarrierEnabled;
        public byte padding;
        public byte PlayerBarrier;
        public byte LeftHanded;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] NameHash;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SceneryInstanceStruct
    {
        public float MinX;
        public float MinZ;
        public float MinY;
        public float MaxX;
        public float MaxZ;
        public float MaxY;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Huita;
        public float PosX;
        public float PosZ;
        public float PosY;
        public short Rot00;
        public short Rot01;
        public short Rot02;
        public short Rot10;
        public short Rot11;
        public short Rot12;
        public short Rot20;
        public short Rot21;
        public short Rot22;
        public ushort SceneryInfoNumber;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SceneryInfoStruct
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string Name;
        
        public uint NameHash0;
        public uint NameHash1;
        public uint NameHash2;
        public uint NameHash3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] ModelPtrs;

        public float Radius;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] Unknown1;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ModelInfoStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Unknown;
        
        public uint Hash;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 44)]
        public byte[] Unknown2;
        
        public float m0_0;
        public float m0_1;
        public float m0_2;
        public float m0_3;
        
        public float m1_0;
        public float m1_1;
        public float m1_2;
        public float m1_3;
        
        public float m2_0;
        public float m2_1;
        public float m2_2;
        public float m2_3;
        
        public float m3_0;
        public float m3_1;
        public float m3_2;
        public float m3_3;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Unknown3;
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ModelGroupStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] Unknown;
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 56)]
        public string Name;
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string Chunk;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BloomTrigger
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Unknown;
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Name;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Unknown2;
        
        public float PosX;
        public float PosZ;
        public float PosY;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] Params;
        
        public float Unknown3;
        public float Unknown4;
        
        public float BigNumber;
        
        public float Unknown5;
        public float Unknown6;
        public float Unknown7;
        
        public float One;
        
        public float Unknown8;
        public float Unknown9;
        public float Unknown10;
    }

    public class ModelSimplified
    {
        public ModelSimplified(ModelGroupStruct _group, ModelInfoStruct _info)
        {
            group = _group;
            info = _info;
        }

        public ModelGroupStruct group;
        public ModelInfoStruct info;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StreamingSection
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string SectionName; // 44363600 00000000
        public short SectionNumber; // D201
        public byte WasRendered; // 00
        public byte CurrentlyVisible; // 00 
        public int Status; // 00000000 
        public int FileType; // 01000000 
        public int FileOffset; // 00886C0C 
        public int Size; // 30720000 
        public int CompressedSize; // 30720000 
        public int PermSize; // 30720000 
        public int SectionPriority; // E2280000 
        public float CentreX; // A6450245 
        public float CentreZ; // 18E7D943 
        public float Radius; // 43442743 
        public uint Checksum; // 32E140A3 
        public int LastNeededTimestamp; // 00000000 
        public uint UnactivatedFrameCount; // 00000000 
        public int LoadedTime; // 00000000 
        public int BaseLoadingPriority; // 00000000 
        public int LoadingPriority; // 00000000 
        public int pMemory; // 00000000 
        public int pDiscBundle; // 00000000 
        public int LoadedSize; // 00000000 
        public int pBoundary; // 00000000
    }

    public static object RawDeserialize(byte[] rawData, int position, Type anyType)
    {
        int rawsize = Marshal.SizeOf(anyType);
        IntPtr buffer = Marshal.AllocHGlobal(rawsize);
        Marshal.Copy(rawData, position, buffer, rawsize);
        object retobj = Marshal.PtrToStructure(buffer, anyType);
        Marshal.FreeHGlobal(buffer);
        return retobj;
    }
    
    public static byte[] RawSerialize(object obj) {
        int size = Marshal.SizeOf(obj);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(obj, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        return arr;
    }

    byte[] bStringHash(string a1)
    {
        byte[] v1; //edx
        byte v2; // cl
        int result; //eax

        int pointer = 0;

        v1 = System.Text.Encoding.UTF8.GetBytes(a1 + "\0");
        v2 = v1[0];
        for (result = -1; pointer < v1.Length - 1; pointer++)
        {
            result = v2 + 33 * result;
            v2 = v1[pointer + 1];
        }
        return BitConverter.GetBytes(result);
    }

    public class StreamingSectionInfo
    {
        public List<SceneryInstanceStruct> instanceArray;
        public List<SceneryInfoStruct> infoArray;
        public List<int> actualIndex;
    }

    private List<ModelSimplified> modelArray;

    private List<StreamingSection> streamSections;
    private List<StreamingSectionInfo> streamSectionsInfo;

    public static void LoadModelData(Dictionary<uint, ModelSimplified> modelArray, BinaryReader f)
    {
        int s3 = Marshal.SizeOf(typeof(ModelInfoStruct));
        int s4 = Marshal.SizeOf(typeof(ModelGroupStruct));
        ModelGroupStruct group = new ModelGroupStruct();
        ModelInfoStruct modelInfo;
        byte[] bytes;
        while (f.BaseStream.Position < f.BaseStream.Length)
        {
            int b = f.ReadByte();
            int b2 = f.ReadByte();
            int b3 = f.ReadByte();
            int b4 = f.ReadByte();
            if (b == 0x01 && b2 == 0x40 && b3 == 0x13 && b4 == 0x80)
            {
                f.ReadInt32();
                bytes = f.ReadBytes(s4);
                group = (ModelGroupStruct)RawDeserialize(bytes, 0, typeof(ModelGroupStruct));
                while (f.BaseStream.Position % 4 != 0)
                    f.ReadByte();
            }
            else if (b == 0x11 && b2 == 0x40 && b3 == 0x13 && b4 == 0x00)
            {
                f.ReadInt32();
                do {
                    b = f.ReadByte();
                    b2 = f.ReadByte();
                    b3 = f.ReadByte();
                    b4 = f.ReadByte();
                } while (b == 0x11 && b2 == 0x11 && b3 == 0x11 && b4 == 0x11);
                f.BaseStream.Position -= 4;
                bytes = f.ReadBytes(s3);
                modelInfo = (ModelInfoStruct)RawDeserialize(bytes, 0, typeof(ModelInfoStruct));
                if (!modelArray.ContainsKey(modelInfo.Hash))
                    modelArray.Add(modelInfo.Hash, new ModelSimplified(group, modelInfo));
                while (f.BaseStream.Position % 4 != 0)
                    f.ReadByte();
            }
        }
    }

    public static void LoadData(List<StreamingSection> streamSections, List<StreamingSectionInfo> streamSectionsInfo, byte[] fSmol)
    {
        int sectionLength = Marshal.SizeOf(typeof(StreamingSection));
        StreamingSection curSection;
        StreamingSectionInfo curInfo;
        for (int j = 0; j < fSmol.Length; j += 4)
        {
            if (fSmol[j] == 0x10 && fSmol[j + 1] == 0x41 && fSmol[j + 2] == 0x03 && fSmol[j + 3] == 0x00)
            {
                j += 4;
                int metaEnd = j + BitConverter.ToInt32(fSmol, j);
                j += 4;
                while (j < metaEnd - 8)
                {
                    curSection = (StreamingSection)RawDeserialize(fSmol, j, typeof(StreamingSection));
                    curInfo = new StreamingSectionInfo();
                    streamSections.Add(curSection);
                    streamSectionsInfo.Add(curInfo);
                    j += sectionLength;
                }
                break;
            }
        }
    }

    public static void LoadSectionData(BinaryReader f, StreamingSection section, out List<SceneryInstanceStruct> instanceArray, out List<SceneryInfoStruct> infoArray, out List<int> actualIndex)
    {
        int end;
        instanceArray = new List<SceneryInstanceStruct>();
        infoArray = new List<SceneryInfoStruct>();
        actualIndex = new List<int>();
        int s1 = Marshal.SizeOf(typeof(SceneryInstanceStruct));
        int s2 = Marshal.SizeOf(typeof(SceneryInfoStruct));
        f.BaseStream.Seek(section.FileOffset, SeekOrigin.Begin);
        byte[] bytes;
        while (f.BaseStream.Position < section.FileOffset + section.Size)
        {
            int b = f.ReadByte();
            int b2 = f.ReadByte();
            int b3 = f.ReadByte();
            int b4 = f.ReadByte();
            if (b2 != 0x41 || b3 != 0x03 || b4 != 0x00)
            {
                continue;
            }
            switch (b)
            {
                case 0x03:
                    end = (int)f.BaseStream.Position + f.ReadInt32();
                    f.ReadInt32();
                    f.ReadInt32();
                    f.ReadInt32();
                    while (f.BaseStream.Position < end - 8)
                    {
                        bytes = f.ReadBytes(s1);
                        instanceArray.Add((SceneryInstanceStruct) RawDeserialize(bytes, 0, typeof(SceneryInstanceStruct)));
                        actualIndex.Add(infoArray.Count + instanceArray[instanceArray.Count - 1].SceneryInfoNumber);
                    }
                    while (f.BaseStream.Position % 4 != 0)
                        f.ReadByte();
                    break;
                case 0x02:
                    end = (int)f.BaseStream.Position + f.ReadInt32();
                    while (f.BaseStream.Position < end - 8)
                    {
                        bytes = f.ReadBytes(s2);
                        infoArray.Add((SceneryInfoStruct) RawDeserialize(bytes, 0, typeof(SceneryInfoStruct)));
                    }
                    while (f.BaseStream.Position % 4 != 0)
                        f.ReadByte();
                    break;
            }
        }
    }

    void Update() {
        /*if (CalculateHashes)
        {
            CalculateHashes = false;
            List<string> output = new List<string>();
            List<(string, int, int)> source = File.ReadAllLines("/Users/henrytownsend/Desktop/output.txt").Where(x => x.Length > 2).Select(x =>
            {
                string[] s = x.Replace("\0", "").Replace(".obj", "").Split(new [] {" "}, StringSplitOptions.RemoveEmptyEntries);
                if (s.Length == 4)
                {
                    s = new[] { s[0] + " " + s[1], s[2], s[3] };
                }
                try
                {
                    return (s[0], int.Parse(s[1]), int.Parse(s[2]));
                }
                catch
                {
                    throw new Exception(x);
                }
            }).ToList();
            foreach (var v in source)
                output.Add(BitConverter.ToInt32(NISLoader.BinHash(v.Item1.Split('-')[2]), 0) + " " + v.Item1.Split('-')[2]);
            File.WriteAllLines(Application.dataPath + "/Resources/Stream/hashes.txt", output);
        }
        if (RenameModels)
        {
            RenameModels = false;
#if UNITY_EDITOR
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            List<(string, int, int)> source = File.ReadAllLines("/Users/henrytownsend/Desktop/output.txt").Where(x => x.Length > 2).Select(x =>
            {
                string[] s = x.Replace("\0", "").Replace(".obj", "").Split(new [] {" "}, StringSplitOptions.RemoveEmptyEntries);
                if (s.Length == 4)
                {
                    s = new[] { s[0] + " " + s[1], s[2], s[3] };
                }
                try
                {
                    return (s[0], int.Parse(s[1]), int.Parse(s[2]));
                }
                catch
                {
                    throw new Exception(x);
                }
            }).Where(x => x.Item1.Length >= 63).ToList();
            
            string[] models = Directory.GetFiles(Application.dataPath + "/Resources/Stream/Geometry").Where(x => !x.EndsWith(".meta")).Select(x => Path.GetFileNameWithoutExtension(x)).Where(x => x.Length == 63 || x.Contains(".obj")).ToArray();
            foreach (string model in models)
            {
                if (!File.Exists(Application.dataPath + "/Resources/Stream/Geometry/" + model + ".prefab"))
                {
                    continue;
                }

                string effectiveName = model.Contains(".") ? model.Split('.')[0] : model;

                if (effectiveName.Length > 63)
                    effectiveName = effectiveName.Substring(0, 63);

                Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Models/Geometry/WorldStream/" + model + ".fbx", typeof(Mesh));
                if (m == null)
                {
                    m = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Resources/Stream/Collisions/Surfaces/" + model + ".fbx", typeof(Mesh));
                    if (m == null)
                        throw new Exception(model + " not found?");
                }
                Debug.Log("Looking up " + model + "...");
                for (int i = 0; i < source.Count; i++)
                {
                    if (source[i].Item1.StartsWith(effectiveName))
                    {
                        if (m.subMeshCount == source[i].Item2)
                        {
                            bool succeeded = true;
                            if (model != source[i].Item1)
                            {
                                Debug.Log(model + " -> " + source[i].Item1);
                                try
                                {
                                    File.Move(Application.dataPath + "/Resources/Stream/Geometry/" + model + ".prefab", Application.dataPath + "/Resources/Stream/Geometry/" + source[i].Item1 + ".prefab");
                                    File.Move(Application.dataPath + "/Resources/Stream/Geometry/" + model + ".prefab.meta", Application.dataPath + "/Resources/Stream/Geometry/" + source[i].Item1 + ".prefab.meta");
                                }
                                catch
                                {
                                    succeeded = false;
                                    Debug.Log("failed to rename " + model);
                                }
                            }
                            if (succeeded)
                            {
                                source.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
            AssetDatabase.Refresh();
            #endif
        }
        if (LoadParticles)
        {
            LoadParticles = false;
            
            HashManager.LoadDictionary(Application.persistentDataPath + "/VLT/hashes.txt");
            try {
                new ModuleDef().Load();
            } catch { }
            (Database db, Dictionary<string, IList<Vault>> fd) = VLT.LoadDatabase("attributes.bin", "fe_attrib.bin", "gameplay.bin");
            VltCollection[] root = db.RowManager.EnumerateCollections("emittergroup").ToArray();
            List<string> particleNames = new List<string>();
            ParticleExtraction(root, particleNames);
            
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            byte[] f = File.ReadAllBytes(path);
            int end;
            List<string> names = new List<string>();
            List<Vector3> positions = new List<Vector3>();
            List<Quaternion> rotations = new List<Quaternion>();
            for (int i = 0; i < f.Length; i += 4)
            {
                if (f[i] == 0x00 && f[i + 1] == 0xBC && f[i + 2] == 0x03 && f[i + 3] == 0x00)
                {
                    i += 4;
                    end = i + BitConverter.ToInt32(f, i);
                    i += 4;
                    while (f[i] == 0x11 && f[i + 1] == 0x11 && f[i + 2] == 0x11 && f[i + 3] == 0x11)
                        i += 4;
                    i += 4 * 4;
                    while (i < end - 8)
                    {
                        byte[] nHash = f.Skip(i).Take(4).ToArray();
                        int n = BitConverter.ToInt32(nHash, 0);
                        i += 4 * 4;
                        Matrix4x4 m = new Matrix4x4();

                        m[0, 0] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[1, 0] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[2, 0] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[3, 0] = BitConverter.ToSingle(f, i);
                        i += 4;

                        m[0, 1] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[1, 1] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[2, 1] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[3, 1] = BitConverter.ToSingle(f, i);
                        i += 4;

                        m[0, 2] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[1, 2] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[2, 2] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[3, 2] = BitConverter.ToSingle(f, i);
                        i += 4;

                        m[0, 3] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[1, 3] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[2, 3] = BitConverter.ToSingle(f, i);
                        i += 4;
                        m[3, 3] = BitConverter.ToSingle(f, i);
                        i += 4;

                        if (m[3, 3] != 1f)
                        {
                            i -= 4 * 4 * 4;
                            continue;
                        }

                        string finalN = BitConverter.ToString(nHash).Replace("-", "");

                        foreach (string particleName in particleNames)
                        {
                            int hash = BitConverter.ToInt32(NISLoader.BinHash(particleName), 0);
                            int hash2 = BitConverter.ToInt32(NISLoader.VltHash(particleName), 0);
                            int hash3 = BitConverter.ToInt32(NISLoader.BinHash(particleName).Reverse().ToArray(), 0);
                            int hash4 = BitConverter.ToInt32(NISLoader.VltHash(particleName).Reverse().ToArray(), 0);
                            if (n == hash || n == hash2 || n == hash3 || n == hash4)
                            {
                                finalN = particleName;
                                break;
                            }
                        }
                        
                        names.Add(finalN);
                        positions.Add(new Vector3(m[0, 3], m[2, 3], m[1, 3]));
                        rotations.Add(Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1)));
                    }
                    i -= 4;
                }
            }

            for (int i = 0; i < names.Count; i++)
            {
                GameObject obj = new GameObject(names[i]);
                obj.transform.SetParent(ParticleParent);
                obj.transform.position = positions[i];
                obj.transform.rotation = rotations[i];
            }
        }
        if (PutBackUnsorted)
        {
            PutBackUnsorted = false;
            List<Transform> tr = new List<Transform>();
            foreach (Transform child in UnsortedParent)
                tr.Add(child);
            foreach (Transform trrr in tr)
                trrr.SetParent(transform.Find(trrr.name.Split('-')[0]));
        }
        /*if (coords_extract)
        {
            coords_extract = false;
            Debug.Log("Init....");
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            byte[] f = File.ReadAllBytes(path);
            byte[] fSmol = File.ReadAllBytes(path2);
            Debug.Log("Extracting data...");
            streamSections = new List<StreamingSection>();
            streamSectionsInfo = new List<StreamingSectionInfo>();
            modelArray = new List<ModelSimplified>();
            
            LoadData(modelArray, streamSections, streamSectionsInfo, f, fSmol);
            
            Debug.Log("Applying culling...");
/*#if UNITY_EDITOR
            WorldCullingManager.BtnCall_CreateLevelStreamDBData(streamSections);
#endif*/
            /*Debug.Log("Done");
        }*/

        /*if (RemoveDuplicates)
        {
            RemoveDuplicates = false;
            MeshRenderer[] rends = Resources.FindObjectsOfTypeAll<MeshRenderer>();
            Dictionary<string, GameObject> objs = new Dictionary<string, GameObject>();
            foreach (MeshRenderer rend in rends)
            {
                if (!rend.name.Contains('-')) continue;
                if (rend.gameObject.scene.name == null) continue;
                string sss = rend.GetComponent<MeshFilter>().sharedMesh.name;
                rend.gameObject.name = sss;
                rend.transform.SetParent(UnsortedParent);
                if (objs.ContainsKey(sss))
                {
                    DestroyImmediate(rend.gameObject);
                    continue;
                }
                objs.Add(sss, rend.gameObject);
            }
        }*/
        /*if (apply)
        {
            apply = false;
            Debug.Log("Collecting model objects from scene...");
            MeshRenderer[] rends = Resources.FindObjectsOfTypeAll<MeshRenderer>();
            Dictionary<string, GameObject> objs = new Dictionary<string, GameObject>();
            foreach (MeshRenderer rend in rends)
            {
                if (!rend.name.Contains('-')) continue;
                if (rend.gameObject.scene.name == null) continue;
                string sss = rend.GetComponent<MeshFilter>().sharedMesh.name;
                rend.gameObject.name = sss;
                rend.transform.SetParent(UnsortedParent);
                if (objs.ContainsKey(sss))
                {
                    DestroyImmediate(rend.gameObject);
                    continue;
                }
                objs.Add(sss, rend.gameObject);
            }
            Debug.Log("Applying transforms...");
            Dictionary<string, int> counter = new Dictionary<string, int>();
            Dictionary<string, int> subcounter = new Dictionary<string, int>();
            Dictionary<string, GameObject> obj_cache = new Dictionary<string, GameObject>();
            Transform par;
            for (int sectionNum = 0; sectionNum < streamSections.Count; sectionNum++)
            {
                Debug.Log("Applying section " + streamSections[sectionNum].SectionName + "(" + streamSections[sectionNum].SectionNumber + ")...");
                for (int ins = 0; ins < streamSectionsInfo[sectionNum].instanceArray.Count; ins++)
                {
                    string modelName = streamSectionsInfo[sectionNum].infoArray[streamSectionsInfo[sectionNum].actualIndex[ins]].Name;

                    // I don't use fake shadows/reflections and I don't wanna waste time on this
                    // also panoramas are stored separately
                    //if (modelName.StartsWith("SHD_", StringComparison.InvariantCultureIgnoreCase) || modelName.StartsWith("PAN_", StringComparison.InvariantCultureIgnoreCase) || modelName.StartsWith("RFL_", StringComparison.InvariantCultureIgnoreCase))
                    if (!modelName.StartsWith("RFL_", StringComparison.InvariantCultureIgnoreCase)) // to process RFL
                        continue;

                    int modelhash = streamSectionsInfo[sectionNum].infoArray[streamSectionsInfo[sectionNum].actualIndex[ins]].NameHash0;
                    string fullhash = streamSectionsInfo[sectionNum].infoArray[streamSectionsInfo[sectionNum].actualIndex[ins]].NameHash0 + "-" +streamSectionsInfo[sectionNum].infoArray[streamSectionsInfo[sectionNum].actualIndex[ins]].NameHash1 + "-" + streamSectionsInfo[sectionNum].infoArray[streamSectionsInfo[sectionNum].actualIndex[ins]].NameHash2 + "-" + streamSectionsInfo[sectionNum].infoArray[streamSectionsInfo[sectionNum].actualIndex[ins]].NameHash3;

                    if (!counter.ContainsKey(fullhash))
                        counter.Add(fullhash, 0);
                    counter[fullhash]++;
                    GameObject obj = null;

                    par = transform.Find(streamSections[sectionNum].SectionName.Replace("\0", ""));
                    if (par == null)
                    {
                        par = new GameObject(streamSections[sectionNum].SectionName.Replace("\0", "")).transform;
                        par.SetParent(transform);
                        par.localPosition = Vector3.zero;
                        par.localRotation = Quaternion.identity;
                        par.localScale = Vector3.one;
                    }
                    if (counter[fullhash] > 1)
                    {
                        if (obj_cache[fullhash] == null) continue;
                        obj = Instantiate(obj_cache[fullhash], par);
                        obj.name = obj_cache[fullhash].name + "_dup_" + (counter[fullhash] - 1);
                    } else {
                        // if we didn't...
                        foreach (ModelSimplified model in modelArray)
                        {
                            if (modelhash == model.Hash)
                            {
                                string modelPrefix = model.Chunk + "-" + model.Group;
                                string[] variants = objs.Keys.Where(x => x.StartsWith(modelPrefix, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.Length).ToArray();
                                if (variants.Length == 0)
                                    continue;
                                string actualname = "???";
                                // first let's check if there are models with exact same hash
                                foreach (string variant in variants)
                                {
                                    string subname = variant.Split('-')[2];
                                    if (BitConverter.ToInt32(bStringHash(subname), 0) == modelhash)
                                    {
                                        actualname = subname;
                                        break;
                                    }
                                }
                                // if failed, let's do some very unsafe way to determine model
                                if (actualname == "???")
                                {
                                    foreach (string variant in variants)
                                    {
                                        string subname = variant.Split('-')[2];
                                        if (subname.Contains("."))
                                        {
                                            string subsubname = subname.Split('.')[0];
                                            if (modelName.StartsWith(subsubname, StringComparison.InvariantCultureIgnoreCase) || model.Name.StartsWith(subsubname, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                if (!subcounter.ContainsKey(subsubname))
                                                    subcounter.Add(subsubname, 0);
                                                do
                                                {
                                                    actualname =
                                                        subsubname + "." + subcounter[subsubname].ToString()
                                                            .PadLeft(3, '0');
                                                    subcounter[subsubname]++;
                                                    if (subcounter[subsubname] > 128)
                                                    {
                                                        actualname = "???";
                                                        break;
                                                    }
                                                } while (!objs.ContainsKey(modelPrefix + "-" + actualname));

                                                if (actualname == "???")
                                                    continue;
                                                break;
                                            }
                                        }
                                        else if (modelName.StartsWith(subname, StringComparison.InvariantCultureIgnoreCase) || model.Name.StartsWith(subname, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            actualname = subname;
                                            break;
                                        }
                                    }
                                }

                                // if failed too, let's do it the other way around
                                if (actualname == "???")
                                {
                                    foreach (string variant in variants)
                                    {
                                        string subname = variant.Split('-')[2];
                                        if (subname.Contains("."))
                                        {
                                            string subsubname = subname.Split('.')[0];
                                            if (subsubname.StartsWith(modelName, StringComparison.InvariantCultureIgnoreCase) || subsubname.StartsWith(model.Name, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                if (!subcounter.ContainsKey(subsubname))
                                                    subcounter.Add(subsubname, 0);
                                                do
                                                {
                                                    actualname =
                                                        subsubname + "." + subcounter[subsubname].ToString()
                                                            .PadLeft(3, '0');
                                                    subcounter[subsubname]++;
                                                    if (subcounter[subsubname] > 128)
                                                    {
                                                        actualname = "???";
                                                        break;
                                                    }
                                                } while (!objs.ContainsKey(modelPrefix + "-" + actualname));

                                                if (actualname == "???")
                                                    continue;
                                                break;
                                            }
                                        }
                                        else if (subname.StartsWith(modelName, StringComparison.InvariantCultureIgnoreCase) || subname.StartsWith(model.Name, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            actualname = subname;
                                            break;
                                        }
                                    }
                                }
                                if (actualname == "???") continue;
                                string final_name = modelPrefix + "-" + actualname;
                                obj = objs[final_name];
                                obj.transform.SetParent(par);
                                break;
                            }
                        }
                        // if everything is failed, let's forget about this model for now and move on
                        if (obj == null)
                        {
                            obj_cache.Add(fullhash, null);
                            Debug.LogError("Skipping " + streamSectionsInfo[sectionNum].infoArray[streamSectionsInfo[sectionNum].actualIndex[ins]].Name + "... (" + (ins + 1) + "/" + streamSectionsInfo[sectionNum].instanceArray.Count + ")");
                            continue;
                        }
                        obj_cache.Add(fullhash, obj);
                    }
                    // applying actual transform values
                    obj.transform.position = new Vector3(streamSectionsInfo[sectionNum].instanceArray[ins].PosX, streamSectionsInfo[sectionNum].instanceArray[ins].PosY, streamSectionsInfo[sectionNum].instanceArray[ins].PosZ);
                    if (streamSectionsInfo[sectionNum].instanceArray[ins].Rot00 == 0x2000 && streamSectionsInfo[sectionNum].instanceArray[ins].Rot11 == 0x2000 && streamSectionsInfo[sectionNum].instanceArray[ins].Rot22 == 0x2000)
                    {
                        obj.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        obj.transform.localScale = Vector3.one;
                    } else {
                        // correct usage is (02, 12, 22), (01, 11, 21) but I swapped them around because EAGL uses different positioning system (X, Z, Y)
                        // also Y seems to be flipped and matrix seems to be not column major???
                        obj.transform.rotation = Quaternion.LookRotation(
                            new Vector3(streamSectionsInfo[sectionNum].instanceArray[ins].Rot20 / 8192f, -streamSectionsInfo[sectionNum].instanceArray[ins].Rot22 / 8192f, streamSectionsInfo[sectionNum].instanceArray[ins].Rot21 / 8192f),
                            new Vector3(streamSectionsInfo[sectionNum].instanceArray[ins].Rot10 / 8192f, -streamSectionsInfo[sectionNum].instanceArray[ins].Rot12 / 8192f, streamSectionsInfo[sectionNum].instanceArray[ins].Rot11 / 8192f));
                        // additionally rotate model to compensate for different positioning
                        obj.transform.Rotate(new Vector3(0f, 180f, 0f), Space.World);
                        obj.transform.Rotate(new Vector3(-90f, 0f, 0f), Space.Self);
                        // applying scale
                        // also relocated rots cause of different positioning system, correct usage is:
                        // (00, 10, 20)
                        // (01, 11, 21)
                        // (02, 12, 22)
                        obj.transform.localScale = new Vector3(
                            new Vector3(streamSectionsInfo[sectionNum].instanceArray[ins].Rot00 / 8192f, -streamSectionsInfo[sectionNum].instanceArray[ins].Rot02 / 8192f, streamSectionsInfo[sectionNum].instanceArray[ins].Rot01 / 8192f).magnitude,
                            new Vector3(streamSectionsInfo[sectionNum].instanceArray[ins].Rot20 / 8192f, -streamSectionsInfo[sectionNum].instanceArray[ins].Rot22 / 8192f, streamSectionsInfo[sectionNum].instanceArray[ins].Rot21 / 8192f).magnitude,
                            new Vector3(streamSectionsInfo[sectionNum].instanceArray[ins].Rot10 / 8192f, -streamSectionsInfo[sectionNum].instanceArray[ins].Rot12 / 8192f, streamSectionsInfo[sectionNum].instanceArray[ins].Rot11 / 8192f).magnitude);
                        // (dividing by 8192f looks like fucking hack but it works and I'm down for hacks if it works)
                    }
                }
            }
            Debug.Log("Done");
        }*/
    }
}
//#endif