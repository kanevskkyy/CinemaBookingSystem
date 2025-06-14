using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Halls;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface IHallService
    {
        Task<List<HallResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<HallResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<HallResponseDTO> CreateAsync(HallCreateDTO dto, CancellationToken cancellationToken = default);
        Task<HallResponseDTO?> UpdateAsync(Guid id, HallUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
