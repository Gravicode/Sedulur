using Microsoft.EntityFrameworkCore;
using Sedulur.Data;
using Sedulur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sedulur.Data
{
    public class RepostService : ICrud<Repost>
    {
        SedulurDB db;

        public RepostService()
        {
            if (db == null) db = new SedulurDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Reposts.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Reposts.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Repost> FindByKeyword(string Keyword)
        {
            var data = from x in db.Reposts
                       where x.RepostByUserName.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Repost> GetAllData()
        {
            return db.Reposts.OrderBy(x => x.Id).ToList();
        }

        public Repost GetDataById(object Id)
        {
            return db.Reposts.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Repost data)
        {
            try
            {
                db.Reposts.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(Repost data)
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
            return db.Reposts.Max(x => x.Id);
        }
    }

}
/*











 */
