using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AMS.Reposiroty
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private AMSEntities db = null;
        private DbSet<T> table = null;
        public GenericRepository()
        {
            this.db = new AMSEntities();
            table = db.Set<T>();
        }

        public GenericRepository(AMSEntities db)
        {
            this.db = db;
            table = db.Set<T>();
        }
        public IEnumerable<T> List
        {
            get
            {
                return table.ToList();
            }
        }

        public void Add(T entity)
        {
            table.Add(entity);
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }

        public void Delete(T entity)
        {
            table.Remove(entity);
            db.SaveChanges();
        }

        public T FindById(object Id)
        {
            return table.Find(Id);
        }

        public void Update(T entity)
        {
            //table.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();

        }
    }
}