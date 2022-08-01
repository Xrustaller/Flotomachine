using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

[Table("card_id")]
public class CardId
{
    [Key, Column("_id")]
    public int Id { get; set; }

    [Column("user")]
    public int UserId { get; set; }
    public User User { get; set; }

    [Column("card_bytes")]
    public byte[] CardBytes { get; set; }

    public CardId()
    {

    }

    public CardId(int user, byte[] cardBytes)
    {
        UserId = user;
        CardBytes = cardBytes;
    }

    public CardId(User user, byte[] cardBytes)
    {
        UserId = user.Id;
        CardBytes = cardBytes;
    }
}