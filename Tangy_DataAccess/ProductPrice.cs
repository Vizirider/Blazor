using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangy_DataAccess;
public class ProductPrice
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; }
    public string size { get; set; }
    public double Price { get; set; }
}
