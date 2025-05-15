using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAppDemo.DataLayer.Models;

[Table("GitHubUsers", Schema = "CustomConnector")]
public class GitHubUser
{

    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Login { get; set; }

    [StringLength(200)]
    public string AvatarUrl { get; set; }

    [StringLength(200)]
    public string ProfileUrl { get; set; }

    public ICollection<GitHubIssue> Issues { get; set; }
}
