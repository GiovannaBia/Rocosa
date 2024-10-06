using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rocosa_AccesoDatos.Datos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rocosa_AccesoDatos.Datos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ApplicationDBContext _db;
        internal DbSet<T> dbSet;
        public Repositorio(ApplicationDBContext db)
        {
            _db = db;
            this.dbSet = db.Set<T>();
        }
        public void Agregar(T entidad)
        {
            dbSet.Add(entidad);
        }

        public void Guardar()
        {
            _db.SaveChanges();
        }

        public T ObtenerPrimero(Expression<Func<T, bool>> filtro = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro);
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp); // ejm "Categoria, TipoAplicacion"
                }
            }
           
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }

            return query.FirstOrDefault();
        }

        public IEnumerable<T> ObtenerTodos(Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro);    
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in  incluirPropiedades.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp); // ejm "Categoria, TipoAplicacion"
                }
            }
            if (orderBy != null) {
                query = orderBy(query);
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }

            return query.ToList();
        }

        public T Obtenter(int id)
        {
            return dbSet.Find(id);
        }

        public void Remover(T entidad)
        {
            dbSet.Remove(entidad);
        }
    }
}
