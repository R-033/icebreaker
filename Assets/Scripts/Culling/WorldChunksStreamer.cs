using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using Common.Geometry.Data;
using Common.Textures.Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace World.Culling
{
    public class WorldChunksStreamer : MonoBehaviour
    {
        private class LoadBigChunkTask
        {
            public Chunk TargetChunk;
            public GameObject InstantiatedResult;

            public LoadBigChunkTask(Chunk targetChunk)
            {
                TargetChunk = targetChunk;
            }
        }

        private Dictionary<string, GameObject> LoadedChunks =
            new Dictionary<string, GameObject>();

        private Dictionary<string, LoadBigChunkTask> CurrentlyAsyncLoadingChunks =
            new Dictionary<string, LoadBigChunkTask>();

        private Dictionary<string, List<GameObject>> matLinks = new Dictionary<string, List<GameObject>>();

        private static List<CoordDebug.StreamingSectionInfo> streamSectionsInfo;
        private static List<CoordDebug.StreamingSection> streamSections;

        static BinaryReader f;
        
        static Dictionary<uint, SolidObject> models = new Dictionary<uint, SolidObject>();
        static Dictionary<uint, Common.Textures.Data.Texture> textures = new Dictionary<uint, Common.Textures.Data.Texture>();

        public static void Initialize()
        {
	        sh = Shader.Find("Standard");

	        FileStream f2 = new FileStream(Main.GameDirectory + "/TRACKS/STREAML2RA.BUN", FileMode.Open);
            f = new BinaryReader(f2);
            
			ChunkManager chunkManager = new ChunkManager(GameDetector.Game.MostWanted); // todo multiple games
			chunkManager.Open(f2, 0, (int)f2.Length);
			List<ChunkManager.Chunk> chunks = chunkManager.Chunks;
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
				} else if (chunk.Resource is SolidList solidList)
				{
					foreach (var solidObject in solidList.Objects)
					{
						if (!models.ContainsKey(solidObject.Hash))
						{
							models.Add(solidObject.Hash, solidObject);
						}
					}
				}
			}

			f2.Position = 0;
            
            byte[] fSmol = File.ReadAllBytes(Main.GameDirectory + "/TRACKS/L2RA.BUN");
            streamSectionsInfo = new List<CoordDebug.StreamingSectionInfo>();
            streamSections = new List<CoordDebug.StreamingSection>();

            CoordDebug.LoadData(streamSections, streamSectionsInfo, fSmol);
        }

        public void RequestInitialNeededChunks()
        {
            GetNearestNeededChunksToTransform();
        }

        private List<string> LoadedChunksNames = new List<string>();

        private void InitiateBigChunkLoading(Chunk targetChunk)
        {
            if (CurrentlyAsyncLoadingChunks.ContainsKey(targetChunk.ID))
            {
                return;
            }
            if (LoadedChunksNames.Contains(targetChunk.ID))
            {
                return;
            }
            LoadedChunksNames.Add(targetChunk.ID);
            LoadBigChunkTask loadTask = new LoadBigChunkTask(targetChunk);
            CurrentlyAsyncLoadingChunks.Add(targetChunk.ID, loadTask);

            StartCoroutine(LoadingBigChunkAwaiter(loadTask));
        }

        private void InitiateBigChunkUnloading(string chunkId)
        {
            LoadedChunksNames.Remove(chunkId);
            GameObject targetGroup = LoadedChunks[chunkId];
            Destroy(targetGroup);
            LoadedChunks.Remove(chunkId);
            if (matLinks.ContainsKey(chunkId))
                matLinks.Remove(chunkId);
        }

        static Shader sh;
        Dictionary<uint, Material> materialcache = new Dictionary<uint, Material>();
        
        static byte[] Dxt3_compressedAlphas = new byte[16];
        static byte[] Dxt3_Alphas = new byte[16];
        static Color[] Dxt3_colorTable = new Color[4];
        static Color32[] Dxt1Colors = new Color32[16];
        static uint[] Dxt1colorIndices = new uint[16];
        static byte[] Dxt1colorIndexBytes = new byte[4];
        
        public static ulong GetBits(uint bitOffset, uint bitCount, byte[] bytes)
        {
	        Debug.Assert((bitCount <= 64) && ((bitOffset + bitCount) <= (8 * bytes.Length)));

	        ulong bits = 0;
	        uint remainingBitCount = bitCount;
	        uint byteIndex = bitOffset / 8;
	        uint bitIndex = bitOffset - (8 * byteIndex);

	        uint numBitsLeftInByte = 0;
	        uint numBitsReadNow = 0;
	        uint unmaskedBits = 0;
	        uint bitMask = 0;
	        uint bitsReadNow = 0;

	        while (remainingBitCount > 0)
	        {
		        // Read bits from the byte array.
		        numBitsLeftInByte = 8 - bitIndex;
		        numBitsReadNow = Math.Min(remainingBitCount, numBitsLeftInByte);
		        unmaskedBits = (uint)bytes[byteIndex] >> (int)(8 - (bitIndex + numBitsReadNow));
		        bitMask = 0xFFu >> (int)(8 - numBitsReadNow);
		        bitsReadNow = unmaskedBits & bitMask;

		        // Store the bits we read.
		        bits <<= (int)numBitsReadNow;
		        bits |= bitsReadNow;

		        // Prepare for the next iteration.
		        bitIndex += numBitsReadNow;

		        if(bitIndex == 8)
		        {
			        byteIndex++;
			        bitIndex = 0;
		        }

		        remainingBitCount -= numBitsReadNow;
	        }

	        return bits;
        }
        
        private static Color32[] DecodeDXT1TexelBlock(byte[] reader, ref int ind, Color[] colorTable)
        {
	        Debug.Assert(colorTable.Length == 4);
	        
	        for (int i = 0; i < Dxt1colorIndexBytes.Length; i++)
	        {
		        Dxt1colorIndexBytes[i] = reader[ind++];
	        }

	        const uint bitsPerColorIndex = 2;

	        uint rowIndex = 0;
	        uint columnIndex = 0;
	        uint rowBaseColorIndexIndex = 0;
	        uint rowBaseBitOffset = 0;
	        uint bitOffset = 0;

	        for (rowIndex = 0; rowIndex < 4; rowIndex++)
	        {
		        rowBaseColorIndexIndex = 4 * rowIndex;
		        rowBaseBitOffset = 8 * rowIndex;

		        for(columnIndex = 0; columnIndex < 4; columnIndex++)
		        {
			        // Color indices are arranged from right to left.
			        bitOffset = rowBaseBitOffset + (bitsPerColorIndex * (3 - columnIndex));

			        Dxt1colorIndices[rowBaseColorIndexIndex + columnIndex] = (uint)GetBits(bitOffset, bitsPerColorIndex, Dxt1colorIndexBytes);
		        }
	        }

	        // Calculate pixel colors.
	        //var colors = new Color32[16];

	        for(rowIndex = 0; rowIndex < 16; rowIndex++)
	        {
		        Dxt1Colors[rowIndex] = colorTable[Dxt1colorIndices[rowIndex]];
	        }

	        return Dxt1Colors;
        }
        
        public static Color R5G6B5ToColor(ushort R5G6B5)
        {
	        var R5 = ((R5G6B5 >> 11) & 31);
	        var G6 = ((R5G6B5 >> 5) & 63);
	        var B5 = (R5G6B5 & 31);

	        return new Color((float)R5 / 31, (float)G6 / 63, (float)B5 / 31, 1);
        }

        private static Color32[] DecodeDXT3TexelBlock(byte[] reader, ref int ind)
        {
	        // Read compressed pixel alphas.
	        //var compressedAlphas = new byte[16];
	        int rowIndex = 0;
	        int columnIndex = 0;
	        ushort compressedAlphaRow = 0;

	        for (rowIndex = 0; rowIndex < 4; rowIndex++)
	        {
		        compressedAlphaRow = BitConverter.ToUInt16(reader, ind);
		        ind += 2;

		        for(columnIndex = 0; columnIndex < 4; columnIndex++)
		        {
			        // Each compressed alpha is 4 bits.
			        Dxt3_compressedAlphas[(4 * rowIndex) + columnIndex] = (byte)((compressedAlphaRow >> (columnIndex * 4)) & 0xF);
		        }
	        }

	        // Calculate pixel alphas.
	        //var Dxt3_Alphas = new byte[16];
	        int i = 0;
	        for(i = 0; i < 16; i++)
	        {
		        Dxt3_Alphas[i] = (byte)Mathf.RoundToInt(
			        ((float)Dxt3_compressedAlphas[i] / 15)
			        * 255);
	        }

	        // Create the color table.
	        //var colorTable = new Color[4];
	        Dxt3_colorTable[0] = R5G6B5ToColor(BitConverter.ToUInt16(reader, ind));
	        ind += 2;
	        Dxt3_colorTable[1] = R5G6B5ToColor(BitConverter.ToUInt16(reader, ind));
	        ind += 2;
	        Dxt3_colorTable[2] = Color.Lerp(Dxt3_colorTable[0], Dxt3_colorTable[1], 1.0f / 3);
	        Dxt3_colorTable[3] = Color.Lerp(Dxt3_colorTable[0], Dxt3_colorTable[1], 2.0f / 3);

	        // Calculate pixel colors.
	        Color32[] colors = DecodeDXT1TexelBlock(reader, ref ind, Dxt3_colorTable);

	        for(i = 0; i < 16; i++)
	        {
		        colors[i].a = Dxt3_Alphas[i];
	        }

	        return colors;
        }
        
        private static void CopyDecodedTexelBlock(Color32[] decodedTexels, byte[] argb, int baseRowIndex, int baseColumnIndex, int textureWidth, int textureHeight)
        {
	        for(int i = 0; i < 4; i++) // row
	        {
		        for(int j = 0; j < 4; j++) // column
		        {
			        var rowIndex = baseRowIndex + i;
			        var columnIndex = baseColumnIndex + j;

			        // Don't copy padding on mipmaps.
			        if((rowIndex < textureHeight) && (columnIndex < textureWidth))
			        {
				        var decodedTexelIndex = (4 * i) + j;
				        var color = decodedTexels[decodedTexelIndex];

				        var ARGBPixelOffset = (textureWidth * rowIndex) + columnIndex;
				        var basePixelARGBIndex = 4 * ARGBPixelOffset;

				        argb[basePixelARGBIndex] = color.a;
				        argb[basePixelARGBIndex + 1] = color.r;
				        argb[basePixelARGBIndex + 2] = color.g;
				        argb[basePixelARGBIndex + 3] = color.b;
			        }
		        }
	        }
        }
        
        private static byte[] DecodeDXT3ToARGB(byte[] compressedData, uint width, uint height)
        {
	        var argb = new byte[width * height * 4];

	        int ind = 0;
	        
	        for(int rowIndex = 0; rowIndex < height; rowIndex += 4)
	        {
		        for(int columnIndex = 0; columnIndex < width; columnIndex += 4)
		        {
			        Color32[] colors = DecodeDXT3TexelBlock(compressedData, ref ind);
					CopyDecodedTexelBlock(colors, argb, rowIndex, columnIndex, (int)width, (int)height);
		        }

	        }

	        return argb;
        }

        Dictionary<uint, Mesh> cache_1 = new Dictionary<uint, Mesh>();
        Dictionary<uint, Material[]> cache_2 = new Dictionary<uint, Material[]>();
        List<uint> loading = new List<uint>();

        private IEnumerator LoadingSubWait(Chunk chunk, int ins, Transform parent)
        {
            CoordDebug.SceneryInstanceStruct inst = chunk.sectionInfo.instanceArray[ins];
            CoordDebug.SceneryInfoStruct inf = chunk.sectionInfo.infoArray[chunk.sectionInfo.actualIndex[ins]];
            GameObject obj;
            Vector3 pos = new Vector3(inst.PosX, inst.PosY, inst.PosZ);
            if (!models.ContainsKey(inf.NameHash0))
            {
	            FinishRegister[chunk.ID]++;
	            yield break;
            }
            SolidObject solidObject = models[inf.NameHash0];
            if (solidObject.Name.Contains("SHD") || solidObject.Name.Contains("RFL") || solidObject.Name.Contains("PAN") || solidObject.Name.Contains("LOD") || solidObject.Name.Contains("TRACKBARRIER") || solidObject.Name.Contains("SKYDOME") || solidObject.Name.Contains("SHADOW") || solidObject.Name == "SKY_SPECULAR")
            {
	            FinishRegister[chunk.ID]++;
	            yield break;
            }
            
            obj = new GameObject(inf.NameHash0.ToString("X"), typeof(MeshFilter), typeof(MeshRenderer));
			obj.transform.SetParent(parent);
			obj.transform.position = pos;
            Vector3 upwards = new Vector3(inst.Rot20, inst.Rot22, inst.Rot21);
            Vector3 forward = new Vector3(inst.Rot10, inst.Rot12, inst.Rot11);
            obj.transform.rotation = Quaternion.LookRotation(forward, upwards);
            obj.transform.localScale = new Vector3(
                new Vector3(inst.Rot00, inst.Rot02, inst.Rot01).magnitude / 8192f,
                upwards.magnitude / 8192f,
                forward.magnitude / 8192f);

            bool loaded = cache_1.ContainsKey(inf.NameHash0);
            if (!loaded)
            {
                if (loading.Contains(inf.NameHash0))
                {
                    do
                    {
                        yield return 0;
                    } while (loading.Contains(inf.NameHash0));
                    loaded = true;
                }
            }

            if (loaded)
            {
                obj.GetComponent<MeshFilter>().sharedMesh = cache_1[inf.NameHash0];
                obj.GetComponent<MeshRenderer>().sharedMaterials = cache_2[inf.NameHash0];

                if (obj.transform.position == Vector3.zero)
                {
                    MeshCollider col = obj.AddComponent<MeshCollider>();
                    col.sharedMesh = cache_1[inf.NameHash0];
                }

                FinishRegister[chunk.ID]++;

                yield break;
            }

            loading.Add(inf.NameHash0);
            
            Material[] materials = new Material[solidObject.Materials.Count];
			for (int i = 0; i < materials.Length; i++)
			{
				MostWantedMaterial mat = (MostWantedMaterial) solidObject.Materials[i];
				string n = mat.Name.Replace("<", "").Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "");
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
							case TextureCompression.Dxt3:
								im = DecodeDXT3ToARGB(im, textures[h].Width, textures[h].Height);
								break;
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
			}

			yield return 0;

			Mesh mesh = new Mesh();
			mesh.name = solidObject.Name;
			List<Vector3> vertices = new List<Vector3>(solidObject.Vertices.Length);
			List<Vector2> uv = new List<Vector2>(solidObject.Vertices.Length);
			List<Vector3> normals = new List<Vector3>(solidObject.Vertices.Length);
			for (int i = 0; i < solidObject.Vertices.Length; i++)
			{
				vertices.Add(new Vector3(solidObject.Vertices[i].X, solidObject.Vertices[i].Z, solidObject.Vertices[i].Y));
				uv.Add(new Vector2(solidObject.Vertices[i].U, solidObject.Vertices[i].V));
				normals.Add(new Vector3(solidObject.Vertices[i].NormalX, solidObject.Vertices[i].NormalZ, solidObject.Vertices[i].NormalY));
			}

			mesh.SetVertices(vertices);
			mesh.SetUVs(0, uv);
			mesh.SetNormals(normals);
			mesh.subMeshCount = materials.Length;
			for (int i = 0; i < materials.Length; i++)
			{
				var faces = solidObject.Faces.Where(f => f.MaterialIndex == i).ToList();
				List<int> tris = new List<int>(faces.Count * 3);
				foreach (var face in faces)
				{
					tris.Add(face.Vtx2);
					tris.Add(face.Vtx3);
					tris.Add(face.Vtx1);
				}

				mesh.SetTriangles(tris, i);
			}

			mesh.RecalculateTangents();
			mesh.RecalculateBounds();

			obj.GetComponent<MeshFilter>().sharedMesh = mesh;
			obj.GetComponent<MeshRenderer>().sharedMaterials = materials;

			if (obj.transform.position == Vector3.zero)
			{
				MeshCollider col = obj.AddComponent<MeshCollider>();
				col.sharedMesh = mesh;
			}
            
            FinishRegister[chunk.ID]++;

            cache_1.Add(inf.NameHash0, mesh);
            cache_2.Add(inf.NameHash0, materials);
            loading.Remove(inf.NameHash0);
        }
        
        Dictionary<string, int> FinishRegister = new Dictionary<string, int>();

        bool IsOnLine((Vector2, Vector2, float) line, Vector2 point, Vector2 forward)
        {
            return Vector2.Dot((line.Item1 - point).normalized, forward) > 0.8f && 
                   Vector2.Distance(line.Item1, point) + Vector2.Distance(line.Item2, point) < line.Item3;
        }

        public Transform parent;

        private IEnumerator LoadingBigChunkAwaiter(LoadBigChunkTask loadTask)
        {
            Transform _parent = new GameObject(loadTask.TargetChunk.ID).transform;
            _parent.SetParent(parent);
            int c = loadTask.TargetChunk.sectionInfo.instanceArray.Count;
            if (c == 0)
            {
                loadTask.InstantiatedResult = _parent.gameObject;
                PostLoadChunkTask(loadTask);
                yield break;
            }
            FinishRegister.Add(loadTask.TargetChunk.ID, 0);
            for (int ins = 0; ins < loadTask.TargetChunk.sectionInfo.instanceArray.Count; ins++)
            {
	            StartCoroutine(LoadingSubWait(loadTask.TargetChunk, ins, _parent));
                yield return 0;
            }
            do {
                yield return 0;
            } while (FinishRegister[loadTask.TargetChunk.ID] < c);
            FinishRegister.Remove(loadTask.TargetChunk.ID);
            loadTask.InstantiatedResult = _parent.gameObject;
            PostLoadChunkTask(loadTask);
        }

        private int ch;
        private Transform tr;
        private MeshFilter m;

        public Main main;

        private void PostLoadChunkTask(LoadBigChunkTask loadTask)
        {
            LoadedChunks.Add(loadTask.TargetChunk.ID, loadTask.InstantiatedResult);
            CurrentlyAsyncLoadingChunks.Remove(loadTask.TargetChunk.ID);
        }

        public class Chunk
        {
	        public Chunk(string _name)
	        {
		        ID = _name;
	        }

	        public string ID;
	        public CoordDebug.StreamingSection section;
	        public CoordDebug.StreamingSectionInfo sectionInfo;
	        public List<CoordDebug.SceneryInstanceStruct> instanceArray = new List<CoordDebug.SceneryInstanceStruct>();
	        public List<CoordDebug.SceneryInfoStruct> infoArray = new List<CoordDebug.SceneryInfoStruct>();
	        public List<int> actualIndex = new List<int>();
        }

        public Dictionary<string, Chunk> chunkData = new Dictionary<string, Chunk>();


        public IEnumerator ChunksAutoOnDemandLoaderCoroutine()
        {
            yield return 0;
	        Chunk[] data = new Chunk[chunkData.Count];
            int ind = 0;
            foreach (string c in chunkData.Keys)
                data[ind++] = chunkData[c];
            Vector2[] centres = new Vector2[data.Length];
            for (int i = 0; i < data.Length; i++)
                centres[i] = new Vector2(data[i].section.CentreX, data[i].section.CentreZ);
            Chunk cur;
            yield return 0;
            Vector3 campos;
            while (true)
            {
	            campos = main.tabnum < 3 ? main.EditorCamera.transform.position : main.SceneCamera.transform.position;
                for (int i = 0; i < data.Length; i++)
                {
                    cur = data[i];
                    if (!LoadedChunksNames.Contains(cur.ID))
                    {
                        if (cur.section.Radius == 0f || Vector2.Distance(new Vector2(campos.x, campos.z), centres[i]) <= cur.section.Radius + 300f)
                        {
                            InitiateBigChunkLoading(cur);
                            yield return 0;
                        }
                    }
                    else if (LoadedChunks.ContainsKey(cur.ID))
                    {
                        if (Vector2.Distance(new Vector2(campos.x, campos.z), centres[i]) > cur.section.Radius + 310f)
                        {
                            InitiateBigChunkUnloading(cur.ID);
                            yield return 0;
                        }
                    }
                }
                yield return 0;
            }
        }

        private void GetNearestNeededChunksToTransform()
        {
	        for (int i = 0; i < streamSections.Count; i++)
            {
                bool found = false;
                foreach (string bakedChunksData in chunkData.Keys)
                {
                    if (streamSections[i].SectionName == chunkData[bakedChunksData].ID)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
	                chunkData.Add(streamSections[i].SectionName, new Chunk(streamSections[i].SectionName));
                }
            }

	        foreach (var bakedChunksData in chunkData)
            {
                if (bakedChunksData.Value.sectionInfo == null)
                {
                    for (int i = 0; i < streamSections.Count; i++)
                    {
                        if (streamSections[i].SectionName == bakedChunksData.Value.ID)
                        {
                            bakedChunksData.Value.section = streamSections[i];
                            bakedChunksData.Value.sectionInfo = streamSectionsInfo[i];
                            CoordDebug.LoadSectionData(f, bakedChunksData.Value.section, out bakedChunksData.Value.sectionInfo.instanceArray, out bakedChunksData.Value.sectionInfo.infoArray, out bakedChunksData.Value.sectionInfo.actualIndex);
                            (CoordDebug.SceneryInstanceStruct, int)[] pair = new (CoordDebug.SceneryInstanceStruct, int)[bakedChunksData.Value.sectionInfo.instanceArray.Count];
                            for (int j = 0; j < pair.Length; j++)
                                pair[j] = (bakedChunksData.Value.sectionInfo.instanceArray[j], bakedChunksData.Value.sectionInfo.actualIndex[j]);
                            pair = pair.OrderByDescending(x => {
                                Vector3 min = new Vector3(x.Item1.MinX, x.Item1.MinY, x.Item1.MinZ);
                                Vector3 max = new Vector3(x.Item1.MaxX, x.Item1.MaxY, x.Item1.MaxZ);
                                return (max - min).sqrMagnitude;
                            }).ToArray();
                            for (int j = 0; j < pair.Length; j++)
                            {
                                bakedChunksData.Value.sectionInfo.instanceArray[j] = pair[j].Item1;
                                bakedChunksData.Value.sectionInfo.actualIndex[j] = pair[j].Item2;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}