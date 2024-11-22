using GridHub.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using GridHub.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridHub.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly FIAPDBContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(FIAPDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Adicionar uma nova entidade
        public async Task<T> Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "A entidade não pode ser nula.");
            }

            await _context.AddAsync(entity); 
            await SaveChanges(); 
            return entity; 
        }

        // Remover uma entidade
        public async Task Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "A entidade não pode ser nula.");
            }

            _context.Remove(entity); 
            await SaveChanges(); 
        }

        // Atualizar uma entidade existente
        public async Task<T> Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "A entidade não pode ser nula.");
            }

            _context.Entry(entity).State = EntityState.Modified; 
            await SaveChanges(); 
            return entity; 
        }

        // Obter todas as entidades (assíncrono)
        public async Task<IEnumerable<T>> GetAll()
        {
            if (_dbSet == null)
            {
                throw new InvalidOperationException("DbSet não está inicializado.");
            }

            return await _dbSet.ToListAsync(); 
        }

        // Obter uma entidade pelo ID
        public async Task<T> GetById(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "O ID não pode ser nulo.");
            }

            return await _dbSet.FindAsync(id); 
        }

        // Método assíncrono para salvar mudanças
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync(); 
        }
    }
}
