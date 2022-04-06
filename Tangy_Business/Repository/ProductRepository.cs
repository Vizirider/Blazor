using AutoMapper;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tangy_Business.Repository.IRepository;

using Tangy_DataAccess;
using Tangy_DataAccess.Data;

using Tangy_Models;

namespace Tangy_Business.Repository;
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ProductRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductDTO> Create(ProductDTO productDTO)
    {
        var product = _mapper.Map<ProductDTO, Product>(productDTO);

        var addedProduct = _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return _mapper.Map<Product, ProductDTO>(addedProduct.Entity);
    }

    public async Task<int> Delete(int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (product != null)
        {
            _db.Products.Remove(product);
            return await _db.SaveChangesAsync();
        }

        return 0;
    }

    public async Task<IEnumerable<ProductDTO>> GetAll()
    {
        return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(_db.Products.Include(x => x.Category));
    }

    public async Task<ProductDTO> GetById(int id)
    {
        var product = await _db.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
        if (product != null)
        {
            return _mapper.Map<Product, ProductDTO>(product);
        }
        return new ProductDTO();
    }

    public async Task<ProductDTO> Update(ProductDTO productDTO)
    {
        var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == productDTO.Id);
        if (product != null)
        {
            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.ImageUrl = productDTO.ImageUrl;
            product.CategoryId = productDTO.CategoryId;
            product.Color = productDTO.Color;
            product.ShopFavorites = productDTO.ShopFavorites;
            product.CustomerFavorites = productDTO.CustomerFavorites;
            _db.Products.Update(product);
            _db.SaveChanges();
            return _mapper.Map<Product, ProductDTO>(product);
        }
        return productDTO;
    }
}
