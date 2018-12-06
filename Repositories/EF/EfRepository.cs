using System;
using System.Linq;
using System.Collections.Generic;
using MerryClosets.Models;
using MerryClosets.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MerryClosets.Repositories.EF
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly MerryClosetsContext _dbContext;

        protected EfRepository(MerryClosetsContext dbContext)
        {
            _dbContext = dbContext;
        }


        /*
         * Returns all entities of type T that are active or not, in an object that implements interface IQueryable.
         * Prefered method when there is a need to include attributes that are "lazy".
         */
        protected IQueryable<T> GetQueryable()
        {
            return _dbContext.Set<T>();
        }

        /*
         * Returns all entities of type T that are active, in an object that implements interface IQueryable.
         * Prefered method when there is a need to include attributes that are "lazy".
         */
        protected IQueryable<T> GetActiveQueryable()
        {
            return _dbContext.Set<T>().Where(e => e.IsActive);
        }

        /*
         * Not currently being used.
         */
        protected IQueryable<T> GetQueryable(ISpecification<T> spec)
        {
            return _dbContext.Set<T>()
                .Include(spec.Include)
                .Where(spec.Criteria);
        }

        /*
        * Gets the entity with the given database ID.
        */
        public virtual T GetById(long id)
        {
            return _dbContext.Set<T>().FirstOrDefault(e => e.Id == id);
        }

        /*
         * Gets the entity with the given reference.
         */
        public virtual T GetByReference(string reference)
        {
            return _dbContext.Set<T>().FirstOrDefault(e => e.Reference == reference);
        }

        /*
         * Returns all entities of type T that are active in the form of List<T>. 
         * Method to be overriden when in need to retrieve all categories including additional "lazy" atributtes.
         */
        public virtual List<T> List()
        {
            return this.GetActiveQueryable().ToList();
        }

        /*
         * Not currently being used.
         */
        public List<T> List(ISpecification<T> spec)
        {
            return this.GetQueryable(spec).ToList();
        }

        /*
         * Creates the passed entity in the DB.
         */
        public T Add(T entity)
        {
            Console.WriteLine("1. Entity -> {0} -------- EntityState -> {1}\n", entity.Reference,
                _dbContext.Entry(entity).State);
            
            entity.IsActive = true;
            entity.Version = 1;
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
            
            Console.WriteLine("2. Entity -> {0} -------- EntityState -> {1}\n", entity.Reference,
                _dbContext.Entry(entity).State);

            _dbContext.Entry(entity).State = EntityState.Added;
            
            Console.WriteLine("3. Entity -> {0} -------- EntityState -> {1}\n", entity.Reference,
                _dbContext.Entry(entity).State);
            
            return entity;
        }

        /*
         * "Deactivates" the passed entity. (Soft delete)
         */
        public T Delete(T entity)
        {
            entity.IsActive = false;
            Update(entity);

            return entity;
        }

        /*
         * Updates the passed entity.
         */
        public virtual T Update(T entity)
        {
            //entity.Version++;

            /*Console.WriteLine("\nReference: " + GetByReference(entity.Reference).Reference);
            Console.WriteLine("Id: {0} ------ Reference: {1} ", GetById(entity.Id).Id, GetById(entity.Id).Reference);

            Console.WriteLine("Entity -> {0} -------- EntityState -> {1}", entity.Reference,
                _dbContext.Entry(entity).State);

            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                Console.WriteLine("*******");
                //_dbContext.Set<T>().Attach(entity);
                //_dbContext.Set<T>().Add(entity);
                //_dbContext.Entry(entity).State = EntityState.Unchanged;
                //_dbContext.Entry(entity).State = EntityState.Added;
                Console.WriteLine("*******");
            }*/
            _dbContext.Entry(entity).State = EntityState.Modified;

            /*Console.WriteLine("Entity -> {0} -------- EntityState -> {1}\n", entity.Reference,
                _dbContext.Entry(entity).State);*/
            _dbContext.SaveChanges();
            return entity;
        }
    }
}