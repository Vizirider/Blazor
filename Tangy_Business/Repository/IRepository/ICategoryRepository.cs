using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tangy_Models;

namespace Tangy_Business.Repository.IRepository;
public interface ICategoryRepository
{
    public Task<CategoryDTO> Create(CategoryDTO categoryDTO);
    public Task<CategoryDTO> Update(CategoryDTO categoryDTO);
    public Task<int> Delete(int id);
    public Task<CategoryDTO> GetById(int id);
    public Task<IEnumerable<CategoryDTO>> GetAll();
}
