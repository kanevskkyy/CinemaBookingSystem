using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CinemaBookingSystemBLL.DTO.Review;
using CinemaBookingSystemBLL.Exceptions;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CinemaBookingSystemBLL.Services
{
    public class ReviewService : IReviewService
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<ReviewResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List<Review> reviews = await unitOfWork.Review.GetAllAsync(cancellationToken);
            return mapper.Map<List<ReviewResponseDTO>>(reviews);
        }

        public async Task<ReviewResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Review review = await unitOfWork.Review.GetByIdAsync(id, cancellationToken);
            if (review == null) throw new NotFoundException("Review", id);

            return mapper.Map<ReviewResponseDTO>(review);
        }

        public async Task<List<ReviewResponseDTO>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            List<Review> reviews = await unitOfWork.Review.GetByUserIdAsync(userId, cancellationToken);
            if (!Guid.TryParse(userId, out Guid guidId)) throw new ArgumentException("Invalid user ID format");
            if (reviews.IsNullOrEmpty()) throw new NotFoundException("Review", guidId); 
            
            return mapper.Map<List<ReviewResponseDTO>>(reviews);
        }

        public async Task<ReviewResponseDTO> CreateAsync(ReviewCreateDTO dto, string userId, CancellationToken cancellationToken = default)
        {
            bool exists = await unitOfWork.Review.ExistsByUserAndMovieAsync(userId, dto.MovieId, cancellationToken);
            if (exists) throw new ReviewAlreadyExistsException(userId, dto.MovieId);

            Review review = mapper.Map<Review>(dto);
            review.CreatedAt = DateTime.UtcNow.ToUniversalTime();
            review.UserId = userId;

            await unitOfWork.Review.CreateAsync(review, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<ReviewResponseDTO>(review);
        }

        public async Task<ReviewResponseDTO?> UpdateAsync(Guid id, ReviewUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            Review review = await unitOfWork.Review.GetByIdAsync(id, cancellationToken);
            if (review == null) throw new NotFoundException("Review", id);

            if (!string.IsNullOrEmpty(dto.Text)) review.Text = dto.Text;
            if (dto.Rating.HasValue) review.Rating = dto.Rating.Value;

            unitOfWork.Review.Update(review);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<ReviewResponseDTO>(review);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Review review = await unitOfWork.Review.GetByIdAsync(id, cancellationToken);
            if (review == null) throw new NotFoundException("Review", id);

            unitOfWork.Review.Delete(review);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<PagedList<ReviewResponseDTO>> GetFilteredReviewsAsync(Guid movieId, ReviewFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<Review> queryable = unitOfWork.Review.GetReviewsByMovieId(movieId);

            if (!string.IsNullOrEmpty(filter.UserId)) queryable = queryable.Where(r => r.UserId == filter.UserId);
            if (filter.MinRating.HasValue) queryable = queryable.Where(r => r.Rating >= filter.MinRating.Value);
            if (filter.MaxRating.HasValue) queryable = queryable.Where(r => r.Rating <= filter.MaxRating.Value);
            if (!string.IsNullOrEmpty(filter.TextContains)) queryable = queryable.Where(r => r.Text.ToLower().Contains(filter.TextContains.ToLower()));
            if (filter.CreatedAfter.HasValue) queryable = queryable.Where(r => r.CreatedAt >= filter.CreatedAfter.Value);
            if (filter.CreatedBefore.HasValue) queryable = queryable.Where(r => r.CreatedAt <= filter.CreatedBefore.Value);

            queryable = queryable.OrderByDescending(r => r.CreatedAt.ToUniversalTime());
            PagedList<Review> items = await PagedList<Review>.ToPagedListAsync(queryable, pageNumber, pageSize, cancellationToken);
            List<ReviewResponseDTO> result = mapper.Map<List<ReviewResponseDTO>>(items.Items);
            
            return new PagedList<ReviewResponseDTO>(result, items.TotalCount, items.CurrentPage, items.PageSize);
        }

        public async Task<PagedList<ReviewResponseDTO>> GetPagedReviewsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<Review> query = unitOfWork.Review.GetAll().OrderByDescending(r => r.CreatedAt);
            PagedList<Review> pagedReviews = await PagedList<Review>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);
            List<ReviewResponseDTO> reviewDtos = mapper.Map<List<ReviewResponseDTO>>(pagedReviews.Items);

            return new PagedList<ReviewResponseDTO>(reviewDtos, pagedReviews.TotalCount, pagedReviews.CurrentPage, pagedReviews.PageSize);
        }
    }
}