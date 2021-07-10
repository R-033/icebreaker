namespace Hypercooled.Shared.Interfaces
{
	/// <summary>
	/// Represetns a collection of methods that perform block parsing.
	/// </summary>
	public interface IParser
	{
		/// <summary>
		/// Block ID that this parser performs operations onto.
		/// </summary>
		uint BlockID { get; }

		/// <summary>
		/// Prepares parser for processing and disassembling data.
		/// </summary>
		/// <param name="block"><see cref="Structures.BinBlock"/> with offset, ID and size info.</param>
		void Prepare(Structures.BinBlock block);

		/// <summary>
		/// Processes the internal data and disassembles it into separate elements that can be retrieved
		/// after.
		/// </summary>
		/// <param name="br"><see cref="System.IO.BinaryReader"/> to read data with.</param>
		void Disassemble(System.IO.BinaryReader br);
	}
}
