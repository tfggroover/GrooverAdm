using GrooverAdm.Business.Services.Playlist;
using GrooverAdm.Business.Services.Song;
using GrooverAdm.Business.Services.User;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.Dao;
using GrooverAdm.DataAccess.Firestore.Model;
using GrooverAdm.Mappers.Interface;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.Test.Services
{
    [TestFixture]

    public class PlaylistServiceTest
    {
        private Mock<IPlaylistDao<Playlist>> dao;
        private Mock<IPlaylistMapper<Playlist>> mapper;
        private Mock<ISongService> song;
        private IPlaylistService playlistService;

        [SetUp]
        public void SetUp()
        {
            string place = "test";
            var dbplaylist = new Playlist();
            var playlist = new Entities.Application.Playlist();
            dao = new Mock<IPlaylistDao<Playlist>>();
            mapper = new Mock<IPlaylistMapper<Playlist>>();
            song = new Mock<ISongService>();
            playlistService = new PlaylistService(dao.Object, mapper.Object, song.Object, null);

            dao.Setup(a => a.GetPlaylist(place)).Returns(Task.FromResult(dbplaylist));
            dao.Setup(a => a.GetPlaylist("")).Returns(Task.FromResult<Playlist>(null));
            mapper.Setup(a => a.ToApplicationEntity(dbplaylist)).Returns(playlist);
            song.Setup(a => a.GetSongsFromPlaylist(place, "mainPlaylist", 1, int.MaxValue)).Returns(() => Task.FromResult(new List<Entities.Application.Song> { new Entities.Application.Song() }));
        }

        [Test]
        public async Task GetMainPlaylistFromPlace_ExistingPlace_NoSongs()
        {

            var res = await playlistService.GetMainPlaylistFromPlace("test", false, 1, int.MaxValue);

            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Songs);
            Assert.IsFalse(res.Changed);

        }

        [Test]
        public async Task GetMainPlaylistFromPlace_ExistingPlace_Songs()
        {
            var res = await playlistService.GetMainPlaylistFromPlace("test", true, 1, int.MaxValue);

            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Songs);
            Assert.IsFalse(res.Changed);
            Assert.IsNotEmpty(res.Songs);
        }

        [Test]
        public async Task GetMainPlaylistFromPlace_NonExistingPlace_Null()
        {


            var res = await playlistService.GetMainPlaylistFromPlace("", true, 1, int.MaxValue);

            Assert.IsNull(res);
        }



    }
}
