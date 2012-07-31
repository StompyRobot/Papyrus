namespace Papyrus
{
	public enum RecordMode
	{

		/// <summary>
		/// Append to the end of the database list. Default behaviour
		/// </summary>
		Append,

		/// <summary>
		///  Merge with the record at the location. Appends any list data from this item to the target record.
		/// </summary>
		Merge,

		/// <summary>
		/// Replace the record at the location.
		/// </summary>
		Replace

	}
}