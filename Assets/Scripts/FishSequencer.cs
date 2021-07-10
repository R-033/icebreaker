using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FishSequencer : MonoBehaviour
{
    class UData
    {
        public int fTag;
        public int fIndexed;
        public int fEmbedded;
        public int fAllocated;
        public int fSection;
        public int fSize;
        public int fCount;
        public int fOffset;
    }

    public static void Read(byte[] bytes)
    {
        int i = 0;

        while (bytes[i] == 0x11 && bytes[i + 1] == 0x11 && bytes[i + 2] == 0x11 && bytes[i + 3] == 0x11)
            i += 4;
        
        int mCrpSize = BitConverter.ToInt32(bytes, i); i += 4;
        int mSectionNumber = BitConverter.ToInt32(bytes, i); i += 4;
        int mFlags = BitConverter.ToInt32(bytes, i); i += 4;
        uint mLastAddress = BitConverter.ToUInt32(bytes, i); i += 4;
        
        if (bytes[i] != 0x11 || bytes[i + 1] != 0x11 || bytes[i + 2] != 0x11 || bytes[i + 3] != 0x11)
        {
            Debug.LogError("No fish found");
            return;
        }

        i += 4;
        int bitfield = BitConverter.ToInt32(bytes, i); i += 4;
        int fIndexed = bitfield & 1;
        int fEmbedded = (bitfield >> 1) & 1;
        int fAllocated = (bitfield >> 2) & 1;
        int fDataSorted = (bitfield >> 3) & 1;
        int fGroupSorted = (bitfield >> 4) & 1;
        int fGroupCount = (bitfield >> 5) & 1;
        int fDataCount = BitConverter.ToInt32(bytes, i); i += 4;
        int entry_2b9d0fa = BitConverter.ToInt32(bytes, i); i += 4;

        if (fGroupCount != 0)
            Debug.Log("fGroupCount is " + fGroupCount);
        
        List<UData> udata = new List<UData>();
        
        while (i < bytes.Length)
        {
            udata.Add(new UData());
            int start = i;
            udata[udata.Count - 1].fTag = BitConverter.ToInt32(bytes, i); i += 4;
            bitfield = BitConverter.ToInt32(bytes, i); i += 4;
            udata[udata.Count - 1].fIndexed = bitfield & 1;
            udata[udata.Count - 1].fEmbedded = (bitfield >> 1) & 1;
            udata[udata.Count - 1].fAllocated = (bitfield >> 2) & 1;
            udata[udata.Count - 1].fSection = (bitfield >> 3) & 31;
            udata[udata.Count - 1].fSize = bitfield >> 8;
            udata[udata.Count - 1].fCount = BitConverter.ToInt32(bytes, i); i += 4;
            udata[udata.Count - 1].fOffset = start + BitConverter.ToInt32(bytes, i); i += 4;

            if (bytes[i] == 0xAA && bytes[i + 1] == 0xAA && bytes[i + 2] == 0xAA && bytes[i + 3] == 0xAA)
            {
                break;
            }
        }

        foreach (UData currentData in udata)
        {
            if (currentData.fTag == 0x53747273) // String Pool
            {
                // todo

                continue;
            }
            
            short index = (short) (currentData.fTag >> 16);
            short uDataType = (short) (currentData.fTag & 0xffff);

            switch (uDataType)
            {
                case 0x6353: // C-string
                    break;
                
                case 0x6541: // Action
                    break;
                
                case 0x6545: // Engine
                    break;
                
                case 0x6553: // System
                    break;
                
                case 0x6554: // State
                    break;
                
                case 0x656C: // Event List
                    break;
                
                case 0x694E: // Engine Name
                    break;
                
                default:
                    Debug.LogError("Unknown UData type 0x" + uDataType.ToString("X4"));
                    break;
            }
        }
    }
}
