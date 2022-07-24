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

    [Column("root")]
    public int Root { get; set; }

    [Column("delete")]
    public int Delete { get; set; }

    public User()
    {

    }

    public User(string username, string password, int root = 0)
    {
        Username = username;
        PassHash = GenerateHash(username, password);
    }

    public bool CheckPass(string username, string password)
    {
        string hash = GenerateHash(username, password);
        return hash == PassHash;
    }

    public static string GenerateHash(string username, string password)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(username + password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }

    public override string ToString() => $"Id: {Id}, Login: {Username}, Hash: {PassHash}, Root: {Root == 1}";

    public bool IsRoot() => Root == 1;
    public bool IsDelete() => Delete == 1;
    public bool IsUnableToDelete() => Delete == -1;
}