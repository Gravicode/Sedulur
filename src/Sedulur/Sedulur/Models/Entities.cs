using GemBox.Document;
using Sedulur.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Sedulur.Models
{
    #region helpers model
    public class TempFollow
    {

        public bool IsFollow
        {
            get; set;
        }
        public UserProfile User { get; set; }
    }
    public class PeopleByJob
    {
        public string Job { get; set; }
        public List<UserProfile> Users { get; set; }
    }
    public record PopularPeople(string Username, int TotalFollower, UserProfile User);
    #endregion

    [Table("trending")]
    public class Trending
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Hashtag { set; get; }
        public DateTime CreatedDate { set; get; }
        public string? Location { set; get; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
    }

    [Table("follow")]
    public class Follow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        //user
        [ForeignKey(nameof(User)), Column(Order = 0)]
        public long UserId { get; set; }
        public UserProfile User { set; get; }
        public string UserName { set; get; }
        //follow
        public string FollowUserName { set; get; }

        [ForeignKey(nameof(FollowUser)), Column(Order = 1)]
        public long FollowUserId { get; set; }
        public UserProfile FollowUser { set; get; }
        public DateTime FollowDate { set; get; }
    }

    [Table("repost")]
    public class Repost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }        
        public Post Post { set; get; }
        [ForeignKey("Post")]
        public long PostId { set; get; }
        public string RepostByUserName { set; get; }
        [ForeignKey("UserProfile")]
        public long RepostByUserId { set; get; }
        public UserProfile RepostByUser { set; get; }
    }


    [Table("postlike")]
    public class PostLike
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public Post Post { set; get; }
        [ForeignKey("Post")]
        public long PostId { set; get; }
        [ForeignKey("UserProfile")]
        public long LikedByUserId { set; get; }
        public string LikedByUserName { set; get; }
        public UserProfile LikedByUser { set; get; }
        public DateTime CreatedDate { set; get; }
    }

    [Table("postcomment")]
    public class PostComment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Comment { set; get; }
        public Post Post { set; get; }
        [ForeignKey("Post")]
        public long PostId { set; get; }
        [ForeignKey("UserProfile")]
        public long UserId { set; get; }
        public string Username { set; get; }
        public DateTime CreatedDate { set; get; }
        public UserProfile User { set; get; }
        public ICollection<CommentLike> CommentLikes { get; set; }

    }
    [Table("commentlike")]
    public class CommentLike
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public PostComment Comment { set; get; }
        [ForeignKey("Comment")]
        public long CommentId { set; get; }
        [ForeignKey("UserProfile")]
        public long LikedByUserId { set; get; }
        public string LikedByUserName { set; get; }
        public UserProfile LikedByUser { set; get; }
        public DateTime CreatedDate { set; get; }
    }

    [Table("contact")]
    public class Contact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Email { set; get; }
        public string FullName { set; get; }
        public string Phone { set; get; }
        public string Message { set; get; }
        public DateTime CreatedDate { set; get; }
        public bool IsReplied { get; set; } = false;
        public string? ReplyBy { get; set; }
        public string? ReplyMessage { get; set; }
        public DateTime? ReplyDate { get; set; }
    }

    [Table("post")]
    public class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [ForeignKey("UserProfile")]
        public long UserId { set; get; }
        public string UserName { set; get; }
        public DateTime CreatedDate { set; get; }
        public string Message { set; get; }
        public string? Mentions { set; get; }
        public string? LinkUrls { set; get; }
        public string? VideoUrls { set; get; }
        public string? DocUrls { set; get; }
        public string? ImageUrls { set; get; }
        public string? Hashtags { set; get; }

        public ICollection<Repost> Reposts { get; set; }
        public UserProfile User { get; set; }

        public ICollection<PostLike> PostLikes { get; set; }
        public ICollection<PostComment> PostComments { get; set; }

        public ICollection<CommentLike> CommentLikes { get; set; }
    }

    public enum LogCategory
    {
        Info, Error, Warning
    }
    [Table("logs")]
    public class Log
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LogDate { get; set; }
        public string Message { get; set; }
        public LogCategory Category { get; set; }
    }
  
    [Table("userprofile")]
    public class UserProfile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool IsBlueBadge { get; set; } = true;
        public string? Pekerjaan { get; set; } = "Manusia";
        public string? Website { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Alamat { get; set; }
        public string? KTP { get; set; }
        public string? PicUrl { get; set; }
        public bool Aktif { get; set; } = true;
        public string? Daerah { get; set; }
        public string? Desa { get; set; }
        public string? Kelompok { get; set; }
        public Char Gender { get; set; } = 'N';
        public Roles Role { set; get; } = Roles.User;
        public string? AboutMe { get; set; } = "Manusia biasa.";
        public DateTime CreatedDate { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        [InverseProperty(nameof(Follow.FollowUser))]
        public ICollection<Follow> Follows { get; set; }

        [InverseProperty(nameof(Follow.User))]
        public ICollection<Follow> FollowedBy { get; set; }
        public ICollection<Repost> Reposts { get; set; }
        public ICollection<PostLike> PostLikes { get; set; }
        public ICollection<PostComment> PostComments { get; set; }
        public ICollection<Post> Posts { get; set; }

      

    }

    public enum Roles { Admin, User, Pengurus }
    

    #region helper models
    public class OutputCls
    {
        public OutputCls()
        {
            this.IsSucceed = false;
            this.Message = "";
        }
        public object Data { get; set; }
        public string Message { get; set; }
        public bool IsSucceed { get; set; }
    }
    #endregion
}
