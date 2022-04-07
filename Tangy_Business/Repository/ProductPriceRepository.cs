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
public class ProductPriceRepository : IProductPriceRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ProductPriceRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    async Task<ProductPriceDTO> IProductPriceRepository.Create(ProductPriceDTO productPriceDTO)
    {
        var productPrice = _mapper.Map<ProductPriceDTO, ProductPrice>(productPriceDTO);

        var addedProductPrice = _db.ProductPrices.Add(productPrice);
        await _db.SaveChangesAsync();

        return _mapper.Map<ProductPrice, ProductPriceDTO>(addedProductPrice.Entity);
    }

    async Task<int> IProductPriceRepository.Delete(int id)
    {
        var productPrice = await _db.ProductPrices.FirstOrDefaultAsync(x => x.Id == id);
        if (productPrice != null)
        {
            _db.ProductPrices.Remove(productPrice);
            return await _db.SaveChangesAsync();
        }

        return 0;
    }

    async Task<IEnumerable<ProductPriceDTO>> IProductPriceRepository.GetAll(int? id = null)
    {
        if (id != null && id > 0)
        {
            return _mapper.Map<IEnumerable<ProductPrice>, IEnumerable<ProductPriceDTO>>
                (_db.ProductPrices.Where(u => u.ProductId == id));
        }
        else
        {
            return _mapper.Map<IEnumerable<ProductPrice>, IEnumerable<ProductPriceDTO>>(_db.ProductPrices);
        }
    }

    async Task<ProductPriceDTO> IProductPriceRepository.GetById(int id)
    {
        var productPrice = await _db.ProductPrices.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);
        if (productPrice != null)
        {
            return _mapper.Map<ProductPrice, ProductPriceDTO>(productPrice);
        }
        return new ProductPriceDTO();
    }

    public async Task<ProductPriceDTO> Update(ProductPriceDTO productPriceDTO)
    {
        var productPrice = await _db.ProductPrices.FirstOrDefaultAsync(x => x.Id == productPriceDTO.Id);
        if (productPrice != null)
        {
            productPrice.ProductId = productPriceDTO.ProductId;
            productPrice.size = productPriceDTO.Size;
            productPrice.Price = productPriceDTO.Price;

            _db.ProductPrices.Update(productPrice);
            _db.SaveChanges();
            return _mapper.Map<ProductPrice, ProductPriceDTO>(productPrice);
        }
        return productPriceDTO;
    }
}
