using AutoMapper;
using OrderApi.Application.DTOs;
using OrderApi.Domain.Entities;

namespace OrderApi.Persentation.Mapper
{
    public class OrderProfile:Profile
    {

        public OrderProfile()
        { 
            CreateMap<Order,OrderDto>().ReverseMap();


        }

    }
}
