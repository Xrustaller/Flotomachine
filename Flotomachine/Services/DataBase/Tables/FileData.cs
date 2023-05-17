using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

[Table("file_data")]
public class FileData
{
	[Key, Column("_id")]
	public int Id { get; set; }

	[Column("name")]
	public string Name { get; set; }

	[Column("ext")]
	public string Extension { get; set; }

	[Column("data")]
	public byte[] Data { get; set; }
}