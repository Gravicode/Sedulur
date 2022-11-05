using Microsoft.EntityFrameworkCore;
using Sedulur.Data;
using Sedulur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sedulur.Data
{
    public class FollowService : ICrud<Follow>
    {
        SedulurDB db;

        public FollowService()
        {
            if (db == null) db = new SedulurDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Follows.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Follows.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Follow> FindByKeyword(string Keyword)
        {
            var data = from x in db.Follows
                       where x.UserName.Contains(Keyword) || x.FollowUserName.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Follow> GetAllData()
        {
            return db.Follows.OrderBy(x => x.Id).ToList();
        }

        public Follow GetDataById(object Id)
        {
            return db.Follows.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Follow data)
        {
            try
            {
                db.Follows.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(Follow data)
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
            return db.Follows.Max(x => x.Id);
        }
    }

}
/*











 */
