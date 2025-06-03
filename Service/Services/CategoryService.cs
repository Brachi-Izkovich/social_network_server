using AutoMapper;
using Common.Dto;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CategoryService : IService<CategoryDto>
    {
        private readonly IRepository<Category> repository;
        private readonly IMapper mapper;
        public CategoryService(IRepository<Category> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<CategoryDto> Add(CategoryDto category)
        {
            return mapper.Map<Category, CategoryDto>(await repository.Add(mapper.Map<CategoryDto, Category>(category)));
        }

        public async Task Delete(int id)
        {
            await repository.Delete(id);
        }

        public async Task<List<CategoryDto>> GetAll()
        {
            return mapper.Map<List<Category>, List<CategoryDto>>(await repository.GetAll());
        }

        public async Task<CategoryDto> GetById(int id)
        {
            return mapper.Map<Category, CategoryDto>(await repository.GetById(id));
        }

        public async Task Update(int id, CategoryDto item)
        {
            await repository.Update(id, mapper.Map<CategoryDto, Category>(item));
        }
    }
}
