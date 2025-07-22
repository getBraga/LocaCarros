using AutoMapper;
using LocaCarros.Application.Services;
using LocaCarros.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.Tests
{
    public class AluguelServiceTest
    {
        private readonly Mock<IAluguelRepository> _aluguelRepository;
        private readonly Mock<ICarroRepository> _carroRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly AluguelService _aluguelService;
        private readonly DateTime _dateTimeNow = DateTime.Now;
        public AluguelServiceTest()
        {
            _aluguelRepository = new Mock<IAluguelRepository>();
            _carroRepository = new Mock<ICarroRepository>();
            _mapper = new Mock<IMapper>();
            AluguelService _aluguelService = new AluguelService(
                _aluguelRepository.Object,
                _carroRepository.Object,
                _mapper.Object
            );
        }
    }
}
