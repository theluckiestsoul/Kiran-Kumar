﻿using AutoMapper;
using chsakell_SPA.Infrastructure.core;
using chsakell_SPA.Models;
using HomeCinema.DataRepositories.Extensions;
using HomeCinema.DataRepositories.Infrastructure;
using HomeCinema.DataRepositories.Repositories;
using HomeCinema.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace chsakell_SPA.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/rentals")]
    public class RentalsController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Rental> _rentalsRepository;
        private readonly IEntityBaseRepository<Customer> _customersRepository;
        private readonly IEntityBaseRepository<Stock> _stocksRepository;
        private readonly IEntityBaseRepository<Movie> _moviesRepository;

        public RentalsController(IEntityBaseRepository<Rental> rentalsRepository,
            IEntityBaseRepository<Customer> customersRepository, IEntityBaseRepository<Movie> moviesRepository,
            IEntityBaseRepository<Stock> stocksRepository,
            IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork,IMapper mapper)
            : base(_errorsRepository, _unitOfWork,mapper)
        {
            _rentalsRepository = rentalsRepository;
            _moviesRepository = moviesRepository;
            _customersRepository = customersRepository;
            _stocksRepository = stocksRepository;
        }

        private List<RentalHistoryPerDate> GetMovieRentalHistoryPerDates(int movieId)
        {
            List<RentalHistoryPerDate> listHistory = new List<RentalHistoryPerDate>();
            List<RentalHistoryViewModel> _rentalHistory = GetMovieRentalHistory(movieId);
            if (_rentalHistory.Count > 0)
            {
                List<DateTime> _distinctDates = new List<DateTime>();
                _distinctDates = _rentalHistory.Select(h => h.RentalDate.Date).Distinct().ToList();

                foreach (var distinctDate in _distinctDates)
                {
                    var totalDateRentals = _rentalHistory.Count(r => r.RentalDate.Date == distinctDate);
                    RentalHistoryPerDate _movieRentalHistoryPerDate = new RentalHistoryPerDate()
                    {
                        Date = distinctDate,
                        TotalRentals = totalDateRentals
                    };

                    listHistory.Add(_movieRentalHistoryPerDate);
                }

                listHistory.Sort((r1, r2) => r1.Date.CompareTo(r2.Date));
            }

            return listHistory;
        }
        private List<RentalHistoryViewModel> GetMovieRentalHistory(int movieId)
        {
            List<RentalHistoryViewModel> _rentalHistory = new List<RentalHistoryViewModel>();
            List<Rental> rentals = new List<Rental>();

            var movie = _moviesRepository.GetSingle(movieId);

            foreach (var stock in movie.Stocks)
            {
                rentals.AddRange(stock.Rentals);
            }

            foreach (var rental in rentals)
            {
                RentalHistoryViewModel _historyItem = new RentalHistoryViewModel()
                {
                    ID = rental.Id,
                    StockId = rental.StockId,
                    RentalDate = rental.RentalDate,
                    ReturnedDate = rental.ReturnedDate.HasValue ? rental.ReturnedDate : null,
                    Status = rental.Status,
                    Customer = _customersRepository.GetCustomerFullName(rental.CustomerId)
                };

                _rentalHistory.Add(_historyItem);
            }

            _rentalHistory.Sort((r1, r2) => r2.RentalDate.CompareTo(r1.RentalDate));

            return _rentalHistory;
        }

        [HttpGet]
        [Route("{id:int}/rentalhistory")]
        public HttpResponseMessage RentalHistory(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                List<RentalHistoryViewModel> _rentalHistory = GetMovieRentalHistory(id);

                response = request.CreateResponse(HttpStatusCode.OK, _rentalHistory);

                return response;
            });
        }

        [HttpPost]
        [Route("return/{rentalId:int}")]
        public HttpResponseMessage Return(HttpRequestMessage request, int rentalId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var rental = _rentalsRepository.GetSingle(rentalId);

                if (rental == null)
                    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid rental");
                else
                {
                    rental.Status = "Returned";
                    rental.Stock.IsAvailable = true;
                    rental.ReturnedDate = DateTime.Now;

                    UnitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }

        [HttpPost]
        [Route("rent/{customerId:int}/{stockId:int}")]
        public HttpResponseMessage Rent(HttpRequestMessage request, int customerId, int stockId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var customer = _customersRepository.GetSingle(customerId);
                var stock = _stocksRepository.GetSingle(stockId);

                if (customer == null || stock == null)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid Customer or Stock");
                }
                else
                {
                    if (stock.IsAvailable)
                    {
                        Rental _rental = new Rental()
                        {
                            CustomerId = customerId,
                            StockId = stockId,
                            RentalDate = DateTime.Now,
                            Status = "Borrowed"
                        };

                        _rentalsRepository.Add(_rental);

                        stock.IsAvailable = false;

                        UnitOfWork.Commit();

                        RentalViewModel rentalVm = Mapper.Map<Rental, RentalViewModel>(_rental);

                        response = request.CreateResponse<RentalViewModel>(HttpStatusCode.Created, rentalVm);
                    }
                    else
                        response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Selected stock is not available anymore");
                }

                return response;
            });
        }

        [HttpGet]
        [Route("rentalhistory")]
        public HttpResponseMessage TotalRentalHistory(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                List<TotalRentalHistoryViewModel> _totalMoviesRentalHistory = new List<TotalRentalHistoryViewModel>();

                var movies = _moviesRepository.GetAll();

                foreach (var movie in movies)
                {
                    TotalRentalHistoryViewModel _totalRentalHistory = new TotalRentalHistoryViewModel()
                    {
                        ID = movie.Id,
                        Title = movie.Title,
                        Image = movie.Image,
                        Rentals = GetMovieRentalHistoryPerDates(movie.Id)
                    };

                    if (_totalRentalHistory.TotalRentals > 0)
                        _totalMoviesRentalHistory.Add(_totalRentalHistory);
                }

                response = request.CreateResponse<List<TotalRentalHistoryViewModel>>(HttpStatusCode.OK, _totalMoviesRentalHistory);

                return response;
            });
        }
    }
}
