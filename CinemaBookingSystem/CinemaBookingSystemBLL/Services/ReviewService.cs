using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IUnitOfWork unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<List<ReviewResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var reviews = await unitOfWork.Review.GetAllAsync(cancellationToken);
            var ordered = reviews.OrderBy(r => r.Id);

            return ordered.Select(review => new ReviewResponseDTO
            {
                Id = review.Id,
                MovieId = review.MovieId,
                UserId = review.UserId,
                Text = review.Text,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt.ToUniversalTime()
            }).ToList();
        }

        public async Task<ReviewResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var review = await unitOfWork.Review.GetByIdAsync(id, cancellationToken);
            if (review == null) return null;
            
            ReviewResponseDTO result = new ReviewResponseDTO
            {
                Id = review.Id,
                MovieId = review.MovieId,
                UserId = review.UserId,
                Text = review.Text,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt.ToUniversalTime()
            };
            
            return result;
        }

        public async Task<List<ReviewResponseDTO>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var reviews = await unitOfWork.Review.GetByUserIdAsync(userId, cancellationToken);

            return reviews.Select(review => new ReviewResponseDTO
            {
                Id = review.Id,
                MovieId = review.MovieId,
                UserId = review.UserId,
                Text = review.Text,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt.ToUniversalTime()
            }).ToList();
        }

        public async Task<List<ReviewResponseDTO>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
        {
            var reviews = await unitOfWork.Review.GetByMovieIdAsync(movieId, cancellationToken);

            return reviews.Select(review => new ReviewResponseDTO
            {
                Id = review.Id,
                MovieId = review.MovieId,
                UserId = review.UserId,
                Text = review.Text,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt.ToUniversalTime()
            }).ToList();
        }

        public async Task<ReviewResponseDTO> CreateAsync(CreateReviewDTO dto, CancellationToken cancellationToken = default)
        {
            Review review = new Review
            {
                MovieId = dto.MovieId,
                UserId = dto.UserId,
                Text = dto.Text,
                Rating = dto.Rating,
                CreatedAt = DateTime.UtcNow.ToUniversalTime()
            };

            await unitOfWork.Review.CreateAsync(review, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            ReviewResponseDTO result = new ReviewResponseDTO
            {
                Id = review.Id,
                MovieId = review.MovieId,
                UserId = review.UserId,
                Text = review.Text,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt.ToUniversalTime()
            };
            
            return result;
        }

        public async Task<ReviewResponseDTO?> UpdateAsync(int id, UpdateReviewDTO dto, CancellationToken cancellationToken = default)
        {
            var review = await unitOfWork.Review.GetByIdAsync(id, cancellationToken);
            if (review == null) return null;

            if (dto.Text != null && dto.Text.IsNullOrEmpty()) review.Text = dto.Text;
            if (dto.Rating.HasValue) review.Rating = dto.Rating.Value;

            unitOfWork.Review.Update(review);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            ReviewResponseDTO result = new ReviewResponseDTO
            {
                Id = review.Id,
                MovieId = review.MovieId,
                UserId = review.UserId,
                Text = review.Text,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt.ToUniversalTime()
            };
            return result;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var review = await unitOfWork.Review.GetByIdAsync(id, cancellationToken);
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
            var result = items.Select(review => new ReviewResponseDTO
            {
                Id = review.Id,
                MovieId = review.MovieId,
                UserId = review.UserId,
                Text = review.Text,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt.ToUniversalTime()
            }).ToList();

            return new PagedList<ReviewResponseDTO>(result, items.TotalCount, items.CurrentPage, items.PageSize);
        }

        public async Task<List<ReviewResponseDTO>> GetTop10BestReviewsAsync(CancellationToken cancellationToken = default)
        {
            var reviews = await unitOfWork.Review.GetTop10BestReviewsAsync(cancellationToken);

            return reviews.Select(review => new ReviewResponseDTO
            {
                Id = review.Id,
                MovieId = review.MovieId,
                UserId = review.UserId,
                Text = review.Text,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt.ToUniversalTime()
            }).ToList();
        }

        public async Task<List<ReviewResponseDTO>> GetTop10WorstReviewsAsync(CancellationToken cancellationToken = default)
        {
            var reviews = await unitOfWork.Review.GetTop10WorstReviewsAsync(cancellationToken);

            return reviews.Select(review => new ReviewResponseDTO
            {
                Id = review.Id,
                MovieId = review.MovieId,
                UserId = review.UserId,
                Text = review.Text,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt.ToUniversalTime()
            }).ToList();
        }

        public async Task<PagedList<ReviewResponseDTO>> GetPagedReviewsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = unitOfWork.Review.GetAll().OrderByDescending(r => r.CreatedAt);
            PagedList<Review> pagedReviews = await PagedList<Review>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);

            List<ReviewResponseDTO> reviewDtos = pagedReviews.Select(review => new ReviewResponseDTO
            {
                Id = review.Id,
                MovieId = review.MovieId,
                UserId = review.UserId,
                Text = review.Text,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt.ToUniversalTime()
            }).ToList();

            return new PagedList<ReviewResponseDTO>(reviewDtos, pagedReviews.TotalCount, pagedReviews.CurrentPage, pagedReviews.PageSize);
        }
    }
}