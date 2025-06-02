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

            CreateMap<Movie, MovieResponseDTO>();
            CreateMap<MovieCreateDTO, Movie>();
            CreateMap<MovieUpdateDTO, Movie>().ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Review, ReviewResponseDTO>().ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToUniversalTime()));
            CreateMap<CreateReviewDTO, Review>().ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            CreateMap<UpdateReviewDTO, Review>().ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Seat, SeatResponseDTO>();
            CreateMap<SeatCreateDTO, Seat>();
            CreateMap<SeatUpdateDTO, Seat>().ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Session, SessionResponseDTO>();
            CreateMap<Session, SessionResponseDTO>()
                .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
                .ForMember(dest => dest.HallName, opt => opt.MapFrom(src => src.Hall.Name));
            CreateMap<SessionUpdateDTO, Session>().ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Ticket, TicketResponseDTO>()
                .ForMember(dest => dest.SeatInfo, opt => opt.MapFrom(src => $"Row: {src.Seat.RowNumber}, Seat: {src.Seat.SeatNumber}"))
                .ForMember(dest => dest.SessionMovieTitle, opt => opt.MapFrom(src => src.Session.Movie.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<TicketCreateDTO, Ticket>();


            CreateMap<User, UserResponseDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Role, opt => opt.Ignore()); 
            CreateMap<UserUpdateDTO, User>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));
        }
    }
}
