using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Flotomachine.Services;

[Table("user")]
public class User
{
    [Key, Column("_id")]
    public int Id { get; set; }

    [Column("username")]
    public string Username { get; set; }

    [Column("pass_hash")]
    public string PassHash { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("root")]
    public bool? Root { get; set; }

    [Column("delete")]
    public bool Delete { get; set; }

    public User()
    {

    }

    public User(string username, string password, bool root = false)
    {
        Username = username;
        PassHash = GenerateHash(username, password);
        Name = null;
        Root = root;
        Delete = false;
    }

    public bool CheckPass(string username, string password)
    {
        string hash = GenerateHash(username, password);
        return hash == PassHash;
    }

    public bool CheckPass(string password)
    {
        string hash = GenerateHash(Username, password);
        return hash == PassHash;
    }

    public static string GenerateHash(string username, string password)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(username + password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }

    public override string ToString() => $"Id: {Id}, Login: {Username}, Hash: {PassHash}, Root: {Root}";

}