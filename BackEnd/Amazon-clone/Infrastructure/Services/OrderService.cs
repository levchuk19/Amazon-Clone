﻿using AutoMapper;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Infrastructure.Enum_s;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderedProductRepository _orderedProductRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IProductImageService _productImageService;
        
        private readonly EmailService _emailService;


        private readonly IMapper _mapper;
        public OrderService(IMapper mapper, IOrderRepository orderRepository,IOrderedProductRepository orderedProductRepository, EmailService emailService, IUserRepository userRepository, ICardRepository cardRepository, IProductImageService productImageService)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _orderedProductRepository = orderedProductRepository;
            _emailService = emailService;
            _userRepository = userRepository;
            _cardRepository = cardRepository;
            _productImageService = productImageService;
        }

        public async Task<OrderVM> AddOrderAsync(OrderDTO model)
        {
            var order = _mapper.Map<OrderDTO, Order>(model);
            await _orderRepository.Create(order);

            foreach (var orderedProdct_tmp in model.OrderedProducts_)
            {
                var orderedProduct = _mapper.Map<OrderedProductDTO, OrderedProduct>(orderedProdct_tmp);
                orderedProduct.OrderId = order.Id;
                orderedProduct.canLeaveComment = true;

                await _orderedProductRepository.Create(orderedProduct);
            }

            //await _orderRepository.Update(order);
            var orderVm = _mapper.Map<Order, OrderVM>(order);

            var user = await _userRepository.GetUserByIdAsync(model.UserId.ToString()); 

            await _emailService.SendEmailAsync(user.Email, "THANK YOU FOR YOUR PURCHASE", "</div><h1>Your order id:" + order.Id +"</h1>" + 
                "<p>" +
                "You can check your order status in your profile" +
                "<p>" +
                "<p>" +
                "If you have any questions write us: allmarkt@gmail.com" +
                "</p></div>");

            return orderVm;
        }

        public List<Order> GetAllOrdersAsync()
        {
            return _orderRepository.GetAll().ToList();
        }

        public async Task<List<OrderVM>> GetOrdersByUserIdAsync(int id)
        {
           var orders = _orderRepository.GetAll()
            .Where(order=>order.UserId == id)
            .Include(order => order.OrderedProducts)
                .ThenInclude(orderedProduct => orderedProduct.Product) // Include the related products
            .ToList();


            var orderVMs = new List<OrderVM>();

            foreach (var order in orders)
            {
                //var card = await _cardRepository.GetById((int)order.CardId);
                //tmpOrder.UserName = card.OwnerName;

                var tmpOrder = _mapper.Map<Order, OrderVM>(order);

                foreach (var item in order.OrderedProducts)
                {
                    

                    if (tmpOrder.Products == null) { tmpOrder.Products = new List<OrderedProductUpdatedVM>(); }

                    var ordered = new OrderedProductUpdatedVM
                    {
                        Count = item.Count,
                        Product = _mapper.Map<Product, ProductVM>(item.Product),
                    };

                    if (item.ProductId != null)
                    {
                        var image = await _productImageService.GetMainImageByIdAsync((int)item.ProductId);
                        var url = $@"https://amazonclone.monster/api/images/{image.Name + "_" + (int)Qualities.QualitiesSelector.LOW + ".jpg"}";
                        ordered.Product.Image = url;
                    }


                    tmpOrder.Products.Add(ordered);
                }
                orderVMs.Add(tmpOrder);

            }

            return orderVMs;
        }

        public List<OrderVM> GetOrdersByCompanyIdAsync(int id)
        {
            var orders = _orderRepository.GetAll()
            .Where(order=>order.isBought==false)
            .Include(order => order.OrderedProducts)
                .ThenInclude(orderedProduct => orderedProduct.Product) // Include the related products
            .ToList();

            var selectedOrders = new List<Order>();


            foreach(var order in orders)
            {
                var selectedProducts = new List<OrderedProduct>();
                var orderedProductsList = order.OrderedProducts.ToList();

                for (int i = 0; i < orderedProductsList.Count; i++)
                {
                    if (orderedProductsList[i].ProductId != null)
                    if (orderedProductsList[i].Product.CompanyId==id)
                    {
                        selectedProducts.Add(orderedProductsList[i]);
                    }
                }

                if(selectedProducts.Count>0) {
                    order.OrderedProducts = selectedProducts;
                    selectedOrders.Add(order);
                }
            }

            var selectedOrdersUpdated = new List<OrderVM>();

            var orderVMs = new List<OrderVM>();

            foreach (var order in selectedOrders)
            {
                var tmpOrder = _mapper.Map<Order, OrderVM>(order);

                foreach (var item in order.OrderedProducts)
                {
                    if (tmpOrder.Products == null) { tmpOrder.Products = new List<OrderedProductUpdatedVM>(); }
                    tmpOrder.Products.Add(new OrderedProductUpdatedVM
                    {
                        Count = item.Count,
                        Product= _mapper.Map<Product, ProductVM>(item.Product),
                    });
                }
                orderVMs.Add(tmpOrder);

            }

            return orderVMs;
        }


        public async Task<ServiceResponse> CloseAnOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetById(id);
            
            if (order != null)
            {
                order.isBought = true;
                await _orderRepository.Update(order);
                return new ServiceResponse()
                {
                    Message = "Order was closed"
                };
            }
            else
            {
                return new ServiceResponse()
                {
                    Message = "this order does'nt exist",
                    Errors = new List<string>() { "this order does'nt exist" }
                };
            }

        }
    }
}
