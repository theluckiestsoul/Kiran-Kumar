﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using chsakell_SPA.Infrastructure.core;
using System.Web.Http;
using AutoMapper;
using chsakell_SPA.Models;
using HomeCinema.DataRepositories.Infrastructure;
using HomeCinema.DataRepositories.Repositories;
using HomeCinema.Entities.Entities;
using System.IO;
using chsakell_SPA.Infrastructure.Extensions;

namespace chsakell_SPA.Controllers
{
    [Authorize(Roles="Admin")]
    [RoutePrefix("api/movies")]
    public class MoviesController: ApiControllerBase
    {
        private readonly IEntityBaseRepository<Movie> _moviesRepository;

        public MoviesController(IEntityBaseRepository<Movie> moviesRepository,
            IEntityBaseRepository<Error> errorsRepository, IUnitOfWork unitOfWork,IMapper mapper)
            : base(errorsRepository, unitOfWork,mapper)
        {
            _moviesRepository = moviesRepository;
        }
        [AllowAnonymous]
        [Route("latest")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, ()=>
            {
                HttpResponseMessage response = null;
                var movies = _moviesRepository.GetAll().OrderByDescending(m => m.ReleaseDate).Take(6).ToList();

                IEnumerable<MovieViewModel> moviesVm = Mapper.Map<IEnumerable<Movie>, IEnumerable<MovieViewModel>>(movies);

                response = request.CreateResponse(HttpStatusCode.OK, moviesVm);

                return response;
            });
        }
        [AllowAnonymous]
        [Route("{page:int=0}/{pageSize=3}/{filter?}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Movie> movies = null;
                int totalMovies = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    movies = _moviesRepository.GetAll()
                        .OrderBy(m => m.Id)
                        .Where(m => m.Title.ToLower()
                        .Contains(filter.ToLower().Trim()))
                        .ToList();
                }
                else
                {
                    movies = _moviesRepository.GetAll().ToList();
                }

                totalMovies = movies.Count();
                movies = movies.Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                IEnumerable<MovieViewModel> moviesVM = Mapper.Map<IEnumerable<Movie>, IEnumerable<MovieViewModel>>(movies);

                PaginationSet<MovieViewModel> pagedSet = new PaginationSet<MovieViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMovies,
                    TotalPages = (int)Math.Ceiling((decimal)totalMovies / currentPageSize),
                    Items = moviesVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }


        [Route("details/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var movie = _moviesRepository.GetSingle(id);

                MovieViewModel movieVM = Mapper.Map<Movie, MovieViewModel>(movie);

                response = request.CreateResponse(HttpStatusCode.OK, movieVM);

                return response;
            });
        }

        [MimeMultipart]
        [Route("images/upload")]
        public HttpResponseMessage Post(HttpRequestMessage request, int movieId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var movieOld = _moviesRepository.GetSingle(movieId);
                if (movieOld == null)
                    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid movie.");
                else
                {
                    var uploadPath = HttpContext.Current.Server.MapPath("~/Content/images/movies");

                    var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);

                    // Read the MIME multipart asynchronously 
                    Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

                    string _localFileName = multipartFormDataStreamProvider
                        .FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();

                    // Create response
                    FileUploadResult fileUploadResult = new FileUploadResult
                    {
                        LocalFilePath = _localFileName,

                        FileName = Path.GetFileName(_localFileName),

                        FileLength = new FileInfo(_localFileName).Length
                    };

                    // update database
                    movieOld.Image = fileUploadResult.FileName;
                    _moviesRepository.Edit(movieOld);
                    UnitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK, fileUploadResult);
                }

                return response;
            });
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, MovieViewModel movie)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var movieDb = _moviesRepository.GetSingle(movie.Id);
                    if (movieDb == null)
                        response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid movie.");
                    else
                    {
                        movieDb.UpdateMovie(movie);
                        movie.Image = movieDb.Image;
                        _moviesRepository.Edit(movieDb);

                        UnitOfWork.Commit();
                        response = request.CreateResponse(HttpStatusCode.OK, movie);
                    }
                }

                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Add(HttpRequestMessage request, MovieViewModel movie)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    Movie newMovie = new Movie();
                    newMovie.UpdateMovie(movie);

                    for (int i = 0; i < movie.NumberOfStocks; i++)
                    {
                        Stock stock = new Stock()
                        {
                            IsAvailable = true,
                            Movie = newMovie,
                            UniqueKey = Guid.NewGuid()
                        };
                        newMovie.Stocks.Add(stock);
                    }

                    _moviesRepository.Add(newMovie);

                    UnitOfWork.Commit();

                    // Update view model
                    movie = Mapper.Map<Movie, MovieViewModel>(newMovie);
                    response = request.CreateResponse(HttpStatusCode.Created, movie);
                }

                return response;
            });
        }
    }

}