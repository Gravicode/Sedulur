﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Sedulur.Data;
using Sedulur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sedulur.Data
{
    public class PostService : ICrud<Post>
    {
        SedulurDB db;

        public PostService()
        {
            if (db == null) db = new SedulurDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Posts.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Posts.Remove(selData);
            db.SaveChanges();
            return true;
        }
        public bool UnLikePost(long userid,long postid)
        {
            try
            {
                var removePost = db.PostLikes.Where(x => x.LikedByUserId == userid && x.PostId == postid).FirstOrDefault();
                if (removePost != null)
                {
                    db.PostLikes.Remove(removePost);
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public bool LikePost(long userid,string username, long postid)
        {
            try
            {
                var newLike = new PostLike() { CreatedDate = DateHelper.GetLocalTimeNow(), LikedByUserName = username, LikedByUserId = userid, PostId = postid };
                db.PostLikes.Add(newLike);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
        public List<Post> FindByKeyword(string Keyword)
        {
            var data = from x in db.Posts
                       where x.Message.Contains(Keyword) 
                       select x;
            return data.ToList();
        }
        
        public List<Post> GetTimeline(string Username)
        {
            if (string.IsNullOrEmpty(Username))
            {
                var data = from x in db.Posts.Include(c => c.PostComments).Include(c => c.PostLikes).Include(c => c.Reposts).Include(c => c.User)
                           
                           orderby x.Id descending
                           select x;
                return data.Take(100).ToList();
            }
            else
            {
                var data = from x in db.Posts.Include(c => c.PostComments).Include(c => c.PostLikes).Include(c => c.Reposts).Include(c => c.User)
                           where x.UserName == Username
                           orderby x.Id descending
                           select x;
                var followedUser = db.Follows.Where(x => x.UserName == Username).Select(x => x.FollowUserId).ToList();
                var data2 = from x in db.Posts.Include(c => c.PostComments).Include(c => c.PostLikes).Include(c => c.Reposts).Include(c => c.User)
                            where followedUser.Contains(x.UserId)
                            orderby x.Id descending
                            select x;
                var union = data.Union(data2).OrderByDescending(x => x.Id);
                return union.Take(100).ToList();
            }
        }

        public List<Post> GetAllData()
        {
            return db.Posts.OrderBy(x=>x.Id).ToList();
        }

        public Post GetDataById(object Id)
        {
            return db.Posts.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Post data)
        {
            try
            {
                db.Posts.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(Post data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                /*
                if (sel != null)
                {
                    sel.Nama = data.Nama;
                    sel.Keterangan = data.Keterangan;
                    sel.Tanggal = data.Tanggal;
                    sel.DocumentUrl = data.DocumentUrl;
                    sel.StreamUrl = data.StreamUrl;
                    return true;

                }*/
                return true;
            }
            catch
            {

            }
            return false;
        }

        public long GetLastId()
        {
            return db.Posts.Max(x => x.Id);
        }
    }

}
/*











 */
