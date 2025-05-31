using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Review;
using CinemaBookingSystemDAL.Pagination;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ReviewResponseDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<ReviewResponseDTO>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<List<ReviewResponseDTO>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default);
        Task<ReviewResponseDTO> CreateAsync(CreateReviewDTO dto, CancellationToken cancellationToken = default);
        Task<ReviewResponseDTO> UpdateAsync(int id, UpdateReviewDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedList<ReviewResponseDTO>> GetPagedReviewsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedList<ReviewResponseDTO>> GetFilteredReviewsAsync(FilterReviewDto filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<List<ReviewResponseDTO>> GetTop10BestReviewsAsync(CancellationToken cancellationToken = default);
        Task<List<ReviewResponseDTO>> GetTop10WorstReviewsAsync(CancellationToken cancellationToken = default);
    }
}
