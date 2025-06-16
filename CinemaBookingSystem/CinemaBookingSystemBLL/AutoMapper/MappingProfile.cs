using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CinemaBookingSystemBLL.DTO.Genres;
using CinemaBookingSystemBLL.DTO.Halls;
using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.DTO.Review;
using CinemaBookingSystemBLL.DTO.Seats;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.DTO.Tickets;
using CinemaBookingSystemBLL.DTO.Users;
using CinemaBookingSystemBLL.Validations.Tickets;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemBLL.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Genre, GenreResponseDTO>();
            CreateMap<GenreCreateDTO, Genre>();
            CreateMap<GenreUpdateDTO, Genre>();

            CreateMap<Hall, HallResponseDTO>();
            CreateMap<HallCreateDTO, Hall>();
            CreateMap<HallUpdateDTO, Hall>().ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Movie, MovieResponseDTO>().ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => mg.Genre)));
            CreateMap<MovieCreateDTO, Movie>().ForMember(dest => dest.MovieGenres, opt => opt.MapFrom(src => src.GenreIds.Select(id => new MovieGenre { GenreId = id })));
            CreateMap<MovieUpdateDTO, Movie>()
                .ForMember(dest => dest.MovieGenres, opt => opt.MapFrom(src => src.GenreIds.Select(id => new MovieGenre { GenreId = id })))
                .ForMember(dest => dest.Id, opt => opt.Ignore());


            CreateMap<Review, ReviewResponseDTO>().ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToUniversalTime()));
            CreateMap<ReviewCreateDTO, Review>().ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            CreateMap<ReviewUpdateDTO, Review>().ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Seat, SeatResponseDTO>();

            CreateMap<Session, SessionResponseDTO>()
                .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
                .ForMember(dest => dest.HallName, opt => opt.MapFrom(src => src.Hall.Name));

            CreateMap<SessionCreateDTO, Session>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())  
                .ForMember(dest => dest.Hall, opt => opt.Ignore())
                .ForMember(dest => dest.Movie, opt => opt.Ignore());

            CreateMap<SessionUpdateDTO, Session>().ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Ticket, TicketResponseDTO>()
                .ForMember(dest => dest.SeatInfo, opt => opt.MapFrom(src => $"Row: {src.Seat.RowNumber}, Seat: {src.Seat.SeatNumber}"))
                .ForMember(dest => dest.SessionMovieTitle, opt => opt.MapFrom(src => src.Session.Movie.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<TicketCreateDTO, Ticket>()
                .ForMember(dest => dest.SeatId, opt => opt.MapFrom(src => src.SeatId))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseTime, opt => opt.MapFrom(src => DateTime.UtcNow.ToUniversalTime())); 
            CreateMap<User, UserResponseDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Role, opt => opt.Ignore()); 
            
            CreateMap<UserUpdateDTO, User>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));
            CreateMap<UserCreateDTO, User>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));
            CreateMap<UserCreateCustomerDTO, User>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));
        }
    }
}
