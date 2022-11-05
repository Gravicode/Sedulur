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

        public List<Post> FindByKeyword(string Keyword)
        {
            var data = from x in db.Posts
                       where x.Message.Contains(Keyword) 
                       select x;
            return data.ToList();
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
