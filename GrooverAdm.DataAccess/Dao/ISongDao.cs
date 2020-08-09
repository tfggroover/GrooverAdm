using GrooverAdm.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.DataAccess.Dao
{
    public interface ISongDao<T> where T : Song
    {
        Task<List<T>> GetSongs(string place, string playlist, int page, int pagesize);
        Task<List<Task<T>>> AddSongs(List<T> song);
        Task<T> AddSong(T song);
        Task<bool> DeleteSong(string place, string playlist, string song);
        Task<List<Task<T>>> OverrideSongs(List<T> song, string place, string playlist);

    }
}
