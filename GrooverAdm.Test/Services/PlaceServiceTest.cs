using GrooverAdm.Business.Services;
using GrooverAdm.Business.Services.Places;
using GrooverAdm.Business.Services.Playlist;
using GrooverAdm.Business.Services.Rating;
using GrooverAdm.Business.Services.Song;
using GrooverAdm.Business.Services.User;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.Model;
using GrooverAdm.Mappers.Interface;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Test.Services
{
    [TestFixture]

    public class PlaceServiceTest
    {

        private Mock<IPlacesDao<DataAccess.Firestore.Model.Place>> _dao;
        private Mock<IPlaceMapper<DataAccess.Firestore.Model.Place>> _mapper;
        private Mock<IRatingService> ratingService;
        private Mock<IUserService> userService;
        private Mock<IPlaylistService> playlistService;
        private Mock<ISongService> songService;
        private Mock<RecommendationService> recommendationService;

        private IPlacesService placeService;

        [SetUp]
        public void SetUp()
        {
            _dao = new Mock<IPlacesDao<DataAccess.Firestore.Model.Place>>();
            _mapper = new Mock<IPlaceMapper<DataAccess.Firestore.Model.Place>>();
            ratingService = new Mock<IRatingService>();
            userService = new Mock<IUserService>();
            playlistService = new Mock<IPlaylistService>();
            songService = new Mock<ISongService>();
            recommendationService = new Mock<RecommendationService>();

            placeService = new PlacesService(_dao.Object, _mapper.Object, playlistService.Object, songService.Object, recommendationService.Object, userService.Object, ratingService.Object, null);    
        }


        

        

    }
}
