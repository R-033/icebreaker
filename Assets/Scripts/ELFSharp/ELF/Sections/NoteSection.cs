using System;
using ELFSharp;
using ELFSharp.Utilities;

namespace ELFSharp.ELF.Sections
{
    public sealed class NoteSection<T> : Section<T>, INoteSection where T : struct
    {
        public string NoteName
        {
            get
            {
                return data.Name;
            }
        }
        
        public byte[] Description
        {
            get
            {
                return data.Description;
            }
        }

        public override string ToString()
        {
			return string.Format("{0}: {2}, Type={1}", Name, NoteType, Type);
        }
        
        public T NoteType
        {
            get
            {
                return data.Type.To<T>();
            }
        }
         
        internal NoteSection(SectionHeader header, SimpleEndianessAwareReader reader) : base(header, reader)
        {
            data = new NoteData(header.Offset, header.Size, reader);
        }
        
        private readonly NoteData data;
    }   

}