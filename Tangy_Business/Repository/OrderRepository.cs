using AutoMapper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Tangy_DataAccess;
using Tangy_DataAccess.Data;
using Tangy_DataAccess.ViewModel;

using Tangy_Models;
using Tangy_Business.Repository.IRepository;
using Tangy_Common;

namespace Tangy_Business.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public OrderRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<OrderHeaderDTO> CancelOrder(int id)
    {
        var orderHeader = await _db.OrderHeaders.FindAsync(id);
        if (orderHeader == null)
        {
            return new OrderHeaderDTO();
        }

        if (orderHeader.Status==SD.Status_Pending)
        {
            orderHeader.Status = SD.Status_Cancelled;
            await _db.SaveChangesAsync();
        }
        if (orderHeader.Status == SD.Status_Confirmed)
        {
            var options = new Stripe.RefundCreateOptions
            {
                Reason = Stripe.RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };

            var service = new Stripe.RefundService();
            await _db.SaveChangesAsync();
        }
        return _mapper.Map<OrderHeader, OrderHeaderDTO>(orderHeader);
    }

    public async Task<OrderDTO> Create(OrderDTO orderDTO)
    {
        try
        {
            var order = _mapper.Map<OrderDTO, Order>(orderDTO);
            _db.OrderHeaders.Add(order.OrderHeader);
            await _db.SaveChangesAsync();

            foreach (var details in order.OrderDetails)
            {
                details.OrderHeaderId = order.OrderHeader.Id;
            }
            _db.OrderDetails.AddRange((order.OrderDetails));
            await _db.SaveChangesAsync();

            return new OrderDTO()
            {
                OrderHeader = _mapper.Map<OrderHeader, OrderHeaderDTO>(order.OrderHeader),
                OrderDetails = _mapper.Map<IEnumerable<OrderDetail>, IEnumerable<OrderDetailDTO>>(order.OrderDetails).ToList()
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return orderDTO;
    }

    public async Task<int> Delete(int id)
    {
        var orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(x => x.Id == id);
        if (orderHeader != null)
        {
            IEnumerable<OrderDetail> orderDetail = _db.OrderDetails.Where(x => x.OrderHeaderId == id);

            _db.OrderDetails.RemoveRange(orderDetail);
            _db.OrderHeaders.Remove(orderHeader);
            return _db.SaveChanges();
        }

        return 0;
    }

    public async Task<OrderDTO> Get(int id)
    {
        Order order = new()
        {
            OrderHeader = _db.OrderHeaders.FirstOrDefault(x => x.Id == id),
            OrderDetails = _db.OrderDetails.Where(x => x.OrderHeaderId == id),
        };
        if (order != null)
        {
            return _mapper.Map<Order, OrderDTO>(order);
        }
        return new OrderDTO();
    }

    public async Task<IEnumerable<OrderDTO>> GetAll(string? userId, string? status)
    {
        List<Order> OrderFromDb = new();
        IEnumerable<OrderHeader> orderHeaderList = _db.OrderHeaders;
        IEnumerable<OrderDetail> orderDetailList = _db.OrderDetails;

        foreach (OrderHeader header in orderHeaderList)
        {
            Order order = new()
            {
                OrderHeader = header,
                OrderDetails = orderDetailList.Where(x => x.OrderHeaderId == header.Id),
            };
            OrderFromDb.Add(order);
        }

        return _mapper.Map<IEnumerable<Order>, IEnumerable<OrderDTO>>(OrderFromDb);
    }

    public async Task<OrderHeaderDTO> MarkPaymentSuccessful(int id)
    {
        var data = await _db.OrderHeaders.FindAsync(id);
        if (data == null)
        {
            return new OrderHeaderDTO();
        }
        if (data.Status == SD.Status_Pending)
        {
            data.State = SD.Status_Confirmed;
            await _db.SaveChangesAsync();
            return _mapper.Map<OrderHeader, OrderHeaderDTO>(data);
        }
        return new OrderHeaderDTO();
    }

    public async Task<OrderHeaderDTO> UpdateHeader(OrderHeaderDTO orderHeaderDTO)
    {
        if (orderHeaderDTO != null)
        {
            var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(u=>u.Id==orderHeaderDTO.Id);
            orderHeaderFromDb.Name = orderHeaderDTO.Name;
            orderHeaderFromDb.PhoneNumber = orderHeaderDTO.PhoneNumber;
            orderHeaderFromDb.Carrier = orderHeaderDTO.Carrier;
            orderHeaderFromDb.Tracking = orderHeaderDTO.Tracking;
            orderHeaderFromDb.StreetAddress = orderHeaderDTO.StreetAddress;
            orderHeaderFromDb.City = orderHeaderDTO.City;
            orderHeaderFromDb.State = orderHeaderDTO.State;
            orderHeaderFromDb.PostalCode = orderHeaderDTO.PostalCode;
            orderHeaderFromDb.Status = orderHeaderDTO.Status;

            await _db.SaveChangesAsync();
            return _mapper.Map<OrderHeader, OrderHeaderDTO>(orderHeaderFromDb);
        }
        return new OrderHeaderDTO();
    }

    public async Task<bool> UpdateOrderStatus(int orderId, string status)
    {
        var data = await _db.OrderHeaders.FindAsync(orderId);
        if (data == null)
        {
            return false;
        }
        data.State = status;
        if (data.Status == SD.Status_Shipped)
        {
            data.ShippingDate = DateTime.Now;
        }
        await _db.SaveChangesAsync();
        return true;
    }
}

