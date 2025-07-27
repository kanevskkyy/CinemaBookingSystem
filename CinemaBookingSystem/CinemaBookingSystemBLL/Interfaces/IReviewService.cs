using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Review;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Pagination;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ReviewResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ReviewResponseDTO>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ReviewResponseDTO> CreateAsync(ReviewCreateDTO dto, Guid userId, CancellationToken cancellationToken = default);
        Task<ReviewResponseDTO?> UpdateAsync(Guid id, ReviewUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedList<ReviewResponseDTO>> GetPagedReviewsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedList<ReviewResponseDTO>> GetFilteredReviewsAsync(Guid movieId, ReviewFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
