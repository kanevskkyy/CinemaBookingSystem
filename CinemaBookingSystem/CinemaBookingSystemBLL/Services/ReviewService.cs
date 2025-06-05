using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CinemaBookingSystemBLL.DTO.Review;
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
            var reviews = await unitOfWork.Review.GetAllAsync(cancellationToken);
            return mapper.Map<List<ReviewResponseDTO>>(reviews);
        }

        public async Task<ReviewResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Review review = await unitOfWork.Review.GetByIdAsync(id, cancellationToken);
            if (review == null) return null;

            return mapper.Map<ReviewResponseDTO>(review);
        }

        public async Task<List<ReviewResponseDTO>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var reviews = await unitOfWork.Review.GetByUserIdAsync(userId, cancellationToken);
            return mapper.Map<List<ReviewResponseDTO>>(reviews);
        }

        public async Task<List<ReviewResponseDTO>> GetByMovieIdAsync(Guid movieId, CancellationToken cancellationToken = default)
        {
            var reviews = await unitOfWork.Review.GetByMovieIdAsync(movieId, cancellationToken);
            return mapper.Map<List<ReviewResponseDTO>>(reviews);
        }

        public async Task<ReviewResponseDTO> CreateAsync(CreateReviewDTO dto, CancellationToken cancellationToken = default)
        {
            Review review = mapper.Map<Review>(dto);
            review.CreatedAt = DateTime.UtcNow.ToUniversalTime();

            await unitOfWork.Review.CreateAsync(review, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<ReviewResponseDTO>(review);
        }

        public async Task<ReviewResponseDTO?> UpdateAsync(Guid id, UpdateReviewDTO dto, CancellationToken cancellationToken = default)
        {
            Review review = await unitOfWork.Review.GetByIdAsync(id, cancellationToken);
            if (review == null) return null;

            if (!string.IsNullOrEmpty(dto.Text)) review.Text = dto.Text;
            if (dto.Rating.HasValue) review.Rating = dto.Rating.Value;

            unitOfWork.Review.Update(review);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<ReviewResponseDTO>(review);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Review review = await unitOfWork.Review.GetByIdAsync(id, cancellationToken);
            if (review == null) return false;

            unitOfWork.Review.Delete(review);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<PagedList<ReviewResponseDTO>> GetFilteredReviewsAsync(FilterReviewDto filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = unitOfWork.Review.GetAll();

            if (!string.IsNullOrEmpty(filter.UserId)) query = query.Where(r => r.UserId == filter.UserId);
            if (filter.MovieId.HasValue) query = query.Where(r => r.MovieId == filter.MovieId.Value);
            if (filter.MinRating.HasValue) query = query.Where(r => r.Rating >= filter.MinRating.Value);
            if (filter.MaxRating.HasValue) query = query.Where(r => r.Rating <= filter.MaxRating.Value);
            if (!string.IsNullOrEmpty(filter.TextContains)) query = query.Where(r => r.Text.ToLower().Contains(filter.TextContains.ToLower()));
            if (filter.CreatedAfter.HasValue) query = query.Where(r => r.CreatedAt >= filter.CreatedAfter.Value);
            if (filter.CreatedBefore.HasValue) query = query.Where(r => r.CreatedAt <= filter.CreatedBefore.Value);

            query = query.OrderByDescending(r => r.CreatedAt.ToUniversalTime());

            PagedList<Review> items = await PagedList<Review>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);
            var result = mapper.Map<List<ReviewResponseDTO>>(items.Items);

            return new PagedList<ReviewResponseDTO>(result, items.TotalCount, items.CurrentPage, items.PageSize);
        }

        public async Task<List<ReviewResponseDTO>> GetTop10BestReviewsAsync(CancellationToken cancellationToken = default)
        {
            List<Review> reviews = await unitOfWork.Review.GetTop10BestReviewsAsync(cancellationToken);

            return mapper.Map<List<ReviewResponseDTO>>(reviews);
        }

        public async Task<List<ReviewResponseDTO>> GetTop10WorstReviewsAsync(CancellationToken cancellationToken = default)
        {
            List<Review> reviews = await unitOfWork.Review.GetTop10WorstReviewsAsync(cancellationToken);

            return mapper.Map<List<ReviewResponseDTO>>(reviews);
        }

        public async Task<PagedList<ReviewResponseDTO>> GetPagedReviewsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = unitOfWork.Review.GetAll().OrderByDescending(r => r.CreatedAt);
            PagedList<Review> pagedReviews = await PagedList<Review>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);
            List<ReviewResponseDTO> reviewDtos = mapper.Map<List<ReviewResponseDTO>>(pagedReviews.Items);

            return new PagedList<ReviewResponseDTO>(reviewDtos, pagedReviews.TotalCount, pagedReviews.CurrentPage, pagedReviews.PageSize);
        }
    }
}